namespace Flyingdarts.Metadata.Services.Dtos;

/// <summary>
/// Represents the complete metadata for an X01 darts game, including game state, players, and darts information.
/// </summary>
public class Metadata
{
    /// <summary>
    /// Gets or sets the game information.
    /// </summary>
    public GameDto? Game { get; set; }

    /// <summary>
    /// Gets or sets the ordered list of players in the game.
    /// </summary>
    public IOrderedEnumerable<PlayerDto>? Players { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of darts organized by player ID.
    /// </summary>
    public Dictionary<string, List<DartDto>>? Darts { get; set; }

    /// <summary>
    /// Gets or sets the ID of the player who should throw next.
    /// </summary>
    public string? NextPlayer { get; set; }

    /// <summary>
    /// Gets or sets the ID of the winning player, if the game has ended.
    /// </summary>
    public string? WinningPlayer { get; set; }

    /// <summary>
    /// Converts the metadata to a dictionary for serialization.
    /// </summary>
    /// <returns>A dictionary representation of the metadata.</returns>
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object?>
        {
            { "Game", Game },
            { "Players", Players },
            { "Darts", Darts },
            { "NextPlayer", NextPlayer },
            { "WinningPlayer", WinningPlayer }
        };
    }
}
