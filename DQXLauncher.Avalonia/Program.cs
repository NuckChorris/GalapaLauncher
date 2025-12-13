using System;
using System.IO;
using System.Linq;
using Avalonia;
using DQXLauncher.Avalonia.Services;
using DQXLauncher.Avalonia.ViewModels;
using DQXLauncher.Avalonia.ViewModels.AppFrame;
using DQXLauncher.Avalonia.Views;
using DQXLauncher.Core.Game.ConfigFile;
using DQXLauncher.Core.Services;
using DryIoc;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Velopack;
using Velopack.Logging;
using ILogger = Serilog.ILogger;

namespace DQXLauncher.Avalonia;

internal sealed class Program
{
    public static IContainer Services;

    public static ConsoleVelopackLogger Log { get; } = new();

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

    private static IContainer CreateServiceProvider()
    {
        var container = new Container();

        // Register singletons
        container.Register<MainWindow>(Reuse.Singleton);
        container.Register<MainWindowViewModel>(Reuse.Singleton);
        container.Register<AppFrameViewModel>(Reuse.Singleton);
        container.Register<HomePageViewModel>(Reuse.Singleton);
        container.Register<SettingsPageViewModel>(Reuse.Singleton);
        container.Register<GamepadInputService>(Reuse.Singleton);

        // Register Settings with factory
        container.RegisterDelegate<Settings>(_ => Settings.Load(), Reuse.Singleton);

        // Register logging
        var logger = BuildLogger();
        var loggerFactory = new SerilogLoggerFactory(logger, true);
        container.RegisterInstance<ILoggerFactory>(loggerFactory);

        // Register ILogger<T> using the generic CreateLogger<T> extension method
        var createLoggerMethod = typeof(LoggerFactoryExtensions)
            .GetMethods()
            .First(m => m.Name == "CreateLogger" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 1);

        container.Register(
            typeof(ILogger<>),
            made: Made.Of(FactoryMethod.Of(createLoggerMethod), Parameters.Of.Type<ILoggerFactory>()),
            reuse: Reuse.Transient);

        return container;
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
        var settings = Services.Resolve<Settings>();
        ConfigFile.RootDirectory = settings.SaveFolderPath;

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithDeveloperTools()
            .WithInterFont()
            .LogToTrace();
    }
}