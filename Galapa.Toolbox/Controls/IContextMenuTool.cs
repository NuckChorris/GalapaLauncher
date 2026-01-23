using System.Threading.Tasks;
using Avalonia.Controls;

namespace Galapa.Toolbox.Controls;

/// <summary>
///     A file tool that provides a context menu action.
/// </summary>
public interface IContextMenuTool : IFileTool
{
    /// <summary>
    ///     The display label shown in the context menu.
    /// </summary>
    string MenuLabel { get; }

    /// <summary>
    ///     Executes the tool's action on the given item.
    /// </summary>
    /// <param name="item">The file or folder item to act on.</param>
    /// <param name="topLevel">The top-level window for showing dialogs.</param>
    Task ExecuteAsync(FolderExplorerItem item, TopLevel topLevel);
}