using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceFactory
{
    public static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddValidatorsFromAssemblyContaining<OnConnectCommandValidator>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OnConnectCommand).Assembly));
        return services.BuildServiceProvider();
    }
}