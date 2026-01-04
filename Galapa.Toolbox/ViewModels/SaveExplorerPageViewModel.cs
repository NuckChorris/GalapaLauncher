using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Galapa.Toolbox.Services;

namespace Galapa.Toolbox.ViewModels;

public class SaveExplorerPageViewModel : ObservableObject
{
    public ObservableCollection<SaveExplorerItem> Items { get; } =
        [new() { Path = Settings.Instance.SaveFolderPath }];
}