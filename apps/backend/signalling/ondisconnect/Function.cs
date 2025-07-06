using Flyingdarts.Backend.Signalling.OnDisconnect;
using Flyingdarts.Backend.Signalling.OnDisconnect.CQRS;
using Flyingdarts.Backend.Signalling.OnDisconnect.Models;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services.GetRequiredService<IMediator>());
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request) =>
{
    var socketRequest = new SocketMessage<OnDisconnectCommand>
    {
        Message = new OnDisconnectCommand
        {
            ConnectionId = request.RequestContext.ConnectionId
        }
    };
    return await innerHandler.Handle(socketRequest);
};

await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

