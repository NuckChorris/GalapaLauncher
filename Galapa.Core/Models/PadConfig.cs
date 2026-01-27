namespace Galapa.Core.Models;

/// <summary>
///     Root configuration for a gamepad device in Dragon Quest X.
/// </summary>
public record PadConfig
{
    public required PadInfo PadInfo { get; set; }
    public required PadAction Action { get; set; }
    public required PadButtonCaption ButtonCaption { get; set; }
}

/// <summary>
///     Device identification and configuration metadata.
/// </summary>
public record PadInfo
{
    /// <summary>
    ///     DirectInput device GUID.
    /// </summary>
    public required Guid DeviceGuid { get; set; }

    /// <summary>
    ///     Special decision type (0 = default).
    /// </summary>
    public int SpecialDecideType { get; set; }

    /// <summary>
    ///     Whether the pad is inactive (1 = inactive, 0 = active).
    /// </summary>
    public int PadNonActive { get; set; }

    /// <summary>
    ///     Deadzone/sensitivity bias (0-100, default 50).
    /// </summary>
    public int PadBias { get; set; }

    /// <summary>
    ///     Button caption display type (0 = Xbox style, 3 = Switch style).
    /// </summary>
    public int ButtonCaptionType { get; set; }

    /// <summary>
    ///     Preset type identifier (e.g., "PadPresetTypeXBox", "CustomSetting").
    /// </summary>
    public required string PadPresetType { get; set; }
}

/// <summary>
///     Maps game actions to DirectInput button/axis indices.
///     Values represent DirectInput indices: 0-15 (buttons), 32-47 (axes/POV).
/// </summary>
public record PadAction
{
    public int Convenience { get; set; }
    public int Cancel { get; set; }
    public int AutoRun { get; set; }
    public int Jump { get; set; }
    public int CameraAndModelRotClockwise { get; set; }
    public int CameraAndModelRotAntiClockwise { get; set; }
    public int Menu { get; set; }
    public int Map { get; set; }
    public int Communication { get; set; }
    public int CameraBehind { get; set; }
    public int CursorUp { get; set; }
    public int CursorDown { get; set; }
    public int CursorLeft { get; set; }
    public int CursorRight { get; set; }
    public int CameraUp { get; set; }
    public int CameraDown { get; set; }
    public int CameraLeft { get; set; }
    public int CameraRight { get; set; }
    public int MoveForward { get; set; }
    public int MoveBack { get; set; }
    public int MoveLeft { get; set; }
    public int MoveRight { get; set; }
}

/// <summary>
///     Display mappings for physical button labels (for UI representation).
///     Values 0-15 represent button indices on the physical controller.
/// </summary>
public record PadButtonCaption
{
    public int Button0 { get; set; }
    public int Button1 { get; set; }
    public int Button2 { get; set; }
    public int Button3 { get; set; }
    public int Button4 { get; set; }
    public int Button5 { get; set; }
    public int Button6 { get; set; }
    public int Button7 { get; set; }
    public int Button8 { get; set; }
    public int Button9 { get; set; }
    public int Button10 { get; set; }
    public int Button11 { get; set; }
    public int Button12 { get; set; }
    public int Button13 { get; set; }
    public int Button14 { get; set; }
    public int Button15 { get; set; }
}