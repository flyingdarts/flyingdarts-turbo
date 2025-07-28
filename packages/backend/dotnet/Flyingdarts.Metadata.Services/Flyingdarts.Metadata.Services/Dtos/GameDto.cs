namespace Flyingdarts.Metadata.Services.Dtos;

/// <summary>
/// Represents game information in the metadata.
/// </summary>
public class GameDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the game.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the game.
    /// </summary>
    public GameTypeDto Type { get; set; }

    /// <summary>
    /// Gets or sets the current status of the game.
    /// </summary>
    public GameStatusDto Status { get; set; }

    /// <summary>
    /// Gets or sets the number of players in the game.
    /// </summary>
    public int PlayerCount { get; set; }

    /// <summary>
    /// Gets or sets the X01-specific game settings.
    /// </summary>
    public X01GameSettingsDto? X01 { get; set; }
}
