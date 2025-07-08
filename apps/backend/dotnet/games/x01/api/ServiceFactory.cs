using Flyingdarts.Connection.Services;
using Flyingdarts.Metadata.Services.Services.X01;

namespace Flyingdarts.Backend.Games.X01.Api;

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
            .AddSystemsManager(
                $"/{Environment.GetEnvironmentVariable("EnvironmentName")}/Application"
            )
            .Build();

        // Create a new service collection.
        var services = new ServiceCollection();

        // Configure AWS services.
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>(configuration.GetAWSOptions("DynamoDb"));

        // Register application options.
        services.AddOptions<ApplicationOptions>();

        // Register DynamoDbContext
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

        // Register DynaomDbService
        services.AddSingleton<IDynamoDbService, DynamoDbService>();

        // Register a caching service
        services.AddTransient<CachingService<X01State>>();

        // Register metadata services
        services.AddTransient<X01MetadataService>();

        // Register Connection service
        services.AddTransient<ConnectionService>();

        // Register MediatR and register services from the assembly containing JoinX01GameCommand.
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(CreateX01GameCommand).Assembly)
        );

        // Api Gateway Client.
        services.AddAWSService<IAmazonApiGatewayManagementApi>(
            configuration.GetAWSOptions("ApiGateway")
        );

        return services.BuildServiceProvider();
    }
}
