using Galapa.Core.Game.Authentication;
using Galapa.Core.Configuration;
using Galapa.TestUtilities;
using Moq;

namespace Galapa.Core.Tests.Services;

[Collection("Sequential")]
public class PlayerListJsonTests
{
    [Fact]
    public async Task TestLoadAsync_WhenFileDoesNotExist()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        var playerList = await PlayerListJson.LoadAsync();
        Assert.NotNull(playerList);
        Assert.Null(playerList.Trial);
        Assert.Empty(playerList.Players);
    }

    [Fact]
    public async Task TestSaveAndLoadAsync()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        var playerList = new PlayerListJson
        {
            Selected = 1,
            Trial = new PlayerListJson.TrialPlayer { Id = "trial1", Code = "code", Token = "trialToken" }
        };
        playerList.Players.Add("dummy",
            new PlayerListJson.SavedPlayer { Number = 1, Name = "TestPlayer", Token = "dummyToken" });

        await playerList.SaveAsync();

        // Loading saved data
        var loaded = await PlayerListJson.LoadAsync();
        Assert.Equal(playerList.Selected, loaded.Selected);

        // Replace direct indexing with TryGetValue
        Assert.True(loaded.Players.TryGetValue("dummy", out var savedPlayer));
        Assert.Equal("TestPlayer", savedPlayer.Name);

        Assert.NotNull(loaded.Trial);
        Assert.Equal("trial1", loaded.Trial.Id);
    }

    [Fact]
    public async Task TestLoadName_UsesSavedPlayerLoginStrategy()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        // Arrange: Create a mock for SavedPlayerLoginStrategy.
        var mockLoginStrategy = new Mock<SavedPlayerLoginStrategy>("dummyToken") { CallBase = false };
        // Setup Step to return an AskPassword with Username "MockUser".
        mockLoginStrategy.Setup(s => s.Start())
            .ReturnsAsync(new AskPassword("MockUser"));

        // Override the factory to return the mock.
        var originalFactory = PlayerListJson.SavedPlayerLoginStrategyFactory;
        PlayerListJson.SavedPlayerLoginStrategyFactory = (_, _) => mockLoginStrategy.Object;

        var savedPlayer = new PlayerListJson.SavedPlayer { Number = 1, Token = "anyToken", Name = null };

        // Act: Call LoadName.
        await savedPlayer.LoadName();

        // Assert: Name should be updated from the mock.
        Assert.Equal("MockUser", savedPlayer.Name);
        // Cleanup: Reset the factory.
        PlayerListJson.SavedPlayerLoginStrategyFactory = originalFactory;
    }

    [Fact]
    public async Task TestSaveAsync_WhenPlayerNameIsNull_CallsLoadName()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        // Arrange: Create a mock for SavedPlayerLoginStrategy.
        var mockLoginStrategy = new Mock<SavedPlayerLoginStrategy>("dummyToken") { CallBase = false };
        mockLoginStrategy.Setup(s => s.Start())
            .ReturnsAsync(new AskPassword("MockUser"));

        // Override the factory to return the mock.
        var originalFactory = PlayerListJson.SavedPlayerLoginStrategyFactory;
        PlayerListJson.SavedPlayerLoginStrategyFactory = (_, _) => mockLoginStrategy.Object;

        var playerList = new PlayerListJson();
        playerList.Players.Add("dummy",
            new PlayerListJson.SavedPlayer { Number = 1, Token = "dummyToken", Name = null });

        // Act: Call SaveAsync.
        await playerList.SaveAsync();

        // Assert: Verify LoadName was called and Name was updated.
        Assert.True(playerList.Players.TryGetValue("dummy", out var savedPlayer));
        Assert.Equal("MockUser", savedPlayer.Name);

        // Cleanup: Reset the factory.
        PlayerListJson.SavedPlayerLoginStrategyFactory = originalFactory;
    }
}