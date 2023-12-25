using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request) =>
{
    var query = new GetUserProfileQuery
    {
        CognitoUserName =
        request.QueryStringParameters["cognitoUserName"]
    };
    return await innerHandler.Handle(query);
};

await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

