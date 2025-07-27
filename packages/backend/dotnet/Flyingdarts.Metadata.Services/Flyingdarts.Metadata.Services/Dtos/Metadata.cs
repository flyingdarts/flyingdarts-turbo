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
    /// Gets or sets the meeting identifier.
    /// </summary>
    public Guid? MeetingIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the meeting token.
    /// </summary>
    public string? MeetingToken { get; set; }

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
        var dictionary = new Dictionary<string, object>();

        if (Game != null)
            dictionary.Add("Game", Game);

        if (Players != null)
            dictionary.Add("Players", Players);

        if (Darts != null)
            dictionary.Add("Darts", Darts);

        if (NextPlayer != null)
            dictionary.Add("NextPlayer", NextPlayer);

        if (WinningPlayer != null)
            dictionary.Add("WinningPlayer", WinningPlayer);

        if (MeetingIdentifier != null)
            dictionary.Add("MeetingIdentifier", MeetingIdentifier);

        if (MeetingToken != null)
            dictionary.Add("MeetingToken", MeetingToken);

        return dictionary;
    }
}
