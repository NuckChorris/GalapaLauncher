using CommunityToolkit.Mvvm.ComponentModel;
using Galapa.Core.Game.Authentication;

namespace Galapa.Launcher.ViewModels;

public partial class LoginFlowState : ObservableObject
{
    [ObservableProperty] private bool _savePassword;
    [ObservableProperty] private bool _saveUser;
    [ObservableProperty] private LoginStrategy? _strategy;
}