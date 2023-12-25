using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Flyingdarts.Backend.Shared.Extensions;
using System.Text.Json;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request) =>
{
    var command = JsonSerializer.Deserialize<UpdateUserProfileCommand>(request.Body);
    return await innerHandler.Handle(command);
};

await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

