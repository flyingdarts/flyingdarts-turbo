using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Games.X01.Queue.CQRS;

public class HandleX01QueueCommand : IRequest
{
    public List<X01Queue> Records { get; } = new();
    public X01Queue Owner { get; set; }
}