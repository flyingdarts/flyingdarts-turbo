using Flyingdarts.Shared;
using System.Text.Json;
using StackExchange.Redis;
using Amazon.ElastiCache.Model;
using Amazon.ElastiCache;

namespace Flyingdarts.Persistence;
public class DynamoDbService : IDynamoDbService
{
    private IDatabase Cache;
    private IDynamoDBContext DbContext;
    private IOptions<ApplicationOptions> ApplicationOptions;
    public DynamoDbService(IRedisService redisService, IDynamoDBContext dbContext, IOptions<ApplicationOptions> applicationOptions)
    {
        this.Cache = redisService.GetDatabase();
        this.DbContext = dbContext;
        this.ApplicationOptions = applicationOptions;
    }

    public async Task<List<Game>> ReadGameAsync(long gameId, CancellationToken cancellationToken)
    {
        var cacheKey = $"Game#{gameId}";
        var cachedValue = Cache.StringGet(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return JsonSerializer.Deserialize<List<Game>>(cachedValue);
        }

        var games = await DbContext.FromQueryAsync<Game>(QueryGameConfig(gameId.ToString()), ApplicationOptions.Value.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);

        Cache.StringSet(cacheKey, JsonSerializer.Serialize(games), TimeSpan.FromMinutes(30));

        return games;
    }

    public async Task<List<GamePlayer>> ReadGamePlayersAsync(long gameId, CancellationToken cancellationToken)
    {
        var cacheKey = $"GamePlayers#{gameId}";
        var cachedValue = Cache.StringGet(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return JsonSerializer.Deserialize<List<GamePlayer>>(cachedValue);
        }

        var gamePlayers = await DbContext.FromQueryAsync<GamePlayer>(QueryGamePlayersConfig(gameId.ToString()), ApplicationOptions.Value.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);

        Cache.StringSet(cacheKey, JsonSerializer.Serialize(gamePlayers), TimeSpan.FromMinutes(30));

        return gamePlayers;
    }

    public async Task<List<GameDart>> ReadGameDartsAsync(long gameId, CancellationToken cancellationToken)
    {
        var cacheKey = $"GameDarts#{gameId}";
        var cachedValue = Cache.StringGet(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return JsonSerializer.Deserialize<List<GameDart>>(cachedValue);
        }

        var gameDarts = await DbContext.FromQueryAsync<GameDart>(QueryGameDartsConfig(gameId.ToString()), ApplicationOptions.Value.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);

        Cache.StringSet(cacheKey, JsonSerializer.Serialize(gameDarts), TimeSpan.FromMinutes(30));

        return gameDarts.ToList();
    }

    public async Task<List<User>> ReadUsersAsync(string[] userIds, CancellationToken cancellationToken)
    {
        List<User> users = new List<User>();
        for (var i = 0; i < userIds.Length; i++)
        {
            var cacheKey = $"Users#{userIds[i]}";
            var cachedValue = Cache.StringGet(cacheKey);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                return JsonSerializer.Deserialize<List<User>>(cachedValue);
            }

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
        string cacheKey = $"Users#{user.UserId}";
        string cachedValue = Cache!.StringGet(cacheKey)!;
        var users = new List<User>();

        if (!string.IsNullOrEmpty(cachedValue))
        {
            users = JsonSerializer.Deserialize<List<User>>(cachedValue!);
        }

        users!.Add(user);

        Cache.StringSet(cacheKey, JsonSerializer.Serialize(users), TimeSpan.FromMinutes(30));

        var userWrite = DbContext.CreateBatchWrite<User>(ApplicationOptions.Value.ToOperationConfig());

        userWrite.AddPutItem(user);

        await userWrite.ExecuteAsync(cancellationToken);
    }
    public async Task WriteGameAsync(Game game, CancellationToken cancellationToken)
    {
        var cacheKey = $"Game#{game.GameId}";
        var cachedValue = Cache.StringGet(cacheKey);
        var games = new List<Game>();

        if (!string.IsNullOrEmpty(cachedValue))
        {
            games = JsonSerializer.Deserialize<List<Game>>(cachedValue!);
        }

        games!.Add(game);

        Cache.StringSet(cacheKey, JsonSerializer.Serialize(games), TimeSpan.FromMinutes(30));

        var gameWrite = DbContext.CreateBatchWrite<Game>(ApplicationOptions.Value.ToOperationConfig());

        gameWrite.AddPutItem(game);

        await gameWrite.ExecuteAsync(cancellationToken);
    }
    public async Task WriteGamePlayerAsync(GamePlayer player, CancellationToken cancellationToken)
    {
        var cacheKey = $"GamePlayers#{player.GameId}";
        var cachedValue = Cache.StringGet(cacheKey);
        var players = new List<GamePlayer>();

        if (!string.IsNullOrEmpty(cachedValue))
        {
            players = JsonSerializer.Deserialize<List<GamePlayer>>(cachedValue!);
        }

        players!.Add(player);

        Cache.StringSet(cacheKey, JsonSerializer.Serialize(players), TimeSpan.FromMinutes(30));

        var write = DbContext.CreateBatchWrite<GamePlayer>(ApplicationOptions.Value.ToOperationConfig());

        write.AddPutItem(player);

        await write.ExecuteAsync(cancellationToken);
    }

    public async Task WriteGameDartAsync(GameDart dart, CancellationToken cancellationToken)
    {
        var cacheKey = $"GameDarts#{dart.GameId}";
        var cachedValue = Cache.StringGet(cacheKey);
        var darts = new List<GameDart>();

        if (!string.IsNullOrEmpty(cachedValue))
        {
            darts = JsonSerializer.Deserialize<List<GameDart>>(cachedValue!);
        }

        darts!.Add(dart);

        Cache.StringSet(cacheKey, JsonSerializer.Serialize(darts), TimeSpan.FromMinutes(30));

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