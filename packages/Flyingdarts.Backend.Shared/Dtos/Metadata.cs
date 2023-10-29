namespace Flyingdarts.Backend.Shared.Dtos;

public class Metadata
{
    public GameDto Game { get; set; }
    public IOrderedEnumerable<PlayerDto> Players { get; set; }
    public Dictionary<string, List<DartDto>> Darts { get; set; }
    public string NextPlayer { get; set; }
    public string WinningPlayer { get; set; }
    public Dictionary<string, object> toDictionary()
    {
        var result = new Dictionary<string, object>
        {
            { "Game", Game },
            { "Players", Players },
            { "Darts", Darts },
            { "NextPlayer", NextPlayer },
            { "WinningPlayer", WinningPlayer }
        };

        return result;
    }
}
