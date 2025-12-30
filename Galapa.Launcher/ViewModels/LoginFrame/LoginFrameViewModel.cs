using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Galapa.Core.Game.Authentication;
using Galapa.Launcher.Services;

namespace Galapa.Launcher.ViewModels.LoginFrame;

public partial class LoginFrameViewModel : ObservableObject
{
    private readonly Func<AskPasswordPageViewModel> _askPasswordPage;
    private readonly Func<AskUsernamePasswordPageViewModel> _askUsernamePasswordPage;
    private readonly Func<PlayerSelectPageViewModel> _playerSelectPage;
    [ObservableProperty] private bool _canReturnToPlayerSelect;
    [ObservableProperty] private bool _isTransitionReversed;
    [ObservableProperty] private LoginPageViewModel _page;

    public LoginFrameViewModel(LoginNavigationService login,
        Func<AskUsernamePasswordPageViewModel> askUsernamePasswordPage,
        Func<PlayerSelectPageViewModel> playerSelectPage,
        Func<AskPasswordPageViewModel> askPasswordPage)
    {
        this._askUsernamePasswordPage = askUsernamePasswordPage;
        this._playerSelectPage = playerSelectPage;
        this._askPasswordPage = askPasswordPage;
        this.Page = this.PageForStep(null);
        login.StepChanged += (_, stepChange) =>
        {
            this.CanReturnToPlayerSelect = true;
            this.IsTransitionReversed = stepChange.Direction == LoginNavigationService.StepChangeDirection.Backward;
            this.Page = this.PageForStep(stepChange.Step);
        };
    }


    [RelayCommand]
    public void ReturnToPlayerSelect()
    {
        this.CanReturnToPlayerSelect = false;
        this.IsTransitionReversed = true;
        this.Page = this.PageForStep(null);
    }

    private LoginPageViewModel PageForStep(LoginStep? step)
    {
        return step switch
        {
            AskUsernamePassword s => this._askUsernamePasswordPage().Prefilled(s),
            AskPassword s => this._askPasswordPage().Prefilled(s),
            null => this._playerSelectPage()
        };
    }
}