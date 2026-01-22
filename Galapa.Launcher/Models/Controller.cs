using System;
using System.Collections.Immutable;
using SDL3;

namespace Galapa.Launcher.Models;

/// <summary>
/// Represents a controller that has been correlated between SDL3 and DirectInput.
/// </summary>
public class Controller
{
    /// <summary>
    /// Delay before button repeat starts (milliseconds).
    /// </summary>
    public const int RepeatDelayMs = 400;

    /// <summary>
    /// Interval between button repeats (milliseconds).
    /// </summary>
    public const int RepeatIntervalMs = 80;

    /// <summary>
    /// Unique identifier for correlation tracking (format: "VID:PID:Index").
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Human-readable name of the controller.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// USB Vendor ID.
    /// </summary>
    public ushort VendorId { get; init; }

    /// <summary>
    /// USB Product ID.
    /// </summary>
    public ushort ProductId { get; init; }

    /// <summary>
    /// Instance index for distinguishing multiple identical controllers.
    /// </summary>
    public int InstanceIndex { get; init; }

    /// <summary>
    /// The SDL3 gamepad type for this controller.
    /// </summary>
    public SDL.GamepadType GamepadType { get; init; }

    /// <summary>
    /// The SDL3 Joystick ID for this controller.
    /// </summary>
    internal uint Sdl3JoystickId { get; init; }

    /// <summary>
    /// The DirectInput Instance GUID for this controller.
    /// </summary>
    internal Guid DirectInputInstanceGuid { get; init; }

    // Internal state for polling
    internal ControllerState LastState { get; set; } = ControllerState.Empty;
    internal ImmutableDictionary<ControllerButton, DateTime> ButtonHeldSince { get; set; } =
        ImmutableDictionary<ControllerButton, DateTime>.Empty;
    internal ImmutableDictionary<ControllerButton, DateTime> ButtonLastRepeat { get; set; } =
        ImmutableDictionary<ControllerButton, DateTime>.Empty;

    /// <summary>
    /// Raised when a button is initially pressed.
    /// </summary>
    public event EventHandler<ControllerButtonEventArgs>? ButtonPressed;

    /// <summary>
    /// Raised when a button is released.
    /// </summary>
    public event EventHandler<ControllerButtonEventArgs>? ButtonReleased;

    /// <summary>
    /// Raised when a held button triggers a repeat.
    /// </summary>
    public event EventHandler<ControllerButtonEventArgs>? ButtonRepeat;

    internal void RaiseButtonPressed(ControllerButton button)
    {
        this.ButtonPressed?.Invoke(this, new ControllerButtonEventArgs
        {
            Controller = this,
            Button = button
        });
    }

    internal void RaiseButtonReleased(ControllerButton button)
    {
        this.ButtonReleased?.Invoke(this, new ControllerButtonEventArgs
        {
            Controller = this,
            Button = button
        });
    }

    internal void RaiseButtonRepeat(ControllerButton button)
    {
        this.ButtonRepeat?.Invoke(this, new ControllerButtonEventArgs
        {
            Controller = this,
            Button = button
        });
    }
}
