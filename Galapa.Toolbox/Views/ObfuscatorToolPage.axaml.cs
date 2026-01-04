using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Galapa.Core.StreamObfuscator;
using MsBox.Avalonia;

namespace Galapa.Toolbox.Views;

public partial class ObfuscatorToolPage : UserControl
{
    private string? _selectedFilePath;

    public ObfuscatorToolPage()
    {
        this.InitializeComponent();
    }

    private async void OnSelectFileClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select File",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.All }
        });

        if (files.Count > 0)
        {
            this._selectedFilePath = files[0].Path.LocalPath;
            this.SelectedFileText.Text = this._selectedFilePath;
        }
    }

    private void ObfuscatorCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.ParameterBox is null || this.ParameterHeader is null) return;

        if (this.ObfuscatorCombo.SelectedIndex == 0) // XorObfuscator
        {
            this.ParameterHeader.Text = "Bytes";
            this.ParameterBox.Watermark = "1A 2B 3C";
        }
        else // UsernameObfuscator
        {
            this.ParameterHeader.Text = "Username";
            this.ParameterBox.Watermark = "myusername";
        }
    }

    private async void OnRunClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(this._selectedFilePath))
        {
            await this.ShowDialog("No file selected.");
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var suggestedFileName = (this.ObfuscateRadio.IsChecked == true ? "obfuscated_" : "deobfuscated_") +
                                Path.GetFileName(this._selectedFilePath);

        var saveFile = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save File",
            SuggestedFileName = suggestedFileName,
            FileTypeChoices = new[] { FilePickerFileTypes.All }
        });

        if (saveFile == null) return;

        try
        {
            await using var input = File.OpenRead(this._selectedFilePath);
            await using var output = File.OpenWrite(saveFile.Path.LocalPath);

            Stream obfuscatorStream;
            if (this.ObfuscatorCombo.SelectedIndex == 0) // XorObfuscator
            {
                // Remove whitespace and parse as hex bytes
                var hex = this.ParameterBox.Text?.Replace(" ", "").Replace(",", "").Replace("-", "");
                if (string.IsNullOrWhiteSpace(hex) || hex.Length % 2 != 0)
                {
                    await this.ShowDialog("Please enter a valid even-length hex string for bytes.");
                    return;
                }

                var bytes = new byte[hex.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                    if (!byte.TryParse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, null, out bytes[i]))
                    {
                        await this.ShowDialog($"Invalid hex byte: {hex.Substring(i * 2, 2)}");
                        return;
                    }

                obfuscatorStream = new XorObfuscator(input, bytes);
            }
            else // UsernameObfuscator
            {
                var username = this.ParameterBox.Text;
                if (string.IsNullOrEmpty(username))
                {
                    await this.ShowDialog("Please enter a username.");
                    return;
                }

                obfuscatorStream = new UsernameObfuscator(input, username);
            }

            await obfuscatorStream.CopyToAsync(output);
            await obfuscatorStream.DisposeAsync();
            await this.ShowDialog("Operation complete. Output: " + saveFile.Path.LocalPath);
        }
        catch (Exception ex)
        {
            await this.ShowDialog("Error: " + ex.Message);
        }
    }

    private async Task ShowDialog(string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Obfuscator Tool", message);
        await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
    }
}