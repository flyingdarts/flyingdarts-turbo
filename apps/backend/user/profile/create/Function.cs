using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using System.Text.Json;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    if (request.Resource == "/users/profile")
    {
        switch (request.HttpMethod)
        {
            case "GET":
                return await innerHandler.Handle(new GetUserProfileQuery { CognitoUserName = request.QueryStringParameters["cognitoUserName"] });
            case "POST":
                return await innerHandler.Handle(JsonSerializer.Deserialize<CreateUserProfileCommand>(request.Body));
            case "PUT":
                return await innerHandler.Handle(JsonSerializer.Deserialize<UpdateUserProfileCommand>(request.Body));
        }
    }

    return new APIGatewayProxyResponse
    {
        StatusCode = 404,
        Body = "Resource not found"
    };
};

await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

