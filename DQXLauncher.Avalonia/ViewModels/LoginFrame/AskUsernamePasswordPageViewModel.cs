using DQXLauncher.Core.Game.LoginStrategy;

namespace DQXLauncher.Avalonia.ViewModels.LoginFrame;

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