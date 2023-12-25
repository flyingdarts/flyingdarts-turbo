using Amazon.Lambda.APIGatewayEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

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