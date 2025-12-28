using System;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Core.Services;
using Page = DQXLauncher.Avalonia.Views.OnboardingFrame;

namespace DQXLauncher.Avalonia.ViewModels.OnboardingFrame;

public partial class OnboardingFrameViewModel : ObservableObject
{
    [ObservableProperty] private Type? _currentPage;
    [ObservableProperty] private Settings _settings;

    public OnboardingFrameViewModel(Settings settings)
    {
        this.Settings = settings;
    }

    public Type? NextPage
    {
        get
        {
            if (this.Settings.GameFolderPath is null) return typeof(Page.SelectGameFolderPage);

            if (this.Settings.GameFolderPath is null)
            {
            }

            return null;
        }
    }
}