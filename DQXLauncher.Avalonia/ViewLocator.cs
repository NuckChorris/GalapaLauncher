using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using DQXLauncher.Avalonia.ViewModels;
using DQXLauncher.Avalonia.ViewModels.AppFrame;
using DQXLauncher.Avalonia.Views;
using DQXLauncher.Avalonia.Views.AppFrame;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DQXLauncher.Avalonia;

/// <summary>
///     Given a view model, returns the corresponding view using pattern matching and dependency injection.
///     This implementation is typesafe and doesn't use reflection.
/// </summary>
public class ViewLocator : IDataTemplate
{
    private readonly IServiceProvider _serviceProvider;

    public ViewLocator(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Control? Build(object? param)
    {
        var logger = this._serviceProvider.GetRequiredService<ILogger<ViewLocator>>();
        logger.LogWarning($"Building view for {param?.GetType().Name ?? "null"}");
        return param switch
        {
            null => null,

            // Pattern match each ViewModel to its corresponding View
            MainWindowViewModel => this.ResolveView<MainWindow>(),
            AppFrameViewModel => this.ResolveView<AppFrame>(),
            HomePageViewModel => this.ResolveView<HomePage>(),
            SettingsPageViewModel => this.ResolveView<SettingsPage>(),

            // Fallback for unknown ViewModels
            ViewModelBase vm => new TextBlock { Text = $"View not found for: {vm.GetType().Name}" },

            _ => null
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    /// <summary>
    ///     Resolves a view from the service provider with full dependency injection support.
    ///     Falls back to Activator.CreateInstance if the view isn't registered in the container.
    /// </summary>
    private Control ResolveView<TView>() where TView : Control
    {
        // Try to get the view from DI first
        var view = this._serviceProvider.GetService<TView>();

        // Fallback to Activator if not registered in DI
        return view ?? Activator.CreateInstance<TView>();
    }
}