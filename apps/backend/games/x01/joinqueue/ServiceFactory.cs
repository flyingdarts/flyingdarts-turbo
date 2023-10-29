using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
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

        // Register the DynamoDB context.
        services.AddTransient<IDynamoDBContext, DynamoDBContext>();

        // Register validators from the assembly containing the JoinX01QueueCommandValidator.
        services.AddValidatorsFromAssemblyContaining<JoinX01QueueCommandValidator>();

        // Register MediatR and register services from the assembly containing JoinX01QueueCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(JoinX01QueueCommand).Assembly));

        // Build and return the service provider.
        return services.BuildServiceProvider();
    }
}
