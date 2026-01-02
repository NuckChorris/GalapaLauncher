using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Galapa.Core.Game.Authentication;
using Galapa.Launcher.Services;

namespace Galapa.Launcher.ViewModels.LoginFrame;

public partial class AskUsernamePasswordPageViewModel(
    LoginFlowState flowState,
    LoginNavigationService navigationService
) : LoginPageViewModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public LoginFlowState FlowState { get; } = flowState;

    public AskUsernamePasswordPageViewModel Prefilled(AskUsernamePassword step)
    {
        this.Username = step.Username ?? "";
        this.Password = step.Password ?? "";

        return this;
    }

    [RelayCommand]
    private async Task Login()
    {
        if (flowState.Strategy is null) return;

        var action = new UsernamePasswordAction(this.Username, this.Password);
        var nextStep = await flowState.Strategy.Step(action);
        navigationService.Forward(nextStep);
    }
}