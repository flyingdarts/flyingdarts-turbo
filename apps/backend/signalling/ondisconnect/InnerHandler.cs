using Amazon.DynamoDBv2;
using System.Threading.Tasks;

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