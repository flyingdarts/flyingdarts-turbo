using System;
using Flyingdarts.Backend.Friends.Api.Models;
using Flyingdarts.Backend.Friends.Api.Services;

namespace Flyingdarts.Backend.Friends.Api.Requests.Queries.SearchUsers;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, APIGatewayProxyResponse>
{
    private readonly IFriendsDynamoDbService _friendsDynamoDbService;

    public SearchUsersQueryHandler(IFriendsDynamoDbService friendsDynamoDbService)
    {
        _friendsDynamoDbService = friendsDynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine(
                $"[SearchUsersQueryHandler] Starting user search with term: '{request.SearchTerm}' by user: {request.SearchByUserId}"
            );

            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                Console.WriteLine($"[SearchUsersQueryHandler] Search term is empty, returning 400");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "SearchTerm is required" }),
                    Headers = GetCorsHeaders(),
                };
            }

            var users = await _friendsDynamoDbService.SearchUsersAsync(request.SearchTerm, cancellationToken);

            Console.WriteLine($"[SearchUsersQueryHandler] Found user matching search term: '{request.SearchTerm}'");

            var userDtos = await ConvertToUserSearchDtosAsync(users, request.SearchByUserId, cancellationToken);
            Console.WriteLine($"[SearchUsersQueryHandler] Converted {userDtos.Count} users to search DTOs");

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(userDtos),
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

    private async Task<List<UserSearchDto>> ConvertToUserSearchDtosAsync(
        List<User> users,
        string? searchByUserId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[SearchUsersQueryHandler] Converting {users.Count} users to search DTOs for searcher: {searchByUserId ?? "anonymous"}"
        );

        var userDtos = new List<UserSearchDto>();
        var processedCount = 0;

        foreach (var user in users)
        {
            // Skip the searching user
            if (user.UserId == searchByUserId)
            {
                Console.WriteLine($"[SearchUsersQueryHandler] Skipping self (user {user.UserId}) from search results");
                continue;
            }

            var isAlreadyFriend = false;
            var hasPendingRequest = false;

            if (!string.IsNullOrEmpty(searchByUserId))
            {
                Console.WriteLine($"[SearchUsersQueryHandler] Checking friendship status between {searchByUserId} and {user.UserId}");

                isAlreadyFriend = await _friendsDynamoDbService.CheckIfAlreadyFriendsAsync(searchByUserId, user.UserId, cancellationToken);
                hasPendingRequest = await _friendsDynamoDbService.CheckExistingRequestAsync(searchByUserId, user.UserId, cancellationToken);

                Console.WriteLine(
                    $"[SearchUsersQueryHandler] User {user.UserId}: IsFriend={isAlreadyFriend}, HasPendingRequest={hasPendingRequest}"
                );
            }

            userDtos.Add(
                new UserSearchDto
                {
                    UserId = user.UserId,
                    UserName = user.Profile.UserName ?? "",
                    Country = user.Profile.Country ?? "",
                    IsAlreadyFriend = isAlreadyFriend,
                    HasPendingRequest = hasPendingRequest,
                    Picture = user.Profile.Picture ?? "",
                }
            );

            processedCount++;
            if (processedCount % 5 == 0) // Log progress every 5 users
            {
                Console.WriteLine($"[SearchUsersQueryHandler] Processed {processedCount}/{users.Count} users");
            }
        }

        Console.WriteLine($"[SearchUsersQueryHandler] Completed conversion. Final result: {userDtos.Count} search DTOs");
        return userDtos;
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
