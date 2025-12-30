using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Galapa.Toolbox.Services;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace Galapa.Toolbox.Views;

public sealed partial class SettingsPage
{
    public Settings SettingsModel { get; }

    public SettingsPage()
    {
        this.InitializeComponent();
        SettingsModel = Settings.Instance;
    }

    private async void OnGameFolderBrowseClick(object sender, RoutedEventArgs e)
    {
        var folderPath = await OpenFolderPickerAsync();
        if (!string.IsNullOrEmpty(folderPath))
        {
            SettingsModel.GameFolderPath = folderPath;
            SettingsModel.Save();
            Bindings.Update();
        }
    }

    private async void OnSaveFolderBrowseClick(object sender, RoutedEventArgs e)
    {
        var folderPath = await OpenFolderPickerAsync();
        if (!string.IsNullOrEmpty(folderPath))
        {
            SettingsModel.SaveFolderPath = folderPath;
            SettingsModel.Save();
            Bindings.Update();
        }
    }

    private async Task<string> OpenFolderPickerAsync()
    {
        var picker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            FileTypeFilter = { "*" }
        };

        var hwnd = WindowNative.GetWindowHandle(App.AppWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }
}