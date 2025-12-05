using System;
using Avalonia.Controls.Primitives;
using DQXLauncher.Avalonia.ViewModels.AppFrame;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SelectionChangedEventArgs = Avalonia.Controls.SelectionChangedEventArgs;
using UserControl = Avalonia.Controls.UserControl;

namespace DQXLauncher.Avalonia.Views.AppFrame;

public partial class AppFrame : UserControl
{
    public AppFrame()
    {
        this.DataContext = Program.Services.GetRequiredService<AppFrameViewModel>();
        this.InitializeComponent();
    }

    private AppFrameViewModel ViewModel => (AppFrameViewModel)this.DataContext!;

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var logger = Program.Services.GetRequiredService<ILogger<AppFrame>>();
        if (sender is TabStrip { SelectedItem: TabStripItem { Tag: Type tab } })
            logger.LogInformation("Setting Current Page {tab}", tab);
    }
}