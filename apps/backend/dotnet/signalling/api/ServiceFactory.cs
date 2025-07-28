using Flyingdarts.Backend.Signalling.Api.Requests.Connect;
using Flyingdarts.Backend.Signalling.Api.Requests.Default;
using Flyingdarts.Backend.Signalling.Api.Requests.Disconnect;
using Microsoft.Extensions.DependencyInjection;

namespace Flyingdarts.Backend.Signalling.Api;

public static class ServiceFactory
{
    public static ServiceProvider GetServiceProvider()
    {
        return Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.GetServiceProvider(
            (services, configuration) =>
            {
                // Configure MediatR with all command types from the consolidated API
                Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.ConfigureMediatR(services, typeof(OnConnectCommand));
                Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.ConfigureMediatR(services, typeof(OnDefaultCommand));
                Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.ConfigureMediatR(services, typeof(OnDisconnectCommand));

                // Register DyteApiClient and its factory
                services.AddSingleton<Flyingdarts.Meetings.Service.Factories.DyteApiClientFactory>();
                services.AddSingleton(provider =>
                    provider.GetRequiredService<Flyingdarts.Meetings.Service.Factories.DyteApiClientFactory>().GetClient()
                );

                // Register DyteApiClientWrapper
                services.AddSingleton<
                    Flyingdarts.Meetings.Service.Services.IDyteApiClientWrapper,
                    Flyingdarts.Meetings.Service.Services.DyteApiClientWrapper
                >();

                // Register IMeetingService
                services.AddSingleton<
                    Flyingdarts.Meetings.Service.Services.IMeetingService,
                    Flyingdarts.Meetings.Service.Services.DyteMeetingService.DyteMeetingService
                >();

                // Register DynamoDB services and options
                services.AddOptions<Flyingdarts.Persistence.ApplicationOptions>();
                services.AddTransient<Amazon.DynamoDBv2.DataModel.IDynamoDBContext, Amazon.DynamoDBv2.DataModel.DynamoDBContext>();
                services.AddSingleton<Flyingdarts.DynamoDb.Service.IDynamoDbService, Flyingdarts.DynamoDb.Service.DynamoDbService>();

                // Register Amazon DynamoDB client for OnDefault and OnDisconnect handlers
                services.AddSingleton<Amazon.DynamoDBv2.IAmazonDynamoDB, Amazon.DynamoDBv2.AmazonDynamoDBClient>();
            }
        );
    }
}
