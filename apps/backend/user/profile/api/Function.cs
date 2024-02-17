using System;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    try
    {
        var userId = request.RequestContext.Authorizer.GetValueOrDefault("UserId").ToString();
        if (request.Resource == "/users/profile")
        {
            switch (request.HttpMethod)
            {
                case "GET":
                    return await innerHandler.Handle(new GetUserProfileQuery { AuthProviderUserId = userId });
                case "POST":
                    return await innerHandler.Handle(JsonSerializer.Deserialize<CreateUserProfileCommand>(request.Body));
                case "PUT":
                    var command = JsonSerializer.Deserialize<UpdateUserProfileCommand>(request.Body);
                    command.UserId = userId;
                    Console.WriteLine(command);
                    return await innerHandler.Handle(command);
            }
        }
    }
    catch (Exception ex)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = $"{ex.Message}\n {ex.StackTrace}",
            Headers = new Dictionary<string, string>() {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                }
        };
    }

    return new APIGatewayProxyResponse
    {
        StatusCode = 404,
        Body = "Resource not found",
        Headers = new Dictionary<string, string>() {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                }
    };
};

await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

