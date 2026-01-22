using System;

namespace Galapa.Launcher.Models;

/// <summary>
/// Event arguments for controller button events.
/// </summary>
public class ControllerButtonEventArgs : EventArgs
{
    /// <summary>
    /// The controller that raised the event.
    /// </summary>
    public required Controller Controller { get; init; }

    /// <summary>
    /// The button that was pressed, released, or repeated.
    /// </summary>
    public required ControllerButton Button { get; init; }
}
