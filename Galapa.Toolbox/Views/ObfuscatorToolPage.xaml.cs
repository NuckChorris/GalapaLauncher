using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Galapa.Core.StreamObfuscator;

namespace Galapa.Toolbox.Views;

public sealed partial class ObfuscatorToolPage
{
    private string? _selectedFilePath;

    public ObfuscatorToolPage()
    {
        this.InitializeComponent();
    }

    private async void OnSelectFileClick(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add("*");
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.AppWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            _selectedFilePath = file.Path;
            SelectedFileText.Text = file.Path;
        }
    }

    private void ObfuscatorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ParameterBox is null) return;
        if (ObfuscatorCombo.SelectedIndex == 0) // XorObfuscator
        {
            ParameterBox.Header = "Bytes";
            ParameterBox.PlaceholderText = "1A 2B 3C";
        }
        else // UsernameObfuscator
        {
            ParameterBox.Header = "Username";
            ParameterBox.PlaceholderText = "myusername";
        }
    }

    private async void OnRunClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedFilePath))
        {
            await ShowDialog("No file selected.");
            return;
        }
        var picker = new FileSavePicker();
        picker.FileTypeChoices.Add("All Files", new System.Collections.Generic.List<string> { "." });
        picker.SuggestedFileName = (ObfuscateRadio.IsChecked == true ? "obfuscated" : "deobfuscated") + Path.GetFileName(_selectedFilePath);
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.AppWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var saveFile = await picker.PickSaveFileAsync();
        if (saveFile == null) return;
        try
        {
            using var input = File.OpenRead(_selectedFilePath);
            using var output = File.OpenWrite(saveFile.Path);
            Stream obfuscatorStream;
            if (ObfuscatorCombo.SelectedIndex == 0) // XorObfuscator
            {
                // Remove whitespace and parse as hex bytes
                var hex = ParameterBox.Text?.Replace(" ", "").Replace(",", "").Replace("-", "");
                if (string.IsNullOrWhiteSpace(hex) || hex.Length % 2 != 0)
                {
                    await ShowDialog("Please enter a valid even-length hex string for bytes.");
                    return;
                }
                var bytes = new byte[hex.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (!byte.TryParse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, null, out bytes[i]))
                    {
                        await ShowDialog($"Invalid hex byte: {hex.Substring(i * 2, 2)}");
                        return;
                    }
                }
                obfuscatorStream = new XorObfuscator(input, bytes);
            }
            else // UsernameObfuscator
            {
                var username = ParameterBox.Text;
                obfuscatorStream = new UsernameObfuscator(input, username);
            }
            await obfuscatorStream.CopyToAsync(output);
            await obfuscatorStream.DisposeAsync();
            await ShowDialog("Operation complete. Output: " + saveFile.Path);
        }
        catch (Exception ex)
        {
            await ShowDialog("Error: " + ex.Message);
        }
    }

    private async Task ShowDialog(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Obfuscator Tool",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = App.AppWindow.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }
}