using System;
using System.Collections.Generic;
using Galapa.Core.Models;
using SDL3;

namespace Galapa.Launcher.Models;

/// <summary>
/// Maps controller buttons to semantic actions for a specific controller.
/// </summary>
public class ControllerConfig
{
    private readonly Dictionary<ControllerButton, ControllerAction> _buttonToAction = new();
    private readonly Dictionary<ControllerAction, ControllerButton> _actionToButton = new();

    /// <summary>
    /// The DirectInput device GUID this config applies to.
    /// </summary>
    public Guid DeviceGuid { get; init; }

    /// <summary>
    /// Gets the action mapped to a button, or null if not mapped.
    /// </summary>
    public ControllerAction? GetAction(ControllerButton button)
    {
        return this._buttonToAction.TryGetValue(button, out var action) ? action : null;
    }

    /// <summary>
    /// Gets the button mapped to an action, or null if not mapped.
    /// </summary>
    public ControllerButton? GetButton(ControllerAction action)
    {
        return this._actionToButton.TryGetValue(action, out var button) ? button : null;
    }

    /// <summary>
    /// Maps a button to an action.
    /// </summary>
    public void MapButton(ControllerButton button, ControllerAction action)
    {
        this._buttonToAction[button] = action;
        this._actionToButton[action] = button;
    }

    /// <summary>
    /// Creates a ControllerConfig from a PadConfig loaded from the game's XML.
    /// </summary>
    public static ControllerConfig FromPadConfig(PadConfig padConfig)
    {
        var config = new ControllerConfig
        {
            DeviceGuid = padConfig.PadInfo.DeviceGuid
        };

        // Map directional inputs from cursor settings
        // PadAction values: 0-15 are buttons, 32+ are special (axes/POV)
        MapIfValid(config, padConfig.Action.CursorUp, ControllerAction.Up);
        MapIfValid(config, padConfig.Action.CursorDown, ControllerAction.Down);
        MapIfValid(config, padConfig.Action.CursorLeft, ControllerAction.Left);
        MapIfValid(config, padConfig.Action.CursorRight, ControllerAction.Right);

        // Map confirm/decline
        MapIfValid(config, padConfig.Action.Convenience, ControllerAction.Confirm);
        MapIfValid(config, padConfig.Action.Cancel, ControllerAction.Decline);

        // Add default directional fallbacks (POV hat and stick)
        // These will be checked if no specific cursor mapping matches
        config.MapButton(new ControllerButton(ControllerButton.PovUp), ControllerAction.Up);
        config.MapButton(new ControllerButton(ControllerButton.PovDown), ControllerAction.Down);
        config.MapButton(new ControllerButton(ControllerButton.PovLeft), ControllerAction.Left);
        config.MapButton(new ControllerButton(ControllerButton.PovRight), ControllerAction.Right);
        config.MapButton(new ControllerButton(ControllerButton.StickUp), ControllerAction.Up);
        config.MapButton(new ControllerButton(ControllerButton.StickDown), ControllerAction.Down);
        config.MapButton(new ControllerButton(ControllerButton.StickLeft), ControllerAction.Left);
        config.MapButton(new ControllerButton(ControllerButton.StickRight), ControllerAction.Right);

        // Bumpers/Triggers use default indices (not in PadConfig)
        config.MapButton(new ControllerButton(4), ControllerAction.BumperLeft);   // L1/LB
        config.MapButton(new ControllerButton(5), ControllerAction.BumperRight);  // R1/RB
        config.MapButton(new ControllerButton(6), ControllerAction.TriggerLeft);  // L2/LT
        config.MapButton(new ControllerButton(7), ControllerAction.TriggerRight); // R2/RT

        return config;
    }

    /// <summary>
    /// Creates default config for a controller based on its gamepad type.
    /// </summary>
    public static ControllerConfig CreateDefault(Controller controller)
    {
        var config = new ControllerConfig
        {
            DeviceGuid = controller.DirectInputInstanceGuid
        };

        // Standard Xbox-like layout defaults
        config.MapButton(new ControllerButton(0), ControllerAction.Confirm);   // A
        config.MapButton(new ControllerButton(1), ControllerAction.Decline);   // B
        config.MapButton(new ControllerButton(4), ControllerAction.BumperLeft);
        config.MapButton(new ControllerButton(5), ControllerAction.BumperRight);
        config.MapButton(new ControllerButton(6), ControllerAction.TriggerLeft);
        config.MapButton(new ControllerButton(7), ControllerAction.TriggerRight);

        // Directional inputs via POV and stick
        config.MapButton(new ControllerButton(ControllerButton.PovUp), ControllerAction.Up);
        config.MapButton(new ControllerButton(ControllerButton.PovDown), ControllerAction.Down);
        config.MapButton(new ControllerButton(ControllerButton.PovLeft), ControllerAction.Left);
        config.MapButton(new ControllerButton(ControllerButton.PovRight), ControllerAction.Right);
        config.MapButton(new ControllerButton(ControllerButton.StickUp), ControllerAction.Up);
        config.MapButton(new ControllerButton(ControllerButton.StickDown), ControllerAction.Down);
        config.MapButton(new ControllerButton(ControllerButton.StickLeft), ControllerAction.Left);
        config.MapButton(new ControllerButton(ControllerButton.StickRight), ControllerAction.Right);

        // Adjust for Nintendo controllers (B/A swap)
        if (IsNintendoController(controller.GamepadType))
        {
            config.MapButton(new ControllerButton(0), ControllerAction.Decline);  // B on Nintendo
            config.MapButton(new ControllerButton(1), ControllerAction.Confirm);  // A on Nintendo
        }

        return config;
    }

    private static void MapIfValid(ControllerConfig config, int buttonIndex, ControllerAction action)
    {
        // Only map standard button indices (0-15)
        if (buttonIndex is >= 0 and < 16)
        {
            config.MapButton(new ControllerButton(buttonIndex), action);
        }
    }

    private static bool IsNintendoController(SDL.GamepadType type)
    {
        return type is SDL.GamepadType.NintendoSwitchPro
            or SDL.GamepadType.NintendoSwitchJoyconLeft
            or SDL.GamepadType.NintendoSwitchJoyconRight
            or SDL.GamepadType.NintendoSwitchJoyconPair;
    }
}
