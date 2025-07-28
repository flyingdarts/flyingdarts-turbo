using System;
using System.IO;
using System.Text;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Flyingdarts.Backend.Friends.Api.Services;
using Flyingdarts.Core.Models;

namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.RespondToFriendRequest;

public class RespondToFriendRequestCommandHandler : IRequestHandler<RespondToFriendRequestCommand, APIGatewayProxyResponse>
{
    private readonly IFriendsDynamoDbService _friendsDynamoDbService;
    private readonly IAmazonApiGatewayManagementApi _apiGatewayClient;

    public RespondToFriendRequestCommandHandler(
        IFriendsDynamoDbService friendsDynamoDbService,
        IAmazonApiGatewayManagementApi apiGatewayClient
    )
    {
        _friendsDynamoDbService = friendsDynamoDbService;
        _apiGatewayClient = apiGatewayClient;
    }

    public async Task<APIGatewayProxyResponse> Handle(RespondToFriendRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _friendsDynamoDbService.ReadUserByAuthProviderUserIdAsync(request.UserId, cancellationToken);

            Console.WriteLine(
                $"[RespondToFriendRequestCommandHandler] User {user.UserId} responding to friend request {request.RequestId} with Accept={request.Accept}"
            );

            // Find the friend request
            var friendRequest = await _friendsDynamoDbService.GetFriendRequestAsync(request.RequestId, cancellationToken);

            if (friendRequest == null)
            {
                Console.WriteLine($"[RespondToFriendRequestCommandHandler] Friend request not found: {request.RequestId}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonSerializer.Serialize(new { error = "Friend request not found" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Verify the user is authorized to respond to this request
            if (friendRequest.TargetUserId != user.UserId)
            {
                Console.WriteLine(
                    $"[RespondToFriendRequestCommandHandler] Unauthorized response attempt. User {user.UserId} tried to respond to request for user {friendRequest.TargetUserId}"
                );
                return new APIGatewayProxyResponse
                {
                    StatusCode = 403,
                    Body = JsonSerializer.Serialize(new { error = "Not authorized to respond to this request" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Check if request is still pending
            if (friendRequest.Status != FriendRequestStatus.Pending)
            {
                Console.WriteLine(
                    $"[RespondToFriendRequestCommandHandler] Friend request {request.RequestId} is no longer pending. Current status: {friendRequest.Status}"
                );
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "Friend request is no longer pending" }),
                    Headers = GetCorsHeaders(),
                };
            }

            if (request.Accept)
            {
                Console.WriteLine(
                    $"[RespondToFriendRequestCommandHandler] Accepting friend request {request.RequestId} from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
                );
                await _friendsDynamoDbService.AcceptFriendRequestAsync(friendRequest, cancellationToken);
                Console.WriteLine($"[RespondToFriendRequestCommandHandler] Successfully accepted friend request {request.RequestId}");

                await NotifyFriendsRequestUser(friendRequest.RequesterId, friendRequest, cancellationToken);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(new { message = "Friend request accepted" }),
                    Headers = GetCorsHeaders(),
                };
            }
            else
            {
                Console.WriteLine(
                    $"[RespondToFriendRequestCommandHandler] Declining friend request {request.RequestId} from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
                );
                await _friendsDynamoDbService.DeclineFriendRequestAsync(friendRequest, cancellationToken);
                Console.WriteLine($"[RespondToFriendRequestCommandHandler] Successfully declined friend request {request.RequestId}");

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(new { message = "Friend request declined" }),
                    Headers = GetCorsHeaders(),
                };
            }
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

    private async Task NotifyFriendsRequestUser(
        string requestTargetUserId,
        FriendRequest friendRequest,
        CancellationToken cancellationToken
    )
    {
        var user = await _friendsDynamoDbService.ReadUserAsync(requestTargetUserId, cancellationToken);
        var connectionId = user.ConnectionId;
        var socketMessage = new SocketMessage<string>
        {
            ConnectionId = connectionId,
            Action = "friends/requests/responded",
            Message = JsonSerializer.Serialize(friendRequest),
        };
        var messageJson = JsonSerializer.Serialize(socketMessage);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var messageStream = new MemoryStream(messageBytes);
        var postConnectionRequest = new PostToConnectionRequest { ConnectionId = connectionId, Data = messageStream };

        await _apiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
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
