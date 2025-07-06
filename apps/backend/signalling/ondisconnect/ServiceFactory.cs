using Flyingdarts.Backend.Signalling.OnDisconnect.CQRS;

namespace Flyingdarts.Backend.Signalling.OnDisconnect;

public static class ServiceFactory
{
    public static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddValidatorsFromAssemblyContaining<OnDisconnectCommandValidator>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OnDisconnectCommand).Assembly));
        return services.BuildServiceProvider();
    }
}