namespace Galapa.Launcher.Models;

/// <summary>
///     Determines the button label style to display for controller prompts.
///     Values match the game's ButtonCaptionType for compatibility.
/// </summary>
public enum ControllerLabelStyle
{
    /// <summary>
    ///     Automatic style labels
    /// </summary>
    Automatic = 0,

    /// <summary>
    ///     DirectInput Numeric style labels (1, 2, 3, 4, 5, 6, 7, 8).
    /// </summary>
    Numeric = 1,

    /// <summary>
    ///     Xbox style labels (A, B, X, Y, LB, RB, LT, RT).
    /// </summary>
    Xbox = 2,

    /// <summary>
    ///     Nintendo style labels (B, A, Y, X, L, R, ZL, ZR).
    /// </summary>
    Nintendo = 3
}