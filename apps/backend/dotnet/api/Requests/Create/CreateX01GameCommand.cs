using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Api.Models;
using MediatR;

namespace Flyingdarts.Backend.Api.Requests.Create;

/// <summary>
/// Command for creating a new X01 game.
/// </summary>
public class CreateX01GameCommand : Connectable, IRequest<APIGatewayProxyResponse>
{
    /// <summary>
    /// Gets or sets the unique identifier of the player creating the game.
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of sets in the game.
    /// </summary>
    public int Sets { get; set; }

    /// <summary>
    /// Gets or sets the number of legs per set.
    /// </summary>
    public int Legs { get; set; }

    /// <summary>
    /// Gets or sets the number of players in the game.
    /// </summary>
    public int PlayerCount { get; init; } = 2;

    /// <summary>
    /// Gets or sets the unique identifier of the game.
    /// This is provided by the client and used for game identification.
    /// </summary>
    public string GameId { get; set; } = string.Empty;
}
