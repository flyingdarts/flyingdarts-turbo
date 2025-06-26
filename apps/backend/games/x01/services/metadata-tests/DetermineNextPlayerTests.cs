using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Flyingdarts.Persistence;
using System.Reflection;

namespace metadata_tests;

public class DetermineNextPlayerTests
{
    private static string CallDetermineNextPlayer(List<GamePlayer> players, List<GameDart> darts)
    {
        // Use reflection to call the private static method
        var method = typeof(X01MetadataService).GetMethod("DetermineNextPlayer", BindingFlags.NonPublic | BindingFlags.Static);
        return (string)method.Invoke(null, new object[] { players, darts });
    }

    public static IEnumerable<object[]> GameStartData => new List<object[]>
    {
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            new List<GameDart>(),
            new List<string> { "p1", "p2" } // valid results
        },
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            new List<GameDart>(),
            new List<string> { "p3" } // invalid result
        }
    };

    [Theory]
    [MemberData(nameof(GameStartData))]
    public void GameStart_Theory(List<GamePlayer> players, List<GameDart> darts, List<string> expectedPlayerIds)
    {
        var result = CallDetermineNextPlayer(players, darts);
        Assert.Contains(result, expectedPlayerIds);
    }

    public static IEnumerable<object[]> LegEndData => new List<object[]>
    {
        // Winner did not start, winner should start next
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            new List<GameDart>
            {
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 100 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 50 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(3), GameScore = 50 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(4), GameScore = 0 },
            },
            "p2" // expected
        },
        // Winner did start, other player should start next
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            new List<GameDart>
            {
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 100 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 50 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(3), GameScore = 0 },
            },
            "p2" // expected
        },
        // Incorrect: winner is not a player
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            new List<GameDart>
            {
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 100 },
                new GameDart { PlayerId = "p3", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 0 },
            },
            null // expected: should not match any player
        }
    };

    [Theory]
    [MemberData(nameof(LegEndData))]
    public void LegEnd_Theory(List<GamePlayer> players, List<GameDart> darts, string expectedNextPlayer)
    {
        var result = CallDetermineNextPlayer(players, darts);
        if (expectedNextPlayer == null)
        {
            Assert.DoesNotContain(result, players.Select(p => p.PlayerId));
        }
        else
        {
            Assert.Equal(expectedNextPlayer, result);
        }
    }

    public static IEnumerable<object[]> OngoingLegData => new List<object[]>
    {
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            new List<GameDart>
            {
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 100 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 50 },
            },
            "p1" // expected
        },
        // Incorrect: last thrower is not a player
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            new List<GameDart>
            {
                new GameDart { PlayerId = "p3", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 100 },
            },
            null // expected: should not match any player
        }
    };

    [Theory]
    [MemberData(nameof(OngoingLegData))]
    public void OngoingLeg_Theory(List<GamePlayer> players, List<GameDart> darts, string expectedNextPlayer)
    {
        var result = CallDetermineNextPlayer(players, darts);
        if (expectedNextPlayer == null)
        {
            Assert.DoesNotContain(result, players.Select(p => p.PlayerId));
        }
        else
        {
            Assert.Equal(expectedNextPlayer, result);
        }
    }

    public static IEnumerable<object[]> ThreeSetsFiveLegsData => new List<object[]>
    {
        // Simulate a game with 3 sets, 5 legs per set, two players
        // We'll check next player at the start, after each leg, and after a set
        // For simplicity, p1 always starts first leg, p2 wins first leg, p2 starts next, etc.
        new object[]
        {
            // Players
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            // Darts: Start of game, no darts thrown
            new List<GameDart>(),
            // At game start, next player is either p1 or p2
            new List<string> { "p1", "p2" }
        },
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            // Leg 1: p1 starts, p2 wins
            new List<GameDart>
            {
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 501 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 400 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(3), GameScore = 350 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(4), GameScore = 0 }, // p2 wins
            },
            // p2 did not start, so p2 starts next leg
            new List<string> { "p2" }
        },
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            // Leg 2: p2 starts, p2 wins
            new List<GameDart>
            {
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 501 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 400 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(3), GameScore = 350 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(4), GameScore = 0 },
                // Leg 2
                new GameDart { PlayerId = "p2", Set = 1, Leg = 2, CreatedAt = DateTime.UtcNow.AddSeconds(5), GameScore = 501 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 2, CreatedAt = DateTime.UtcNow.AddSeconds(6), GameScore = 400 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 2, CreatedAt = DateTime.UtcNow.AddSeconds(7), GameScore = 0 }, // p2 wins
            },
            // p2 started and won, so p1 starts next leg
            new List<string> { "p1" }
        },
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "p1"), GamePlayer.Create(1, "p2") },
            // Leg 3: p1 starts, p1 wins
            new List<GameDart>
            {
                // Leg 1
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 501 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 400 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(3), GameScore = 350 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(4), GameScore = 0 },
                // Leg 2
                new GameDart { PlayerId = "p2", Set = 1, Leg = 2, CreatedAt = DateTime.UtcNow.AddSeconds(5), GameScore = 501 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 2, CreatedAt = DateTime.UtcNow.AddSeconds(6), GameScore = 400 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 2, CreatedAt = DateTime.UtcNow.AddSeconds(7), GameScore = 0 },
                // Leg 3
                new GameDart { PlayerId = "p1", Set = 1, Leg = 3, CreatedAt = DateTime.UtcNow.AddSeconds(8), GameScore = 501 },
                new GameDart { PlayerId = "p2", Set = 1, Leg = 3, CreatedAt = DateTime.UtcNow.AddSeconds(9), GameScore = 400 },
                new GameDart { PlayerId = "p1", Set = 1, Leg = 3, CreatedAt = DateTime.UtcNow.AddSeconds(10), GameScore = 0 }, // p1 wins
            },
            // p1 started and won, so p2 starts next leg
            new List<string> { "p2" }
        },
        // ... more legs and sets can be added for further coverage
    };

    [Theory]
    [MemberData(nameof(ThreeSetsFiveLegsData))]
    public void ThreeSetsFiveLegs_Theory(List<GamePlayer> players, List<GameDart> darts, List<string> expectedNextPlayers)
    {
        var result = CallDetermineNextPlayer(players, darts);
        Assert.Contains(result, expectedNextPlayers);
    }

    public static IEnumerable<object[]> PlayerABegins_BWins_BStartsNextData => new List<object[]>
    {
        new object[]
        {
            new List<GamePlayer> { GamePlayer.Create(1, "A"), GamePlayer.Create(1, "B") },
            new List<GameDart>
            {
                new GameDart { PlayerId = "A", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(1), GameScore = 501 },
                new GameDart { PlayerId = "B", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(2), GameScore = 400 },
                new GameDart { PlayerId = "A", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(3), GameScore = 350 },
                new GameDart { PlayerId = "B", Set = 1, Leg = 1, CreatedAt = DateTime.UtcNow.AddSeconds(4), GameScore = 0 }, // B wins
            },
            "B"
        }
    };

    [Theory]
    [MemberData(nameof(PlayerABegins_BWins_BStartsNextData))]
    public void PlayerABegins_BWins_BStartsNext(List<GamePlayer> players, List<GameDart> darts, string expectedNextPlayer)
    {
        var result = CallDetermineNextPlayer(players, darts);
        Assert.Equal(expectedNextPlayer, result);
    }
} 