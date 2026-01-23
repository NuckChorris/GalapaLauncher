using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Galapa.Toolbox.Controls;
using MsBox.Avalonia;

namespace Galapa.Toolbox.Tools;

/// <summary>
///     Base class for tools that export deobfuscated files.
/// </summary>
public abstract class ExportToolBase : IContextMenuTool
{
    public abstract string MenuLabel { get; }

    /// <summary>
    ///     Gets the obfuscator/deobfuscator stream wrapper for the given item.
    /// </summary>
    protected abstract Func<Stream, Stream> GetObfuscator(FolderExplorerItem item);

    /// <summary>
    ///     Gets the suggested filename for the export dialog.
    /// </summary>
    protected virtual string GetSuggestedFileName(FolderExplorerItem item)
    {
        return item.Name;
    }

    public virtual bool CanHandle(FolderExplorerItem item)
    {
        return !item.IsDirectory;
    }

    public async Task ExecuteAsync(FolderExplorerItem item, TopLevel topLevel)
    {
        var saveFile = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Deobfuscated File",
            SuggestedFileName = this.GetSuggestedFileName(item),
            FileTypeChoices = [FilePickerFileTypes.All]
        });

        if (saveFile == null) return;

        try
        {
            await using var input = File.OpenRead(item.Path);
            await using var deobfuscatedStream = this.GetObfuscator(item)(input);
            await using var output = File.OpenWrite(saveFile.Path.LocalPath);
            await deobfuscatedStream.CopyToAsync(output);

            var box = MessageBoxManager.GetMessageBoxStandard(
                "Export Complete",
                $"Deobfuscated file exported to:\n{saveFile.Path.LocalPath}");
            await box.ShowWindowDialogAsync(topLevel as Window);
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", $"Export failed: {ex.Message}");
            await box.ShowWindowDialogAsync(topLevel as Window);
        }
    }
}