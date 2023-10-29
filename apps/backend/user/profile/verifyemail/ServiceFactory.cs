using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Flyingdarts.Shared;
using Microsoft.Extensions.Configuration;
using Amazon.SimpleEmail;

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
        // Create a new service collection.
        var services = new ServiceCollection();

        // Configure AWS services.
        services.AddTransient<IAmazonSimpleEmailService>(sp =>
        {
            var awsRegion = RegionEndpoint.EUWest1; // Replace with your desired region
            return new AmazonSimpleEmailServiceClient(awsRegion);
        });

        // Register validators from the assembly containing the SendVerifyUserEmailCommandValidator.
        services.AddValidatorsFromAssemblyContaining<SendVerifyUserEmailCommandValidator>();

        // Register MediatR and register services from the assembly containing SendVerifyUserEmailCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendVerifyUserEmailCommand).Assembly));

        // Build and return the service provider.
        return services.BuildServiceProvider();
    }
}
