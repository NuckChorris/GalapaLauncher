using System;

namespace Galapa.Launcher.Models;

/// <summary>
/// Event arguments for controller connection state changes.
/// </summary>
public class ControllerEventArgs : EventArgs
{
    /// <summary>
    /// The controller that was connected or disconnected.
    /// </summary>
    public required Controller Controller { get; init; }
}
