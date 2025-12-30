using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.Input;
using Galapa.Core.StreamObfuscator;
using Galapa.Toolbox.Services;
using Galapa.Toolbox.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinRT.Interop;

namespace Galapa.Toolbox.Views;

public sealed partial class SaveExplorerPage
{
    public SaveExplorerPage()
    {
        this.InitializeComponent();
    }

    private ObservableCollection<SaveExplorerItem> Items { get; set; } =
        [new SaveExplorerItem { Path = Settings.Instance.SaveFolderPath }];

    private async Task ExportDeobfuscated(SaveExplorerItem item, Func<Stream, Stream> deobfuscator)
    {
        using var stream = File.OpenRead(item.Path);
        using var deobfuscatedStream = deobfuscator(stream);
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        picker.FileTypeChoices.Add("All Files", new List<string> { "." });
        if (item.DeobfuscatedName != null) picker.SuggestedFileName = item.DeobfuscatedName;

        var hwnd = WindowNative.GetWindowHandle(App.AppWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        StorageFile file = await picker.PickSaveFileAsync();
        if (file != null)
        {
            using var output = File.OpenWrite(file.Path);
            await deobfuscatedStream.CopyToAsync(output);
            var dialog = new ContentDialog
            {
                Title = "Export Complete",
                Content = $"Deobfuscated file exported to: {file.Path}",
                CloseButtonText = "OK",
                XamlRoot = App.AppWindow.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    private void UIElement_OnContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        if (sender is FrameworkElement element && element.DataContext is SaveExplorerItem item)
        {
            if (sender.ContextFlyout is not null) return;
            var flyout = new MenuFlyout();
            if (item.FileInfo != null)
            {
                flyout.Items.Add(new MenuFlyoutItem
                {
                    Text = "Export deobfuscated",
                    Icon = new SymbolIcon(Symbol.Save),
                    Command = new AsyncRelayCommand(() => ExportDeobfuscated(item, item.FileInfo.Obfuscator))
                });
            }

            flyout.Items.Add(new MenuFlyoutItem
            {
                Text = "Export with FixedObfuscator",
                Icon = new SymbolIcon(Symbol.Save),
                Command = new AsyncRelayCommand(() => ExportDeobfuscated(item, FixedObfuscator.Factory))
            });
            flyout.Items.Add(new MenuFlyoutItem
            {
                Text = "Export with UsernameObfuscator",
                Icon = new SymbolIcon(Symbol.Save),
                Command = new AsyncRelayCommand(() => ExportDeobfuscated(item, UsernameObfuscator.Factory))
            });
            flyout.Items.Add(new MenuFlyoutSeparator());
            flyout.Items.Add(new MenuFlyoutItem
            {
                Text = "Open in Explorer",
                Icon = new SymbolIcon(Symbol.OpenFile),
                Command = new RelayCommand(() => Process.Start("explorer.exe", $"/select,{item.Path}"))
            });

            if (args.TryGetPosition(sender, out var point))
            {
                flyout.ShowAt(sender, point);
            }
            else
            {
                flyout.ShowAt((FrameworkElement)sender);
            }


            sender.ContextFlyout = flyout;
        }
    }
}