namespace Galapa.Launcher.Models;

/// <summary>
/// Represents a button on a controller by its DirectInput index.
/// </summary>
/// <param name="Index">The DirectInput button index (0-127 for buttons, 128+ reserved for axes/POV).</param>
public readonly record struct ControllerButton(int Index)
{
    /// <summary>
    /// Special index indicating the POV hat up direction.
    /// </summary>
    public const int PovUp = 200;

    /// <summary>
    /// Special index indicating the POV hat down direction.
    /// </summary>
    public const int PovDown = 201;

    /// <summary>
    /// Special index indicating the POV hat left direction.
    /// </summary>
    public const int PovLeft = 202;

    /// <summary>
    /// Special index indicating the POV hat right direction.
    /// </summary>
    public const int PovRight = 203;

    /// <summary>
    /// Special index indicating the left stick up direction.
    /// </summary>
    public const int StickUp = 210;

    /// <summary>
    /// Special index indicating the left stick down direction.
    /// </summary>
    public const int StickDown = 211;

    /// <summary>
    /// Special index indicating the left stick left direction.
    /// </summary>
    public const int StickLeft = 212;

    /// <summary>
    /// Special index indicating the left stick right direction.
    /// </summary>
    public const int StickRight = 213;

    /// <summary>
    /// Whether this button represents a physical button (not POV/stick).
    /// </summary>
    public bool IsPhysicalButton => this.Index is >= 0 and < 128;

    /// <summary>
    /// Whether this button represents a POV hat direction.
    /// </summary>
    public bool IsPovDirection => this.Index is >= PovUp and <= PovRight;

    /// <summary>
    /// Whether this button represents a stick direction.
    /// </summary>
    public bool IsStickDirection => this.Index is >= StickUp and <= StickRight;

    public override string ToString() => this.Index switch
    {
        PovUp => "POV Up",
        PovDown => "POV Down",
        PovLeft => "POV Left",
        PovRight => "POV Right",
        StickUp => "Stick Up",
        StickDown => "Stick Down",
        StickLeft => "Stick Left",
        StickRight => "Stick Right",
        _ => $"Button {this.Index}"
    };
}
