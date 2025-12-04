using System;
using Avalonia.Controls;
using Avalonia.Input;
using DQXLauncher.Avalonia.Services;
using DQXLauncher.Avalonia.ViewModels;
using Microsoft.Extensions.Logging;

namespace DQXLauncher.Avalonia.Views;

public partial class MainWindow : Window
{
    private readonly GamepadInputService _gamepadService;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(MainWindowViewModel mainWindowViewModel, GamepadInputService gamepadService,
        ILogger<MainWindow> logger)
    {
        DataContext = mainWindowViewModel;
        _gamepadService = gamepadService;
        _logger = logger;

        InitializeComponent();
        ExtendClientAreaToDecorationsHint = true;

        _gamepadService.NavigationKeyPressed += OnGamepadNavigationKeyPressed;
        _gamepadService.Start();

        Closed += OnClosed;
    }

    private void OnGamepadNavigationKeyPressed(object? sender, Key key)
    {
        _logger.LogInformation("Got gamepad event {key}", key);
        var focusManager = GetTopLevel(this)?.FocusManager;
        if (focusManager?.GetFocusedElement() is not InputElement focusedElement) focusedElement = this;

        var keyEventArgs = new KeyEventArgs
        {
            Key = key,
            RoutedEvent = KeyDownEvent
        };

        focusedElement.RaiseEvent(keyEventArgs);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _gamepadService.NavigationKeyPressed -= OnGamepadNavigationKeyPressed;
        _gamepadService.Stop();
    }
}