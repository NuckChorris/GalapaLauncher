using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Avalonia.Services;

namespace DQXLauncher.Avalonia.ViewModels.Pages.Onboarding;

public partial class GameFolderPageViewModel : OnboardingPageViewModel
{
    public override string Title { get; } = "Game Location";

    [ObservableProperty] private Settings _settings;

    public GameFolderPageViewModel(Settings settings)
    {
        Settings = settings;
    }

    [Required]
    [CustomValidation(typeof(GameFolderPageViewModel), "ValidateGameFolderPath")]
    public string? GameFolderPath
    {
        get => Settings.GameFolderPath;
        set => Settings.GameFolderPath = value;
    }
}