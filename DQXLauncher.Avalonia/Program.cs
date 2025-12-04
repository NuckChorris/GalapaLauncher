using System;
using System.IO;
using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Avalonia.Services;
using DQXLauncher.Avalonia.ViewModels;
using DQXLauncher.Avalonia.ViewModels.Pages.App;
using DQXLauncher.Avalonia.Views;
using DQXLauncher.Core.Game.ConfigFile;
using DQXLauncher.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Velopack;
using Velopack.Logging;

namespace DQXLauncher.Avalonia;

internal sealed class Program
{
    internal class Lazier<T> : Lazy<T> where T : class
    {
        public Lazier(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>())
        {
        }
    }

    public static ConsoleVelopackLogger Log { get; } = new();
    public static ServiceProvider Services;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // TODO: Unfuck this whole startup sequence
        VelopackApp.Build().SetLogger(Log).Run();

        // Boot the application
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();
        services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<AppFrameViewModel>();
        services.AddSingleton<HomePageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();
        services.AddSingleton<GamepadInputService>();
        services.AddSingleton<Settings>(_ => Settings.Load());
        services.AddLogging(lb => { lb.AddSerilog(BuildLogger(), true); });

        return services.BuildServiceProvider();
    }

    private static ILogger BuildLogger()
    {
        var cfg = new LoggerConfiguration();
        cfg.WriteTo.Console();
        cfg.Enrich.FromLogContext();

        return cfg.CreateLogger();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        // Set up Paths for DQXLauncher.Core
        Paths.AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DQXLauncher");
        Paths.Create();

        // We can't create our service provider until the Paths are configured to allow the Settings to be loaded
        // TODO: move Paths.AppData into the launcher itself (it's not core-related)
        Services = CreateServiceProvider();
        Ioc.Default.ConfigureServices(Services);
        var settings = Services.GetRequiredService<Settings>();
        ConfigFile.RootDirectory = settings.SaveFolderPath;

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithDeveloperTools()
            .WithInterFont()
            .LogToTrace();
    }
}