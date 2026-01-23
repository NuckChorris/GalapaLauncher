namespace Galapa.Toolbox.Controls;

/// <summary>
///     A file tool that provides filename annotations (secondary text display).
/// </summary>
public interface IFileAnnotationTool : IFileTool
{
    /// <summary>
    ///     Gets the annotation text to display for the given item.
    /// </summary>
    /// <param name="item">The file or folder item to annotate.</param>
    /// <returns>The annotation text, or null if no annotation should be displayed.</returns>
    string? GetAnnotation(FolderExplorerItem item);
}