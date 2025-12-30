using Galapa.Core.Game.Authentication;

namespace Galapa.Launcher.ViewModels.LoginFrame;

public class AskUsernamePasswordPageViewModel : LoginPageViewModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    public AskUsernamePasswordPageViewModel Prefilled(AskUsernamePassword step)
    {
        this.Username = step.Username ?? "";
        this.Password = step.Password ?? "";

        return this;
    }
}