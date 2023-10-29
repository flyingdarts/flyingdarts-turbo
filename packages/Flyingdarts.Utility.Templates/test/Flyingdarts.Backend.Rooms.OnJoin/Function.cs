using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

var innerHandler = new InnerHandler(mediator);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request) =>
{
    var socketRequest = request.To<JoinRoomCommand>(serializer);
    return await innerHandler.Handle();
};

await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();