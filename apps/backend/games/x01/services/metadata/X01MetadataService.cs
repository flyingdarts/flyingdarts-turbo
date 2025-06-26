using Flyingdarts.Backend.Games.X01.Services.Metadata.Dtos;
using Flyingdarts.Persistence;
using System.Text.Json;

public class X01MetadataService : MetadataService<X01State>
{
    private X01State GameState;
    private readonly CachingService<X01State> _cacheService;
    public X01MetadataService(CachingService<X01State> cachingService) : base(cachingService)
    {
        _cacheService = cachingService;
    }

    public async Task<Metadata> GetMetadata(string gameId, CancellationToken cancellationToken)
    {
        await _cacheService.Load(gameId, cancellationToken);

        GameState = _cacheService.State;

        return Metadata;
    }

    private Metadata Metadata
    {
        get
        {
            Console.WriteLine(JsonSerializer.Serialize(GameState));
            var metadata = CreateMetaData(GameState.Game, GameState.Darts, GameState.Players, GameState.Users);

            if (HasAnyPlayerWon(GameState.Darts, GameState.Game!.X01.Legs, GameState.Game.X01.Sets, GameState.Players.Select(x => x.PlayerId.ToString()).ToList()))
            {
                metadata!.NextPlayer = null;
                metadata.WinningPlayer = GetWinningPlayer(GameState.Darts, GameState.Game.X01.Legs,
                    GameState.Game.X01.Sets, GameState.Players.Select(x => x.PlayerId.ToString()).ToList());
            }

            return metadata;
        }
    }

    private static Metadata CreateMetaData(Game game, List<GameDart> darts, List<GamePlayer> players, List<User> users)
    {
        Metadata data = new Metadata();

        if (game is not null)
        {
            data.Game = new GameDto
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

        if (darts is not null && darts.Any())
        {
            data.Darts = new();
            players.ForEach(p =>
            {
                data.Darts.Add(p.PlayerId, new());
                data.Darts[p.PlayerId] = darts
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.PlayerId == p.PlayerId)
                    .Select(x => new DartDto
                    {
                        Id = x.Id,
                        Score = x.Score,
                        GameScore = x.GameScore,
                        Set = x.Set,
                        Leg = x.Leg,
                        CreatedAt = x.CreatedAt.Ticks
                    })
                    .ToList();
            });
        }

        if (players is not null && players.Any())
        {
            var orderedPlayers = players.Select(x =>
            {
                var stats = CalculatePlayerStats(darts, x.PlayerId, game.X01.Legs);
                var newPlayer = new PlayerDto
                {
                    PlayerId = x.PlayerId,
                    PlayerName = users.Single(y => y.UserId == x.PlayerId).Profile.UserName,
                    Country = users.Single(y => y.UserId == x.PlayerId).Profile.Country.ToLower(),
                    CreatedAt = x.PlayerId,
                    Legs = stats.legsWon.ToString(),
                    Sets = stats.setsWon.ToString()
                };
                return newPlayer;
            }).OrderBy(x => x.CreatedAt);

            if (orderedPlayers.Any((x) => CalculatePlayerStats(darts, x.PlayerId, game.X01.Legs).shouldResetLegs))
            {
                foreach (var orderedPlayer in orderedPlayers)
                {
                    orderedPlayer.Legs = "0";
                }
            }

            data.Players = orderedPlayers;
        }

        EnsureLegsReset(data, darts ?? new List<GameDart>());

        data.NextPlayer = DetermineNextPlayer(players, darts);

        try
        {
            var lastFinisher = darts!.OrderBy(x => x.CreatedAt).Last(x => x.GameScore == 0);
            data.Darts[data.Darts.Keys.First()] =
                data.Darts[data.Darts.Keys.First()].Where(x => x.CreatedAt > lastFinisher.CreatedAt.Ticks).ToList();

            data.Darts[data.Darts.Keys.Last()] =
                data.Darts[data.Darts.Keys.Last()].Where(x => x.CreatedAt > lastFinisher.CreatedAt.Ticks).ToList();
        }
        catch { }

        return data;
    }

    private static bool HasPlayerWonLastSet(List<GameDart> darts, string playerId, int bestOfLegs)
    {
        // Find the last set number
        int lastSetNumber = darts.Max(dart => dart.Set);

        // Filter darts for the last set
        var lastSetDarts = darts.Where(dart => dart.Set == lastSetNumber).ToList();

        // Calculate the number of legs needed to win the set
        int legsToWinSet = (bestOfLegs + 1) / 2;

        // Calculate legs won for all players in the last set
        var playerLegWins = CalculateLegsInCurrentSet(lastSetDarts);

        // Check if the specified player has won the last set
        return playerLegWins.ContainsKey(playerId) && playerLegWins[playerId] >= legsToWinSet;
    }

    private static bool HasAnyPlayerWon(List<GameDart> darts, int legsRequired, int bestOfSets, List<string> playerIds)
    {
        foreach (var playerId in playerIds)
        {
            int setsWonByPlayer = CalculatePlayerStats(darts, playerId, legsRequired).setsWon;
            int setsRequiredToWin = (bestOfSets + 1) / 2;

            // If any player has won the required number of sets in a "best of" scenario, the game is won.
            if (setsWonByPlayer >= setsRequiredToWin)
            {
                return true;
            }
        }

        // If no player has won enough sets, the game is still ongoing.
        return false;
    }

    private static string GetWinningPlayer(List<GameDart> darts, int bestOfLegs, int bestOfSets, List<string> playerIds)
    {
        foreach (var playerId in playerIds)
        {
            int setsWonByPlayer = CalculatePlayerStats(darts, playerId, bestOfLegs).setsWon;
            int setsRequiredToWin = (bestOfSets + 1) / 2;

            // If the player has won the required number of sets in a "best of" scenario, they've won the game.
            if (setsWonByPlayer >= setsRequiredToWin)
            {
                return playerId;
            }
        }

        // If no player has won enough sets, return null to indicate that the game is still ongoing.
        return null;
    }

    private static (int setsWon, int legsWon, bool shouldResetLegs) CalculatePlayerStats(List<GameDart> darts, string playerId, int bestOfLegs)
    {
        var sets = darts.GroupBy(dart => dart.Set);

        int totalSetsWon = 0;
        int currentLegsWon = 0;
        bool shouldResetLegs = false;

        foreach (var set in sets)
        {
            int legsToWinSet = (bestOfLegs + 1) / 2;

            // Calculate legs won for all players in the current set
            var playerLegWins = CalculateLegsInCurrentSet(set.ToList());

            // Check if the specified player has won the set
            if (playerLegWins.ContainsKey(playerId) && playerLegWins[playerId] >= legsToWinSet)
            {
                totalSetsWon++;
                currentLegsWon = 0; // Reset legs won since the set is won
                shouldResetLegs = true;
            }
            else
            {
                // Update current legs won for the player
                currentLegsWon = playerLegWins.ContainsKey(playerId) ? playerLegWins[playerId] : 0;
            }
        }

        return (totalSetsWon, currentLegsWon, shouldResetLegs);
    }

    private static Dictionary<string, int> CalculateLegsInCurrentSet(List<GameDart> darts)
    {
        var legsInCurrentSet = darts.GroupBy(dart => dart.Leg);

        Dictionary<string, int> playerLegWins = new Dictionary<string, int>();

        foreach (var leg in legsInCurrentSet)
        {
            // Find the player who won the leg
            var winningPlayer = leg.FirstOrDefault(dart => dart.GameScore == 0)?.PlayerId;

            if (winningPlayer != null)
            {
                if (!playerLegWins.ContainsKey(winningPlayer))
                {
                    playerLegWins[winningPlayer] = 0;
                }

                playerLegWins[winningPlayer]++;
            }
        }

        return playerLegWins;
    }


    private static void EnsureLegsReset(Metadata metadata, List<GameDart> gameDarts)
    {
        // Check if there are players and darts data available
        if (metadata.Players == null || !metadata.Players.Any() || !gameDarts.Any())
        {
            return;
        }

        // Get the list of player IDs
        var playerIds = metadata.Players.Select(p => p.PlayerId).ToList();

        // Assuming all players have the same game settings, get the first player's game settings
        var gameSettings = metadata.Game.X01;

        // Iterate over each player to check if they have won the last set
        foreach (var playerId in playerIds)
        {
            // Check if the current player has won the last set
            if (HasPlayerWonLastSet(gameDarts, playerId, gameSettings.Legs))
            {
                // Reset the legs for the other player(s)
                foreach (var otherPlayerId in playerIds.Where(id => id != playerId))
                {
                    var playerDto = metadata.Players.FirstOrDefault(p => p.PlayerId == otherPlayerId);
                    if (playerDto != null)
                    {
                        playerDto.Legs = "0";
                    }
                }
                break; // Exit the loop once a winner is found and legs are reset
            }
        }
    }

    private static void SetRandomStartingPlayer(Metadata metadata)
    {
        var random = new Random();
        int index = random.Next(0, 2);
        metadata.NextPlayer = metadata.Players.ToArray()[index].PlayerId;
    }
    private static string DetermineNextPlayer(List<GamePlayer> players, List<GameDart> darts)
    {
        string nextPlayer = null;

        if (players.Any() && players.Count == 2)
        {
            if (!darts.Any())
            {
                // Game start: pick a random player
                nextPlayer = players[new Random().Next(0, 2)].PlayerId;
            }
            else if (darts.Last().GameScore == 0)
            {
                // Leg just finished
                var lastDart = darts.Last();
                var lastSet = lastDart.Set;
                var lastLeg = lastDart.Leg;

                // All darts in the just-finished leg
                var dartsInLastLeg = darts.Where(x => x.Set == lastSet && x.Leg == lastLeg).OrderBy(x => x.CreatedAt).ToList();
                var starterId = dartsInLastLeg.First().PlayerId;
                var winnerId = dartsInLastLeg.Last(x => x.GameScore == 0).PlayerId;

                if (winnerId == starterId)
                {
                    // If winner started, the other player starts next leg
                    nextPlayer = players.First(x => x.PlayerId != winnerId).PlayerId;
                }
                else
                {
                    // If winner did not start, winner starts next leg
                    nextPlayer = winnerId;
                }
            }
            else
            {
                // Ongoing leg: alternate
                var playerThatLastThrew = darts.Last().PlayerId;
                nextPlayer = players.First(x => x.PlayerId != playerThatLastThrew).PlayerId;
            }
        }
        return nextPlayer;
    }

}

