using Flyingdarts.Backend.User.Profile.VerifyEmail.CQRS;

namespace Flyingdarts.Backend.User.Profile.VerifyEmail;

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