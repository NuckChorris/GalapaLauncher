using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Galapa.Core.Configuration;
using Page = Galapa.Launcher.Views.OnboardingFrame;

namespace Galapa.Launcher.ViewModels.OnboardingFrame;

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
            if (this.Settings.GameFolderPath is null) return typeof(SelectGameFolderPage);

            if (this.Settings.GameFolderPath is null)
            {
            }

            return null;
        }
    }
}