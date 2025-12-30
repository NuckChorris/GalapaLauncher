using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Galapa.Core.Game.Authentication;
using Galapa.Core.Models;
using Galapa.Launcher.Services;

namespace Galapa.Launcher.ViewModels.LoginFrame;

public abstract class PlayerListItem
{
    public virtual string Icon => "/Assets/Icons/solar--user-rounded-bold-duotone.svg";
    public virtual string Text => null!;
    public virtual LoginStrategy LoginStrategy => null!;
}

public class SavedPlayerItem : PlayerListItem
{
    public required SavedPlayer Player { get; init; }
    public override string Icon => "/Assets/Icons/solar--user-rounded-bold-duotone.svg";
    public override string Text => this.Player.Name ?? $"Player {this.Player.Number}";
    public override SavedPlayerLoginStrategy LoginStrategy => this.Player.LoginStrategy;
}

public class NewPlayerItem : PlayerListItem
{
    public override string Icon => "/Assets/Icons/solar--user-plus-rounded-bold-duotone.svg";
    public override string Text => "New Player";
    public override NewPlayerLoginStrategy LoginStrategy => new();
}

public partial class PlayerSelectPageViewModel(
    LoginNavigationService login,
    LoginFlowState flow,
    PlayerList list
) : LoginPageViewModel
{
    [ObservableProperty] private ObservableCollection<PlayerListItem> _players = new();

    [RelayCommand]
    public async Task Load()
    {
        await list.LoadAsync();
        this.Players =
            new ObservableCollection<PlayerListItem>(
                list.Players
                    .Select(player => new SavedPlayerItem { Player = player })
                    .Append<PlayerListItem>(new NewPlayerItem()));
    }

    [RelayCommand]
    public async Task SelectPlayer(PlayerListItem player)
    {
        flow.Strategy = player.LoginStrategy;
        var step = await player.LoginStrategy.Start();
        login.Forward(step);
    }
}