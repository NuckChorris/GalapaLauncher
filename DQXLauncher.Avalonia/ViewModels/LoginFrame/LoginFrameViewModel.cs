using System;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Avalonia.Services;
using DQXLauncher.Core.Game.LoginStrategy;

namespace DQXLauncher.Avalonia.ViewModels.LoginFrame;

public partial class LoginFrameViewModel : ObservableObject
{
    private readonly Func<AskUsernamePasswordPageViewModel> _askUsernamePasswordPage;
    private readonly Func<PlayerSelectPageViewModel> _playerSelectPage;
    [ObservableProperty] private bool _isTransitionReversed;
    [ObservableProperty] private LoginPageViewModel _page;

    public LoginFrameViewModel(LoginNavigationService login, PlayerSelectPageViewModel page,
        Func<AskUsernamePasswordPageViewModel> askUsernamePasswordPage,
        Func<PlayerSelectPageViewModel> playerSelectPage)
    {
        this._askUsernamePasswordPage = askUsernamePasswordPage;
        this._playerSelectPage = playerSelectPage;
        this.Page = page;
        login.StepChanged += (_, stepChange) =>
        {
            this.IsTransitionReversed = stepChange.Direction == LoginNavigationService.StepChangeDirection.Backward;
            this.Page = this.PageForStep(stepChange.Step);
        };
    }

    private LoginPageViewModel PageForStep(LoginStep step)
    {
        return step switch
        {
            AskUsernamePassword s => this._askUsernamePasswordPage().Prefilled(s),
            AskPassword s => this._askPasswordPage().Prefilled(s),
            null => this._playerSelectPage()
        };
    }
}