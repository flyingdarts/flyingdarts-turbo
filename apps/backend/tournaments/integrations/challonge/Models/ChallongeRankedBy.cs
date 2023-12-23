using System.ComponentModel;

namespace Flyingdarts.Backend.Tournaments.Challonge.Models
{
    // TODO: create description converter
    public enum ChallongeRankedBy
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
