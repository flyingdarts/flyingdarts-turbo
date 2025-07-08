namespace Flyingdarts.Metadata.Services.Dtos;

/// <summary>
/// Represents X01-specific game settings and configuration.
/// </summary>
public class X01GameSettingsDto
{
    /// <summary>
    /// Gets or sets the number of sets in the game.
    /// </summary>
    public int Sets { get; set; }

    /// <summary>
    /// Gets or sets the number of legs per set.
    /// </summary>
    public int Legs { get; set; }

    /// <summary>
    /// Gets or sets whether players must start with a double.
    /// </summary>
    public bool DoubleIn { get; set; }

    /// <summary>
    /// Gets or sets whether players must finish with a double.
    /// </summary>
    public bool DoubleOut { get; set; }

    /// <summary>
    /// Gets or sets the starting score for the game (typically 501).
    /// </summary>
    public int StartingScore { get; set; }
}
