namespace Flyingdarts.Backend.Games.X01.Services.Metadata.Dtos;

public class DartDto
{
    public Guid Id { get; set; }
    public long GameId { get; set; }
    public string PlayerId { get; set; }
    public int Score { get; set; }
    public int GameScore { get; set; }
    public long CreatedAt { get; set; }
    public int Leg { get; set; }
    public int Set { get; set; }
}
