using Galapa.Launcher.Models;

namespace Galapa.Launcher.Input;

/// <summary>
/// Interface for UI elements that can handle controller input.
/// Implement this on controls that need to respond to controller actions.
/// </summary>
public interface IControllerInputHandler
{
    /// <summary>
    /// Handles a controller input action.
    /// </summary>
    /// <param name="action">The semantic action to handle.</param>
    /// <param name="isRepeat">Whether this is a repeat event from holding the button.</param>
    /// <returns>True if the action was handled and should not bubble further, false otherwise.</returns>
    bool HandleControllerInput(ControllerAction action, bool isRepeat);
}
