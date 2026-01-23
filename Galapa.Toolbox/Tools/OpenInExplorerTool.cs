using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Galapa.Toolbox.Controls;

namespace Galapa.Toolbox.Tools;

/// <summary>
///     Opens the file or folder location in Windows Explorer.
/// </summary>
public class OpenInExplorerTool : IContextMenuTool
{
    public string MenuLabel => "Open in Explorer";

    public bool CanHandle(FolderExplorerItem item)
    {
        return true;
    }

    public Task ExecuteAsync(FolderExplorerItem item, TopLevel topLevel)
    {
        Process.Start("explorer.exe", $"/select,\"{item.Path}\"");
        return Task.CompletedTask;
    }
}