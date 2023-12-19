using System.ComponentModel;

namespace Flyingdarts.Backend.Tournaments.Challonge.Models
{
    public enum RankedBy
    {
        [Description("match wins")]
        MatchWins,
        [Description("game wins")]
        GameWins,
        [Description("points scored")]
        PointsScored,
        [Description("points difference")]
        PointsDifference,
        [Description("custom")]
        Custom
    }
}
