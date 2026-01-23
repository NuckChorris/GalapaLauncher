using System;
using System.Collections.Generic;
using System.IO;
using Galapa.Core.StreamObfuscator;

namespace Galapa.Toolbox.Models;

public class KnownFile
{
    public static readonly Dictionary<string, KnownFile> KnownFiles = new()
    {
        { "BurakqOnn!pcs--!qca", new KnownFile { Name = "PresetPad.win32.xml", Obfuscator = FixedObfuscator.Factory } },
        {
            "ChqstfVjmkw!zmi--!vmh",
            new KnownFile { Name = "PresetMouse.win32.xml", Obfuscator = FixedObfuscator.Factory }
        },
        {
            "MghxsiOmzymxff!wvo$$!bvz",
            new KnownFile { Name = "PresetKeyboard.win32.xml", Obfuscator = FixedObfuscator.Factory }
        },
        { "FbrBrpgbkOhwj!ouc", new KnownFile { Name = "KeyConfigFile.xml", Obfuscator = FixedObfuscator.Factory } },
        { "cxjYxsgheGzie!iyx", new KnownFile { Name = "dqxPlayerList.xml", Obfuscator = UsernameObfuscator.Factory } },
        {
            "NeaRxpzuwPlieoha!oub]#!xui",
            new KnownFile { Name = "PadButtonCaption.win32.xml", Obfuscator = FixedObfuscator.Factory }
        },
        {
            "YzjadjylzqvBmsvub!zni]]!vnh",
            new KnownFile { Name = "EnvironmentOption.win32.xml", Obfuscator = FixedObfuscator.Factory }
        },
        {
            "shpkoLhdcPvwNmiitivstexqdlrcowjlXvnxry!dfj@@!qvr",
            new KnownFile
                { Name = "eventTextEtcWindowsgamepadmousesClient.win32.etp", Obfuscator = FixedObfuscator.Factory }
        },
        {
            "aufjzAunbNycMdhhkhyczqfhgxmkyhSourjz!bjx$$!kop",
            new KnownFile
                { Name = "eventTextEtcWindowsconfigtoolsClient.win32.etp", Obfuscator = FixedObfuscator.Factory }
        }
    };

    public required string Name;
    public required Func<Stream, Stream> Obfuscator;
}