namespace Flyingdarts.Persistence;

public interface IDynamoDbService
{
    Task<List<Game>> ReadGameAsync(long gameId, CancellationToken cancellationToken);
    Task<List<Game>> ReadStartedGameAsync(long gameId, CancellationToken cancellationToken);
    Task<List<GamePlayer>> ReadGamePlayersAsync(long gameId, CancellationToken cancellationToken);
    Task<List<GameDart>> ReadGameDartsAsync(long gameId, CancellationToken cancellationToken);
    Task<List<User>> ReadUsersAsync(string[] userIds, CancellationToken cancellationToken);
    Task<User> ReadUserAsync(string userId, CancellationToken cancellationToken);
    Task WriteUserAsync(User user, CancellationToken cancellationToken);
    Task WriteGameAsync(Game game, CancellationToken cancellationToken);
    Task WriteGamePlayerAsync(GamePlayer player, CancellationToken cancellationToken);
    Task WriteGameDartAsync(GameDart dart, CancellationToken cancellationToken);
}