using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Flyingdarts.Backend.Games.X01.Queue.CQRS;
using Flyingdarts.Persistence;

// Get the service provider
var services = ServiceFactory.GetServiceProvider();

// Create an instance of the InnerHandler using the service provider
var innerHandler = new InnerHandler(services);

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (DynamoDBEvent dynamoEvent, ILambdaContext context) =>
{
    foreach (var record in dynamoEvent.Records)
    {
        if (record.EventName == OperationType.INSERT)
        {
            // Deserialize the new item from the DynamoDB stream record
            var newImage = record.Dynamodb.NewImage;
            var deserializedMessage = JsonSerializer.Deserialize<X01Queue>(ConvertDictionaryToJson(newImage));

            if (deserializedMessage != null)
            {
                Console.WriteLine("Succesfully deserialized a message");
                var command = new HandleX01QueueCommand
                {
                    Owner = deserializedMessage
                };
                await innerHandler.Handle(command, context);
            }
        }
    }
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer)
.Build()
    .RunAsync();

static string ConvertDictionaryToJson(Dictionary<string, AttributeValue> dictionary)
{
    // Create a Dictionary<string, object> to hold the deserialized values
    Dictionary<string, object> convertedDictionary = new Dictionary<string, object>();

    // Iterate over the original dictionary and convert AttributeValues to their .NET equivalents
    foreach (var kvp in dictionary)
    {
        string key = kvp.Key;
        AttributeValue attributeValue = kvp.Value;

        // Convert AttributeValue to .NET equivalent
        object value = GetDotNetValueFromAttributeValue(attributeValue);

        // Add the key-value pair to the converted dictionary
        convertedDictionary.Add(key, value);
    }

    // Serialize the converted dictionary to a JSON string
    return JsonSerializer.Serialize(convertedDictionary);
}

static object GetDotNetValueFromAttributeValue(AttributeValue attributeValue)
{
    // Handle different types of AttributeValue (S, N, etc.)
    if (attributeValue.S != null)
    {
        return attributeValue.S;
    }
    else if (attributeValue.N != null)
    {
        // Assuming N represents a number, parse it to the appropriate .NET numeric type
        return int.Parse(attributeValue.N);
    }
    // Add more cases as needed for other AttributeValue types

    // If no type matches, return null or handle the case accordingly
    return null;
}