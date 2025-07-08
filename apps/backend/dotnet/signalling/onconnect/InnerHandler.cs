using System.Threading.Tasks;
using Flyingdarts.Backend.Signalling.OnConnect.CQRS;
using Flyingdarts.Backend.Signalling.OnConnect.Models;

namespace Flyingdarts.Backend.Signalling.OnConnect;

public class InnerHandler
{
    private IMediator _mediator;

    public InnerHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<APIGatewayProxyResponse> Handle(SocketMessage<OnConnectCommand> request)
    {
        return await _mediator.Send(request.Message);
    }
}