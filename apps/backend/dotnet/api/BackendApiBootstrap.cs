using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using Flyingdarts.Backend.Api.Requests.Create;
using Flyingdarts.Backend.Api.Requests.Join;
using Flyingdarts.Backend.Api.Requests.Score;
using Flyingdarts.Core.Extensions;
using Flyingdarts.Lambda.Core.Handlers;
using Flyingdarts.Lambda.Core.Infrastructure;
using MediatR;

namespace Flyingdarts.Backend.Api;

public class BackendApi : ApiGatewayLambdaBootstrap<IRequest<APIGatewayProxyResponse>>
{
    private readonly IMediator _mediator;

    public BackendApi(IMediator mediator)
        : base(new MediatRLambdaHandler<IRequest<APIGatewayProxyResponse>>(mediator))
    {
        _mediator = mediator;
    }

    protected override IRequest<APIGatewayProxyResponse> ConvertRequest(
        APIGatewayProxyRequest request
    )
    {
        if (request.RequestContext?.RouteKey == null)
        {
            throw new ArgumentException("Invalid request context");
        }

        var requestContext = request.RequestContext!;
        var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

        return requestContext.RouteKey switch
        {
            X01ApiWebSocketMethods.Create => ConvertCreateRequest(request, serializer),
            X01ApiWebSocketMethods.Join => ConvertJoinRequest(request, serializer),
            X01ApiWebSocketMethods.Score => ConvertScoreRequest(request, serializer),
            _ => throw new ArgumentException($"Unknown route key: {requestContext.RouteKey}"),
        };
    }

    private IRequest<APIGatewayProxyResponse> ConvertCreateRequest(
        APIGatewayProxyRequest request,
        DefaultLambdaJsonSerializer serializer
    )
    {
        var create = request.To<CreateX01GameCommand>(serializer);
        if (create?.Message == null)
        {
            throw new ArgumentException("Invalid request message");
        }
        create.Message.ConnectionId = request.RequestContext!.ConnectionId ?? string.Empty;
        create.Message.PlayerId =
            request.RequestContext!.Authorizer?.GetValueOrDefault("UserId")?.ToString()
            ?? string.Empty;
        return create.Message;
    }

    private IRequest<APIGatewayProxyResponse> ConvertJoinRequest(
        APIGatewayProxyRequest request,
        DefaultLambdaJsonSerializer serializer
    )
    {
        var join = request.To<JoinX01GameCommand>(serializer);
        if (join?.Message == null)
        {
            throw new ArgumentException("Invalid request message");
        }
        join.Message.ConnectionId = request.RequestContext!.ConnectionId ?? string.Empty;
        join.Message.PlayerId =
            request.RequestContext!.Authorizer?.GetValueOrDefault("UserId")?.ToString()
            ?? string.Empty;
        return join.Message;
    }

    private IRequest<APIGatewayProxyResponse> ConvertScoreRequest(
        APIGatewayProxyRequest request,
        DefaultLambdaJsonSerializer serializer
    )
    {
        var score = request.To<CreateX01ScoreCommand>(serializer);
        if (score?.Message == null)
        {
            throw new ArgumentException("Invalid request message");
        }
        score.Message.ConnectionId = request.RequestContext!.ConnectionId ?? string.Empty;
        score.Message.PlayerId =
            request.RequestContext!.Authorizer?.GetValueOrDefault("UserId")?.ToString()
            ?? string.Empty;
        return score.Message;
    }
}
