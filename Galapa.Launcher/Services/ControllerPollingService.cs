using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Avalonia.Threading;
using Galapa.Launcher.Models;
using Microsoft.Extensions.Logging;
using Vortice.DirectInput;

namespace Galapa.Launcher.Services;

/// <summary>
/// Service that polls DirectInput devices and fires button events on Controllers.
/// Runs at ~120Hz using a DispatcherTimer.
/// </summary>
public class ControllerPollingService : IDisposable
{
    private const int PollIntervalMs = 8; // ~120Hz

    private readonly ILogger<ControllerPollingService> _logger;
    private readonly ControllerListService _controllerListService;
    private readonly IDirectInput8 _directInput;
    private readonly DispatcherTimer _pollTimer;
    private readonly Dictionary<string, IDirectInputDevice8> _devices = new();

    private bool _started;

    public ControllerPollingService(
        ILogger<ControllerPollingService> logger,
        ControllerListService controllerListService)
    {
        this._logger = logger;
        this._controllerListService = controllerListService;
        this._directInput = DInput.DirectInput8Create();
        this._pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(PollIntervalMs)
        };
        this._pollTimer.Tick += this.OnPollTick;
    }

    public void Start()
    {
        if (this._started)
            return;

        this._logger.LogInformation("Starting controller polling service");
        this._started = true;

        this._controllerListService.ControllerConnected += this.OnControllerConnected;
        this._controllerListService.ControllerDisconnected += this.OnControllerDisconnected;
        this._controllerListService.Start();

        // Acquire existing controllers
        foreach (var controller in this._controllerListService.Controllers)
        {
            this.AcquireController(controller);
        }

        this._pollTimer.Start();
    }

    public void Stop()
    {
        if (!this._started)
            return;

        this._logger.LogInformation("Stopping controller polling service");
        this._started = false;

        this._pollTimer.Stop();
        this._controllerListService.ControllerConnected -= this.OnControllerConnected;
        this._controllerListService.ControllerDisconnected -= this.OnControllerDisconnected;

        // Release all devices
        foreach (var (id, device) in this._devices)
        {
            try
            {
                device.Unacquire();
                device.Dispose();
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "Error releasing device {Id}", id);
            }
        }
        this._devices.Clear();
    }

    private void OnControllerConnected(object? sender, ControllerEventArgs e)
    {
        this.AcquireController(e.Controller);
    }

    private void OnControllerDisconnected(object? sender, ControllerEventArgs e)
    {
        this.ReleaseController(e.Controller);
    }

    private void AcquireController(Controller controller)
    {
        if (this._devices.ContainsKey(controller.Id))
            return;

        try
        {
            var device = this._directInput.CreateDevice(controller.DirectInputInstanceGuid);
            device.SetDataFormat<RawJoystickState>();
            device.SetCooperativeLevel(IntPtr.Zero, CooperativeLevel.NonExclusive | CooperativeLevel.Background);
            device.Acquire();

            this._devices[controller.Id] = device;
            this._logger.LogDebug("Acquired DirectInput device for controller {Name}", controller.Name);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to acquire DirectInput device for controller {Name}", controller.Name);
        }
    }

    private void ReleaseController(Controller controller)
    {
        if (!this._devices.TryGetValue(controller.Id, out var device))
            return;

        try
        {
            device.Unacquire();
            device.Dispose();
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Error releasing device for controller {Name}", controller.Name);
        }

        this._devices.Remove(controller.Id);

        // Clear controller state
        controller.LastState = ControllerState.Empty;
        controller.ButtonHeldSince = ImmutableDictionary<ControllerButton, DateTime>.Empty;
        controller.ButtonLastRepeat = ImmutableDictionary<ControllerButton, DateTime>.Empty;
    }

    private void OnPollTick(object? sender, EventArgs e)
    {
        var now = DateTime.UtcNow;

        foreach (var controller in this._controllerListService.Controllers)
        {
            if (!this._devices.TryGetValue(controller.Id, out var device))
                continue;

            try
            {
                device.Poll();
                var rawState = device.GetCurrentJoystickState();
                var newState = this.CreateControllerState(rawState);

                this.ProcessStateChange(controller, newState, now);
                controller.LastState = newState;
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "Failed to poll controller {Name}, releasing", controller.Name);
                this.ReleaseController(controller);
            }
        }
    }

    private ControllerState CreateControllerState(JoystickState rawState)
    {
        // Convert button states to immutable array
        var buttons = ImmutableArray.CreateBuilder<bool>(128);
        for (var i = 0; i < 128; i++)
        {
            buttons.Add(rawState.Buttons[i]);
        }

        // Get POV hat (first one)
        var pov = rawState.PointOfViewControllers[0];

        // Get stick position (centered at 32767, convert to -32768 to 32767)
        var stickX = rawState.X - 32767;
        var stickY = rawState.Y - 32767;

        return new ControllerState(buttons.ToImmutable(), pov, stickX, stickY);
    }

    private void ProcessStateChange(Controller controller, ControllerState newState, DateTime now)
    {
        var oldActive = controller.LastState.GetActiveButtons();
        var newActive = newState.GetActiveButtons();

        // Find newly pressed buttons
        foreach (var button in newActive)
        {
            if (!oldActive.Contains(button))
            {
                // Button just pressed
                controller.ButtonHeldSince = controller.ButtonHeldSince.SetItem(button, now);
                controller.ButtonLastRepeat = controller.ButtonLastRepeat.SetItem(button, now);
                controller.RaiseButtonPressed(button);
            }
            else
            {
                // Button held - check for repeat
                if (controller.ButtonHeldSince.TryGetValue(button, out var heldSince) &&
                    controller.ButtonLastRepeat.TryGetValue(button, out var lastRepeat))
                {
                    var timeSincePress = (now - heldSince).TotalMilliseconds;
                    var timeSinceLastRepeat = (now - lastRepeat).TotalMilliseconds;

                    if (timeSincePress >= Controller.RepeatDelayMs &&
                        timeSinceLastRepeat >= Controller.RepeatIntervalMs)
                    {
                        controller.ButtonLastRepeat = controller.ButtonLastRepeat.SetItem(button, now);
                        controller.RaiseButtonRepeat(button);
                    }
                }
            }
        }

        // Find released buttons
        foreach (var button in oldActive)
        {
            if (!newActive.Contains(button))
            {
                // Button just released
                controller.ButtonHeldSince = controller.ButtonHeldSince.Remove(button);
                controller.ButtonLastRepeat = controller.ButtonLastRepeat.Remove(button);
                controller.RaiseButtonReleased(button);
            }
        }
    }

    public void Dispose()
    {
        this.Stop();
        this._pollTimer.Tick -= this.OnPollTick;
        this._directInput.Dispose();
    }
}
