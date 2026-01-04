using System.Diagnostics.CodeAnalysis;

namespace Galapa.Core.Configuration;

public static class Paths
{
    private static string? _appDataOverride;

    public static string AppData
    {
        get => _appDataOverride ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GalapaLauncher"
        );
        [param: AllowNull] set => _appDataOverride = value;
    }

    public static string Cache => Path.Combine(AppData, "Cache");
    public static string Settings => Path.Combine(AppData, "Settings.json");

    public static void Initialize()
    {
        Directory.CreateDirectory(AppData);
        Directory.CreateDirectory(Cache);
    }
}