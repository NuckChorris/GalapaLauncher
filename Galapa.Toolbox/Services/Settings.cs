using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Galapa.Toolbox.Services;

public partial class Settings : ObservableObject
{
    private static Settings? _instance;
    public static Settings Instance => _instance ??= Load();

    [ObservableProperty] public string gameFolderPath = "C:\\";

    [ObservableProperty] public string saveFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "My Games", "Dragon Quest X");

    private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GalapaToolbox", "settings.json");

    [JsonConstructor]
    private Settings()
    {
    }

    public static Settings Load()
    {
        if (File.Exists(SettingsPath))
        {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }

        return new Settings();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this));
    }
}