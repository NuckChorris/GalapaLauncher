using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Galapa.Core.Services;

public partial class Settings : ObservableValidator
{
    [ObservableProperty] [Required] [CustomValidation(typeof(Settings), "ValidateGameFolderPath")]
    private string? _gameFolderPath;

    [ObservableProperty] [Required] private string? _saveFolderPath;

    [ObservableProperty] [Required] private bool? _errorReporting;

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
        if (File.Exists(Paths.Settings))
        {
            try
            {
                var json = File.ReadAllText(Paths.Settings);
                return JsonSerializer.Deserialize<Settings>(json) ?? GetDefaults();
            }
            catch (JsonException)
            {
                return GetDefaults();
            }
        }

        return GetDefaults();
    }

    public void Save()
    {
        File.WriteAllText(Paths.Settings, JsonSerializer.Serialize(this));
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