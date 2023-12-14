using Flyingdarts.Backend.Games.X01.Queue.Messages;
using MediatR;

namespace Flyingdarts.Backend.Games.X01.Queue.CQRS;

public class HandleQueueCommand : IRequest
{
    public List<PlayerJoinedQueueMessage> Messages { get; } = new();
    public List<PlayerJoinedQueueMessage> MatchingPlayers { get; set; }
} 