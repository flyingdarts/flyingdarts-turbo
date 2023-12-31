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

        // Register DynamoDbContext
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

        // Register DynaomDbService
        services.AddSingleton<IDynamoDbService, DynamoDbService>();


        // Register a caching service
        services.AddTransient<CachingService<X01State>>();

        // Register a metadata service
        services.AddTransient<X01MetadataService>();

        // Register Connection service
        services.AddTransient<ConnectionService>();

        // Register validators from the assembly containing the JoinX01GameCommandValidator.
        services.AddValidatorsFromAssemblyContaining<CreateX01GameCommandValidator>();

        // Register MediatR and register services from the assembly containing JoinX01GameCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateX01GameCommand).Assembly));

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
