using System;
using System.Linq;
using Flyingdarts.Backend.Friends.Api.Models;
using Flyingdarts.Backend.Friends.Api.Services;

namespace Flyingdarts.Backend.Friends.Api.Requests.Queries.GetFriendRequests;

public class GetFriendRequestsQueryHandler : IRequestHandler<GetFriendRequestsQuery, APIGatewayProxyResponse>
{
    private readonly IFriendsDynamoDbService _friendsDynamoDbService;

    public GetFriendRequestsQueryHandler(IFriendsDynamoDbService friendsDynamoDbService)
    {
        _friendsDynamoDbService = friendsDynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(GetFriendRequestsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _friendsDynamoDbService.ReadUserByAuthProviderUserIdAsync(request.UserId, cancellationToken);

            Console.WriteLine($"[GetFriendRequestsQueryHandler] Starting to get friend requests for user: {request.UserId}");

            var incomingRequests = await _friendsDynamoDbService.GetIncomingFriendRequestsAsync(user.UserId, cancellationToken);
            Console.WriteLine(
                $"[GetFriendRequestsQueryHandler] Found {incomingRequests.Count} incoming friend requests for user: {user.UserId}"
            );

            var outgoingRequests = await _friendsDynamoDbService.GetOutgoingFriendRequestsAsync(user.UserId, cancellationToken);
            Console.WriteLine(
                $"[GetFriendRequestsQueryHandler] Found {outgoingRequests.Count} outgoing friend requests for user: {user.UserId}"
            );

            var requestDtos = await ConvertToFriendRequestDtosAsync(incomingRequests, outgoingRequests, cancellationToken);
            Console.WriteLine($"[GetFriendRequestsQueryHandler] Successfully converted {requestDtos.Count} friend requests to DTOs");

            var response = new
            {
                IncomingRequests = requestDtos.Where(r => r.TargetUserId == user.UserId).ToList(),
                OutgoingRequests = requestDtos.Where(r => r.RequesterId == user.UserId).ToList(),
            };

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(response),
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

    private async Task<List<FriendRequestDto>> ConvertToFriendRequestDtosAsync(
        List<FriendRequest> incomingRequests,
        List<FriendRequest> outgoingRequests,
        CancellationToken cancellationToken
    )
    {
        var requestDtos = new List<FriendRequestDto>();

        // Process incoming requests
        foreach (var request in incomingRequests)
        {
            try
            {
                var requesterUser = await _friendsDynamoDbService.ReadUserAsync(request.RequesterId, cancellationToken);
                requestDtos.Add(
                    new FriendRequestDto
                    {
                        RequestId = $"{request.TargetUserId}#{request.RequesterId}",
                        RequesterId = request.RequesterId,
                        RequesterUserName = requesterUser.Profile.UserName ?? "",
                        TargetUserId = request.TargetUserId,
                        Message = request.Message,
                        CreatedAt = request.CreatedAt,
                        Status = request.Status,
                    }
                );
            }
            catch
            {
                // Skip if user not found
                continue;
            }
        }

        // Process outgoing requests
        foreach (var request in outgoingRequests)
        {
            try
            {
                var targetUser = await _friendsDynamoDbService.ReadUserAsync(request.TargetUserId, cancellationToken);
                requestDtos.Add(
                    new FriendRequestDto
                    {
                        RequestId = $"{request.TargetUserId}#{request.RequesterId}#{request.CreatedAt.Ticks}",
                        RequesterId = request.RequesterId,
                        RequesterUserName = targetUser.Profile.UserName ?? "", // For outgoing, show target user name
                        TargetUserId = request.TargetUserId,
                        Message = request.Message,
                        CreatedAt = request.CreatedAt,
                        Status = request.Status,
                    }
                );
            }
            catch
            {
                // Skip if user not found
                continue;
            }
        }

        return requestDtos.OrderByDescending(r => r.CreatedAt).ToList();
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
