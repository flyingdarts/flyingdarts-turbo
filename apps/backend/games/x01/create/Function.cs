

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
    var socketRequest = request.To<CreateX01GameCommand>(serializer);
    socketRequest.Message.ConnectionId = request.RequestContext.ConnectionId;
    // Handle the socketRequest using the innerHandler
    return await innerHandler.Handle(socketRequest);
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();