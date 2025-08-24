using System.Text.Json;

namespace Flyingdarts.Persistence;

public class CachingService<T> : ICachingService<T>
    where T : IGameState<T>
{
    private IDynamoDBContext DbContext = null!;

    public CachingService() { }

    public CachingService(IDynamoDBContext dbContext)
    {
        DbContext = dbContext;
    }

    public T State { get; set; } = default!;

    public async Task Load(string gameId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[DEBUG] CachingService.Load - Starting with gameId: {gameId}");

        try
        {
            var results = await DbContext
                .FromQueryAsyncCompat<T>(Query(gameId), OperationConfig)
                .GetRemainingAsync(cancellationToken);

            Console.WriteLine(
                $"[DEBUG] CachingService.Load - Query completed, found {results.Count()} results"
            );
            Console.WriteLine(
                $"[DEBUG] CachingService.Load - Query results: {JsonSerializer.Serialize(results)}"
            );

            if (results != null && results.Any())
            {
                State = results.Single();
                Console.WriteLine(
                    $"[DEBUG] CachingService.Load - State loaded successfully. GameId={State.Game?.GameId}, Players={State.Players?.Count ?? 0}, Darts={State.Darts?.Count ?? 0}"
                );
            }
            else
            {
                Console.WriteLine(
                    $"[WARNING] CachingService.Load - No state found for gameId: {gameId}"
                );
                State = default!;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ERROR] CachingService.Load - Failed to load state for gameId {gameId}: {ex.Message}"
            );
            Console.WriteLine($"[ERROR] CachingService.Load - Exception type: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR] CachingService.Load - Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task Save(CancellationToken cancellationToken)
    {
        Console.WriteLine($"[DEBUG] CachingService.Save - Starting");
        Console.WriteLine($"[DEBUG] CachingService.Save - Current State is null: {State == null}");

        if (State == null)
        {
            Console.WriteLine($"[ERROR] CachingService.Save - State is null, cannot save");
            throw new InvalidOperationException("State is null, cannot save");
        }

        Console.WriteLine(
            $"[DEBUG] CachingService.Save - State details: GameId={State.Game?.GameId}, Players={State.Players?.Count ?? 0}, Darts={State.Darts?.Count ?? 0}, Users={State.Users?.Count ?? 0}"
        );

        try
        {
            var stateWrite = DbContext.CreateBatchWriteCompat<T>(OperationConfig);
            Console.WriteLine($"[DEBUG] CachingService.Save - Created batch write operation");

            stateWrite.AddPutItem(State);
            Console.WriteLine($"[DEBUG] CachingService.Save - Added state to batch write");

            Console.WriteLine($"[DEBUG] CachingService.Save - Executing batch write");
            await stateWrite.ExecuteAsync(cancellationToken);
            Console.WriteLine($"[DEBUG] CachingService.Save - Batch write executed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] CachingService.Save - Failed to save state: {ex.Message}");
            Console.WriteLine($"[ERROR] CachingService.Save - Exception type: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR] CachingService.Save - Stack trace: {ex.StackTrace}");
            throw;
        }
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
                $"Flyingdarts-{stateType.Name}-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}";
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
        Console.WriteLine(
            $"[DEBUG] CachingService.AddDart - Starting with dart: Id={dart.Id}, GameId={dart.GameId}, PlayerId={dart.PlayerId}"
        );
        Console.WriteLine(
            $"[DEBUG] CachingService.AddDart - Current State is null: {State == null}"
        );

        if (State == null)
        {
            Console.WriteLine($"[ERROR] CachingService.AddDart - State is null, cannot add dart");
            throw new InvalidOperationException("State is null, cannot add dart");
        }

        var currentState = State;
        Console.WriteLine(
            $"[DEBUG] CachingService.AddDart - Current darts count before adding: {currentState?.Darts?.Count ?? 0}"
        );
        currentState!.Darts.Add(dart);
        Console.WriteLine(
            $"[DEBUG] CachingService.AddDart - Dart added successfully. New darts count: {currentState.Darts.Count}"
        );
    }

    public void AddUser(User user)
    {
        if (State != null && !State.Users.Any(x => x.UserId == user.UserId))
            State.Users.Add(user);
    }
}
