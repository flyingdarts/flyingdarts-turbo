namespace Flyingdarts.E2E.TestData;

/// <summary>
/// Test data for game settings configurations
/// </summary>
public static class GameSettingsData
{
    /// <summary>
    /// Default game settings
    /// </summary>
    public static class Default
    {
        public const int Sets = 3;
        public const int Legs = 5;
    }

    /// <summary>
    /// Quick game settings (shorter games)
    /// </summary>
    public static class Quick
    {
        public const int Sets = 1;
        public const int Legs = 3;
    }

    /// <summary>
    /// Long game settings (extended games)
    /// </summary>
    public static class Long
    {
        public const int Sets = 5;
        public const int Legs = 7;
    }

    /// <summary>
    /// Tournament settings
    /// </summary>
    public static class Tournament
    {
        public const int Sets = 7;
        public const int Legs = 9;
    }

    /// <summary>
    /// Minimum valid settings
    /// </summary>
    public static class Minimum
    {
        public const int Sets = 1;
        public const int Legs = 1;
    }

    /// <summary>
    /// Maximum valid settings (reasonable limits)
    /// </summary>
    public static class Maximum
    {
        public const int Sets = 10;
        public const int Legs = 15;
    }

    /// <summary>
    /// Invalid settings for negative testing
    /// </summary>
    public static class Invalid
    {
        public const int NegativeSets = -1;
        public const int NegativeLegs = -5;
        public const int ZeroSets = 0;
        public const int ZeroLegs = 0;
        public const int ExcessiveSets = 100;
        public const int ExcessiveLegs = 200;
    }

    /// <summary>
    /// Get a custom game settings configuration
    /// </summary>
    /// <param name="sets">Number of sets</param>
    /// <param name="legs">Number of legs</param>
    /// <returns>Game settings configuration</returns>
    public static (int Sets, int Legs) Custom(int sets, int legs)
    {
        return (sets, legs);
    }

    /// <summary>
    /// Get all valid game settings configurations for testing
    /// </summary>
    /// <returns>Array of valid game settings</returns>
    public static (int Sets, int Legs)[] GetAllValidConfigurations()
    {
        return new[]
        {
            Custom(Default.Sets, Default.Legs),
            Custom(Quick.Sets, Quick.Legs),
            Custom(Long.Sets, Long.Legs),
            Custom(Tournament.Sets, Tournament.Legs),
            Custom(Minimum.Sets, Minimum.Legs),
            Custom(Maximum.Sets, Maximum.Legs),
        };
    }

    /// <summary>
    /// Get all invalid game settings configurations for negative testing
    /// </summary>
    /// <returns>Array of invalid game settings</returns>
    public static (int Sets, int Legs)[] GetAllInvalidConfigurations()
    {
        return new[]
        {
            Custom(Invalid.NegativeSets, Invalid.NegativeLegs),
            Custom(Invalid.ZeroSets, Invalid.ZeroLegs),
            Custom(Invalid.ExcessiveSets, Invalid.ExcessiveLegs),
        };
    }
}
