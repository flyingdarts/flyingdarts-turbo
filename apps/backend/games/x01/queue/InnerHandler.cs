using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Flyingdarts.Backend.Games.X01.Queue.CQRS;

/// <summary>
/// Represents the inner handler for processing APIGatewayProxyRequest.
/// </summary>
public class InnerHandler
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the InnerHandler class.
    /// </summary>
    public InnerHandler()
    {
    }

    /// <summary>
    /// Initializes a new instance of the InnerHandler class with a service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public InnerHandler(ServiceProvider serviceProvider)
    {
        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Handles the socket message request and returns the APIGatewayProxyResponse.
    /// </summary>
    /// <param name="request">The socket message request containing the SendVerifyUserEmailCommand.</param>
    /// <returns>The APIGatewayProxyResponse.</returns>
    public async Task Handle(HandleX01QueueCommand queueMessage, ILambdaContext context)
    {
        // Send the message to the mediator for further processing
        await _mediator.Send(queueMessage);
    }
}