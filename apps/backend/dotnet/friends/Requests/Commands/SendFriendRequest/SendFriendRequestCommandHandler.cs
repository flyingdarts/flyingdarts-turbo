using System;
using System.IO;
using System.Text;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Flyingdarts.Backend.Friends.Api.Services;
using Flyingdarts.Core.Models;

namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.SendFriendRequest;

public class SendFriendRequestCommandHandler
    : IRequestHandler<SendFriendRequestCommand, APIGatewayProxyResponse>
{
    private readonly IFriendsDynamoDbService _friendsDynamoDbService;
    private readonly IAmazonApiGatewayManagementApi _apiGatewayClient;

    public SendFriendRequestCommandHandler(
        IFriendsDynamoDbService friendsDynamoDbService,
        IAmazonApiGatewayManagementApi apiGatewayClient
    )
    {
        _friendsDynamoDbService = friendsDynamoDbService;
        _apiGatewayClient = apiGatewayClient;
    }

    public async Task<APIGatewayProxyResponse> Handle(
        SendFriendRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Console.WriteLine(
                $"[SendFriendRequestCommandHandler] Starting friend request from {request.RequesterId} to {request.TargetUserId}"
            );

            var requester = await _friendsDynamoDbService.ReadUserByAuthProviderUserIdAsync(
                request.RequesterId,
                cancellationToken
            );

            // Validate request
            if (requester.UserId == request.TargetUserId)
            {
                Console.WriteLine(
                    $"[SendFriendRequestCommandHandler] Self-friend request attempted by user: {requester.UserId}"
                );
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(
                        new { error = "Cannot send friend request to yourself" }
                    ),
                    Headers = GetCorsHeaders(),
                };
            }

            // Check if target user exists
            var targetUserExists = await _friendsDynamoDbService.CheckUserExistsAsync(
                request.TargetUserId,
                cancellationToken
            );
            if (!targetUserExists)
            {
                Console.WriteLine(
                    $"[SendFriendRequestCommandHandler] Target user not found: {request.TargetUserId}"
                );
                return new APIGatewayProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonSerializer.Serialize(new { error = "Target user not found" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Check if already friends
            var alreadyFriends = await _friendsDynamoDbService.CheckIfAlreadyFriendsAsync(
                request.RequesterId,
                request.TargetUserId,
                cancellationToken
            );
            if (alreadyFriends)
            {
                Console.WriteLine(
                    $"[SendFriendRequestCommandHandler] Users already friends: {requester.UserId} and {request.TargetUserId}"
                );
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "Users are already friends" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Check if request already exists
            var existingRequest = await _friendsDynamoDbService.CheckExistingRequestAsync(
                request.RequesterId,
                request.TargetUserId,
                cancellationToken
            );
            if (existingRequest)
            {
                Console.WriteLine(
                    $"[SendFriendRequestCommandHandler] Friend request already exists from {requester.UserId} to {request.TargetUserId}"
                );
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "Friend request already sent" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Create and save friend request
            var friendRequest = CreateFriendRequest(
                requester.UserId,
                request.TargetUserId,
                request.Message ?? string.Empty
            );

            await _friendsDynamoDbService.SaveFriendRequestAsync(friendRequest, cancellationToken);
            Console.WriteLine(
                $"[SendFriendRequestCommandHandler] Successfully sent friend request from {requester.UserId} to {request.TargetUserId}"
            );

            await NotifyTargetUser(request.TargetUserId, friendRequest, cancellationToken);

            return new APIGatewayProxyResponse
            {
                StatusCode = 201,
                Body = JsonSerializer.Serialize(
                    new { message = "Friend request sent successfully" }
                ),
                Headers = GetCorsHeaders(),
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { error = ex.Message }),
                Headers = GetCorsHeaders(),
            };
        }
    }

    private async Task NotifyTargetUser(
        string requestTargetUserId,
        FriendRequest friendRequest,
        CancellationToken cancellationToken
    )
    {
        var user = await _friendsDynamoDbService.ReadUserAsync(
            requestTargetUserId,
            cancellationToken
        );
        var connectionId = user.ConnectionId;
        var socketMessage = new SocketMessage<string>
        {
            ConnectionId = connectionId,
            Action = "friends/requests/received",
            Message = JsonSerializer.Serialize(friendRequest),
        };
        var messageJson = JsonSerializer.Serialize(socketMessage);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var messageStream = new MemoryStream(messageBytes);
        var postConnectionRequest = new PostToConnectionRequest
        {
            ConnectionId = connectionId,
            Data = messageStream,
        };

        await _apiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
    }

    private static FriendRequest CreateFriendRequest(
        string requesterId,
        string targetUserId,
        string message
    )
    {
        return FriendRequest.Create(requesterId, targetUserId, message);
    }

    private static Dictionary<string, string> GetCorsHeaders()
    {
        return new Dictionary<string, string>
        {
            { "Access-Control-Allow-Origin", "*" },
            { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
            { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
        };
    }
}
