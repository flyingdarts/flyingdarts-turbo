using Flyingdarts.Persistence;
using System;

namespace Flyingdarts.Backend.Games.X01.JoinQueue;

public class PlayerJoinedQueueMessage
{
    public string PlayerId { get; set; }
    
    public X01GameSettings? X01 { get; set; }

    public int Average { get; set; }

    public DateTime Joined { get; set; }

    public DateTime LastProcessed { get; set; }

    public bool DoneProcessing { get; set; }

    public string ConnectionId { get; set; }

}