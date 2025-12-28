using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Core.Services;

namespace DQXLauncher.Avalonia.ViewModels.AppFrame;

public class AppFrameTabBase<TViewModel>(Lazy<TViewModel> viewModel, string icon)
{
    public Lazy<TViewModel> ViewModel { get; } = viewModel;
    public string Icon { get; } = icon;
}

public class AppFrameTab(Lazy<AppPageViewModel> viewModel, string icon)
    : AppFrameTabBase<AppPageViewModel>(viewModel, icon);

public partial class AppFrameViewModel(
    Settings settings,
    Lazy<HomePageViewModel> homePage,
    Lazy<SettingsPageViewModel> settingsPage
) : ObservableObject
{
    [ObservableProperty] private AppFrameTab? _selectedPage;
    [ObservableProperty] private Settings? _settings = settings;

    public List<AppFrameTab> Pages { get; } =
    [
        new(new Lazy<AppPageViewModel>(() => homePage.Value),
            "/Assets/Icons/solar--rocket-bold-duotone.svg"),

        new(new Lazy<AppPageViewModel>(() => settingsPage.Value),
            "/Assets/Icons/solar--settings-bold-duotone.svg")
    ];
}