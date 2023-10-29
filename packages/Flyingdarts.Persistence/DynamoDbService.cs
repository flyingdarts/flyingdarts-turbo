using Flyingdarts.Shared;
namespace Flyingdarts.Persistence;
public record DynamoDbService(IDynamoDBContext DbContext, IOptions<ApplicationOptions> ApplicationOptions) : IDynamoDbService
{
    public async Task<Game> ReadGameAsync(long gameId, CancellationToken cancellationToken)
    {
        var games = await DbContext.FromQueryAsync<Game>(QueryGameConfig(gameId.ToString()), ApplicationOptions.Value.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);
        return games.Single();
    }

    public async Task<List<GamePlayer>> ReadGamePlayersAsync(long gameId, CancellationToken cancellationToken)
    {
        var gamePlayers = await DbContext.FromQueryAsync<GamePlayer>(QueryGamePlayersConfig(gameId.ToString()), ApplicationOptions.Value.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);
        return gamePlayers.ToList();
    }

    public async Task<List<GameDart>> ReadGameDartsAsync(long gameId, CancellationToken cancellationToken)
    {
        var gameDarts = await DbContext.FromQueryAsync<GameDart>(QueryGameDartsConfig(gameId.ToString()), ApplicationOptions.Value.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);
        return gameDarts.ToList();
    }

    public async Task<List<User>> ReadUsersAsync(string[] userIds, CancellationToken cancellationToken)
    {
        List<User> users = new List<User>();
        for (var i = 0; i < userIds.Length; i++)
        {
            users.Add(await ReadUserAsync(userIds[i], cancellationToken));
        }
        return users;
    }
    public async Task<User> ReadUserAsync(string userId, CancellationToken cancellationToken)
    {
        var results = await DbContext.FromQueryAsync<User>(QueryUserConfig(userId))
                                     .GetRemainingAsync(cancellationToken);

        return results.Single();
    }
    public async Task WriteUserAsync(User user, CancellationToken cancellationToken)
    {
        var userWrite = DbContext.CreateBatchWrite<User>(ApplicationOptions.Value.ToOperationConfig());

        userWrite.AddPutItem(user);

        await userWrite.ExecuteAsync(cancellationToken);
    }
    public async Task WriteGameAsync(Game game, CancellationToken cancellationToken)
    {
        var gameWrite = DbContext.CreateBatchWrite<Game>(ApplicationOptions.Value.ToOperationConfig());

        gameWrite.AddPutItem(game);

        await gameWrite.ExecuteAsync(cancellationToken);
    }
    public async Task WriteGamePlayerAsync(GamePlayer player, CancellationToken cancellationToken)
    {
        var write = DbContext.CreateBatchWrite<GamePlayer>(ApplicationOptions.Value.ToOperationConfig());

        write.AddPutItem(player);

        await write.ExecuteAsync(cancellationToken);
    }

    public async Task WriteGameDartAsync(GameDart dart, CancellationToken cancellationToken)
    {
        var write = DbContext.CreateBatchWrite<GameDart>(ApplicationOptions.Value.ToOperationConfig());

        write.AddPutItem(dart);

        await write.ExecuteAsync(cancellationToken);
    }
    private static QueryOperationConfig QueryUserConfig(string userId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, userId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryGameConfig(string gameId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.Game);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, gameId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryGamePlayersConfig(string gameId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.GamePlayer);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, gameId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryGameDartsConfig(string gameId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.GameDart);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, gameId);
        return new QueryOperationConfig { Filter = queryFilter };
    }
}