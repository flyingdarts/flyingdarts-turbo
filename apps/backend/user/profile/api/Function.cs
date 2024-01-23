using System;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    try
    {
        if (request.Resource == "/users/profile")
        {
            switch (request.HttpMethod)
            {
                case "GET":
                    return await innerHandler.Handle(new GetUserProfileQuery { AuthProviderUserId = request.RequestContext.Authorizer.GetValueOrDefault("UserId").ToString() });
                case "POST":
                    return await innerHandler.Handle(JsonSerializer.Deserialize<CreateUserProfileCommand>(request.Body));
                case "PUT":
                    return await innerHandler.Handle(JsonSerializer.Deserialize<UpdateUserProfileCommand>(request.Body));
            }
        }
    }
    catch (Exception ex)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = $"{ex.Message}\n {ex.StackTrace}"
        };
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

