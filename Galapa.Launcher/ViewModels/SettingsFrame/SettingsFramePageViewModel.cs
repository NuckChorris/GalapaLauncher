using CommunityToolkit.Mvvm.ComponentModel;

namespace Galapa.Launcher.ViewModels.SettingsFrame;

/// <summary>
///     Base class for all settings page view models within the SettingsFrame.
/// </summary>
public abstract class SettingsFramePageViewModel : ObservableObject
{
    /// <summary>
    ///     Gets the title displayed for this settings page in the navigation.
    /// </summary>
    public abstract string Title { get; }

    /// <summary>
    ///     Gets the icon path for this settings page in the navigation.
    /// </summary>
    public abstract string Icon { get; }
}