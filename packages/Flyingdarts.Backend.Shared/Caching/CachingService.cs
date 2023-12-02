using Amazon.DynamoDBv2.DocumentModel;
using Flyingdarts.Persistence;

namespace Flyingdarts.Backend.Shared.Caching;

public class CachingService<T> : ICachingService<T> where T : IGameState
{
    private IDynamoDBContext DbContext;
    
    public CachingService(IDynamoDBContext dbContext)
    {
        DbContext = dbContext;
    }

    public T State;

    public async Task Load(string gameId, CancellationToken cancellationToken)
    {
        var results = await DbContext.FromQueryAsync<T>(Query(gameId), OperationConfig)
            .GetRemainingAsync(cancellationToken);
        State = results.Single();
    }
    
    public async Task Save(CancellationToken cancellationToken)
    {
        var stateWrite = DbContext.CreateBatchWrite<T>(OperationConfig);

        stateWrite.AddPutItem(State);

        await stateWrite.ExecuteAsync(cancellationToken);
    }
    
    private static QueryOperationConfig Query(string gameId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, gameId);
        return new QueryOperationConfig { Filter = queryFilter };
    }   

    private DynamoDBOperationConfig OperationConfig
    {
        get
        {
            var stateType = typeof(T);
            var tableName = $"Flyingdarts-{stateType}-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}";
            return new DynamoDBOperationConfig { OverrideTableName = tableName };
        }
    }

    public void AddGame(Game game)
    {
        State.Game = game;
    }

    public void AddPlayer(GamePlayer player)
    {
        State.Players.Add(player);
    }

    public void AddDart(GameDart dart)
    {
        State.Darts.Add(dart);
    }

    public void AddUser(User user)
    {
        State.Users.Add(user);
    }
}