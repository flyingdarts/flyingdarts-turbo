using System;
using Flyingdarts.Backend.Friends.Api.Models;
using Flyingdarts.Backend.Friends.Api.Services;

namespace Flyingdarts.Backend.Friends.Api.Requests.Queries.GetFriends;

public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, APIGatewayProxyResponse>
{
    private readonly IFriendsDynamoDbService _friendsDynamoDbService;

    public GetFriendsQueryHandler(IFriendsDynamoDbService friendsDynamoDbService)
    {
        _friendsDynamoDbService = friendsDynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _friendsDynamoDbService.ReadUserByAuthProviderUserIdAsync(request.UserId, cancellationToken);

            Console.WriteLine($"[GetFriendsQueryHandler] Starting to get friends for user: {user.UserId}");

            var friends = await _friendsDynamoDbService.GetUserFriendsAsync(user.UserId, cancellationToken);

            Console.WriteLine($"[GetFriendsQueryHandler] Found {friends.Count} friend relationships for user: {user.UserId}");

            var friendDtos = await ConvertToFriendDtosAsync(friends, cancellationToken);
            Console.WriteLine(
                $"[GetFriendsQueryHandler] Successfully converted {friendDtos.Count} friends to DTOs for user: {user.UserId}"
            );

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(friendDtos),
                Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
                },
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { error = ex.Message }),
                Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
                },
            };
        }
    }

    private async Task<List<FriendDto>> ConvertToFriendDtosAsync(
        List<FriendRelationship> relationships,
        CancellationToken cancellationToken
    )
    {
        var friendDtos = new List<FriendDto>();

        foreach (var relationship in relationships)
        {
            try
            {
                var friendUser = await _friendsDynamoDbService.ReadUserAsync(relationship.FriendId, cancellationToken);

                var openGameId = await OpenGameId(friendUser.UserId, cancellationToken);

                friendDtos.Add(
                    new FriendDto
                    {
                        UserId = friendUser.UserId,
                        UserName = friendUser.Profile.UserName,
                        Country = friendUser.Profile.Country,
                        FriendsSince = relationship.AcceptedAt ?? relationship.CreatedAt,
                        IsOnline = !string.IsNullOrEmpty(friendUser.ConnectionId),
                        ConnectionId = friendUser.ConnectionId,
                        Picture = friendUser.Profile.Picture,
                        OpenGameId = openGameId,
                    }
                );
            }
            catch
            {
                // Skip if user not found
                continue;
            }
        }

        return friendDtos;
    }

    private async Task<string?> OpenGameId(string userId, CancellationToken cancellationToken)
    {
        if (await _friendsDynamoDbService.GetOpenGameByUserIdAsync(long.Parse(userId), cancellationToken) is var game && game is not null)
        {
            return game.GameId.ToString();
        }
        return null;
    }
}
