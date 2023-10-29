namespace Flyingdarts.Backend.Shared.Dtos;

public class GameDto
{
    public string Id { get; set; }
    public GameTypeDto Type { get; set; }
    public GameStatusDto Status { get; set; }
    public int PlayerCount { get; set; }
    public X01GameSettingsDto X01 { get; set; }
}
