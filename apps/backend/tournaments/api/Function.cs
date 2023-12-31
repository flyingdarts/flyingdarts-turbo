

// Get the service provider
var services = ServiceFactory.GetServiceProvider();

// Create an instance of the InnerHandler using the service provider
var innerHandler = new InnerHandler(services);

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    if (request.Resource == "/tournaments/create")
    {
        switch (request.HttpMethod)
        {
            case "POST":
                return await innerHandler.Handle(JsonSerializer.Deserialize<CreateTournamentCommand>(request.Body));
            case "PUT":
                return await innerHandler.Handle(JsonSerializer.Deserialize<StartTournamentCommand>(request.Body));
        }
    }

    if (request.Resource == "/tournaments/participants")
    {
        switch (request.HttpMethod)
        {
            case "POST":
                return await innerHandler.Handle(JsonSerializer.Deserialize<CreateTournamentParticipantCommand>(request.Body));
        }
    }

    if (request.Resource == "/tournaments/matches")
    {
        switch (request.HttpMethod)
        {
            case "PUT":
                return await innerHandler.Handle(JsonSerializer.Deserialize<UpdateTournamentMatchCommand>(request.Body));
        }
    }

    return new APIGatewayProxyResponse
    {
        StatusCode = 404,
        Body = "Resource not found"
    };
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

