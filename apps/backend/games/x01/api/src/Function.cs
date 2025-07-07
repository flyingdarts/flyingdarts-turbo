// Get the service provider
var services = ServiceFactory.GetServiceProvider();

// Create an instance of the InnerHandler using the service provider
var innerHandler = new InnerHandler(services);

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (APIGatewayProxyRequest request) =>
{
    // Convert the APIGatewayProxyRequest to the specified JoinX01QueueCommand type using the serializer
    switch (request.RequestContext.RouteKey)
    {
        case "games/x01/create":
            var create = request.To<CreateX01GameCommand>(serializer);
            create.Message!.ConnectionId = request.RequestContext.ConnectionId;
            return await innerHandler.Handle(create.Message!);
        case "games/x01/join":
            var join = request.To<JoinX01GameCommand>(serializer);
            join.Message!.ConnectionId = request.RequestContext.ConnectionId;
            return await innerHandler.Handle(join.Message!);
        case "games/x01/joinqueue":
            var joinQueue = request.To<JoinX01QueueCommand>(serializer);
            joinQueue.Message!.ConnectionId = request.RequestContext.ConnectionId;
            return await innerHandler.Handle(joinQueue.Message!);
        case "games/x01/score":
            var score = request.To<CreateX01ScoreCommand>(serializer);
            score.Message!.ConnectionId = request.RequestContext.ConnectionId;
            return await innerHandler.Handle(score.Message!);
    }

    return new APIGatewayProxyResponse { StatusCode = 404, Body = "Resource not found" };
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer).Build().RunAsync();
