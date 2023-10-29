using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Shared.Models;
using MediatR;
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
        try
        {
            return await _mediator.Send(request.Message);
        }
        catch (AmazonDynamoDBException e)
        {
            return Responses.InternalServerError($"Failed to send message: {e.Message}");
        }
    }

    
}