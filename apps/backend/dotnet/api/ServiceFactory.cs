using Amazon.DynamoDBv2.DataModel;
using Flyingdarts.Backend.Games.X01.Api.Requests.Create;
using Flyingdarts.Connection.Services;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Meetings.Service.Factories;
using Flyingdarts.Meetings.Service.Generated.Dyte;
using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Meetings.Service.Services.DyteMeetingService;
using Flyingdarts.Metadata.Services.Services.X01;
using Flyingdarts.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Flyingdarts.Backend.Games.X01.Api;

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
        return Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.GetServiceProvider(
            (services, configuration) =>
            {
                // Register DynamoDB services
                services.AddTransient<IDynamoDBContext, DynamoDBContext>();
                services.AddSingleton<IDynamoDbService, DynamoDbService>();

                // Configure X01-specific services
                services.AddScoped<CachingService<X01State>>();
                services.AddScoped<X01MetadataService>();
                services.AddScoped<ConnectionService>();

                // Register Meeting services
                services.AddSingleton<DyteApiClientFactory>(sp => new DyteApiClientFactory(sp.GetRequiredService<HttpClient>()));
                services.AddTransient<DyteApiClient>(sp => sp.GetRequiredService<DyteApiClientFactory>().GetClient());
                services.AddTransient<IDyteApiClientWrapper, DyteApiClientWrapper>();
                services.AddSingleton<IMeetingService, DyteMeetingService>();

                // Configure MediatR and validation for this assembly
                Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.ConfigureMediatR(services, typeof(CreateX01GameCommand));
                Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.ConfigureValidation(services, typeof(CreateX01GameCommand));
            }
        );
    }
}
