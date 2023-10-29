using System;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Flyingdarts.Shared;
using Microsoft.Extensions.Options;
using Flyingdarts.Backend.Shared.Models;

public class InnerHandler
{
    private readonly IMediator _mediator;
    private readonly ApplicationOptions _applicationOptions;
    public InnerHandler()
    {
    }
    public InnerHandler(ServiceProvider serviceProvider)
    {
        _mediator = serviceProvider.GetRequiredService<IMediator>();
        _applicationOptions = serviceProvider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    }
    public async Task<APIGatewayProxyResponse> Handle(SocketMessage<CreateUserProfileCommand> request, ILambdaContext context)
    {
        try
        {
            if (request?.Message is null)
                throw new BadRequestException("Unable to parse request.", typeof(CreateUserProfileCommand));
            context.Logger.LogInformation(_applicationOptions.DynamoDbTable);

            return await _mediator.Send(request.Message);
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"{ex.Message}\n{ex.StackTrace}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = ex.Message
            };
        }
    }
}