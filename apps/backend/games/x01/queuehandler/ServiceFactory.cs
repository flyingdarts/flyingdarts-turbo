using Amazon;
using Microsoft.Extensions.DependencyInjection;
using Amazon.SimpleEmail;
using Flyingdarts.Backend.Games.X01.Queue.CQRS;

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

        // Register MediatR and register services from the assembly containing SendVerifyUserEmailCommand.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HandleQueueCommand).Assembly));

        // Build and return the service provider.
        return services.BuildServiceProvider();
    }
}