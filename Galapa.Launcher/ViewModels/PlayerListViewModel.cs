using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Galapa.Core.Game.Authentication;
using Galapa.Core.Models;

namespace Galapa.Launcher.ViewModels;

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

public partial class PlayerListViewModel(PlayerList playerList) : ObservableObject
{
    private PlayerList PlayerList { get; } = playerList;
    public ObservableCollection<PlayerListItem> List { get; } = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        await this.PlayerList.LoadAsync();
        this.RebuildDisplayPlayers();
    }

    public SavedPlayer AddPlayer(string token, string? name)
    {
        var player = this.PlayerList.Add(token);
        if (name is not null) player.Name = name;

        this.List.Add(new SavedPlayerItem { Player = player });

        return player;
    }

    public async Task SaveAsync()
    {
        await this.PlayerList.SaveAsync();
    }

    private void RebuildDisplayPlayers()
    {
        this.List.Clear();

        foreach (var savedPlayer in this.PlayerList.Players)
            this.List.Add(new SavedPlayerItem { Player = savedPlayer });

        if (this.PlayerList.Players.Count < 4) this.List.Add(new NewPlayerItem());
    }
}