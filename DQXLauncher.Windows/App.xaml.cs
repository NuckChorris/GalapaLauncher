using System;
using System.IO;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Core.Game;
using DQXLauncher.Core.Game.ConfigFile;
using DQXLauncher.Core.Models;
using DQXLauncher.Core.Services;
using DQXLauncher.Windows.Services;
using DQXLauncher.Windows.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using Velopack;

namespace DQXLauncher.Windows;

public partial class App : Application
{
    public static MainWindow AppWindow;
    private readonly ServiceProvider _services;

    public App()
    {
        VelopackApp.Build()
            .Run();
        InitializeComponent();

        _services = CreateServiceProvider();
        Ioc.Default.ConfigureServices(_services);

        // Set up DQXLauncher.Core
        Paths.AppData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            , "DQXLauncher");
        Paths.Create();
        var settings = _services.GetRequiredService<LauncherSettings>();
        ConfigFile.RootDirectory = settings.SaveFolderPath;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppWindow = _services.GetRequiredService<MainWindow>();
        AppWindow.Activate();
    }

    private ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();
        services.AddTransient<MainWindow>();
        services.AddSingleton<PlayerList<PlayerCredential>>();
        services.AddSingleton<PlayerListViewModel>();
        services.AddSingleton<LauncherSettings>(_ => LauncherSettings.Load());
        services.AddSingleton<Launcher, Win32Launcher>();
        services.AddSingleton<MainFrameViewModel>();
        services.AddSingleton<LoginFrameViewModel>();
        services.AddLogging(lb => { lb.AddSerilog(BuildLogger(), true); });

        return services.BuildServiceProvider();
    }

    private ILogger BuildLogger()
    {
        var cfg = new LoggerConfiguration();
        cfg.Enrich.FromLogContext();

        return cfg.CreateLogger();
    }
}