using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Galapa.Toolbox.Controls;

public partial class FolderExplorer : UserControl
{
    public static readonly StyledProperty<string?> RootPathProperty =
        AvaloniaProperty.Register<FolderExplorer, string?>(nameof(RootPath));

    public static readonly StyledProperty<IEnumerable<IFileTool>> ToolsProperty =
        AvaloniaProperty.Register<FolderExplorer, IEnumerable<IFileTool>>(
            nameof(Tools),
            Array.Empty<IFileTool>());

    public static readonly StyledProperty<IEnumerable<MenuSection>> MenuSectionsProperty =
        AvaloniaProperty.Register<FolderExplorer, IEnumerable<MenuSection>>(
            nameof(MenuSections),
            Array.Empty<MenuSection>());

    public FolderExplorer()
    {
        this.InitializeComponent();
    }

    /// <summary>
    ///     The root directory path to explore.
    /// </summary>
    public string? RootPath
    {
        get => this.GetValue(RootPathProperty);
        set => this.SetValue(RootPathProperty, value);
    }

    /// <summary>
    ///     All registered tools (including annotation tools).
    /// </summary>
    public IEnumerable<IFileTool> Tools
    {
        get => this.GetValue(ToolsProperty);
        set => this.SetValue(ToolsProperty, value);
    }

    /// <summary>
    ///     The menu layout definition. Each section is separated by a menu separator.
    /// </summary>
    public IEnumerable<MenuSection> MenuSections
    {
        get => this.GetValue(MenuSectionsProperty);
        set => this.SetValue(MenuSectionsProperty, value);
    }

    /// <summary>
    ///     The tree items. Populated from RootPath.
    /// </summary>
    public ObservableCollection<FolderExplorerItem> Items { get; } = new();

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == RootPathProperty) this.RefreshItems();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Recursively clear all loaded children before detaching to prevent visual tree errors
        foreach (var item in this.Items) item.ClearLoadedChildren();

        this.Items.Clear();
        base.OnDetachedFromVisualTree(e);
    }

    private void RefreshItems()
    {
        this.Items.Clear();
        if (!string.IsNullOrEmpty(this.RootPath) && Directory.Exists(this.RootPath))
        {
            var rootItem = this.CreateItem(this.RootPath);
            this.Items.Add(rootItem);
        }
    }

    private FolderExplorerItem CreateItem(string path)
    {
        var item = new FolderExplorerItem
        {
            Path = path,
            ItemFactory = this.CreateItem // Pass factory so children also get annotations
        };
        this.ApplyAnnotations(item);
        return item;
    }

    private void ApplyAnnotations(FolderExplorerItem item)
    {
        foreach (var tool in this.Tools.OfType<IFileAnnotationTool>())
            if (tool.CanHandle(item))
            {
                var annotation = tool.GetAnnotation(item);
                if (!string.IsNullOrEmpty(annotation))
                {
                    item.Annotation = annotation;
                    break; // First matching annotation wins
                }
            }
    }

    private void OnContextMenuOpening(object? sender, CancelEventArgs e)
    {
        if (sender is not ContextMenu contextMenu) return;

        // Get the selected item from the TreeView
        if (this.FileTree.SelectedItem is not FolderExplorerItem item)
        {
            e.Cancel = true;
            return;
        }

        // Ensure annotations are applied (for lazily-loaded children)
        if (item.Annotation == null) this.ApplyAnnotations(item);

        contextMenu.Items.Clear();

        var isFirstSection = true;
        foreach (var section in this.MenuSections)
        {
            var applicableTools = section.Tools
                .Where(t => t.CanHandle(item))
                .ToList();

            if (applicableTools.Count == 0) continue;

            // Add separator between sections (not before the first)
            if (!isFirstSection) contextMenu.Items.Add(new Separator());

            isFirstSection = false;

            foreach (var tool in applicableTools)
            {
                var menuItem = new MenuItem
                {
                    Header = tool.MenuLabel,
                    Tag = (tool, item)
                };
                menuItem.Click += this.OnToolMenuItemClick;
                contextMenu.Items.Add(menuItem);
            }
        }

        // Cancel if no items
        if (contextMenu.Items.Count == 0) e.Cancel = true;
    }

    private async void OnToolMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: (IContextMenuTool tool, FolderExplorerItem item) }) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        await tool.ExecuteAsync(item, topLevel);
    }
}