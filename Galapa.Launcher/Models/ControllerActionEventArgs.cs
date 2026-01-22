using System;

namespace Galapa.Launcher.Models;

/// <summary>
/// Event arguments for controller action events.
/// </summary>
public class ControllerActionEventArgs : EventArgs
{
    /// <summary>
    /// The controller that raised the event.
    /// </summary>
    public required Controller Controller { get; init; }

    /// <summary>
    /// The semantic action that was triggered.
    /// </summary>
    public required ControllerAction Action { get; init; }

    /// <summary>
    /// Whether this is a repeat event (button held down).
    /// </summary>
    public required bool IsRepeat { get; init; }
}
