using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Avalonia.Services;

namespace DQXLauncher.Avalonia.ViewModels.OnboardingFrame;

public partial class GameFolderPageViewModel : OnboardingPageViewModel
{
    [ObservableProperty] private Settings _settings;

    public GameFolderPageViewModel(Settings settings)
    {
        this.Settings = settings;
    }

    public override string Title { get; } = "Game Location";

    [Required]
    [CustomValidation(typeof(GameFolderPageViewModel), "ValidateGameFolderPath")]
    public string? GameFolderPath
    {
        get => this.Settings.GameFolderPath;
        set => this.Settings.GameFolderPath = value;
    }
}