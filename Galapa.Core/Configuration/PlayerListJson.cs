using System.Text.Json;
using Galapa.Core.Game.LoginStrategy;

namespace Galapa.Core.Configuration;

/// <summary>
///     Wraps access to the PlayerListJson.json file we use instead of dqxPlayerList.xml
/// </summary>
/// <remarks>
///     We do things a bit differently than the original launcher, separating saved and trial players, as well as storing
///     the account name for each player instead of reading the character name from the game config.
/// </remarks>
public class PlayerListJson
{
    private static string FilePath => Path.Combine(Paths.AppData, "PlayerList.json");

    public int? Selected { get; set; }
    public Dictionary<string, SavedPlayer> Players { get; set; } = new();
    public TrialPlayer? Trial { get; set; }

    // Added factory to allow injecting a custom SavedPlayerLoginStrategy instance.
    public static Func<string, int, SavedPlayerLoginStrategy> SavedPlayerLoginStrategyFactory { get; set; } =
        (token, number) => new SavedPlayerLoginStrategy(token);

    /// <summary>
    ///     Load and parse the PlayerList.json file into memory, creating it if it doesn't exist.
    /// </summary>
    /// <returns>The PlayerListJson instance</returns>
    public static async Task<PlayerListJson> LoadAsync()
    {
        try
        {
            return JsonSerializer.Deserialize<PlayerListJson>(await File.ReadAllTextAsync(FilePath)) ??
                   new PlayerListJson();
        }
        catch (Exception)
        {
            return new PlayerListJson();
        }
    }

    public async Task SaveAsync()
    {
        // Find names for all the players which are missing them
        foreach (var player in this.Players.Values.Where(p => p.Name is null)) await player.LoadName();

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        await File.WriteAllTextAsync(FilePath,
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }

    public class SavedPlayer
    {
        public int Number { get; set; }
        public string? Name { get; set; }
        public required string Token { get; init; }

        /// <summary>
        ///     Load the player's account name (sqexid) from the token using the SavedPlayerLoginStrategy. This can fail, in
        ///     which case the name will be unchanged.
        /// </summary>
        public async Task LoadName()
        {
            var login = SavedPlayerLoginStrategyFactory(this.Token, this.Number - 1);
            var step = await login.Start();

            if (step is AskPassword action) this.Name = action.Username;
        }
    }

    public class TrialPlayer
    {
        public string Id { get; set; }
        public required string Token { get; init; }
        public string Code { get; set; }
    }
}