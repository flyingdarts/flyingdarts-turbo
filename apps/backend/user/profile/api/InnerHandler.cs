namespace Flyingdarts.Backend.User.Profile.Api;

public class InnerHandler
{
    private readonly IMediator _mediator;
    
    public InnerHandler()
    {
    }
    public InnerHandler(ServiceProvider serviceProvider)
    {
        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }
    public async Task<APIGatewayProxyResponse> Handle(IRequest<APIGatewayProxyResponse> request)
    {
        return await _mediator.Send(request);
    }
}