namespace Flyingdarts.Backend.Friends.Api.Requests.Queries.SearchUsers;

public record SearchUsersQuery : IRequest<APIGatewayProxyResponse>
{
    public string SearchTerm { get; set; } = string.Empty;
    public string? SearchByUserId { get; set; } // The user performing the search
}
