namespace Flyingdarts.Lambda.Core.Infrastructure;

/// <summary>
/// Base service factory that provides common DI setup for Lambda functions
/// </summary>
public abstract class ServiceFactory
{
    /// <summary>
    /// Gets the service provider with common services configured
    /// </summary>
    /// <returns>The configured service provider</returns>
    public static ServiceProvider GetServiceProvider()
    {
        var configuration = BuildConfiguration();
        var services = new ServiceCollection();

        ConfigureCommonServices(services, configuration);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Gets the service provider with common services and additional configuration
    /// </summary>
    /// <param name="configureServices">Action to configure additional services</param>
    /// <returns>The configured service provider</returns>
    public static ServiceProvider GetServiceProvider(
        Action<IServiceCollection, IConfiguration> configureServices
    )
    {
        var configuration = BuildConfiguration();
        var services = new ServiceCollection();

        ConfigureCommonServices(services, configuration);
        configureServices(services, configuration);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Builds the configuration using AWS Systems Manager Parameter Store
    /// </summary>
    /// <returns>The configuration</returns>
    protected static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddSystemsManager(
                $"/{Environment.GetEnvironmentVariable("EnvironmentName")}/Application"
            )
            .Build();
    }

    /// <summary>
    /// Configures common services that are used across all Lambda functions
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    protected static void ConfigureCommonServices(
        IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configure AWS services
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>(configuration.GetAWSOptions("DynamoDb"));

        // Configure IAmazonApiGatewayManagementApi with WebSocket endpoint
        services.AddTransient<IAmazonApiGatewayManagementApi>(provider =>
        {
            var config = new AmazonApiGatewayManagementApiConfig
            {
                ServiceURL = System.Environment.GetEnvironmentVariable("WebSocketApiUrl")!
            };
            return new AmazonApiGatewayManagementApiClient(config);
        });

        // Register HttpClient for API clients
        services.AddHttpClient();
    }

    /// <summary>
    /// Configures MediatR with the specified assembly
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblyMarker">The assembly containing MediatR handlers</param>
    public static void ConfigureMediatR(IServiceCollection services, Type assemblyMarker)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assemblyMarker.Assembly));
    }

    /// <summary>
    /// Configures FluentValidation with the specified assembly
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblyMarker">The assembly containing validators</param>
    public static void ConfigureValidation(IServiceCollection services, Type assemblyMarker)
    {
        services.AddValidatorsFromAssemblyContaining(assemblyMarker);
    }
}
