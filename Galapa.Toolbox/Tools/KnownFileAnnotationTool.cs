using System.IO;
using Galapa.Toolbox.Controls;
using Galapa.Toolbox.Models;

namespace Galapa.Toolbox.Tools;

/// <summary>
///     Provides filename annotations for known DQX save files.
/// </summary>
public class KnownFileAnnotationTool : IFileAnnotationTool
{
    public bool CanHandle(FolderExplorerItem item)
    {
        if (item.IsDirectory) return false;
        var fileName = Path.GetFileName(item.Path);
        return KnownFile.KnownFiles.ContainsKey(fileName);
    }

    public string? GetAnnotation(FolderExplorerItem item)
    {
        var fileName = Path.GetFileName(item.Path);
        if (KnownFile.KnownFiles.TryGetValue(fileName, out var knownFile))
        {
            // Store the KnownFile in the tag for use by ExportKnownFileTool
            item.Tag = knownFile;
            return knownFile.Name;
        }

        return null;
    }
}