using System.Linq;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Flyingdarts.Backend.Games.X01.Queue.CQRS;
using Flyingdarts.Backend.Games.X01.Queue.Messages;

// Get the service provider
var services = ServiceFactory.GetServiceProvider();

// Create an instance of the InnerHandler using the service provider
var innerHandler = new InnerHandler(services);

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (SQSRecordSet records, ILambdaContext context) =>
{
    var handleQueueCommand = new HandleQueueCommand();
    foreach (var message in records.Records)
    {
        var deserializedMessage = JsonSerializer.Deserialize<PlayerJoinedQueueMessage>(message.Body);
        if (deserializedMessage != null)
        {
            handleQueueCommand.Messages.Add(deserializedMessage);
        }
    }

    await innerHandler.Handle(handleQueueCommand, context);
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

public class SQSRecordSet 
{
    public SQSEvent.SQSMessage[] Records { get; set; }
}