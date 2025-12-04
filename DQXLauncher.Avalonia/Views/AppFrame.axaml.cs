using System;
using Avalonia.Controls.Primitives;
using DQXLauncher.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SelectionChangedEventArgs = Avalonia.Controls.SelectionChangedEventArgs;
using UserControl = Avalonia.Controls.UserControl;

namespace DQXLauncher.Avalonia.Views;

public partial class AppFrame : UserControl
{
    private AppFrameViewModel ViewModel => (AppFrameViewModel)DataContext!;

    public AppFrame()
    {
        DataContext = Program.Services.GetRequiredService<AppFrameViewModel>();
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var logger = Program.Services.GetRequiredService<ILogger<AppFrame>>();
        if (sender is TabStrip { SelectedItem: TabStripItem { Tag: Type tab } })
            logger.LogInformation("Setting Current Page {tab}", tab);
    }
}