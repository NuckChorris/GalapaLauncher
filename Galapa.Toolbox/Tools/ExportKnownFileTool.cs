using System;
using System.IO;
using Galapa.Core.StreamObfuscator;
using Galapa.Toolbox.Controls;
using Galapa.Toolbox.Models;

namespace Galapa.Toolbox.Tools;

/// <summary>
///     Exports known DQX save files using their native obfuscator.
/// </summary>
public class ExportKnownFileTool : ExportToolBase
{
    public override string MenuLabel => "Export deobfuscated";

    public override bool CanHandle(FolderExplorerItem item)
    {
        if (item.IsDirectory) return false;
        var fileName = Path.GetFileName(item.Path);
        return KnownFile.KnownFiles.ContainsKey(fileName);
    }

    protected override Func<Stream, Stream> GetObfuscator(FolderExplorerItem item)
    {
        // Try to get from tag (set by annotation tool), otherwise look up
        if (item.Tag is KnownFile knownFile) return knownFile.Obfuscator;

        var fileName = Path.GetFileName(item.Path);
        if (KnownFile.KnownFiles.TryGetValue(fileName, out var kf)) return kf.Obfuscator;

        // Fallback (shouldn't happen if CanHandle is correct)
        return FixedObfuscator.Factory;
    }

    protected override string GetSuggestedFileName(FolderExplorerItem item)
    {
        // Try to get from tag (set by annotation tool), otherwise look up
        if (item.Tag is KnownFile knownFile) return knownFile.Name;

        var fileName = Path.GetFileName(item.Path);
        if (KnownFile.KnownFiles.TryGetValue(fileName, out var kf)) return kf.Name;

        return item.Name;
    }
}