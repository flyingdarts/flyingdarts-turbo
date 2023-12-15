using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Flyingdarts.Backend.Shared;
using Flyingdarts.Backend.Shared.Extensions;
using Flyingdarts.Backend.Tournaments.Create.CQRS;

// Get the service provider
var services = ServiceFactory.GetServiceProvider();

// Create an instance of the InnerHandler using the service provider
var innerHandler = new InnerHandler(services);

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    //// Convert the APIGatewayProxyRequest to the specified CreateX01ScoreCommand type using the serializer
    //var command = request.ToRequest<CreateTournamentCommand>(serializer);

    //// Handle the socketRequest using the innerHandler
    //return await innerHandler.Handle(command);
    return new APIGatewayProxyResponse
    {
        StatusCode = 200,
        Body = "A yeet"
    };
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

