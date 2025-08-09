using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Api.Models;
using MediatR;

namespace Flyingdarts.Backend.Api.Requests.Score;

/// <summary>
/// Command for creating a score entry in an X01 game.
/// </summary>
public class CreateX01ScoreCommand : Connectable, IRequest<APIGatewayProxyResponse>
{
    /// <summary>
    /// Gets or sets the unique identifier of the game.
    /// </summary>
    public string GameId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the player making the score.
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the score value for the dart throw.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the input value for the dart throw.
    /// </summary>
    public int Input { get; set; }
}
