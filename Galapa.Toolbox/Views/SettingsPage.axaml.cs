using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Galapa.Toolbox.Services;

namespace Galapa.Toolbox.Views;

public partial class SettingsPage : UserControl
{
    public Settings SettingsModel { get; }

    public SettingsPage()
    {
        this.InitializeComponent();
        this.SettingsModel = Settings.Instance;
        this.DataContext = this.SettingsModel;
    }

    private async void OnGameFolderBrowseClick(object? sender, RoutedEventArgs e)
    {
        var folderPath = await this.OpenFolderPickerAsync();
        if (!string.IsNullOrEmpty(folderPath))
        {
            this.SettingsModel.GameFolderPath = folderPath;
            this.SettingsModel.Save();
        }
    }

    private async void OnSaveFolderBrowseClick(object? sender, RoutedEventArgs e)
    {
        var folderPath = await this.OpenFolderPickerAsync();
        if (!string.IsNullOrEmpty(folderPath))
        {
            this.SettingsModel.SaveFolderPath = folderPath;
            this.SettingsModel.Save();
        }
    }

    private async Task<string?> OpenFolderPickerAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return null;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folder",
            AllowMultiple = false
        });

        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }
}