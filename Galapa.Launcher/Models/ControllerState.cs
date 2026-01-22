using System;
using System.Collections.Immutable;

namespace Galapa.Launcher.Models;

/// <summary>
/// Immutable snapshot of a controller's input state.
/// </summary>
public sealed class ControllerState
{
    /// <summary>
    /// Default deadzone for analog stick input (DirectInput range is -32768 to 32767).
    /// </summary>
    public const int DefaultStickDeadzone = 8000;

    /// <summary>
    /// An empty state with all buttons released.
    /// </summary>
    public static readonly ControllerState Empty = new(
        ImmutableArray<bool>.Empty,
        -1,
        0,
        0);

    /// <summary>
    /// Button states indexed by button number.
    /// </summary>
    public ImmutableArray<bool> Buttons { get; }

    /// <summary>
    /// POV hat value in hundredths of degrees, or -1 if centered.
    /// </summary>
    public int PovHat { get; }

    /// <summary>
    /// Left stick X axis (-32768 to 32767, centered at 0).
    /// </summary>
    public int StickX { get; }

    /// <summary>
    /// Left stick Y axis (-32768 to 32767, centered at 0).
    /// </summary>
    public int StickY { get; }

    public ControllerState(ImmutableArray<bool> buttons, int povHat, int stickX, int stickY)
    {
        this.Buttons = buttons;
        this.PovHat = povHat;
        this.StickX = stickX;
        this.StickY = stickY;
    }

    /// <summary>
    /// Gets whether a specific button is pressed.
    /// </summary>
    public bool IsButtonPressed(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= this.Buttons.Length)
            return false;
        return this.Buttons[buttonIndex];
    }

    /// <summary>
    /// Gets whether the POV hat is in the up direction.
    /// </summary>
    public bool IsPovUp => this.PovHat != -1 && (this.PovHat >= 31500 || this.PovHat <= 4500);

    /// <summary>
    /// Gets whether the POV hat is in the down direction.
    /// </summary>
    public bool IsPovDown => this.PovHat is >= 13500 and <= 22500;

    /// <summary>
    /// Gets whether the POV hat is in the left direction.
    /// </summary>
    public bool IsPovLeft => this.PovHat is >= 22500 and <= 31500;

    /// <summary>
    /// Gets whether the POV hat is in the right direction.
    /// </summary>
    public bool IsPovRight => this.PovHat is >= 4500 and <= 13500;

    /// <summary>
    /// Gets whether the stick is pushed up past the deadzone.
    /// </summary>
    public bool IsStickUp(int deadzone = DefaultStickDeadzone) => this.StickY < -deadzone;

    /// <summary>
    /// Gets whether the stick is pushed down past the deadzone.
    /// </summary>
    public bool IsStickDown(int deadzone = DefaultStickDeadzone) => this.StickY > deadzone;

    /// <summary>
    /// Gets whether the stick is pushed left past the deadzone.
    /// </summary>
    public bool IsStickLeft(int deadzone = DefaultStickDeadzone) => this.StickX < -deadzone;

    /// <summary>
    /// Gets whether the stick is pushed right past the deadzone.
    /// </summary>
    public bool IsStickRight(int deadzone = DefaultStickDeadzone) => this.StickX > deadzone;

    /// <summary>
    /// Checks if a virtual button (physical button, POV direction, or stick direction) is active.
    /// </summary>
    public bool IsActive(ControllerButton button, int stickDeadzone = DefaultStickDeadzone)
    {
        if (button.IsPhysicalButton)
            return this.IsButtonPressed(button.Index);

        return button.Index switch
        {
            ControllerButton.PovUp => this.IsPovUp,
            ControllerButton.PovDown => this.IsPovDown,
            ControllerButton.PovLeft => this.IsPovLeft,
            ControllerButton.PovRight => this.IsPovRight,
            ControllerButton.StickUp => this.IsStickUp(stickDeadzone),
            ControllerButton.StickDown => this.IsStickDown(stickDeadzone),
            ControllerButton.StickLeft => this.IsStickLeft(stickDeadzone),
            ControllerButton.StickRight => this.IsStickRight(stickDeadzone),
            _ => false
        };
    }

    /// <summary>
    /// Gets the set of currently active virtual buttons.
    /// </summary>
    public ImmutableHashSet<ControllerButton> GetActiveButtons(int stickDeadzone = DefaultStickDeadzone)
    {
        var builder = ImmutableHashSet.CreateBuilder<ControllerButton>();

        // Physical buttons
        for (var i = 0; i < this.Buttons.Length; i++)
        {
            if (this.Buttons[i])
                builder.Add(new ControllerButton(i));
        }

        // POV directions
        if (this.IsPovUp) builder.Add(new ControllerButton(ControllerButton.PovUp));
        if (this.IsPovDown) builder.Add(new ControllerButton(ControllerButton.PovDown));
        if (this.IsPovLeft) builder.Add(new ControllerButton(ControllerButton.PovLeft));
        if (this.IsPovRight) builder.Add(new ControllerButton(ControllerButton.PovRight));

        // Stick directions
        if (this.IsStickUp(stickDeadzone)) builder.Add(new ControllerButton(ControllerButton.StickUp));
        if (this.IsStickDown(stickDeadzone)) builder.Add(new ControllerButton(ControllerButton.StickDown));
        if (this.IsStickLeft(stickDeadzone)) builder.Add(new ControllerButton(ControllerButton.StickLeft));
        if (this.IsStickRight(stickDeadzone)) builder.Add(new ControllerButton(ControllerButton.StickRight));

        return builder.ToImmutable();
    }
}
