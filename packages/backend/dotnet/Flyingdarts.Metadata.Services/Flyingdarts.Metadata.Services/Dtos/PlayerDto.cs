namespace Flyingdarts.Metadata.Services.Dtos;

/// <summary>
/// Represents player information in the metadata.
/// </summary>
public class PlayerDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the player.
    /// </summary>
    public string? PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the display name of the player.
    /// </summary>
    public string? PlayerName { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp as a string.
    /// </summary>
    public string? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the country code of the player.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the number of sets won by the player as a string.
    /// </summary>
    public string? Sets { get; set; }

    /// <summary>
    /// Gets or sets the number of legs won by the player as a string.
    /// </summary>
    public string? Legs { get; set; }
}