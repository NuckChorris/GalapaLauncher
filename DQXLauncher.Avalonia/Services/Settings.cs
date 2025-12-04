using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Core.Services;

namespace DQXLauncher.Avalonia.Services;

public partial class Settings : ObservableValidator
{
    [ObservableProperty] [Required] [CustomValidation(typeof(Settings), "ValidateGameFolderPath")]
    private string? _gameFolderPath;

    [ObservableProperty] [Required] private string? _saveFolderPath;

    [ObservableProperty] [Required] private bool? _errorReporting;

    private static readonly string SettingsPath = Path.Combine(Paths.AppData, "Settings.json");

    private static Settings GetDefaults()
    {
        return new Settings
        {
            GameFolderPath = InstallInfo.Location,
            SaveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "My Games", "Dragon Quest X"),
            ErrorReporting = false
        };
    }

    public static Settings Load()
    {
        if (File.Exists(SettingsPath))
        {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<Settings>(json) ?? GetDefaults();
        }

        return GetDefaults();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this));
    }

    public static ValidationResult ValidateGameFolderPath(string gameFolderPath, ValidationContext context)
    {
        if (!Directory.Exists(gameFolderPath))
            return new ValidationResult("Folder does not exist");
        if (!File.Exists(Path.Combine(gameFolderPath, "Game\\DQXGame.exe")))
            return new ValidationResult("DQXGame.exe does not exist");

        return ValidationResult.Success!;
    }
}