using Amazon.ApiGatewayManagementApi;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using Flyingdarts.Backend.Signalling.Api.Requests.Connect;
using Flyingdarts.Backend.Signalling.Api.Requests.Default;
using Flyingdarts.Backend.Signalling.Api.Requests.Disconnect;
using Flyingdarts.Core.Extensions;
using Flyingdarts.Lambda.Core.Handlers;
using Flyingdarts.Lambda.Core.Infrastructure;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api;

public class SignallingApiBootstrap : ApiGatewayLambdaBootstrap<IRequest<APIGatewayProxyResponse>>
{
    private readonly IMediator _mediator;

    public SignallingApiBootstrap(IMediator mediator)
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
            SignallingApiWebSocketMethods.Connect => ConvertConnectRequest(request, serializer),
            SignallingApiWebSocketMethods.Default => ConvertDefaultRequest(request, serializer),
            SignallingApiWebSocketMethods.Disconnect => ConvertDisconnectRequest(
                request,
                serializer
            ),
            _ => throw new ArgumentException($"Unknown route key: {requestContext.RouteKey}"),
        };
    }

    private IRequest<APIGatewayProxyResponse> ConvertConnectRequest(
        APIGatewayProxyRequest request,
        DefaultLambdaJsonSerializer serializer
    )
    {
        return new OnConnectCommand
        {
            ConnectionId = request.RequestContext!.ConnectionId ?? string.Empty,
            AuthProviderUserId =
                request.RequestContext!.Authorizer?.GetValueOrDefault("UserId")?.ToString()
                ?? string.Empty,
            AuthressToken = NormalizeAuthressToken(request.QueryStringParameters["idToken"]),
        };
    }

    public string NormalizeAuthressToken(string authressToken)
    {
        if (string.IsNullOrEmpty(authressToken))
        {
            return string.Empty;
        }
        var normalizedToken = authressToken.Trim();
        if (normalizedToken.StartsWith("user="))
        {
            return normalizedToken.Substring(5);
        }

        return authressToken;
    }

    private IRequest<APIGatewayProxyResponse> ConvertDefaultRequest(
        APIGatewayProxyRequest request,
        DefaultLambdaJsonSerializer serializer
    )
    {
        var defaultCommand = request.To<OnDefaultCommand>(serializer);
        if (defaultCommand?.Message == null)
        {
            throw new ArgumentException("Invalid request message");
        }

        defaultCommand.Message.ConnectionId = request.RequestContext!.ConnectionId ?? string.Empty;

        // Set up the API Gateway Management API client for broadcasting messages
        var webSocketApiUrl = Environment.GetEnvironmentVariable("WebSocketApiUrl");
        if (!string.IsNullOrEmpty(webSocketApiUrl))
        {
            defaultCommand.Message.ApiGatewayManagementApiClient =
                new AmazonApiGatewayManagementApiClient(
                    new AmazonApiGatewayManagementApiConfig { ServiceURL = webSocketApiUrl }
                );
        }

        return defaultCommand.Message;
    }

    private IRequest<APIGatewayProxyResponse> ConvertDisconnectRequest(
        APIGatewayProxyRequest request,
        DefaultLambdaJsonSerializer serializer
    )
    {
        return new OnDisconnectCommand
        {
            ConnectionId = request.RequestContext!.ConnectionId ?? string.Empty,
        };
    }
}
