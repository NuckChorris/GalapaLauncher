using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace DQXLauncher.Avalonia.Services;

public static class InstallInfo
{
    private static readonly string RegistryPrefix =
        "HKEY_LOCAL_MACHINE\\Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\";

    private static readonly Dictionary<string, string> ExpacIds = new()
    {
        { "1.0", "{300DCC8E-BE61-4FB5-B9D8-FDA19E3AAA38}" },
        { "2.0", "{4FD779A0-9CAE-4A36-A33E-EB01DA36537E}" },
        { "3.0", "{1D79B85A-17B7-40E0-94ED-791572CB082E}" },
        { "4.0", "{B6A99A93-03DB-49EB-8F04-78AA22D571EC}" },
        { "5.0", "{5B536B7A-9189-4908-AF6D-2702E23C3C67}" },
        { "6.0", "{D6C2F5CC-F6F9-45BF-B83B-B28825E74855}" },
        { "7.0", "{4FC73F71-D454-409C-8ADC-85AC0E10F35F}" }
    };

    private static string? GetLocation()
    {
        string? gameExe = null;
        foreach (var kv in ExpacIds)
            if (Registry.GetValue($"{RegistryPrefix}{kv.Value}", "DisplayIcon", null) is string iconFile)
            {
                gameExe = iconFile[..iconFile.LastIndexOf(',')];
                break;
            }

        if (Path.GetDirectoryName(gameExe) is { } gameDir) return Directory.GetParent(gameDir)?.FullName;

        return null;
    }

    private static bool IsInstalled(string version)
    {
        if (Registry.GetValue($"{RegistryPrefix}{ExpacIds[version]}", "DisplayIcon", null) is string) return true;

        return false;
    }

    /// <summary>
    ///     The installation root of the game, parent directory of Boot and Game directories.
    /// </summary>
    /// <remarks>
    ///     This checks the registry for each expansion, and returns the installation location of the first expansion that
    ///     is installed. If no expansions are found, this will return null.
    /// </remarks>
    public static string? Location => GetLocation();

    public static bool HasVer1 => IsInstalled("1.0");
    public static bool HasVer2 => IsInstalled("2.0");
    public static bool HasVer3 => IsInstalled("3.0");
    public static bool HasVer4 => IsInstalled("4.0");
    public static bool HasVer5 => IsInstalled("5.0");
    public static bool HasVer6 => IsInstalled("6.0");
    public static bool HasVer7 => IsInstalled("7.0");
}