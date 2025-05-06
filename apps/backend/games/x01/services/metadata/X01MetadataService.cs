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
                int currentSet = darts != null && darts.Any() ? darts.Max(d => d.Set) : 1; // Assuming current set is the highest set number
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

            data.Players = orderedPlayers;
        }


        DetermineNextPlayer(data);

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

    private static (int setsWon, int legsWon) CalculatePlayerStats(List<GameDart> darts, string playerId, int bestOfLegs)
    {
        var sets = darts.GroupBy(dart => dart.Set);

        int totalSetsWon = 0;
        int currentLegsWon = 0;

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
            }
            else
            {
                // Update current legs won for the player
                currentLegsWon = playerLegWins.ContainsKey(playerId) ? playerLegWins[playerId] : 0;
            }
        }

        return (totalSetsWon, currentLegsWon);
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




    private static void DetermineNextPlayer(Metadata metadata)
    {
        if (metadata.Players != null && metadata.Players.Any() && metadata.Players.Count() == 2)
        {
            if (metadata.Darts == null || !metadata.Darts.Any())
            {
                metadata.NextPlayer = metadata.Players.OrderBy(x => x.CreatedAt).First().PlayerId;
                return;
            }
            var p1_count = metadata.Darts[metadata.Players.First().PlayerId].Count();
            var p2_count = metadata.Darts[metadata.Players.Last().PlayerId].Count();
            if (p1_count > p2_count)
            {
                metadata.NextPlayer = metadata.Players.Last().PlayerId;
            }
            else
            {
                metadata.NextPlayer = metadata.Players.First().PlayerId;
            }
        }
    }
}

