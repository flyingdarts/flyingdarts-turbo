using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Flyingdarts.Shared;
using Microsoft.Extensions.Configuration;
using Amazon.ApiGatewayManagementApi;
using Flyingdarts.Persistence;

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

        // Register the DynamoDB context.
        services.AddTransient<IDynamoDBContext, DynamoDBContext>();
        
        // Register GameService with Reads and Writes.
        services.AddTransient<IDynamoDbService, DynamoDbService>();

        // Register validators from the assembly containing the CreateX01ScoreCommandValidatorr.
        services.AddValidatorsFromAssemblyContaining<CreateX01ScoreCommandValidator>();

        // Register MediatR and register services from the assembly containing CreateX01ScoreCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateX01ScoreCommand).Assembly));

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
