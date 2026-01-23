using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Galapa.Toolbox.Controls;

/// <summary>
///     Represents a file or directory item in the FolderExplorer tree.
/// </summary>
public partial class FolderExplorerItem : ObservableObject
{
    private ObservableCollection<FolderExplorerItem>? _children;
    private bool _childrenLoaded;

    /// <summary>
    ///     The full path to the file or directory.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    ///     Factory function for creating child items. Set by FolderExplorer.
    /// </summary>
    public Func<string, FolderExplorerItem>? ItemFactory { get; init; }

    /// <summary>
    ///     The file or directory name (without parent path).
    /// </summary>
    public string Name => System.IO.Path.GetFileName(this.Path);

    /// <summary>
    ///     True if this item represents a directory.
    /// </summary>
    public bool IsDirectory => Directory.Exists(this.Path);

    /// <summary>
    ///     Secondary annotation text to display (e.g., deobfuscated filename).
    ///     Set by IFileAnnotationTool implementations.
    /// </summary>
    [ObservableProperty] private string? _annotation;

    /// <summary>
    ///     Custom tag data that tools can attach to items.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    ///     Child items for directories. Returns null for files.
    ///     Cached after first access.
    /// </summary>
    public ObservableCollection<FolderExplorerItem>? Children
    {
        get
        {
            if (!this.IsDirectory) return null;

            if (!this._childrenLoaded)
            {
                this._childrenLoaded = true;
                var factory = this.ItemFactory ?? (path => new FolderExplorerItem { Path = path });
                this._children = new ObservableCollection<FolderExplorerItem>(Directory.GetFileSystemEntries(this.Path)
                    .Select(factory)
                    .ToList());
            }

            return this._children;
        }
    }
}