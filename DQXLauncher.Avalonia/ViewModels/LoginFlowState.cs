using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Core.Game.LoginStrategy;

namespace DQXLauncher.Avalonia.ViewModels;

public partial class LoginFlowState : ObservableObject
{
    [ObservableProperty] private bool _savePassword;
    [ObservableProperty] private bool _saveUser;
    [ObservableProperty] private LoginStrategy? _strategy;
}