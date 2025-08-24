using System.Text;
using System.Text.Json;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Connection.Services;
using Flyingdarts.Core.Models;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Meetings.Service.Services.DyteMeetingService.Requests;
using Flyingdarts.Metadata.Services.Services.X01;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Api.Requests.Join;

public record JoinX01GameCommandHandler( //
    IAmazonApiGatewayManagementApi ApiGatewayClient,
    CachingService<X01State> CachingService,
    ConnectionService ConnectionService,
    IDynamoDbService DynamoDbService,
    IMeetingService MeetingService,
    X01MetadataService MetadataService
) : IRequestHandler<JoinX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(
        JoinX01GameCommand request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Starting handle for GameId: {request?.GameId}, PlayerId: {request?.PlayerId}, ConnectionId: {request?.ConnectionId}"
        );

        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.GameId);
        ArgumentNullException.ThrowIfNull(request.ConnectionId);

        var socketMessage = new SocketMessage<JoinX01GameCommand> { Action = "games/x01/join" };

        var playerId = await GetPlayerIdAsync(request.PlayerId, cancellationToken);

        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Updating connection ID for player {playerId}"
        );
        // Update connection ID
        await ConnectionService.UpdateConnectionIdAsync(
            playerId,
            request.ConnectionId,
            cancellationToken
        );

        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Loading game state for GameId: {request.GameId}"
        );
        // Load game state
        await CachingService.Load(request.GameId, cancellationToken);

        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Game loaded. Current player count: {CachingService.State.Players?.Count ?? 0}, User count: {CachingService.State.Users?.Count ?? 0}"
        );

        // Update user connection ID in cache
        await UpdateUserConnectionIdAsync(playerId, request.ConnectionId, cancellationToken);

        // Keep track of game in request
        request.Game = CachingService.State.Game;
        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Game status: {request.Game?.Status}, Meeting ID: {request.Game?.MeetingIdentifier}"
        );

        // Add player to game if not already present
        await AddPlayerToGameAsync(request, playerId, cancellationToken);

        // Start game if we have 2 players
        await StartGameIfReadyAsync(request, cancellationToken);

        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Getting metadata for game {request.Game?.GameId} and player {playerId}"
        );
        var gameIdForMetadata = request.Game?.GameId ?? throw new Exception("GameId is required");
        var metadata = await MetadataService.GetMetadataAsync(
            gameIdForMetadata.ToString(),
            playerId,
            cancellationToken
        );

        // Populate metadata as the final step
        socketMessage.Metadata = metadata.ToDictionary();
        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Metadata populated with {metadata.ToDictionary().Count} entries"
        );

        // // Get all player connection IDs
        var gameUsers = CachingService.State?.Users ?? new List<User>();
        var playerConnectionIds = gameUsers
            .Select(x => x.ConnectionId)
            .Where(id => !string.IsNullOrEmpty(id))
            .ToArray();

        // Notify people in the room
        Console.WriteLine(
            $"[JoinX01GameCommandHandler] Notifying {playerConnectionIds.Length} connections"
        );

        // If its just the owner of the message, don't notify
        if (playerConnectionIds.Any(x => x != socketMessage.ConnectionId))
        {
            var connectionsWithoutMessageOwner = playerConnectionIds.Where(x =>
                x != socketMessage.ConnectionId
            );

            await NotifyRoomAsync(
                socketMessage,
                connectionsWithoutMessageOwner.ToArray(),
                cancellationToken
            );

            var notifiedCount = playerConnectionIds.Any(x => x != socketMessage.ConnectionId)
                ? playerConnectionIds.Where(x => x != socketMessage.ConnectionId).Count()
                : 0;
            Console.WriteLine(
                $"[JoinX01GameCommandHandler] Notified {notifiedCount} connections (excluding message owner)"
            );
        }
        Console.WriteLine($"[JoinX01GameCommandHandler] Successfully completed join request");

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage),
        };
    }

    private async Task<string> GetPlayerIdAsync(
        string authProviderUserId,
        CancellationToken cancellationToken
    )
    {
        var user = await DynamoDbService.ReadUserByAuthProviderUserIdAsync(
            authProviderUserId,
            cancellationToken
        );
        return user.UserId;
    }

    private async Task NotifyRoomAsync(
        SocketMessage<JoinX01GameCommand> request,
        string[] connectionIds,
        CancellationToken cancellationToken
    )
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)));

        foreach (var connectionId in connectionIds)
        {
            if (string.IsNullOrEmpty(connectionId))
                continue;

            var postConnectionRequest = new PostToConnectionRequest
            {
                ConnectionId = connectionId,
                Data = stream,
            };

            stream.Position = 0;
            await ApiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
        }
    }

    private async Task UpdateUserConnectionIdAsync(
        string playerId,
        string connectionId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[UpdateUserConnectionId] Updating connection ID for player {playerId} to {connectionId}"
        );

        var existingUser = CachingService.State.Users.FirstOrDefault(x => x.UserId == playerId);
        if (existingUser is not null)
        {
            Console.WriteLine(
                $"[UpdateUserConnectionId] Found existing user {playerId}, updating connection ID from {existingUser.ConnectionId} to {connectionId}"
            );
            existingUser.ConnectionId = connectionId;
            await CachingService.Save(cancellationToken);
            Console.WriteLine(
                $"[UpdateUserConnectionId] Successfully updated and saved connection ID for user {playerId}"
            );
        }
        else
        {
            Console.WriteLine(
                $"[UpdateUserConnectionId] No existing user found for player {playerId}"
            );
        }
    }

    private async Task AddPlayerToGameAsync(
        JoinX01GameCommand request,
        string playerId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[AddPlayerToGame] Checking if player {playerId} already exists in game"
        );

        // If player is already in the game, do nothing
        if (CachingService.State.Players.Any(x => x.PlayerId == playerId))
        {
            Console.WriteLine(
                $"[AddPlayerToGame] Player {playerId} already exists in game, skipping addition"
            );
            return;
        }

        if (request.Game is null)
        {
            Console.WriteLine("[AddPlayerToGame] ERROR: Game is required but was null");
            throw new Exception("Game is required");
        }

        Console.WriteLine(
            $"[AddPlayerToGame] Adding new player {playerId} to game {request.GameId}"
        );

        var meetingToken = await AddParticipantToMeetingAsync(request, playerId, cancellationToken);
        if (string.IsNullOrEmpty(meetingToken))
        {
            Console.WriteLine(
                $"[AddPlayerToGame] WARNING: Meeting token is null or empty. Using fallback token."
            );
            meetingToken = string.Empty;
        }
        Console.WriteLine(
            $"[AddPlayerToGame] Received meeting token for player {playerId}: {meetingToken?.Substring(0, Math.Min(20, meetingToken.Length))}..."
        );

        var player = GamePlayer.Create(
            long.Parse(request.GameId),
            playerId,
            meetingToken ?? string.Empty
        );
        await DynamoDbService.WriteGamePlayerAsync(player, cancellationToken);
        Console.WriteLine(
            $"[AddPlayerToGame] Written game player to DynamoDB for player {playerId}"
        );

        CachingService.AddPlayer(player);
        Console.WriteLine(
            $"[AddPlayerToGame] Added player to cache. Total players now: {CachingService.State.Players.Count}"
        );

        var user = await DynamoDbService.ReadUserAsync(playerId, cancellationToken);
        CachingService.AddUser(user);
        Console.WriteLine(
            $"[AddPlayerToGame] Added user to cache. Total users now: {CachingService.State.Users.Count}"
        );

        await CachingService.Save(cancellationToken);
        Console.WriteLine(
            $"[AddPlayerToGame] Successfully added player {playerId} to game {request.GameId}"
        );
    }

    private async Task<string> AddParticipantToMeetingAsync(
        JoinX01GameCommand request,
        string playerId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[AddParticipantToMeeting] Adding participant {playerId} to meeting");

        if (request.Game is null)
        {
            Console.WriteLine("[AddParticipantToMeeting] ERROR: Game is required but was null");
            throw new Exception("Game is required");
        }

        Console.WriteLine(
            $"[AddParticipantToMeeting] Getting meeting by ID: {request.Game.MeetingIdentifier}"
        );
        var meeting = await MeetingService.GetByIdAsync(
            request.Game.MeetingIdentifier,
            cancellationToken
        );

        if (meeting is null)
        {
            Console.WriteLine(
                $"[AddParticipantToMeeting] ERROR: Meeting not found for ID {request.Game.MeetingIdentifier}"
            );
            throw new Exception("Meeting not found");
        }

        if (meeting.Id is null)
        {
            Console.WriteLine(
                "[AddParticipantToMeeting] ERROR: Meeting ID is required but was null"
            );
            throw new Exception("Meeting ID is required");
        }

        var meetingId = meeting.Id ?? throw new Exception("Meeting ID is required");
        Console.WriteLine($"[AddParticipantToMeeting] Found meeting with ID: {meetingId}");

        var joinMeetingRequest = new JoinMeetingRequest(meetingId, request.PlayerName, playerId);

        Console.WriteLine(
            $"[AddParticipantToMeeting] Adding participant {request.PlayerName} ({playerId}) to meeting {meetingId}"
        );
        var participantToken = await MeetingService.AddParticipantAsync(
            joinMeetingRequest,
            cancellationToken
        );

        if (participantToken is null)
        {
            Console.WriteLine(
                $"[AddParticipantToMeeting] ERROR: Failed to add participant {playerId} to meeting"
            );
            throw new Exception("Failed to add participant to meeting");
        }

        Console.WriteLine(
            $"[AddParticipantToMeeting] Successfully added participant {playerId} to meeting. Token length: {participantToken?.Length ?? 0}"
        );
        return participantToken ?? throw new Exception("Failed to get participant token");
    }

    private async Task StartGameIfReadyAsync(
        JoinX01GameCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentPlayerCount = CachingService.State.Players.Count;
        Console.WriteLine($"[StartGameIfReady] Current player count: {currentPlayerCount}");

        if (currentPlayerCount != 2)
        {
            Console.WriteLine(
                $"[StartGameIfReady] Game not ready to start, need 2 players but have {currentPlayerCount}"
            );
            return;
        }

        Console.WriteLine(
            $"[StartGameIfReady] Starting game {request.Game!.GameId} with 2 players"
        );

        request.Game!.Status = GameStatus.Started;
        request.Game!.LSI1 = $"{GameStatus.Started}#{request.Game!.GameId}";
        request.Game!.SortKey = $"{request.Game!.GameId}#{GameStatus.Started}";

        Console.WriteLine(
            $"[StartGameIfReady] Updated game status to {GameStatus.Started} for game {request.Game.GameId}"
        );

        await DynamoDbService.PutGameAsync(request.Game, cancellationToken);
        Console.WriteLine($"[StartGameIfReady] Written updated game status to DynamoDB");

        CachingService.AddGame(request.Game);
        await CachingService.Save(cancellationToken);
        Console.WriteLine($"[StartGameIfReady] Successfully started game {request.Game.GameId}");
    }
}
