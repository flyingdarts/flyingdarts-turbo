

namespace Flyingdarts.Metadata.Services.Services.X01;

/// <summary>
/// Service responsible for generating metadata for X01 darts games by aggregating data from multiple sources.
/// This service creates a comprehensive view of the game state including players, darts, scores, and game progression.
/// </summary>
public sealed class X01MetadataService : MetadataService<X01State>
{
    private readonly IDynamoDbService _dynamoDbService;

    /// <summary>
    /// Initializes a new instance of the ArtificialMetadataService class.
    /// </summary>
    /// <param name="dynamoDbService">The DynamoDB service for data access</param>
    /// <param name="cachingService">The caching service used to store the game state</param>
    public X01MetadataService(
        IDynamoDbService dynamoDbService,
        CachingService<X01State> cachingService
    )
        : base(cachingService)
    {
        _dynamoDbService =
            dynamoDbService ?? throw new ArgumentNullException(nameof(dynamoDbService));
    }

    /// <summary>
    /// Retrieves and constructs metadata for a specific X01 game.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game as a string</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>A complete metadata object containing game state, players, and darts information</returns>
    /// <exception cref="ArgumentException">Thrown when the game ID format is invalid</exception>
    /// <exception cref="InvalidOperationException">Thrown when the game is not found</exception>
    public async Task<Dtos.Metadata> GetMetadataAsync(
        string gameId,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        if (!long.TryParse(gameId, out var gameIdLong))
        {
            throw new ArgumentException("Game ID must be a valid long integer", nameof(gameId));
        }

        // Fetch all required data from DynamoDB in parallel for better performance
        var (game, players, darts, users) = await FetchGameDataAsync(gameIdLong, cancellationToken);

        var metadata = CreateMetadata(game, darts, players, users);

        // Check if any player has won the game
        if (
            HasAnyPlayerWon(
                darts,
                game.X01.Legs,
                game.X01.Sets,
                players.Select(p => p.PlayerId).ToList()
            )
        )
        {
            metadata.NextPlayer = null;
            metadata.WinningPlayer = GetWinningPlayer(
                darts,
                game.X01.Legs,
                game.X01.Sets,
                players.Select(p => p.PlayerId).ToList()
            );
        }

        return metadata;
    }

    /// <summary>
    /// Fetches all required game data from DynamoDB in parallel for optimal performance.
    /// </summary>
    private async Task<(
        Game game,
        List<GamePlayer> players,
        List<GameDart> darts,
        List<User> users
    )> FetchGameDataAsync(long gameId, CancellationToken cancellationToken)
    {
        var gameTask = _dynamoDbService.ReadGameAsync(gameId, cancellationToken);
        var playersTask = _dynamoDbService.ReadGamePlayersAsync(gameId, cancellationToken);
        var dartsTask = _dynamoDbService.ReadGameDartsAsync(gameId, cancellationToken);

        await Task.WhenAll(gameTask, playersTask, dartsTask);

        var games = await gameTask;
        var game =
            games.FirstOrDefault()
            ?? throw new InvalidOperationException($"Game with ID {gameId} not found");

        var players = await playersTask;
        var darts = await dartsTask;

        var userIds = players.Select(p => p.PlayerId).ToArray();
        var users = await _dynamoDbService.ReadUsersAsync(userIds, cancellationToken);

        return (game, players, darts, users);
    }

    /// <summary>
    /// Creates the complete metadata object from the provided game data.
    /// </summary>
    private static Dtos.Metadata CreateMetadata(
        Game game,
        List<GameDart> darts,
        List<GamePlayer> players,
        List<User> users
    )
    {
        var metadata = new Dtos.Metadata
        {
            Game = CreateGameDto(game),
            Darts = CreateDartsDictionary(darts, players),
            Players = CreatePlayersList(darts, players, users, game.X01.Legs),
            NextPlayer = DetermineNextPlayer(players, darts, game.X01.Legs)
        };

        // Remove darts after the last finishing dart for cleaner display
        RemoveDartsAfterLastFinisher(metadata.Darts, darts);

        return metadata;
    }

    /// <summary>
    /// Creates a GameDto from the Game entity.
    /// </summary>
    private static GameDto CreateGameDto(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        return new GameDto
        {
            Id = game.GameId.ToString(),
            PlayerCount = game.PlayerCount,
            Status = (GameStatusDto)(int)game.Status,
            Type = (GameTypeDto)(int)game.Type,
            X01 = new X01GameSettingsDto
            {
                DoubleIn = game.X01.DoubleIn,
                DoubleOut = game.X01.DoubleOut,
                Legs = game.X01.Legs,
                Sets = game.X01.Sets,
                StartingScore = game.X01.StartingScore
            }
        };
    }

    /// <summary>
    /// Creates a dictionary of darts organized by player ID using collection expressions.
    /// </summary>
    private static Dictionary<string, List<DartDto>> CreateDartsDictionary(
        List<GameDart> darts,
        List<GamePlayer> players
    )
    {
        if (darts is null or { Count: 0 } || players is null or { Count: 0 })
        {
            return new Dictionary<string, List<DartDto>>();
        }

        var dartsDictionary = new Dictionary<string, List<DartDto>>();

        foreach (var player in players)
        {
            var playerDarts = darts
                .Where(d => d.PlayerId == player.PlayerId)
                .OrderBy(d => d.CreatedAt)
                .Select(
                    d =>
                        new DartDto
                        {
                            Id = d.Id,
                            Score = d.Score,
                            GameScore = d.GameScore,
                            Set = d.Set,
                            Leg = d.Leg,
                            CreatedAt = d.CreatedAt.Ticks
                        }
                )
                .ToList();

            dartsDictionary[player.PlayerId] = playerDarts;
        }

        return dartsDictionary;
    }

    /// <summary>
    /// Creates a list of PlayerDto objects with calculated statistics.
    /// </summary>
    private static IOrderedEnumerable<PlayerDto> CreatePlayersList(
        List<GameDart> darts,
        List<GamePlayer> players,
        List<User> users,
        int bestOfLegs
    )
    {
        if (players is null or { Count: 0 } || users is null or { Count: 0 })
        {
            return Enumerable.Empty<PlayerDto>().OrderBy(p => p.CreatedAt);
        }

        var playersStats = players
            .Select(p => CalculatePlayerStats(darts, p.PlayerId, bestOfLegs))
            .ToList();
        var shouldResetLegs = playersStats.Any(stat => stat.ShouldResetLegs);

        var playerDtos = players.Select(player =>
        {
            var stats = CalculatePlayerStats(darts, player.PlayerId, bestOfLegs);
            var user =
                users.FirstOrDefault(u => u.UserId == player.PlayerId)
                ?? throw new InvalidOperationException(
                    $"User not found for player {player.PlayerId}"
                );

            return new PlayerDto
            {
                PlayerId = player.PlayerId,
                PlayerName = user.Profile.UserName,
                Country = user.Profile.Country.ToLowerInvariant(),
                CreatedAt = player.PlayerId,
                Sets = stats.SetsWon.ToString(),
                Legs = shouldResetLegs ? "0" : stats.LegsWon.ToString()
            };
        });

        return playerDtos.OrderBy(p => p.CreatedAt);
    }

    /// <summary>
    /// Removes darts that were thrown after the last finishing dart for cleaner display.
    /// </summary>
    private static void RemoveDartsAfterLastFinisher(
        Dictionary<string, List<DartDto>>? dartsDictionary,
        List<GameDart> darts
    )
    {
        if (dartsDictionary is null or { Count: 0 } || darts is null or { Count: 0 })
        {
            return;
        }

        try
        {
            var lastFinisher = darts.Where(d => d.GameScore == 0).OrderBy(d => d.CreatedAt).Last();

            var lastFinisherTicks = lastFinisher.CreatedAt.Ticks;

            foreach (var playerId in dartsDictionary.Keys)
            {
                dartsDictionary[playerId] = dartsDictionary[playerId]
                    .Where(d => d.CreatedAt > lastFinisherTicks)
                    .ToList();
            }
        }
        catch (InvalidOperationException)
        {
            // No finishing dart found, keep all darts
        }
    }

    /// <summary>
    /// Determines if any player has won the game based on the required sets.
    /// </summary>
    private static bool HasAnyPlayerWon(
        List<GameDart> darts,
        int legsRequired,
        int bestOfSets,
        List<string> playerIds
    )
    {
        ArgumentNullException.ThrowIfNull(playerIds);

        if (darts is { Count: 0 })
        {
            return false;
        }

        var setsRequiredToWin = (bestOfSets + 1) / 2;

        return playerIds.Any(playerId =>
        {
            var playerStats = CalculatePlayerStats(darts, playerId, legsRequired);
            return playerStats.SetsWon >= setsRequiredToWin;
        });
    }

    /// <summary>
    /// Gets the winning player ID if any player has won the game.
    /// </summary>
    private static string? GetWinningPlayer(
        List<GameDart> darts,
        int bestOfLegs,
        int bestOfSets,
        List<string> playerIds
    )
    {
        ArgumentNullException.ThrowIfNull(playerIds);

        if (darts is { Count: 0 })
        {
            return null;
        }

        var setsRequiredToWin = (bestOfSets + 1) / 2;

        return playerIds.FirstOrDefault(playerId =>
        {
            var playerStats = CalculatePlayerStats(darts, playerId, bestOfLegs);
            return playerStats.SetsWon >= setsRequiredToWin;
        });
    }

    /// <summary>
    /// Calculates player statistics including sets won, legs won, and whether legs should be reset.
    /// </summary>
    private static PlayerStats CalculatePlayerStats(
        List<GameDart> darts,
        string playerId,
        int bestOfLegs
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(playerId);

        if (darts is { Count: 0 })
        {
            return new PlayerStats(0, 0, false);
        }

        var sets = darts.GroupBy(dart => dart.Set).ToList();
        var currentSet = darts.Max(dart => dart.Set);

        var totalSetsWon = 0;
        var currentLegsWon = 0;
        var shouldResetLegs = false;

        foreach (var set in sets)
        {
            var legsToWinSet = (bestOfLegs + 1) / 2;
            var playerLegWins = CalculateLegsInCurrentSet(set.ToList());

            if (playerLegWins.TryGetValue(playerId, out var legsWon) && legsWon >= legsToWinSet)
            {
                totalSetsWon++;

                // Only reset legs if this is the current/last set that was just won
                if (set.Key == currentSet)
                {
                    currentLegsWon = 0;
                    shouldResetLegs = true;
                }
            }
            else if (set.Key == currentSet)
            {
                currentLegsWon = playerLegWins.GetValueOrDefault(playerId, 0);
            }
        }

        return new PlayerStats(totalSetsWon, currentLegsWon, shouldResetLegs);
    }

    /// <summary>
    /// Calculates the number of legs won by each player in the current set.
    /// </summary>
    private static Dictionary<string, int> CalculateLegsInCurrentSet(List<GameDart> darts)
    {
        if (darts is { Count: 0 })
        {
            return new Dictionary<string, int>();
        }

        var currentSet = darts.Max(dart => dart.Set);
        var legsInCurrentSet = darts
            .Where(dart => dart.Set == currentSet)
            .GroupBy(dart => dart.Leg);

        var playerLegWins = new Dictionary<string, int>();

        foreach (var leg in legsInCurrentSet)
        {
            var winningPlayer = leg.FirstOrDefault(dart => dart.GameScore == 0)?.PlayerId;
            if (!string.IsNullOrEmpty(winningPlayer))
            {
                playerLegWins[winningPlayer] =
                    playerLegWins.GetValueOrDefault(winningPlayer, 0) + 1;
            }
        }

        return playerLegWins;
    }

    /// <summary>
    /// Determines which player should throw next based on the current game state.
    /// </summary>
    private static string? DetermineNextPlayer(
        List<GamePlayer> players,
        List<GameDart> darts,
        int bestOfLegs
    )
    {
        if (players is null or { Count: 0 } || players.Count != 2)
        {
            return null;
        }

        if (darts is { Count: 0 })
        {
            // Game start: pick a random player
            return players[Random.Shared.Next(0, 2)].PlayerId;
        }

        var lastDart = darts.Last();

        if (lastDart.GameScore == 0)
        {
            // Leg just finished
            return DetermineNextPlayerAfterLegFinish(players, darts, lastDart, bestOfLegs);
        }

        // Ongoing leg: alternate players
        var playerThatLastThrew = lastDart.PlayerId;
        return players.First(p => p.PlayerId != playerThatLastThrew).PlayerId;
    }

    /// <summary>
    /// Determines the next player after a leg has finished.
    /// </summary>
    private static string DetermineNextPlayerAfterLegFinish(
        List<GamePlayer> players,
        List<GameDart> darts,
        GameDart lastDart,
        int bestOfLegs
    )
    {
        var lastSet = lastDart.Set;
        var lastLeg = lastDart.Leg;

        // Check if this leg was set-deciding
        var currentSetDarts = darts.Where(x => x.Set == lastSet).ToList();
        var playerLegWins = CalculateLegsInCurrentSet(currentSetDarts);
        var legsToWinSet = (bestOfLegs + 1) / 2;

        var setWasWon = playerLegWins.Any(kvp => kvp.Value >= legsToWinSet);

        if (setWasWon)
        {
            // Set was just won - find who started this set and the other player starts next set
            var firstLegOfSet = currentSetDarts.OrderBy(x => x.CreatedAt).First();
            var setStarterId = firstLegOfSet.PlayerId;
            return players.First(x => x.PlayerId != setStarterId).PlayerId;
        }

        // Regular leg finish within a set
        var dartsInLastLeg = darts
            .Where(x => x.Set == lastSet && x.Leg == lastLeg)
            .OrderBy(x => x.CreatedAt)
            .ToList();

        var starterId = dartsInLastLeg.First().PlayerId;
        var winnerId = dartsInLastLeg.Last(x => x.GameScore == 0).PlayerId;

        return winnerId == starterId
            ? players.First(x => x.PlayerId != winnerId).PlayerId // If winner started, other player starts next leg
            : winnerId; // If winner did not start, winner starts next leg
    }

    /// <summary>
    /// Represents player statistics for a game.
    /// </summary>
    /// <param name="SetsWon">Number of sets won by the player</param>
    /// <param name="LegsWon">Number of legs won in the current set</param>
    /// <param name="ShouldResetLegs">Whether legs should be reset (set was just won)</param>
    private readonly record struct PlayerStats(int SetsWon, int LegsWon, bool ShouldResetLegs);
}
