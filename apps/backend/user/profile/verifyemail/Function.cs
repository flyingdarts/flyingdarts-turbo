

// Get the service provider

using Flyingdarts.Backend.User.Profile.VerifyEmail;
using Flyingdarts.Backend.User.Profile.VerifyEmail.CQRS;

var services = ServiceFactory.GetServiceProvider();

// Create an instance of the InnerHandler using the service provider
var innerHandler = new InnerHandler(services);

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (SQSRecordSet records, ILambdaContext context) =>
{
    context.Logger.Log(JsonSerializer.Serialize(records));
    var command = JsonSerializer.Deserialize<SendVerifyUserEmailCommand>(records.Records.First().Body);
    await innerHandler.Handle(command, context);
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();