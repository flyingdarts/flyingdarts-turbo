using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Connection.Services;
using Flyingdarts.Core.Models;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Metadata.Services.Services.X01;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.Create;

record CreateGameRequest(int Sets, int Legs, int PlayerCount, Guid MeetingIdentifier);

public record CreateX01GameCommandHandler(
    IDynamoDBContext DbContext,
    CachingService<X01State> CachingService,
    ConnectionService ConnectionService,
    IDynamoDbService DynamoDbService,
    IMeetingService MeetingService,
    X01MetadataService MetadataService
) : IRequestHandler<CreateX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(
        CreateX01GameCommand request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[INFO] Starting CreateX01GameCommandHandler.Handle for PlayerId: {request.PlayerId}, ConnectionId: {request.ConnectionId}"
        );

        try
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.ConnectionId);

            Console.WriteLine("[DEBUG] Creating socket message for action: games/x01/create");
            var socketMessage = new SocketMessage<CreateX01GameCommand>
            {
                Action = "games/x01/create",
            };

            Console.WriteLine(
                $"[DEBUG] Getting player ID for auth provider user ID: {request.PlayerId}"
            );
            var playerId = await GetPlayerIdAsync(request.PlayerId, cancellationToken);
            Console.WriteLine(
                $"[INFO] Retrieved player ID: {playerId} for auth provider user ID: {request.PlayerId}"
            );

            // Update connection ID
            Console.WriteLine(
                $"[DEBUG] Updating connection ID for player: {playerId} with connection: {request.ConnectionId}"
            );
            await ConnectionService.UpdateConnectionIdAsync(
                playerId,
                request.ConnectionId,
                cancellationToken
            );
            Console.WriteLine("[DEBUG] Successfully updated connection ID");

            Console.WriteLine($"[DEBUG] Reading user data for player ID: {playerId}");
            var gameCreator = await DynamoDbService.ReadUserAsync(playerId, cancellationToken);
            Console.WriteLine(
                $"[INFO] Retrieved game creator: {gameCreator.UserId} with meeting identifier: {gameCreator.MeetingIdentifier}"
            );

            var createGameRequest = new CreateGameRequest(
                request.Sets,
                request.Legs,
                request.PlayerCount,
                gameCreator.MeetingIdentifier
            );
            Console.WriteLine(
                $"[INFO] Creating X01 game with settings - Sets: {request.Sets}, Legs: {request.Legs}, PlayerCount: {request.PlayerCount}"
            );

            var game = await CreateGameAsync(createGameRequest, cancellationToken);
            Console.WriteLine($"[INFO] Successfully created game with ID: {game.GameId}");

            // Populate metadata as the final step
            Console.WriteLine(
                $"[DEBUG] Retrieving metadata for game ID: {game.GameId} and player ID: {playerId}"
            );
            var metadata = await MetadataService.GetMetadataAsync(
                game.GameId.ToString(),
                playerId,
                cancellationToken
            );
            socketMessage.Metadata = metadata.ToDictionary();
            Console.WriteLine($"[DEBUG] Successfully populated metadata for game: {game.GameId}");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(socketMessage)
            };

            Console.WriteLine(
                $"[INFO] Successfully completed CreateX01GameCommandHandler.Handle for game: {game.GameId}"
            );
            return response;
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(
                $"[ERROR] Invalid request parameters in CreateX01GameCommandHandler.Handle: {ex.Message}"
            );
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ERROR] Unexpected error in CreateX01GameCommandHandler.Handle for PlayerId: {request.PlayerId}. Error: {ex.Message}"
            );
            throw;
        }
    }

    private async Task<string> GetPlayerIdAsync(
        string authProviderUserId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[DEBUG] Getting player ID for auth provider user ID: {authProviderUserId}"
        );

        try
        {
            var user = await DynamoDbService.ReadUserByAuthProviderUserIdAsync(
                authProviderUserId,
                cancellationToken
            );

            Console.WriteLine(
                $"[DEBUG] Successfully retrieved player ID: {user.UserId} for auth provider user ID: {authProviderUserId}"
            );
            return user.UserId;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ERROR] Failed to get player ID for auth provider user ID: {authProviderUserId}. Error: {ex.Message}"
            );
            throw;
        }
    }

    private async Task<Game> CreateGameAsync(
        CreateGameRequest request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[DEBUG] Creating game with meeting: {request.MeetingIdentifier}, players: {request.PlayerCount}"
        );

        try
        {
            var game = Game.Create(
                request.PlayerCount,
                X01GameSettings.Create(request.Sets, request.Legs),
                request.MeetingIdentifier
            );

            Console.WriteLine($"[DEBUG] Game object created with ID: {game.GameId}");

            // Initialize cache state
            Console.WriteLine($"[DEBUG] Initializing cache state for game: {game.GameId}");
            CachingService.State = X01State.Create(game.GameId);
            CachingService.AddGame(game);
            await CachingService.Save(cancellationToken);
            Console.WriteLine($"[DEBUG] Successfully saved game to cache: {game.GameId}");

            // Write to database
            Console.WriteLine($"[DEBUG] Writing game to database: {game.GameId}");
            await DynamoDbService.WriteGameAsync(game, cancellationToken);
            Console.WriteLine($"[INFO] Successfully wrote game to database: {game.GameId}");

            return game;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ERROR] Failed to create game with meeting: {request.MeetingIdentifier}. Error: {ex.Message}"
            );
            throw;
        }
    }

    private static DynamoDBOperationConfig GetOperationConfig() =>
        new()
        {
            OverrideTableName =
                $"Flyingdarts-Application-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}"
        };
}
