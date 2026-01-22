using Galapa.Launcher.Input;
using Galapa.Launcher.Models;
using Galapa.Launcher.ViewModels.SettingsFrame;
using UserControl = Avalonia.Controls.UserControl;

namespace Galapa.Launcher.Views.SettingsFrame;

public partial class SettingsFrame : UserControl, IControllerInputHandler
{
    public SettingsFrame()
    {
        this.InitializeComponent();
    }

    public bool HandleControllerInput(ControllerAction action, bool isRepeat)
    {
        // Handle L2/R2 for tab switching
        if (action is not (ControllerAction.TriggerLeft or ControllerAction.TriggerRight))
            return false;

        if (this.DataContext is not SettingsFrameViewModel vm || vm.Pages.Count == 0)
            return false;

        var currentIndex = vm.SelectedPage != null ? vm.Pages.IndexOf(vm.SelectedPage) : 0;
        var newIndex = action switch
        {
            ControllerAction.TriggerLeft => (currentIndex - 1 + vm.Pages.Count) % vm.Pages.Count,
            ControllerAction.TriggerRight => (currentIndex + 1) % vm.Pages.Count,
            _ => currentIndex
        };

        if (newIndex != currentIndex)
        {
            vm.SelectedPage = vm.Pages[newIndex];
            return true;
        }

        return false;
    }
}