using Microsoft.Extensions.DependencyInjection;
using Flyingdarts.Backend.Games.X01.Queue.CQRS;
using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Flyingdarts.Backend.Shared.Services;
using Flyingdarts.Persistence;
using Flyingdarts.Shared;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Factory class for creating the service provider.
/// </summary>
public static class ServiceFactory
{
    /// <summary>
    /// Creates and configures the service provider.
    /// </summary>
    /// <returns>The configured service provider.</returns>
    public static ServiceProvider GetServiceProvider()
    {
        // Build the configuration using AWS Systems Manager Parameter Store.
        var configuration = new ConfigurationBuilder()
            .AddSystemsManager($"/{System.Environment.GetEnvironmentVariable("EnvironmentName")}/Application")
            .Build();

        // Create a new service collection.
        var services = new ServiceCollection();

        // Configure AWS services.
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>(configuration.GetAWSOptions("DynamoDb"));

        // Register application options.
        services.AddOptions<ApplicationOptions>();

        services.AddTransient<IDynamoDBContext, DynamoDBContext>();

        services.AddSingleton<IDynamoDbService, DynamoDbService>();
        // Register a caching service
        services.AddScoped<CachingService<X01State>>();
        // Register a metadata service
        services.AddScoped<X01MetadataService>();

        // Register Connection service
        services.AddScoped<ConnectionService>();

        // Register MediatR and register services from the assembly containing JoinX01GameCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HandleX01QueueCommand).Assembly));

        // Api Gateway Client.
        services.AddTransient<IAmazonApiGatewayManagementApi>(provider =>
        {
            var config = new AmazonApiGatewayManagementApiConfig
            {
                ServiceURL = System.Environment.GetEnvironmentVariable("WebSocketApiUrl")!
            };

            return new AmazonApiGatewayManagementApiClient(config);
        });

        // Build and return the service provider.
        return services.BuildServiceProvider();
    }
}