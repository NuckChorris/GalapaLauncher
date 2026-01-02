using System.Threading.Tasks;
using Galapa.Core.Configuration;
using Galapa.Core.Game;
using Galapa.Core.Game.Authentication;
using Galapa.Core.Models;

namespace Galapa.Launcher.ViewModels.LoginFrame;

public class LoginCompletedPageViewModel(
    LoginFlowState flowState,
    PlayerList playerList,
    GameProcess gameProcess,
    Settings settings
) : LoginPageViewModel
{
    private LoginCompleted? _step;

    public LoginCompletedPageViewModel Initialize(LoginCompleted step)
    {
        this._step = step;
        _ = this.LaunchGameAsync();
        return this;
    }

    private async Task LaunchGameAsync()
    {
        if (this._step is null) return;

        SavedPlayer? savedPlayer = null;
        var needsSave = false;

        if (flowState.Strategy is SavedPlayerLoginStrategy strategy)
        {
            // Find the existing saved player by token
            savedPlayer = playerList.Players.Find(p => p.Token == strategy.Token);

            // Save password if requested and we have one
            if (flowState.SavePassword && savedPlayer is not null && this._step.Password is not null)
            {
                savedPlayer.Password = this._step.Password;
                needsSave = true;
            }
        }
        else
        {
            // Save username if requested (new player flow)
            if (flowState.SaveUser && this._step.Token is not null)
            {
                savedPlayer = playerList.Add(this._step.Token);
                if (this._step.Username is not null) savedPlayer.Name = this._step.Username;
                needsSave = true;
            }

            // Save password if requested (can only happen if we saved username)
            if (flowState.SavePassword && savedPlayer is not null && this._step.Password is not null)
                savedPlayer.Password = this._step.Password;
        }

        // Save player list if we made changes
        if (needsSave) await playerList.SaveAsync();

        // Launch the game
        gameProcess.SessionId = this._step.SessionId;
        if (savedPlayer is not null) gameProcess.PlayerNumber = savedPlayer.Number;

        gameProcess.Start();
    }
}