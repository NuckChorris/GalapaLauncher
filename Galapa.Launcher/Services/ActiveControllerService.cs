using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Galapa.Launcher.Models;
using Microsoft.Extensions.Logging;
using SDL3;

namespace Galapa.Launcher.Services;

/// <summary>
///     Tracks the most recently used controller and exposes its label style
///     for reactive UI updates.
/// </summary>
public class ActiveControllerService : INotifyPropertyChanged, IDisposable
{
    private readonly ControllerActionSource _actionSource;
    private readonly ControllerListService _controllerListService;
    private readonly ILogger<ActiveControllerService> _logger;

    private Controller? _activeController;
    private bool _isControllerConnected;
    private ControllerLabelStyle _labelStyle = ControllerLabelStyle.Xbox;

    public ActiveControllerService(
        ILogger<ActiveControllerService> logger,
        ControllerListService controllerListService,
        ControllerActionSource actionSource)
    {
        this._logger = logger;
        this._controllerListService = controllerListService;
        this._actionSource = actionSource;
    }

    /// <summary>
    ///     The most recently used controller, or null if none.
    /// </summary>
    public Controller? ActiveController
    {
        get => this._activeController;
        private set
        {
            if (this._activeController == value)
                return;

            this._activeController = value;
            this.OnPropertyChanged();

            // Update label style when active controller changes
            this.LabelStyle = value != null
                ? GetLabelStyle(value.GamepadType)
                : ControllerLabelStyle.Xbox;
        }
    }

    /// <summary>
    ///     The label style to use for button prompts based on the active controller.
    /// </summary>
    public ControllerLabelStyle LabelStyle
    {
        get => this._labelStyle;
        private set
        {
            if (this._labelStyle == value)
                return;

            this._labelStyle = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Whether any controller is currently connected.
    /// </summary>
    public bool IsControllerConnected
    {
        get => this._isControllerConnected;
        private set
        {
            if (this._isControllerConnected == value)
                return;

            this._isControllerConnected = value;
            this.OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Starts tracking controller activity.
    /// </summary>
    public void Start()
    {
        this._controllerListService.ControllerConnected += this.OnControllerConnected;
        this._controllerListService.ControllerDisconnected += this.OnControllerDisconnected;
        this._actionSource.ActionTriggered += this.OnActionTriggered;

        // Set initial state
        this.IsControllerConnected = this._controllerListService.Controllers.Count > 0;
        if (this._controllerListService.Controllers.Count > 0)
            this.ActiveController = this._controllerListService.Controllers[0];

        this._logger.LogDebug("ActiveControllerService started");
    }

    /// <summary>
    ///     Stops tracking controller activity.
    /// </summary>
    public void Stop()
    {
        this._controllerListService.ControllerConnected -= this.OnControllerConnected;
        this._controllerListService.ControllerDisconnected -= this.OnControllerDisconnected;
        this._actionSource.ActionTriggered -= this.OnActionTriggered;

        this._logger.LogDebug("ActiveControllerService stopped");
    }

    private void OnControllerConnected(object? sender, ControllerEventArgs e)
    {
        this.IsControllerConnected = true;

        // If no active controller, use this one
        if (this.ActiveController == null) this.ActiveController = e.Controller;
    }

    private void OnControllerDisconnected(object? sender, ControllerEventArgs e)
    {
        this.IsControllerConnected = this._controllerListService.Controllers.Count > 0;

        // If the active controller disconnected, switch to another or clear
        if (this.ActiveController?.Id == e.Controller.Id)
            this.ActiveController = this._controllerListService.Controllers.Count > 0
                ? this._controllerListService.Controllers[0]
                : null;
    }

    private void OnActionTriggered(object? sender, ControllerActionEventArgs e)
    {
        // Update active controller to the one that just sent input
        if (this.ActiveController?.Id != e.Controller.Id)
        {
            this.ActiveController = e.Controller;
            this._logger.LogDebug(
                "Active controller changed to {Name} ({Style})",
                e.Controller.Name,
                this.LabelStyle);
        }
    }

    private static ControllerLabelStyle GetLabelStyle(SDL.GamepadType type)
    {
        return type switch
        {
            SDL.GamepadType.NintendoSwitchPro => ControllerLabelStyle.Nintendo,
            SDL.GamepadType.NintendoSwitchJoyconLeft => ControllerLabelStyle.Nintendo,
            SDL.GamepadType.NintendoSwitchJoyconRight => ControllerLabelStyle.Nintendo,
            SDL.GamepadType.NintendoSwitchJoyconPair => ControllerLabelStyle.Nintendo,
            SDL.GamepadType.Xbox360 => ControllerLabelStyle.Xbox,
            SDL.GamepadType.XboxOne => ControllerLabelStyle.Xbox,
            _ => ControllerLabelStyle.Numeric
        };
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        this.Stop();
    }
}