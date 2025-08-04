using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Api.Models;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Api.Requests.Join;

/// <summary>
/// Command for joining an existing X01 game.
/// </summary>
public class JoinX01GameCommand : Connectable, IRequest<APIGatewayProxyResponse>
{
    /// <summary>
    /// Gets or sets the unique identifier of the game to join.
    /// </summary>
    public string GameId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the player joining the game.
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the player joining the game.
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the game instance. This is typically set internally during processing.
    /// </summary>
    public Game? Game { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the join operation.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
