using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Lambdas.Shared;
using MediatR;

public class InnerHandler
{
    private readonly IMediator _mediator;

    public InnerHandler()
    {
    }
    public InnerHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task<APIGatewayProxyResponse> Handle(SocketMessage<JoinRoomCommand> request)
    {
        throw new NotImplementedException();
    }
}