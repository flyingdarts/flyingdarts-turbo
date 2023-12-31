using System.Threading.Tasks;
using Amazon.DynamoDBv2;

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