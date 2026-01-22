using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using DryIoc;
using Galapa.Launcher.Models;
using Galapa.Launcher.Services;

namespace Galapa.Launcher.Controls;

/// <summary>
/// A control that displays controller button labels that automatically
/// update based on the active controller type.
/// </summary>
public class ControllerIcon : TemplatedControl
{
    public static readonly StyledProperty<ControllerAction> ActionProperty =
        AvaloniaProperty.Register<ControllerIcon, ControllerAction>(nameof(Action));

    public static readonly StyledProperty<bool> HideWhenDisconnectedProperty =
        AvaloniaProperty.Register<ControllerIcon, bool>(nameof(HideWhenDisconnected), defaultValue: false);

    public static readonly DirectProperty<ControllerIcon, string> LabelProperty =
        AvaloniaProperty.RegisterDirect<ControllerIcon, string>(
            nameof(Label),
            o => o.Label);

    private ActiveControllerService? _activeControllerService;
    private string _label = string.Empty;

    public ControllerIcon()
    {
        this.UpdateLabel();
    }

    /// <summary>
    /// The controller action to display a label for.
    /// </summary>
    public ControllerAction Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    /// <summary>
    /// Whether to hide the control when no controller is connected.
    /// </summary>
    public bool HideWhenDisconnected
    {
        get => GetValue(HideWhenDisconnectedProperty);
        set => SetValue(HideWhenDisconnectedProperty, value);
    }

    /// <summary>
    /// The current label text based on the active controller style.
    /// </summary>
    public string Label
    {
        get => this._label;
        private set => SetAndRaise(LabelProperty, ref this._label, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Try to get the ActiveControllerService from the app's service container
        this._activeControllerService = Program.Services?.Resolve<ActiveControllerService>();

        if (this._activeControllerService != null)
        {
            this._activeControllerService.PropertyChanged += this.OnActiveControllerPropertyChanged;
            this.UpdateLabel();
            this.UpdateVisibility();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (this._activeControllerService != null)
        {
            this._activeControllerService.PropertyChanged -= this.OnActiveControllerPropertyChanged;
            this._activeControllerService = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ActionProperty)
        {
            this.UpdateLabel();
        }
        else if (change.Property == HideWhenDisconnectedProperty)
        {
            this.UpdateVisibility();
        }
    }

    private void OnActiveControllerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ActiveControllerService.LabelStyle))
        {
            this.UpdateLabel();
        }
        else if (e.PropertyName == nameof(ActiveControllerService.IsControllerConnected))
        {
            this.UpdateVisibility();
        }
    }

    private void UpdateLabel()
    {
        var style = this._activeControllerService?.LabelStyle ?? ControllerLabelStyle.Xbox;
        this.Label = GetLabel(this.Action, style);
    }

    private void UpdateVisibility()
    {
        if (this.HideWhenDisconnected && this._activeControllerService != null)
        {
            this.IsVisible = this._activeControllerService.IsControllerConnected;
        }
        else
        {
            this.IsVisible = true;
        }
    }

    private static string GetLabel(ControllerAction action, ControllerLabelStyle style)
    {
        return (action, style) switch
        {
            // Bumpers
            (ControllerAction.BumperLeft, ControllerLabelStyle.Xbox) => "LB",
            (ControllerAction.BumperLeft, ControllerLabelStyle.Nintendo) => "L",
            (ControllerAction.BumperLeft, ControllerLabelStyle.Generic) => "L1",

            (ControllerAction.BumperRight, ControllerLabelStyle.Xbox) => "RB",
            (ControllerAction.BumperRight, ControllerLabelStyle.Nintendo) => "R",
            (ControllerAction.BumperRight, ControllerLabelStyle.Generic) => "R1",

            // Triggers
            (ControllerAction.TriggerLeft, ControllerLabelStyle.Xbox) => "LT",
            (ControllerAction.TriggerLeft, ControllerLabelStyle.Nintendo) => "ZL",
            (ControllerAction.TriggerLeft, ControllerLabelStyle.Generic) => "L2",

            (ControllerAction.TriggerRight, ControllerLabelStyle.Xbox) => "RT",
            (ControllerAction.TriggerRight, ControllerLabelStyle.Nintendo) => "ZR",
            (ControllerAction.TriggerRight, ControllerLabelStyle.Generic) => "R2",

            // Confirm (face button)
            (ControllerAction.Confirm, ControllerLabelStyle.Xbox) => "A",
            (ControllerAction.Confirm, ControllerLabelStyle.Nintendo) => "B",
            (ControllerAction.Confirm, ControllerLabelStyle.Generic) => "1",

            // Decline (face button)
            (ControllerAction.Decline, ControllerLabelStyle.Xbox) => "B",
            (ControllerAction.Decline, ControllerLabelStyle.Nintendo) => "A",
            (ControllerAction.Decline, ControllerLabelStyle.Generic) => "2",

            // Directional
            (ControllerAction.Up, _) => "↑",
            (ControllerAction.Down, _) => "↓",
            (ControllerAction.Left, _) => "←",
            (ControllerAction.Right, _) => "→",

            _ => action.ToString()
        };
    }
}
