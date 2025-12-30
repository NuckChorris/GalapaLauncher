using System;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using SDL3;

namespace Galapa.Launcher.Services;

public class GamepadInputService : IDisposable
{
    private const float ThumbstickDeadzone = 0.3f;
    private const int PollIntervalMs = 16; // ~60Hz
    private const int DirectionRepeatDelayMs = 150;
    private readonly ILogger<GamepadInputService> _logger;
    private readonly DispatcherTimer _pollTimer;
    private nint _gamepad = nint.Zero;
    private bool _isInitialized;
    private DateTime _lastDirectionTime = DateTime.MinValue;

    public GamepadInputService(ILogger<GamepadInputService> logger)
    {
        this._logger = logger;
        this._pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(PollIntervalMs)
        };
        this._pollTimer.Tick += this.OnPollTick;
    }

    public event EventHandler<Key>? NavigationKeyPressed;

    public void Start()
    {
        this._logger.LogInformation("Starting gamepad input service");

        if (!this._isInitialized)
        {
            if (!SDL.Init(SDL.InitFlags.Gamepad))
            {
                this._logger.LogError("Failed to initialize SDL gamepad subsystem: {Error}", SDL.GetError());
                return;
            }

            this._isInitialized = true;
        }

        this.FindGamepad();
        this._pollTimer.Start();
    }

    public void Stop()
    {
        this._logger.LogInformation("Stopping gamepad input service");
        this._pollTimer.Stop();
    }

    private void FindGamepad()
    {
        if (this._gamepad != nint.Zero)
            return;

        var gamepadIds = SDL.GetGamepads(out var count);

        if (count > 0 && gamepadIds != null)
        {
            var gamepadId = gamepadIds[0];
            this._gamepad = SDL.OpenGamepad(gamepadId);

            if (this._gamepad != nint.Zero)
            {
                var name = SDL.GetGamepadNameForID(gamepadId);
                this._logger.LogInformation("Found and opened gamepad: {Name} (ID: {Id})", name, gamepadId);
            }
            else
            {
                this._logger.LogWarning("Failed to open gamepad {Id}: {Error}", gamepadId, SDL.GetError());
            }
        }
    }

    private void OnPollTick(object? sender, EventArgs e)
    {
        if (this._gamepad == nint.Zero)
        {
            this.FindGamepad();
            return;
        }

        // Check if the gamepad is still connected
        if (!SDL.GamepadConnected(this._gamepad))
        {
            this._logger.LogWarning("Lost connection to gamepad");
            SDL.CloseGamepad(this._gamepad);
            this._gamepad = nint.Zero;
            return;
        }

        this.ProcessInput();
    }

    private void ProcessInput()
    {
        var now = DateTime.UtcNow;
        if ((now - this._lastDirectionTime).TotalMilliseconds < DirectionRepeatDelayMs) return;

        // Read current button states
        var dpadUp = SDL.GetGamepadButton(this._gamepad, SDL.GamepadButton.DPadUp);
        var dpadDown = SDL.GetGamepadButton(this._gamepad, SDL.GamepadButton.DPadDown);
        var dpadLeft = SDL.GetGamepadButton(this._gamepad, SDL.GamepadButton.DPadLeft);
        var dpadRight = SDL.GetGamepadButton(this._gamepad, SDL.GamepadButton.DPadRight);
        var buttonA = SDL.GetGamepadButton(this._gamepad, SDL.GamepadButton.South);
        var buttonB = SDL.GetGamepadButton(this._gamepad, SDL.GamepadButton.East);

        // Read analog stick values
        var leftStickX = SDL.GetGamepadAxis(this._gamepad, SDL.GamepadAxis.LeftX);
        var leftStickY = SDL.GetGamepadAxis(this._gamepad, SDL.GamepadAxis.LeftY);

        // Normalize thumbstick values to match XInput range (-1.0 to 1.0)
        var normalizedX = leftStickX / 32767.0f;
        var normalizedY = leftStickY / 32767.0f;

        // Check D-pad Up or left-stick up (negative Y in SDL)
        if (dpadUp || normalizedY < -ThumbstickDeadzone)
        {
            this._lastDirectionTime = now;
            this.NavigationKeyPressed?.Invoke(this, Key.Up);
            return;
        }

        // Check D-pad Down or left stick down (positive Y in SDL)
        if (dpadDown || normalizedY > ThumbstickDeadzone)
        {
            this._lastDirectionTime = now;
            this.NavigationKeyPressed?.Invoke(this, Key.Down);
            return;
        }

        // Check D-pad Left or left-stick left (negative X)
        if (dpadLeft || normalizedX < -ThumbstickDeadzone)
        {
            this._lastDirectionTime = now;
            this.NavigationKeyPressed?.Invoke(this, Key.Left);
            return;
        }

        // Check D-pad Right or left stick right (positive X)
        if (dpadRight || normalizedX > ThumbstickDeadzone)
        {
            this._lastDirectionTime = now;
            this.NavigationKeyPressed?.Invoke(this, Key.Right);
            return;
        }

        // Check A button (South button in SDL - bottom button on Xbox controller)
        if (buttonA)
        {
            this._lastDirectionTime = now;
            this.NavigationKeyPressed?.Invoke(this, Key.Enter);
            return;
        }

        // Check B button (East button in SDL - right button on Xbox controller)
        if (buttonB)
        {
            this._lastDirectionTime = now;
            this.NavigationKeyPressed?.Invoke(this, Key.Escape);
        }
    }

    public void Dispose()
    {
        this.Stop();
        this._pollTimer.Tick -= this.OnPollTick;

        if (this._gamepad != nint.Zero)
        {
            SDL.CloseGamepad(this._gamepad);
            this._gamepad = nint.Zero;
        }

        if (this._isInitialized)
        {
            SDL.Quit();
            this._isInitialized = false;
        }
    }
}