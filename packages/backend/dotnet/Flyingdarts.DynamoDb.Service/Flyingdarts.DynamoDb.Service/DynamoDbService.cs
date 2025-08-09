using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Flyingdarts.Persistence;
using Microsoft.Extensions.Options;

namespace Flyingdarts.DynamoDb.Service;

public class DynamoDbService : IDynamoDbService
{
    private IDynamoDBContext DbContext;
    private DynamoDBOperationConfig OperationConfig;

    public DynamoDbService(IDynamoDBContext dbContext, IOptions<ApplicationOptions> applicationOptions)
    {
        DbContext = dbContext;
        OperationConfig = applicationOptions.Value.ToOperationConfig();
    }

    public async Task<List<Game>> ReadGameAsync(long gameId, CancellationToken cancellationToken)
    {
        var games = await DbContext
            .FromQueryAsync<Game>(QueryGameConfig(gameId.ToString()), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        return games;
    }

    public async Task<Game?> GetOpenGameByUserIdAsync(long gameId, CancellationToken cancellationToken)
    {
        var games = await DbContext
            .FromQueryAsync<Game>(QueryGameConfigByGameCreator(gameId.ToString()), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        var lastGameHeader = games.OrderByDescending(x => x.CreationDate).FirstOrDefault();

        if (lastGameHeader is not null && lastGameHeader.Status == GameStatus.Qualifying)
        {
            return lastGameHeader;
        }
        return null;
    }

    public async Task<List<Game>> ReadStartedGameAsync(long gameId, CancellationToken cancellationToken)
    {
        var games = await DbContext
            .FromQueryAsync<Game>(QueryStartedGameConfig(gameId.ToString()), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        return games;
    }

    public async Task<List<GamePlayer>> ReadGamePlayersAsync(long gameId, CancellationToken cancellationToken)
    {
        var gamePlayers = await DbContext
            .FromQueryAsync<GamePlayer>(QueryGamePlayersConfig(gameId.ToString()), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        return gamePlayers;
    }

    public async Task<List<GameDart>> ReadGameDartsAsync(long gameId, CancellationToken cancellationToken)
    {
        var gameDarts = await DbContext
            .FromQueryAsync<GameDart>(QueryGameDartsConfig(gameId.ToString()), OperationConfig)
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
        var results = await DbContext
            .FromQueryAsync<User>(QueryUserConfigByUserId(userId), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        return results.FirstOrDefault() ?? throw new UserNotFoundException(nameof(userId), userId);
    }

    public async Task<User> ReadUserByAuthProviderUserIdAsync(string authProviderUserId, CancellationToken cancellationToken)
    {
        var results = await DbContext
            .FromQueryAsync<User>(QueryUserConfigByAuthProviderUserId(authProviderUserId), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        return results.FirstOrDefault() ?? throw new UserNotFoundException(nameof(authProviderUserId), authProviderUserId);
    }

    public async Task<User?> ReadUserByConnectionIdAsync(string connectionId, CancellationToken cancellationToken)
    {
        var results = await DbContext
            .FromQueryAsync<User>(QueryUserConfigByConnectionId(connectionId), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        return results.FirstOrDefault();
    }

    public async Task<List<User>> ReadAllUsersAsync(CancellationToken cancellationToken)
    {
        var results = await DbContext.FromQueryAsync<User>(QueryAllUsersConfig(), OperationConfig).GetRemainingAsync(cancellationToken);

        return results;
    }

    public class UserNotFoundException : Exception
    {
        public string SearchParam { get; }
        public string UserId { get; }

        public UserNotFoundException(string searchParam, string userId)
            : base($"User not found by {searchParam}: {userId}")
        {
            SearchParam = searchParam;
            UserId = userId;
        }
    }

    public async Task WriteUserAsync(User user, CancellationToken cancellationToken)
    {
        var userWrite = DbContext.CreateBatchWrite<User>(OperationConfig);

        userWrite.AddPutItem(user);

        await userWrite.ExecuteAsync(cancellationToken);
    }

    public async Task WriteGameAsync(Game game, CancellationToken cancellationToken)
    {
        var gameWrite = DbContext.CreateBatchWrite<Game>(OperationConfig);

        gameWrite.AddPutItem(game);

        await gameWrite.ExecuteAsync(cancellationToken);
    }

    public async Task WriteGamePlayerAsync(GamePlayer player, CancellationToken cancellationToken)
    {
        var write = DbContext.CreateBatchWrite<GamePlayer>(OperationConfig);

        write.AddPutItem(player);

        await write.ExecuteAsync(cancellationToken);
    }

    public async Task WriteGameDartAsync(GameDart dart, CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DEBUG] DynamoDbService.WriteGameDartAsync - Starting with dart: Id={dart.Id}, GameId={dart.GameId}, PlayerId={dart.PlayerId}, Score={dart.Score}, GameScore={dart.GameScore}"
        );
        Console.WriteLine(
            $"[DEBUG] DynamoDbService.WriteGameDartAsync - Dart details: PrimaryKey={dart.PrimaryKey}, SortKey={dart.SortKey}, LSI1={dart.LSI1}"
        );

        try
        {
            var write = DbContext.CreateBatchWrite<GameDart>(OperationConfig);
            Console.WriteLine($"[DEBUG] DynamoDbService.WriteGameDartAsync - Created batch write operation");

            write.AddPutItem(dart);
            Console.WriteLine($"[DEBUG] DynamoDbService.WriteGameDartAsync - Added dart to batch write");

            Console.WriteLine($"[DEBUG] DynamoDbService.WriteGameDartAsync - Executing batch write");
            await write.ExecuteAsync(cancellationToken);
            Console.WriteLine($"[DEBUG] DynamoDbService.WriteGameDartAsync - Batch write executed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] DynamoDbService.WriteGameDartAsync - Failed to write dart to database: {ex.Message}");
            Console.WriteLine($"[ERROR] DynamoDbService.WriteGameDartAsync - Exception type: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR] DynamoDbService.WriteGameDartAsync - Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private static QueryOperationConfig QueryUserConfigByUserId(string userId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, userId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryUserConfigByAuthProviderUserId(string authProviderUserId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("AuthProviderUserId", QueryOperator.Equal, authProviderUserId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryUserConfigByConnectionId(string connectionId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("ConnectionId", QueryOperator.Equal, connectionId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryAllUsersConfig()
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryGameConfig(string gameId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.Game);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, gameId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryGameConfigByGameCreator(string gameCreator)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.Game);
        queryFilter.AddCondition("LSI1", QueryOperator.BeginsWith, gameCreator);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryStartedGameConfig(string gameId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.Game);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, $"{gameId}#Started");
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

    public async Task PutGameAsync(Game game, CancellationToken cancellationToken)
    {
        var gameWrite = DbContext.CreateBatchWrite<Game>(OperationConfig);

        gameWrite.AddPutItem(game);

        await gameWrite.ExecuteAsync(cancellationToken);
    }
}
