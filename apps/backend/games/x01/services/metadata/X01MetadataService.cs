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
                return new PlayerDto
                {
                    PlayerId = x.PlayerId,
                    PlayerName = users.Single(y => y.UserId == x.PlayerId).Profile.UserName,
                    Country = users.Single(y => y.UserId == x.PlayerId).Profile.Country.ToLower(),
                    CreatedAt = x.PlayerId,
                    Legs = CalculateLegs(darts!, x.PlayerId).ToString(),
                    Sets = CalculateSets(darts!, x.PlayerId, data.Game.X01.Legs).ToString()
                };
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
            int setsWonByPlayer = CalculateSets(darts, playerId, legsRequired);
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

    private static string GetWinningPlayer(List<GameDart> darts, int legsRequired, int bestOfSets, List<string> playerIds)
    {
        foreach (var playerId in playerIds)
        {
            int setsWonByPlayer = CalculateSets(darts, playerId, legsRequired);
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

    private static int CalculateLegs(List<GameDart> darts, string playerId)
    {
        // Group darts by set and then by player.
        var sets = darts.GroupBy(dart => dart.Set);

        var totalLegsWon = 0;

        // Calculate legs won in each set
        foreach (var set in sets)
        {
            var legsInSet = set.GroupBy(dart => dart.Leg);
            foreach (var leg in legsInSet)
            {
                // Check if the player has won the leg by being the first to finish (score of 0)
                if (leg.Any(dart => dart.PlayerId == playerId && dart.GameScore == 0))
                {
                    totalLegsWon++;
                }
            }
        }

        return totalLegsWon;
    }

    private static int CalculateSets(List<GameDart> darts, string playerId, int legsPerSet)
    {
        // Group darts by set
        var sets = darts.GroupBy(dart => dart.Set);

        int totalSetsWon = 0;

        // Calculate sets won
        foreach (var set in sets)
        {
            int legsWonInCurrentSet = CalculateLegs(set.ToList(), playerId);
            if (legsWonInCurrentSet >= legsPerSet / 2 + 1) // If the player has won the majority of the legs in the set
            {
                totalSetsWon++;
            }
        }

        return totalSetsWon;
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

