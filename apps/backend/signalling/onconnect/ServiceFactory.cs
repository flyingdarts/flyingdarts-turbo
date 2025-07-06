using System;
using Amazon.DynamoDBv2;
using Flyingdarts.Backend.Signalling.OnConnect.CQRS;
using Flyingdarts.Meetings.Service.Extensions;
using Flyingdarts.Meetings.Service.Factories;
using Flyingdarts.Meetings.Service.Generated.Dyte;

namespace Flyingdarts.Backend.Signalling.OnConnect;

public static class ServiceFactory
{
    public static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(OnConnectCommand).Assembly)
        );
        // services.AddOptions<ApplicationOptions>();
        services.AddKiotaHandlers();
        services
            .AddHttpClient<DyteApiClient>(
                (sp, client) =>
                {
                    var rootUrlString =
                        "https://dyte-api-dev.redmoss-62dfd9a6.westeurope.azurecontainerapps.io";
                    // var options = sp.GetRequiredService<IOptions<ApplicationOptions>>();
                    client.BaseAddress = new Uri(rootUrlString);
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", options.Value.GetDyteAuthorizationHeaderValue());
                }
            )
            .AttachKiotaHandlers();

        services.AddTransient(sp => sp.GetRequiredService<DyteApiClientFactory>().GetClient());

        return services.BuildServiceProvider();
    }
}
