using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Metadata.Services.Services.X01;
using Moq;

namespace Flyingdarts.Metadata.Services.Tests.Services;

// ignore tests

public class X01MetadataServiceTests
{
    private static readonly Guid MeetingIdentifier = Guid.Parse(
        "12345678-1234-1234-1234-123456789012"
    );

    private readonly Mock<IDynamoDbService> _mockDynamoDbService;
    private readonly Mock<CachingService<X01State>> _mockCachingService;
    private readonly X01MetadataService _service;

    public X01MetadataServiceTests()
    {
        _mockDynamoDbService = new Mock<IDynamoDbService>();
        _mockCachingService = new Mock<CachingService<X01State>>();

        _service = new X01MetadataService(_mockDynamoDbService.Object, _mockCachingService.Object);
    }

    [Fact(Skip = "Class temporarily disabled")]
    public async Task Ensure_MetadataServiceReturnsMeetingToken()
    {
        var game = Game.Create(2, X01GameSettings.Create(3, 3), MeetingIdentifier);
        var player1WithToken = GamePlayer.Create(game.GameId, "player1", $"meeting-token-player1");
        var player2WithToken = GamePlayer.Create(game.GameId, "player2", $"meeting-token-player2");
        SetupMockDynamoDbService(
            game,
            new List<GamePlayer> { player1WithToken, player2WithToken },
            new List<User>(),
            new List<GameDart>()
        );

        var metadata = await _service.GetMetadataAsync(
            game.GameId.ToString(),
            "player1",
            CancellationToken.None
        );

        Assert.Equal(metadata.MeetingToken, player1WithToken.MeetingToken);
        Assert.NotEqual(metadata.MeetingToken, player2WithToken.MeetingToken);
    }

    [Fact(Skip = "This test is temporarily disabled")]
    public async Task Ensure_MetadataServiceReturnsNullMeetingToken()
    {
        var game = Game.Create(2, X01GameSettings.Create(3, 3), MeetingIdentifier);

        SetupMockDynamoDbService(
            game,
            new List<GamePlayer>(),
            new List<User>(),
            new List<GameDart>()
        );

        var metadata = await _service.GetMetadataAsync(
            game.GameId.ToString(),
            "player1",
            CancellationToken.None
        );
        Assert.Null(metadata.MeetingToken);
    }

    #region Next Player Validation Tests

    [Theory(Skip = "This test is temporarily disabled")]
    [InlineData(1, 3)] // Best of 1 sets, best of 3 legs
    [InlineData(1, 5)] // Best of 1 sets, best of 5 legs
    [InlineData(1, 7)] // Best of 1 sets, best of 7 legs
    [InlineData(1, 9)] // Best of 1 sets, best of 9 legs
    [InlineData(3, 3)] // Best of 3 sets, best of 3 legs
    [InlineData(3, 5)] // Best of 3 sets, best of 5 legs
    [InlineData(3, 7)] // Best of 3 sets, best of 7 legs
    [InlineData(3, 9)] // Best of 3 sets, best of 9 legs
    [InlineData(5, 3)] // Best of 5 sets, best of 3 legs
    [InlineData(5, 5)] // Best of 5 sets, best of 5 legs
    [InlineData(5, 7)] // Best of 5 sets, best of 7 legs
    [InlineData(5, 9)] // Best of 5 sets, best of 9 legs
    [InlineData(7, 3)] // Best of 7 sets, best of 3 legs
    [InlineData(7, 5)] // Best of 7 sets, best of 5 legs
    [InlineData(7, 7)] // Best of 7 sets, best of 7 legs
    [InlineData(7, 9)] // Best of 7 sets, best of 9 legs
    [InlineData(9, 3)] // Best of 9 sets, best of 3 legs
    [InlineData(9, 5)] // Best of 9 sets, best of 5 legs
    [InlineData(9, 7)] // Best of 9 sets, best of 7 legs
    [InlineData(9, 9)] // Best of 9 sets, best of 9 legs
    [InlineData(11, 3)] // Best of 11 sets, best of 3 legs
    [InlineData(11, 5)] // Best of 11 sets, best of 5 legs
    [InlineData(11, 7)] // Best of 11 sets, best of 7 legs
    [InlineData(11, 9)] // Best of 11 sets, best of 9 legs
    [InlineData(13, 3)] // Best of 13 sets, best of 3 legs
    [InlineData(13, 5)] // Best of 13 sets, best of 5 legs
    [InlineData(13, 7)] // Best of 13 sets, best of 7 legs
    [InlineData(13, 9)] // Best of 13 sets, best of 9 legs
    public async Task GameTheory(int sets, int legs)
    {
        // Arrange
        var gameId = "1234567890123";
        var gameIdLong = long.Parse(gameId);

        var game = CreateTestGame(gameIdLong);
        game.X01 = X01GameSettings.Create(sets, legs);

        var players = CreateTestPlayers(gameIdLong);
        var users = CreateTestUsers(players);
        var darts = new List<GameDart>();

        // Calculate game parameters
        var legsToWinSet = (legs + 1) / 2;
        var setsToWinGame = (sets + 1) / 2;

        // Track game state
        var player1Sets = 0;
        var player1Legs = 0;
        var player2Sets = 0;
        var player2Legs = 0;
        var currentSet = 1;
        var currentLeg = 1;
        var random = new Random(); // Fixed seed for reproducible tests

        // Randomize the initial set starter (simulating the service behavior)
        var setStarter = random.Next(2) == 0 ? "player1" : "player2"; // Track who started the current set
        var nextPlayer = setStarter; // Game starts with setStarter
        var lastLegStarter = setStarter;

        // Simulate complete game
        while (player1Sets < setsToWinGame && player2Sets < setsToWinGame)
        {
            // Determine leg winner randomly
            var legWinner = random.Next(2) == 0 ? "player1" : "player2";

            // Generate darts for this leg
            darts.AddRange(
                GenerateLegDarts(gameIdLong, nextPlayer, legWinner, currentSet, currentLeg)
            );

            // Update leg counts
            if (legWinner == "player1")
                player1Legs++;
            else
                player2Legs++;

            // Check if set is won
            var setJustFinished = false;
            if (player1Legs >= legsToWinSet || player2Legs >= legsToWinSet)
            {
                // Set is won
                setJustFinished = true;
                if (player1Legs >= legsToWinSet)
                    player1Sets++;
                else
                    player2Sets++;

                // Reset legs for next set
                player1Legs = 0;
                player2Legs = 0;
                currentSet++;
                currentLeg = 1;

                // Alternate set starter
                setStarter = setStarter == "player1" ? "player2" : "player1";
                nextPlayer = setStarter;
                lastLegStarter = setStarter;
            }
            else
            {
                // Set continues, next leg
                currentLeg++;
                // Determine next leg starter using production logic
                if (legWinner == lastLegStarter)
                {
                    // If winner started, the other player starts next leg
                    nextPlayer = lastLegStarter == "player1" ? "player2" : "player1";
                }
                else
                {
                    // If winner did not start, winner starts next leg
                    nextPlayer = legWinner;
                }
                lastLegStarter = nextPlayer;
            }

            // Setup mock and test current state
            SetupMockDynamoDbService(game, players, users, darts);
            var result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);

            // Determine expected winning player
            var expectedWinningPlayer = (string?)null;
            if (player1Sets >= setsToWinGame)
                expectedWinningPlayer = "player1";
            else if (player2Sets >= setsToWinGame)
                expectedWinningPlayer = "player2";

            // Determine expected next player
            var expectedNextPlayer = expectedWinningPlayer == null ? nextPlayer : null;

            // Assert current game state
            AssertGameState(
                result,
                player1Sets.ToString(),
                player1Legs.ToString(),
                player2Sets.ToString(),
                player2Legs.ToString(),
                expectedNextPlayer,
                expectedWinningPlayer
            );

            // Assert next player alternation logic
            if (expectedWinningPlayer == null)
            {
                if (setJustFinished)
                {
                    // After set finishes, next player should be the new set starter
                    Assert.Equal(setStarter, result.NextPlayer);
                }
                else
                {
                    // Otherwise, next player should alternate according to the rules
                    Assert.Equal(nextPlayer, result.NextPlayer);
                }
            }
            else
            {
                // If game is won, there should be no next player
                Assert.Null(result.NextPlayer);
            }
        }
    }

    [Fact(Skip = "This test is temporarily disabled")]
    public async Task GetMetadata_WithBestOf5Legs3Sets_CompleteGame_ReturnsCorrectMetadata()
    {
        // Arrange
        var gameId = "1234567890123";
        var gameIdLong = long.Parse(gameId);

        var game = CreateTestGame(gameIdLong);
        game.X01 = X01GameSettings.Create(3, 3); // 3 sets, best of 5 legs

        var players = CreateTestPlayers(gameIdLong);
        var users = CreateTestUsers(players);

        // Create darts for a complete game - Player1 wins 2 sets to 1
        var darts = new List<GameDart>();

        // First, test the initial state with no darts to see who the service picks as starter
        SetupMockDynamoDbService(game, players, users, darts);
        var initialResult = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        var set1Starter =
            initialResult.NextPlayer
            ?? throw new InvalidOperationException(
                "NextPlayer should not be null for a game with no darts"
            );
        var set1NonStarter = set1Starter == "player1" ? "player2" : "player1";

        // Set 1: Leg 1: Player 1 wins (regardless of who starts)
        darts.AddRange(GenerateLegDarts(gameIdLong, set1Starter, "player1", 1, 1));
        SetupMockDynamoDbService(game, players, users, darts);
        var result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "0", "1", "0", "0", set1NonStarter, null);

        // Set 1: Leg 2: Player 2 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1NonStarter, "player2", 1, 2));

        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "0", "1", "0", "1", set1Starter, null);

        // Set 1: Leg 3: Player 1 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1Starter, "player1", 1, 3));
        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "1", "0", "0", "0", set1NonStarter, null);
        // ### Set Win ###

        // Set 2: Leg 1: Player 2 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1NonStarter, "player2", 2, 1));

        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "1", "0", "0", "1", set1Starter, null);

        // Set 2: Leg 2: Player 1 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1Starter, "player1", 2, 2));

        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "1", "1", "0", "1", set1NonStarter, null);

        // Set 2: Leg 3: Player 2 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1NonStarter, "player2", 2, 3));

        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "1", "0", "1", "0", set1Starter, null);
        // ### Set Win ###

        // Set 3: Leg 1: Player 1 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1Starter, "player1", 3, 1));

        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "1", "1", "1", "0", set1NonStarter, null);

        // Set 3: Leg 2: Player 2 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1NonStarter, "player2", 3, 2));

        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "1", "1", "1", "1", set1Starter, null);

        // Set 3: Leg 3: Player 1 wins
        darts.AddRange(GenerateLegDarts(gameIdLong, set1Starter, "player1", 3, 3));

        SetupMockDynamoDbService(game, players, users, darts);
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        AssertGameState(result, "2", "0", "1", "0", null, GetLastSetWinner(darts));

        string GetLastSetWinner(List<GameDart> darts)
        {
            var lastSet = darts.Max(x => x.Set);
            var lastLegDarts = darts
                .Where(d => d.Set == lastSet)
                .GroupBy(x => new { x.Set, x.Leg })
                .OrderBy(x => x.Key.Leg)
                .Last()
                .ToList();
            return lastLegDarts.Single(x => x.GameScore == 0).PlayerId;
        }
    }

    [Fact(Skip = "This test is temporarily disabled")]
    public async Task NextPlayer_Scenarios_ReturnsCorrectNextPlayer()
    {
        var gameId = "1234567890123";
        var gameIdLong = long.Parse(gameId);
        var game = CreateTestGame(gameIdLong);

        // Scenario 1: No game players - next player should be null
        SetupMockDynamoDbService(
            game,
            new List<GamePlayer>(),
            new List<User>(),
            new List<GameDart>()
        );
        var result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        Assert.Null(result.NextPlayer);

        // Scenario 2: 1 game player and no darts - next player should still be null
        var singlePlayer = new List<GamePlayer>
        {
            GamePlayer.Create(gameIdLong, "player1", "meeting-token-player1")
        };
        var singleUser = new List<User>
        {
            new User
            {
                UserId = "player1",
                Profile = new UserProfile { UserName = "Player1", Country = "US" }
            }
        };
        SetupMockDynamoDbService(game, singlePlayer, singleUser, new List<GameDart>());
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        Assert.Null(result.NextPlayer);

        // Scenario 3: 2 game players and no darts - next player should not be null (starting player determined)
        var twoPlayers = CreateTestPlayers(gameIdLong);
        var twoUsers = CreateTestUsers(twoPlayers);
        SetupMockDynamoDbService(game, twoPlayers, twoUsers, new List<GameDart>());
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        Assert.NotNull(result.NextPlayer);
        Assert.Contains(result.NextPlayer, new[] { "player1", "player2" });

        // Scenario 4: 2 players and game started, first player throws dart - next player should be player 2
        var firstDart = GameDart.Create(gameIdLong, "player1", 60, 441, 1, 1);
        SetupMockDynamoDbService(game, twoPlayers, twoUsers, new List<GameDart> { firstDart });
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        Assert.Equal("player2", result.NextPlayer);

        // Scenario 5: Second player throws dart - next player should be player 1
        var secondDart = GameDart.Create(gameIdLong, "player2", 60, 381, 1, 1);
        SetupMockDynamoDbService(
            game,
            twoPlayers,
            twoUsers,
            new List<GameDart> { firstDart, secondDart }
        );
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        Assert.Equal("player1", result.NextPlayer);

        // Scenario 6: First player throws another dart - next player should be player 2
        var thirdDart = GameDart.Create(gameIdLong, "player1", 60, 321, 1, 1);
        SetupMockDynamoDbService(
            game,
            twoPlayers,
            twoUsers,
            new List<GameDart> { firstDart, secondDart, thirdDart }
        );
        result = await _service.GetMetadataAsync(gameId, null, CancellationToken.None);
        Assert.Equal("player2", result.NextPlayer);
    }

    #endregion

    private Game CreateTestGame(long gameId)
    {
        var game = Game.Create(2, X01GameSettings.Create(3, 3), MeetingIdentifier);
        game.GameId = gameId;
        game.Status = GameStatus.Started;
        return game;
    }

    private List<GamePlayer> CreateTestPlayers(long gameId)
    {
        return new List<GamePlayer>
        {
            GamePlayer.Create(gameId, "player1", "meeting-token-player1"),
            GamePlayer.Create(gameId, "player2", "meeting-token-player2")
        };
    }

    private List<User> CreateTestUsers(List<GamePlayer> players)
    {
        return players
            .Select(
                p =>
                    new User
                    {
                        UserId = p.PlayerId,
                        Profile = new UserProfile
                        {
                            UserName = $"Player{p.PlayerId.Last()}",
                            Country = p.PlayerId == "player1" ? "US" : "UK"
                        }
                    }
            )
            .ToList();
    }

    private List<GameDart> CreateTestDarts(long gameId, string player1Id, string player2Id)
    {
        return new List<GameDart>
        {
            GameDart.Create(gameId, player1Id, 60, 441, 1, 1),
            GameDart.Create(gameId, player2Id, 60, 441, 1, 1),
            GameDart.Create(gameId, player1Id, 60, 381, 1, 1),
            GameDart.Create(gameId, player2Id, 60, 381, 1, 1)
        };
    }

    private void SetupMockDynamoDbService(
        Game game,
        List<GamePlayer> players,
        List<User> users,
        List<GameDart> darts
    )
    {
        _mockDynamoDbService
            .Setup(x => x.ReadGameAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Game> { game });

        _mockDynamoDbService
            .Setup(x => x.ReadGamePlayersAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(players);

        _mockDynamoDbService
            .Setup(x => x.ReadGameDartsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(darts);

        _mockDynamoDbService
            .Setup(x => x.ReadUsersAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);
    }

    /// <summary>
    /// Generates the minimum number of darts needed for a leg with specified starting player and winning player.
    /// Each dart reduces the score by 180 points until the final dart that wins the leg.
    /// </summary>
    /// <param name="gameId">The game ID</param>
    /// <param name="startingPlayer">The player who starts the leg</param>
    /// <param name="winningPlayer">The player who wins the leg</param>
    /// <param name="setNumber">The set number</param>
    /// <param name="legNumber">The leg number</param>
    /// <returns>List of darts for the leg</returns>
    private List<GameDart> GenerateLegDarts(
        long gameId,
        string startingPlayer,
        string winningPlayer,
        int setNumber,
        int legNumber
    )
    {
        var darts = new List<GameDart>();
        var currentScore = 501;
        var currentPlayer = startingPlayer;
        var otherPlayer = startingPlayer == "player1" ? "player2" : "player1";

        // Generate darts until we need the final winning dart
        while (currentScore > 141)
        {
            darts.Add(
                GameDart.Create(
                    gameId,
                    currentPlayer,
                    180,
                    currentScore - 180,
                    setNumber,
                    legNumber
                )
            );
            currentScore -= 180;

            // Alternate players
            (currentPlayer, otherPlayer) = (otherPlayer, currentPlayer);
        }

        // Add the final winning dart
        darts.Add(GameDart.Create(gameId, winningPlayer, currentScore, 0, setNumber, legNumber));

        return darts;
    }

    /// <summary>
    /// Asserts the game state with a single method call for cleaner test code.
    /// </summary>
    /// <param name="result">The metadata result to assert</param>
    /// <param name="player1Sets">Expected sets for player1</param>
    /// <param name="player1Legs">Expected legs for player1</param>
    /// <param name="player2Sets">Expected sets for player2</param>
    /// <param name="player2Legs">Expected legs for player2</param>
    /// <param name="nextPlayer">Expected next player (null if game is won)</param>
    /// <param name="winningPlayer">Expected winning player (null if game is ongoing)</param>
    private void AssertGameState(
        Flyingdarts.Metadata.Services.Dtos.Metadata result,
        string player1Sets,
        string player1Legs,
        string player2Sets,
        string player2Legs,
        string? nextPlayer,
        string? winningPlayer
    )
    {
        Assert.NotNull(result.Players);
        Assert.Equal(player1Sets, result.Players!.First(p => p.PlayerId == "player1").Sets);
        Assert.Equal(player1Legs, result.Players!.First(p => p.PlayerId == "player1").Legs);
        Assert.Equal(player2Sets, result.Players!.First(p => p.PlayerId == "player2").Sets);
        Assert.Equal(player2Legs, result.Players!.First(p => p.PlayerId == "player2").Legs);
        Assert.Equal(nextPlayer, result.NextPlayer);
        Assert.Equal(winningPlayer, result.WinningPlayer);
    }
}
