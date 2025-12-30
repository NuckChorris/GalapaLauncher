using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Galapa.Toolbox.Models;
using Microsoft.UI.Xaml.Controls;

namespace Galapa.Toolbox.ViewModels;

public class SaveExplorerItem : ObservableObject
{
    public required string Path { get; set; }
    public string Name => System.IO.Path.GetFileName(Path);
    public bool IsDirectory => Directory.Exists(Path);
    public KnownFile? FileInfo => KnownFile.KnownFiles.TryGetValue(Name, out var file) ? file : null;
    public bool IsKnownFile => FileInfo != null;
    public string? DeobfuscatedName => FileInfo?.Name;

    public ObservableCollection<SaveExplorerItem>? Children
    {
        get
        {
            if (!IsDirectory) return null;
            return new(Directory.GetFileSystemEntries(Path)
                .Select(x => new SaveExplorerItem { Path = x }).ToList());
        }
    }

    public MenuFlyout ContextMenu
    {
        get
        {
            var flyout = new MenuFlyout();
            flyout.Items.Add(new MenuFlyoutItem { Text = "Export deobfuscated" });

            return flyout;
        }
    }
}