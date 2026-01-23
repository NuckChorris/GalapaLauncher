namespace Galapa.Toolbox.Controls;

/// <summary>
///     Base interface for all file tools that can be applied to items in the FolderExplorer.
/// </summary>
public interface IFileTool
{
    /// <summary>
    ///     Determines if this tool applies to the given item.
    /// </summary>
    /// <param name="item">The file or folder item to check.</param>
    /// <returns>True if this tool can handle the item.</returns>
    bool CanHandle(FolderExplorerItem item);
}