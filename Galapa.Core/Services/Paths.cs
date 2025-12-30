namespace Galapa.Core.Services;

public static class Paths
{
    public static string AppData => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GalapaLauncher"
    );

    public static string Cache => Path.Combine(AppData, "Cache");
    public static string Settings => Path.Combine(AppData, "Settings.json");

    public static void Initialize()
    {
        Directory.CreateDirectory(AppData);
        Directory.CreateDirectory(Cache);
    }
}