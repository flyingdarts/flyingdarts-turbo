using System;
using Flyingdarts.Backend.Friends.Api;
using Flyingdarts.Backend.Friends.Api.Requests.Commands.InviteFriendToGame;
using Flyingdarts.Backend.Friends.Api.Requests.Commands.RemoveFriend;
using Flyingdarts.Backend.Friends.Api.Requests.Commands.RespondToFriendRequest;
using Flyingdarts.Backend.Friends.Api.Requests.Commands.SendFriendRequest;
using Flyingdarts.Backend.Friends.Api.Requests.Queries;
using Flyingdarts.Backend.Friends.Api.Requests.Queries.GetFriendRequests;
using Flyingdarts.Backend.Friends.Api.Requests.Queries.GetFriends;
using Flyingdarts.Backend.Friends.Api.Requests.Queries.GetUser;
using Flyingdarts.Backend.Friends.Api.Requests.Queries.SearchUsers;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    try
    {
        var userId =
            request.RequestContext.Authorizer?.GetValueOrDefault("UserId")?.ToString()
            ?? string.Empty;
        return request.Resource switch
        {
            "/friends/user" when request.HttpMethod == "GET" => await innerHandler.Handle(
                new GetUserQuery { UserId = userId }
            ),

            "/friends" when request.HttpMethod == "GET" => await innerHandler.Handle(
                new GetFriendsQuery { UserId = userId }
            ),

            "/friends/search" when request.HttpMethod == "POST" => await innerHandler.Handle(
                (
                    JsonSerializer.Deserialize<SearchUsersQuery>(request.Body)
                    ?? new SearchUsersQuery()
                ) with
                {
                    SearchByUserId = userId,
                }
            ),

            "/friends/request" when request.HttpMethod == "POST" => await innerHandler.Handle(
                (
                    JsonSerializer.Deserialize<SendFriendRequestCommand>(request.Body)
                    ?? new SendFriendRequestCommand()
                ) with
                {
                    RequesterId = userId,
                }
            ),

            "/friends/request/{requestId}" when request.HttpMethod == "PUT" =>
                await innerHandler.Handle(
                    (
                        JsonSerializer.Deserialize<RespondToFriendRequestCommand>(request.Body)
                        ?? new RespondToFriendRequestCommand()
                    ) with
                    {
                        RequestId =
                            request.PathParameters != null
                            && request.PathParameters.TryGetValue("requestId", out var rawRequestId)
                                ? Uri.UnescapeDataString(rawRequestId)
                                : string.Empty,
                        UserId = userId,
                    }
                ),

            "/friends/{friendId}" when request.HttpMethod == "DELETE" => await innerHandler.Handle(
                new RemoveFriendCommand
                {
                    UserId = userId,
                    FriendId =
                        request.PathParameters != null
                        && request.PathParameters.TryGetValue("friendId", out var fid)
                            ? fid
                            : string.Empty,
                }
            ),

            "/friends/requests" when request.HttpMethod == "GET" => await innerHandler.Handle(
                new GetFriendRequestsQuery { UserId = userId }
            ),

            "/friends/invite" when request.HttpMethod == "POST" => await innerHandler.Handle(
                (
                    JsonSerializer.Deserialize<InviteFriendToGameCommand>(request.Body)
                    ?? new InviteFriendToGameCommand()
                ) with
                {
                    InviterId = userId,
                }
            ),
            _ => new APIGatewayProxyResponse { StatusCode = 404 },
        };
    }
    catch (Exception ex)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = $"{ex.Message}\n {ex.StackTrace}",
            Headers = new Dictionary<string, string>()
            {
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
            },
        };
    }
};

await LambdaBootstrapBuilder.Create(handler, serializer).Build().RunAsync();
