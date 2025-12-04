using System;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Avalonia.Services;
using Page = DQXLauncher.Avalonia.Pages.OnboardingFrame;

namespace DQXLauncher.Avalonia.ViewModels;

public partial class OnboardingFrameViewModel : ObservableObject
{
    [ObservableProperty] private Settings _settings;
    [ObservableProperty] private Type? _currentPage;

    public OnboardingFrameViewModel(Settings settings)
    {
        Settings = settings;
    }

    public Type? NextPage
    {
        get
        {
            if (Settings.GameFolderPath is null) return typeof(Page.SelectGameFolderPage);

            if (Settings.GameFolderPath is null)
            {
            }

            return null;
        }
    }
}