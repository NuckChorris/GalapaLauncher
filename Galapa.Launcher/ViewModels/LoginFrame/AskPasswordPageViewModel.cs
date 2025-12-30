using Galapa.Core.Game.Authentication;

namespace Galapa.Launcher.ViewModels.LoginFrame;

public class AskPasswordPageViewModel : LoginPageViewModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    public AskPasswordPageViewModel Prefilled(AskPassword step)
    {
        this.Username = step.Username;
        this.Password = step.Password ?? "";

        return this;
    }
}