using System;
using Amazon.ApiGatewayManagementApi;
using Flyingdarts.Backend.Friends.Api.Requests.Commands.SendFriendRequest;
using Flyingdarts.Backend.Friends.Api.Services;
using Flyingdarts.DynamoDb.Service;

namespace Flyingdarts.Backend.Friends.Api;

public static class ServiceFactory
{
    public static ServiceProvider GetServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddSystemsManager($"/{Environment.GetEnvironmentVariable("EnvironmentName")}/Application")
            .Build();

        var services = new ServiceCollection();

        // Configure AWS services
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>(configuration.GetAWSOptions("DynamoDbTableName"));

        // Configure IAmazonApiGatewayManagementApi with WebSocket endpoint
        services.AddTransient<IAmazonApiGatewayManagementApi>(provider =>
        {
            var config = new AmazonApiGatewayManagementApiConfig
            {
                ServiceURL = System.Environment.GetEnvironmentVariable("WebSocketApiUrl")!,
            };
            return new AmazonApiGatewayManagementApiClient(config);
        });

        // Register application options
        services.AddOptions<ApplicationOptions>();

        // Register DynamoDB services
        services.AddTransient<IDynamoDBContext, DynamoDBContext>();
        services.AddTransient<IDynamoDbService, DynamoDbService>();
        services.AddTransient<IFriendsDynamoDbService, FriendsDynamoDbService>();

        // Register MediatR and register services from the assembly containing SendFriendRequestCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendFriendRequestCommand).Assembly));

        return services.BuildServiceProvider();
    }
}
