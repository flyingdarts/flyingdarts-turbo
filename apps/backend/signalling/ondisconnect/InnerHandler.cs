using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Shared.Models;
using MediatR;
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
        try
        {
            return await _mediator.Send(request.Message!);
        }
        catch (AmazonDynamoDBException e)
        {
            return Responses.InternalServerError($"Failed to send message: {e.Message}");
        }
    }
}