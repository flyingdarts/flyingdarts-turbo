using Flyingdarts.Backend.Signalling.OnDisconnect.CQRS;
using Flyingdarts.Backend.Signalling.OnDisconnect.Models;

namespace Flyingdarts.Backend.Signalling.OnDisconnect;

public class InnerHandler
{
    private readonly IMediator _mediator;

    public InnerHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<APIGatewayProxyResponse> Handle(SocketMessage<OnDisconnectCommand> request)
    {
        return await _mediator.Send(request.Message!);
    }
}