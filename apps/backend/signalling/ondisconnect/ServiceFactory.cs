using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;

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