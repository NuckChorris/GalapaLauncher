using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.VisualTree;
using Galapa.Launcher.Input;
using Galapa.Launcher.Models;
using Microsoft.Extensions.Logging;

namespace Galapa.Launcher.Services;

/// <summary>
/// Routes controller actions to UI elements by walking the visual tree
/// from the focused element upward, giving each IControllerInputHandler
/// a chance to handle the action.
/// </summary>
public class ControllerInputRouter : IDisposable
{
    private readonly ILogger<ControllerInputRouter> _logger;
    private readonly ControllerActionSource _actionSource;
    private TopLevel? _topLevel;

    public ControllerInputRouter(
        ILogger<ControllerInputRouter> logger,
        ControllerActionSource actionSource)
    {
        this._logger = logger;
        this._actionSource = actionSource;
    }

    /// <summary>
    /// Attaches the router to a top-level window.
    /// </summary>
    public void Attach(TopLevel topLevel)
    {
        if (this._topLevel != null)
            this.Detach();

        this._topLevel = topLevel;
        this._actionSource.ActionTriggered += this.OnActionTriggered;
        this._actionSource.ActionRepeated += this.OnActionRepeated;
        this._logger.LogDebug("Attached to {TopLevel}", topLevel.GetType().Name);
    }

    /// <summary>
    /// Detaches the router from the current top-level window.
    /// </summary>
    public void Detach()
    {
        if (this._topLevel == null)
            return;

        this._actionSource.ActionTriggered -= this.OnActionTriggered;
        this._actionSource.ActionRepeated -= this.OnActionRepeated;
        this._logger.LogDebug("Detached from {TopLevel}", this._topLevel.GetType().Name);
        this._topLevel = null;
    }

    private void OnActionTriggered(object? sender, ControllerActionEventArgs e)
    {
        this.RouteAction(e.Action, isRepeat: false);
    }

    private void OnActionRepeated(object? sender, ControllerActionEventArgs e)
    {
        this.RouteAction(e.Action, isRepeat: true);
    }

    private void RouteAction(ControllerAction action, bool isRepeat)
    {
        if (this._topLevel == null)
            return;

        // Get the currently focused element
        var focusManager = this._topLevel.FocusManager;
        var focused = focusManager?.GetFocusedElement() as Visual;

        // If nothing focused, start from the top level
        focused ??= this._topLevel;

        // Walk up the visual tree looking for handlers
        Visual? current = focused;
        while (current != null)
        {
            if (current is IControllerInputHandler handler)
            {
                if (handler.HandleControllerInput(action, isRepeat))
                {
                    this._logger.LogDebug(
                        "Action {Action} handled by {Handler}",
                        action,
                        current.GetType().Name);
                    return;
                }
            }
            current = current.GetVisualParent();
        }

        // No handler consumed the action - apply default behavior
        this.HandleDefault(action, isRepeat);
    }

    private void HandleDefault(ControllerAction action, bool isRepeat)
    {
        if (this._topLevel == null)
            return;

        var focusManager = this._topLevel.FocusManager;
        var focused = focusManager?.GetFocusedElement() as InputElement;

        switch (action)
        {
            case ControllerAction.Up:
            case ControllerAction.Down:
            case ControllerAction.Left:
            case ControllerAction.Right:
                this.NavigateXYFocus(action);
                break;

            case ControllerAction.Confirm:
                if (focused != null)
                {
                    // Simulate Enter key press
                    var keyArgs = new KeyEventArgs
                    {
                        Key = Key.Enter,
                        RoutedEvent = InputElement.KeyDownEvent
                    };
                    focused.RaiseEvent(keyArgs);
                }
                break;

            case ControllerAction.Decline:
                if (focused != null)
                {
                    // Simulate Escape key press
                    var keyArgs = new KeyEventArgs
                    {
                        Key = Key.Escape,
                        RoutedEvent = InputElement.KeyDownEvent
                    };
                    focused.RaiseEvent(keyArgs);
                }
                break;

            // Bumpers and triggers have no default action
            case ControllerAction.BumperLeft:
            case ControllerAction.BumperRight:
            case ControllerAction.TriggerLeft:
            case ControllerAction.TriggerRight:
                break;
        }
    }

    private void NavigateXYFocus(ControllerAction action)
    {
        if (this._topLevel == null)
            return;

        var direction = action switch
        {
            ControllerAction.Up => NavigationDirection.Up,
            ControllerAction.Down => NavigationDirection.Down,
            ControllerAction.Left => NavigationDirection.Left,
            ControllerAction.Right => NavigationDirection.Right,
            _ => NavigationDirection.Next
        };

        var focusManager = this._topLevel.FocusManager;
        var focused = focusManager?.GetFocusedElement() as InputElement;

        if (focused != null)
        {
            // Use Avalonia's focus navigation
            var next = KeyboardNavigationHandler.GetNext(focused, direction);
            next?.Focus(NavigationMethod.Directional);
        }
        else
        {
            // Focus the first focusable element
            this._topLevel.Focus(NavigationMethod.Directional);
        }
    }

    public void Dispose()
    {
        this.Detach();
    }
}
