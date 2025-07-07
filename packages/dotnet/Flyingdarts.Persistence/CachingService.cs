using System.Text.Json;

namespace Flyingdarts.Persistence;

public class CachingService<T> : ICachingService<T>
    where T : IGameState<T>
{
    private IDynamoDBContext DbContext;

    public CachingService() { }

    public CachingService(IDynamoDBContext dbContext)
    {
        DbContext = dbContext;
    }

    public T State { get; set; }

    public async Task Load(string gameId, CancellationToken cancellationToken)
    {
        var results = await DbContext
            .FromQueryAsync<T>(Query(gameId), OperationConfig)
            .GetRemainingAsync(cancellationToken);

        Console.WriteLine(JsonSerializer.Serialize(results));

        if (results.Any())
        {
            State = results.Single();
        }
    }

    public async Task Save(CancellationToken cancellationToken)
    {
        var stateWrite = DbContext.CreateBatchWrite<T>(OperationConfig);

        stateWrite.AddPutItem(State);

        await stateWrite.ExecuteAsync(cancellationToken);
    }

    private static QueryOperationConfig Query(string gameId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "X01State");
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, gameId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private DynamoDBOperationConfig OperationConfig
    {
        get
        {
            var stateType = typeof(T);
            var tableName =
                $"Flyingdarts-{stateType}-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}";
            return new DynamoDBOperationConfig { OverrideTableName = tableName };
        }
    }

    public void CreateInitial(T state, Game game)
    {
        State = state;
        AddGame(game);
    }

    public void AddGame(Game game)
    {
        State.Game = game;
    }

    public void AddPlayer(GamePlayer player)
    {
        if (!State.Players.Any(x => x.PlayerId == player.PlayerId))
            State.Players.Add(player);
    }

    public void AddDart(GameDart dart)
    {
        State.Darts.Add(dart);
    }

    public void AddUser(User user)
    {
        if (!State.Users.Any(x => x.UserId == user.UserId))
            State.Users.Add(user);
    }
}
