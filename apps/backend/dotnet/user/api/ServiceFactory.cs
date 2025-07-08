using Flyingdarts.Backend.User.Profile.Api.Requests.Create;

namespace Flyingdarts.Backend.User.Profile.Api;

public static class ServiceFactory
{
    public static ServiceProvider GetServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddSystemsManager($"/{System.Environment.GetEnvironmentVariable("EnvironmentName")}/Application")
            .Build();
        var services = new ServiceCollection();
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>(configuration.GetAWSOptions("DynamoDbTableName"));
        services.AddOptions<ApplicationOptions>();
        services.AddTransient<IDynamoDBContext, DynamoDBContext>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserProfileCommand).Assembly));
        return services.BuildServiceProvider();
    }
}