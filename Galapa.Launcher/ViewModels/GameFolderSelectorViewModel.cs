using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Galapa.Core.Services;

namespace Galapa.Launcher.ViewModels;

public partial class GameFolderSelectorViewModel : ObservableObject
{
    private readonly Settings _settings;
    private readonly IStorageProvider _storage;

    [ObservableProperty] private string? _gameFolderPath;

    [ObservableProperty] private IReadOnlyCollection<ValidationResult> _errors = new List<ValidationResult>();

    public GameFolderSelectorViewModel(Settings settings, IStorageProvider storage)
    {
        _settings = settings;
        _storage = storage;
        GameFolderPath = settings.GameFolderPath;
        _settings.PropertyChanged += OnSettingsChanged;
    }

    // Sync settings changes from the LauncherSettings service into our local copy
    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is Settings settings && e.PropertyName == nameof(settings.GameFolderPath))
            GameFolderPath = settings.GameFolderPath;
    }
}