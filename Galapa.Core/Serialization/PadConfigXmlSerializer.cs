using System.Buffers.Binary;
using System.Xml.Linq;
using Galapa.Core.Models;

namespace Galapa.Core.Serialization;

/// <summary>
///     Handles XML serialization and deserialization for Dragon Quest X gamepad configuration files.
/// </summary>
public static class PadConfigXmlSerializer
{
    /// <summary>
    ///     Loads a PadConfig from a Dragon Quest X PAD_CONFIG XML file.
    /// </summary>
    public static PadConfig Load(string xmlPath)
    {
        var doc = XDocument.Load(xmlPath);
        var padConfigElement = doc.Root?.Element("PAD_CONFIG")
                               ?? throw new InvalidOperationException("Missing PAD_CONFIG element");

        return new PadConfig
        {
            PadInfo = LoadPadInfo(padConfigElement.Element("PAD_INFO")
                                  ?? throw new InvalidOperationException("Missing PAD_INFO element")),
            Action = LoadPadAction(padConfigElement.Element("ACTION")
                                   ?? throw new InvalidOperationException("Missing ACTION element")),
            ButtonCaption = LoadPadButtonCaption(padConfigElement.Element("PadButtonCaption")
                                                 ?? throw new InvalidOperationException(
                                                     "Missing PadButtonCaption element"))
        };
    }

    /// <summary>
    ///     Saves a PadConfig to a Dragon Quest X PAD_CONFIG XML file.
    /// </summary>
    public static void Save(PadConfig config, string xmlPath)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement("DragonQuestX",
                new XElement("PAD_CONFIG",
                    SavePadInfo(config.PadInfo),
                    SavePadAction(config.Action),
                    SavePadButtonCaption(config.ButtonCaption)
                )
            )
        );

        doc.Save(xmlPath);
    }

    private static PadInfo LoadPadInfo(XElement element)
    {
        // Read big-endian GUID components from XML
        var instance1 = GetParamInt(element, "PadGUIDInstance1");
        var instance2 = GetParamInt(element, "PadGUIDInstance2");
        var instance3 = GetParamInt(element, "PadGUIDInstance3");
        var instance4 = GetParamInt(element, "PadGUIDInstance4");
        var instance5 = GetParamInt(element, "PadGUIDInstance5");

        // Convert from big-endian integers to .NET GUID byte layout (RFC 4122)
        // The XML stores BE integers, but GUID expects LE for first 3 components
        Span<byte> beBytes = stackalloc byte[16];
        BinaryPrimitives.WriteInt32BigEndian(beBytes[..4], instance1);
        BinaryPrimitives.WriteUInt16BigEndian(beBytes[4..6], (ushort)instance2);
        BinaryPrimitives.WriteUInt16BigEndian(beBytes[6..8], (ushort)instance3);
        BinaryPrimitives.WriteInt32BigEndian(beBytes[8..12], instance4);
        BinaryPrimitives.WriteInt32BigEndian(beBytes[12..16], instance5);

        // Convert to GUID byte layout (LE for first 3 components, BE for last 2)
        Span<byte> guidBytes = stackalloc byte[16];
        BinaryPrimitives.WriteInt32LittleEndian(guidBytes[..4], BinaryPrimitives.ReadInt32BigEndian(beBytes[..4]));
        BinaryPrimitives.WriteUInt16LittleEndian(guidBytes[4..6], BinaryPrimitives.ReadUInt16BigEndian(beBytes[4..6]));
        BinaryPrimitives.WriteUInt16LittleEndian(guidBytes[6..8], BinaryPrimitives.ReadUInt16BigEndian(beBytes[6..8]));
        beBytes[8..16].CopyTo(guidBytes[8..16]); // Last 8 bytes stay as-is

        return new PadInfo
        {
            DeviceGuid = new Guid(guidBytes),
            SpecialDecideType = GetParamInt(element, "SpecialDecideType"),
            PadNonActive = GetParamInt(element, "PadNonActive"),
            PadBias = GetParamInt(element, "PadBias"),
            ButtonCaptionType = GetParamInt(element, "ButtonCaptionType"),
            PadPresetType = GetParamString(element, "PadPresetType")
        };
    }

    private static XElement SavePadInfo(PadInfo info)
    {
        // Convert GUID to big-endian components (struct layout: BE s32, BE u16, BE u16, BE s32, BE s32)
        // .NET GUID byte layout is RFC 4122: the string "1A6258A0-9C8D-11F0-8005-444553540000"
        // writes as bytes [A0, 58, 62, 1A, 8D, 9C, F0, 11, 80, 05, 44, 45, 53, 54, 00, 00]
        // But we need to interpret this as BE integers for the XML format
        Span<byte> guidBytes = stackalloc byte[16];
        info.DeviceGuid.TryWriteBytes(guidBytes);

        // Reinterpret the bytes as big-endian integers by swapping byte order
        Span<byte> beBytes = stackalloc byte[16];
        BinaryPrimitives.WriteInt32BigEndian(beBytes[..4], BinaryPrimitives.ReadInt32LittleEndian(guidBytes[..4]));
        BinaryPrimitives.WriteUInt16BigEndian(beBytes[4..6], BinaryPrimitives.ReadUInt16LittleEndian(guidBytes[4..6]));
        BinaryPrimitives.WriteUInt16BigEndian(beBytes[6..8], BinaryPrimitives.ReadUInt16LittleEndian(guidBytes[6..8]));
        guidBytes[8..16].CopyTo(beBytes[8..16]); // Last 8 bytes stay as-is

        var instance1 = BinaryPrimitives.ReadInt32BigEndian(beBytes[..4]);
        int instance2 = BinaryPrimitives.ReadUInt16BigEndian(beBytes[4..6]);
        int instance3 = BinaryPrimitives.ReadUInt16BigEndian(beBytes[6..8]);
        var instance4 = BinaryPrimitives.ReadInt32BigEndian(beBytes[8..12]);
        var instance5 = BinaryPrimitives.ReadInt32BigEndian(beBytes[12..16]);

        return new XElement("PAD_INFO",
            CreateParam("PadGUIDInstance1", instance1),
            CreateParam("PadGUIDInstance2", instance2),
            CreateParam("PadGUIDInstance3", instance3),
            CreateParam("PadGUIDInstance4", instance4),
            CreateParam("PadGUIDInstance5", instance5),
            CreateParam("SpecialDecideType", info.SpecialDecideType),
            CreateParam("PadNonActive", info.PadNonActive),
            CreateParam("PadBias", info.PadBias),
            CreateParam("ButtonCaptionType", info.ButtonCaptionType),
            CreateParam("PadPresetType", info.PadPresetType)
        );
    }

    private static PadAction LoadPadAction(XElement element)
    {
        return new PadAction
        {
            Convenience = GetParamInt(element, "CONVENIENCE"),
            Cancel = GetParamInt(element, "CANCEL"),
            AutoRun = GetParamInt(element, "AUTO_RUN"),
            Jump = GetParamInt(element, "JUMP"),
            CameraAndModelRotClockwise = GetParamInt(element, "CAMERA_AND_MODEL_ROT_CLOCKWISE"),
            CameraAndModelRotAntiClockwise = GetParamInt(element, "CAMERA_AND_MODEL_ROT_ANTI_CLOCKWISE"),
            Menu = GetParamInt(element, "MENU"),
            Map = GetParamInt(element, "MAP"),
            Communication = GetParamInt(element, "COMMUNICATION"),
            CameraBehind = GetParamInt(element, "CAMERA_BEHIND"),
            CursorUp = GetParamInt(element, "CURSOR_UP"),
            CursorDown = GetParamInt(element, "CURSOR_DOWN"),
            CursorLeft = GetParamInt(element, "CURSOR_LEFT"),
            CursorRight = GetParamInt(element, "CURSOR_RIGHT"),
            CameraUp = GetParamInt(element, "CAMERA_UP"),
            CameraDown = GetParamInt(element, "CAMERA_DOWN"),
            CameraLeft = GetParamInt(element, "CAMERA_LEFT"),
            CameraRight = GetParamInt(element, "CAMERA_RIGHT"),
            MoveForward = GetParamInt(element, "MOVE_FORWARD"),
            MoveBack = GetParamInt(element, "MOVE_BACK"),
            MoveLeft = GetParamInt(element, "MOVE_LEFT"),
            MoveRight = GetParamInt(element, "MOVE_RIGHT")
        };
    }

    private static XElement SavePadAction(PadAction action)
    {
        return new XElement("ACTION",
            CreateParam("CONVENIENCE", action.Convenience),
            CreateParam("CANCEL", action.Cancel),
            CreateParam("AUTO_RUN", action.AutoRun),
            CreateParam("JUMP", action.Jump),
            CreateParam("CAMERA_AND_MODEL_ROT_CLOCKWISE", action.CameraAndModelRotClockwise),
            CreateParam("CAMERA_AND_MODEL_ROT_ANTI_CLOCKWISE", action.CameraAndModelRotAntiClockwise),
            CreateParam("MENU", action.Menu),
            CreateParam("MAP", action.Map),
            CreateParam("COMMUNICATION", action.Communication),
            CreateParam("CAMERA_BEHIND", action.CameraBehind),
            CreateParam("CURSOR_UP", action.CursorUp),
            CreateParam("CURSOR_DOWN", action.CursorDown),
            CreateParam("CURSOR_LEFT", action.CursorLeft),
            CreateParam("CURSOR_RIGHT", action.CursorRight),
            CreateParam("CAMERA_UP", action.CameraUp),
            CreateParam("CAMERA_DOWN", action.CameraDown),
            CreateParam("CAMERA_LEFT", action.CameraLeft),
            CreateParam("CAMERA_RIGHT", action.CameraRight),
            CreateParam("MOVE_FORWARD", action.MoveForward),
            CreateParam("MOVE_BACK", action.MoveBack),
            CreateParam("MOVE_LEFT", action.MoveLeft),
            CreateParam("MOVE_RIGHT", action.MoveRight)
        );
    }

    private static PadButtonCaption LoadPadButtonCaption(XElement element)
    {
        return new PadButtonCaption
        {
            Button0 = GetParamInt(element, "Button0"),
            Button1 = GetParamInt(element, "Button1"),
            Button2 = GetParamInt(element, "Button2"),
            Button3 = GetParamInt(element, "Button3"),
            Button4 = GetParamInt(element, "Button4"),
            Button5 = GetParamInt(element, "Button5"),
            Button6 = GetParamInt(element, "Button6"),
            Button7 = GetParamInt(element, "Button7"),
            Button8 = GetParamInt(element, "Button8"),
            Button9 = GetParamInt(element, "Button9"),
            Button10 = GetParamInt(element, "Button10"),
            Button11 = GetParamInt(element, "Button11"),
            Button12 = GetParamInt(element, "Button12"),
            Button13 = GetParamInt(element, "Button13"),
            Button14 = GetParamInt(element, "Button14"),
            Button15 = GetParamInt(element, "Button15")
        };
    }

    private static XElement SavePadButtonCaption(PadButtonCaption caption)
    {
        return new XElement("PadButtonCaption",
            CreateParam("Button0", caption.Button0),
            CreateParam("Button1", caption.Button1),
            CreateParam("Button2", caption.Button2),
            CreateParam("Button3", caption.Button3),
            CreateParam("Button4", caption.Button4),
            CreateParam("Button5", caption.Button5),
            CreateParam("Button6", caption.Button6),
            CreateParam("Button7", caption.Button7),
            CreateParam("Button8", caption.Button8),
            CreateParam("Button9", caption.Button9),
            CreateParam("Button10", caption.Button10),
            CreateParam("Button11", caption.Button11),
            CreateParam("Button12", caption.Button12),
            CreateParam("Button13", caption.Button13),
            CreateParam("Button14", caption.Button14),
            CreateParam("Button15", caption.Button15)
        );
    }

    private static int GetParamInt(XElement parent, string name)
    {
        return int.Parse(parent.Elements("param")
            .First(e => e.Attribute("name")?.Value == name)
            .Attribute("value")?.Value ?? "0");
    }

    private static string GetParamString(XElement parent, string name)
    {
        return parent.Elements("param")
            .First(e => e.Attribute("name")?.Value == name)
            .Attribute("value")?.Value ?? string.Empty;
    }

    private static XElement CreateParam(string name, object value)
    {
        return new XElement("param", new XAttribute("name", name), new XAttribute("value", value));
    }
}