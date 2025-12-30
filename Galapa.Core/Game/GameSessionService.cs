namespace Galapa.Core.Game;

public interface IGameSessionService
{
    Task StartAsync(string SessionId, int? PlayerNumber);
}

public class GameSessionService : IGameSessionService
{
    public Task StartAsync() => Task.CompletedTask;
}