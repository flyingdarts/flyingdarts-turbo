using System.Text;
using System.Text.Json;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Connection.Services;
using Flyingdarts.Core.Models;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Metadata.Services.Services.X01;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.Score;

public record CreateX01ScoreCommandHandler(
    IDynamoDbService DynamoDbService,
    IAmazonApiGatewayManagementApi ApiGatewayClient,
    CachingService<X01State> CachingService,
    X01MetadataService MetadataService,
    ConnectionService ConnectionService
) : IRequestHandler<CreateX01ScoreCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(CreateX01ScoreCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DEBUG] CreateX01ScoreCommandHandler.Handle - Starting with request: GameId={request.GameId}, PlayerId={request.PlayerId}, Score={request.Score}, Input={request.Input}"
        );

        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.GameId);
        ArgumentNullException.ThrowIfNull(request.ConnectionId);

        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - All arguments validated successfully");

        var socketMessage = new SocketMessage<CreateX01ScoreCommand> { Action = "games/x01/score" };

        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Created socket message");

        var playerId = await GetPlayerIdAsync(request.PlayerId, cancellationToken);
        // Update connection ID
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Updating connection ID for player {playerId}");
        await ConnectionService.UpdateConnectionIdAsync(playerId, request.ConnectionId, cancellationToken);
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Connection ID updated successfully");

        // Load game state
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Loading game state for game {request.GameId}");
        await CachingService.Load(request.GameId, cancellationToken);
        Console.WriteLine(
            $"[DEBUG] CreateX01ScoreCommandHandler.Handle - Game state loaded. State is null: {CachingService.State == null}"
        );

        // Create and save dart record
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Creating dart record");
        await CreateDartRecordAsync(request, playerId, cancellationToken);
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Dart record created and saved");

        // Check if game is finished and update accordingly
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Checking game completion");
        await HandleGameCompletionAsync(request.GameId, playerId, cancellationToken);
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Game completion check finished");

        // Populate metadata as the final step
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Getting metadata");
        var metadata = await MetadataService.GetMetadataAsync(request.GameId, playerId, cancellationToken);
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Metadata retrieved successfully");

        socketMessage.Metadata = metadata.ToDictionary();

        // Get all player connection IDs
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Getting player connection IDs");
        var gameUsers = CachingService.State.Users;
        var playerConnectionIds = gameUsers.Select(x => x.ConnectionId).Where(id => !string.IsNullOrEmpty(id)).ToArray();
        Console.WriteLine(
            $"[DEBUG] CreateX01ScoreCommandHandler.Handle - Found {playerConnectionIds.Length} player connection IDs: [{string.Join(", ", playerConnectionIds)}]"
        );

        // Notify people in the room
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Notifying room");
        await NotifyRoomAsync(socketMessage, playerConnectionIds, cancellationToken);
        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Room notification completed");

        Console.WriteLine($"[DEBUG] CreateX01ScoreCommandHandler.Handle - Returning successful response");
        return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonSerializer.Serialize(socketMessage) };
    }

    private async Task<string> GetPlayerIdAsync(string authProviderUserId, CancellationToken cancellationToken)
    {
        var user = await DynamoDbService.ReadUserByAuthProviderUserIdAsync(authProviderUserId, cancellationToken);
        return user.UserId;
    }

    private async Task CreateDartRecordAsync(CreateX01ScoreCommand request, string playerId, CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DEBUG] CreateDartRecordAsync - Starting with request: GameId={request.GameId}, PlayerId={playerId}, Score={request.Score}, Input={request.Input}"
        );

        var setsAndLegs = GetSetsAndLegs();
        Console.WriteLine($"[DEBUG] CreateDartRecordAsync - GetSetsAndLegs returned: {setsAndLegs}");

        if (setsAndLegs is null)
        {
            Console.WriteLine($"[ERROR] CreateDartRecordAsync - GetSetsAndLegs returned null, cannot create dart record");
            return;
        }

        Console.WriteLine(
            $"[DEBUG] CreateDartRecordAsync - Creating GameDart with Set={setsAndLegs.Value.Set}, Leg={setsAndLegs.Value.Leg}"
        );
        var gameDart = GameDart.Create(
            long.Parse(request.GameId),
            playerId,
            request.Input,
            request.Score,
            setsAndLegs.Value.Set,
            setsAndLegs.Value.Leg
        );
        Console.WriteLine(
            $"[DEBUG] CreateDartRecordAsync - GameDart created: Id={gameDart.Id}, GameId={gameDart.GameId}, PlayerId={gameDart.PlayerId}, Score={gameDart.Score}, GameScore={gameDart.GameScore}"
        );

        // Write dart to database and cache
        Console.WriteLine($"[DEBUG] CreateDartRecordAsync - Writing dart to database");
        try
        {
            await DynamoDbService.WriteGameDartAsync(gameDart, cancellationToken);
            Console.WriteLine($"[DEBUG] CreateDartRecordAsync - Dart written to database successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] CreateDartRecordAsync - Failed to write dart to database: {ex.Message}");
            Console.WriteLine($"[ERROR] CreateDartRecordAsync - Exception details: {ex}");
            throw;
        }

        Console.WriteLine($"[DEBUG] CreateDartRecordAsync - Adding dart to cache");
        try
        {
            CachingService.AddDart(gameDart);
            Console.WriteLine(
                $"[DEBUG] CreateDartRecordAsync - Dart added to cache successfully. Cache now has {CachingService.State.Darts.Count} darts"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] CreateDartRecordAsync - Failed to add dart to cache: {ex.Message}");
            Console.WriteLine($"[ERROR] CreateDartRecordAsync - Exception details: {ex}");
            throw;
        }

        Console.WriteLine($"[DEBUG] CreateDartRecordAsync - Saving cache");
        try
        {
            await CachingService.Save(cancellationToken);
            Console.WriteLine($"[DEBUG] CreateDartRecordAsync - Cache saved successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] CreateDartRecordAsync - Failed to save cache: {ex.Message}");
            Console.WriteLine($"[ERROR] CreateDartRecordAsync - Exception details: {ex}");
            throw;
        }

        Console.WriteLine($"[DEBUG] CreateDartRecordAsync - Completed successfully");
    }

    private async Task HandleGameCompletionAsync(string gameId, string playerId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[DEBUG] HandleGameCompletionAsync - Starting with gameId={gameId}, playerId={playerId}");

        var metadata = await MetadataService.GetMetadataAsync(gameId, playerId, cancellationToken);
        Console.WriteLine($"[DEBUG] HandleGameCompletionAsync - Metadata retrieved, WinningPlayer={metadata.WinningPlayer}");

        if (metadata.WinningPlayer is not null)
        {
            Console.WriteLine($"[DEBUG] HandleGameCompletionAsync - Game has a winner, updating game status to Finished");
            var game = (await DynamoDbService.ReadStartedGameAsync(long.Parse(gameId), cancellationToken)).Single();
            game.Status = GameStatus.Finished;

            // Write game to database and cache
            Console.WriteLine($"[DEBUG] HandleGameCompletionAsync - Writing finished game to database");
            await DynamoDbService.WriteGameAsync(game, cancellationToken);
            CachingService.AddGame(game);
            await CachingService.Save(cancellationToken);
            Console.WriteLine($"[DEBUG] HandleGameCompletionAsync - Finished game saved successfully");
        }
        else
        {
            Console.WriteLine($"[DEBUG] HandleGameCompletionAsync - No winner yet, game continues");
        }
    }

    private (int Set, int Leg)? GetSetsAndLegs()
    {
        Console.WriteLine($"[DEBUG] GetSetsAndLegs - Starting");
        var darts = CachingService.State.Darts;
        Console.WriteLine($"[DEBUG] GetSetsAndLegs - Current darts count: {darts?.Count ?? 0}");

        if (darts is { Count: 0 })
        {
            Console.WriteLine($"[DEBUG] GetSetsAndLegs - No darts found, returning (1, 1)");
            return (1, 1);
        }

        int currentSet = 1;
        int currentLeg = 1;
        int legsNeededToWinSet = (CachingService.State.Game.X01.Legs + 1) / 2;
        Console.WriteLine(
            $"[DEBUG] GetSetsAndLegs - Initial values: currentSet={currentSet}, currentLeg={currentLeg}, legsNeededToWinSet={legsNeededToWinSet}"
        );

        // Track leg wins per player per set
        var legWinsPerPlayer = new Dictionary<string, int>();

        foreach (var dart in darts)
        {
            Console.WriteLine(
                $"[DEBUG] GetSetsAndLegs - Processing dart: PlayerId={dart.PlayerId}, GameScore={dart.GameScore}, Set={dart.Set}, Leg={dart.Leg}"
            );
            legWinsPerPlayer.TryGetValue(dart.PlayerId, out var currentWins);

            // Check if the player has won the leg
            if (dart.GameScore == 0)
            {
                Console.WriteLine($"[DEBUG] GetSetsAndLegs - Player {dart.PlayerId} won a leg");
                legWinsPerPlayer[dart.PlayerId] = currentWins + 1;

                // Check if the player has won enough legs to win the set
                if (legWinsPerPlayer[dart.PlayerId] >= legsNeededToWinSet)
                {
                    Console.WriteLine($"[DEBUG] GetSetsAndLegs - Player {dart.PlayerId} won the set, moving to next set");
                    currentSet++;
                    currentLeg = 1; // Reset leg count for the new set

                    // Reset leg wins for all players for the new set
                    legWinsPerPlayer.Clear();
                }
                else
                {
                    Console.WriteLine($"[DEBUG] GetSetsAndLegs - Player {dart.PlayerId} won a leg but not the set, moving to next leg");
                    currentLeg++; // Move to the next leg within the same set
                }
            }
        }

        Console.WriteLine($"[DEBUG] GetSetsAndLegs - Final result: Set={currentSet}, Leg={currentLeg}");
        return (currentSet, currentLeg);
    }

    private async Task NotifyRoomAsync(
        SocketMessage<CreateX01ScoreCommand> socketMessage,
        string[] connectionIds,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[DEBUG] NotifyRoomAsync - Starting with {connectionIds.Length} connection IDs");

        if (connectionIds.Length == 0)
        {
            Console.WriteLine($"[DEBUG] NotifyRoomAsync - No connection IDs to notify");
            return;
        }

        var messageJson = JsonSerializer.Serialize(socketMessage);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        Console.WriteLine($"[DEBUG] NotifyRoomAsync - Message serialized, size: {messageBytes.Length} bytes");

        var tasks = connectionIds
            .Select(async connectionId =>
            {
                Console.WriteLine($"[DEBUG] NotifyRoomAsync - Sending message to connection {connectionId}");
                var stream = new MemoryStream(messageBytes);
                var postConnectionRequest = new PostToConnectionRequest { ConnectionId = connectionId, Data = stream };

                try
                {
                    await ApiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
                    Console.WriteLine($"[DEBUG] NotifyRoomAsync - Successfully sent message to connection {connectionId}");
                }
                catch (GoneException)
                {
                    Console.WriteLine($"[DEBUG] NotifyRoomAsync - Connection {connectionId} is no longer available, ignoring");
                    // Connection is no longer available, ignore
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] NotifyRoomAsync - Failed to send message to connection {connectionId}: {ex.Message}");
                }
            })
            .ToArray();

        await Task.WhenAll(tasks);
        Console.WriteLine($"[DEBUG] NotifyRoomAsync - Completed sending messages to all connections");
    }
}
