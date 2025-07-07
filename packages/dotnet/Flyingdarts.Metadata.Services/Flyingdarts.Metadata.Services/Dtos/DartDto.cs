namespace Flyingdarts.Metadata.Services.Dtos;

/// <summary>
/// Represents a single dart throw in the game metadata.
/// </summary>
public class DartDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the dart.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the game identifier.
    /// </summary>
    public long GameId { get; set; }

    /// <summary>
    /// Gets or sets the player identifier who threw this dart.
    /// </summary>
    public string? PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the score achieved with this dart.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the remaining game score after this dart.
    /// </summary>
    public int GameScore { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp in ticks.
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the leg number in which this dart was thrown.
    /// </summary>
    public int Leg { get; set; }

    /// <summary>
    /// Gets or sets the set number in which this dart was thrown.
    /// </summary>
    public int Set { get; set; }
}
