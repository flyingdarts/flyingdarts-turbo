
using Flyingdarts.Connection.Services;
using Flyingdarts.Metadata.Services.Services.X01;

namespace Flyingdarts.Backend.Games.X01.Queue;

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
            .AddSystemsManager($"/{Environment.GetEnvironmentVariable("EnvironmentName")}/Application")
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
        // Register metadata services
        services.AddScoped<X01MetadataService>();
        // Register queue service
        services.AddScoped<QueueService<X01Queue>>();

        // Register Connection service
        services.AddScoped<ConnectionService>();

        // Register MediatR and register services from the assembly containing JoinX01GameCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HandleX01QueueCommand).Assembly));

        // Api Gateway Client.
        services.AddAWSService<IAmazonApiGatewayManagementApi>(configuration.GetAWSOptions("ApiGateway"));

        return services.BuildServiceProvider();
    }
}