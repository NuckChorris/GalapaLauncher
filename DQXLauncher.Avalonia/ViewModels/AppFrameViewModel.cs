using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Avalonia.Services;
using DQXLauncher.Avalonia.ViewModels.Pages.App;

namespace DQXLauncher.Avalonia.ViewModels;

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
    public List<AppFrameTab> Pages { get; } = new()
    {
        new AppFrameTab(new Lazy<AppPageViewModel>(() => homePage.Value),
            "../Assets/Icons/solar--rocket-bold-duotone.svg"),
        new AppFrameTab(new Lazy<AppPageViewModel>(() => settingsPage.Value),
            "../Assets/Icons/solar--settings-bold-duotone.svg")
    };

    [ObservableProperty] private AppFrameTab? _selectedPage;
    [ObservableProperty] private Settings? _settings = settings;
}