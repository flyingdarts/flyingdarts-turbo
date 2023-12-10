using Flyingdarts.Persistence;

namespace Flyingdarts.Backend.Games.X01.Queue.Messages;

public class PlayerJoinedQueueMessage
{
    public string PlayerId { get; set; }
    
    public X01GameSettings? X01 { get; set; }
    public int Average { get; set; }
}