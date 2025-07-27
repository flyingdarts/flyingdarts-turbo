using Flyingdarts.Lambda.Core.Abstractions;

namespace Flyingdarts.Lambda.Core.Handlers;

/// <summary>
/// Base Lambda handler that uses MediatR to process requests
/// </summary>
/// <typeparam name="TRequest">The type of MediatR request to handle</typeparam>
public class MediatRLambdaHandler<TRequest> : ILambdaHandler<TRequest>
    where TRequest : IRequest<APIGatewayProxyResponse>
{
    private readonly IMediator _mediator;

    public MediatRLambdaHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Handles the request using MediatR
    /// </summary>
    /// <param name="request">The MediatR request to handle</param>
    /// <returns>The API Gateway response</returns>
    public async Task<APIGatewayProxyResponse> Handle(TRequest request)
    {
        return await _mediator.Send(request);
    }
}
