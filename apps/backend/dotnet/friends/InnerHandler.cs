using System;

namespace Flyingdarts.Backend.Friends.Api;

public class InnerHandler
{
    private readonly IMediator _mediator;

    public InnerHandler(IServiceProvider serviceProvider)
    {
        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    public async Task<APIGatewayProxyResponse> Handle<T>(T request)
        where T : IRequest<APIGatewayProxyResponse>
    {
        return await _mediator.Send(request);
    }
}
