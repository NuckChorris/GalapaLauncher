namespace Galapa.Launcher.Models;

/// <summary>
/// Determines the button label style to display for controller prompts.
/// Values match the game's ButtonCaptionType for compatibility.
/// </summary>
public enum ControllerLabelStyle
{
    /// <summary>
    /// Xbox style labels (A, B, X, Y, LB, RB, LT, RT).
    /// </summary>
    Xbox = 0,

    /// <summary>
    /// Nintendo style labels (B, A, Y, X, L, R, ZL, ZR).
    /// </summary>
    Nintendo = 3,

    /// <summary>
    /// Generic numbered style (1, 2, 3, 4, L1, R1, L2, R2).
    /// </summary>
    Generic = 99
}
