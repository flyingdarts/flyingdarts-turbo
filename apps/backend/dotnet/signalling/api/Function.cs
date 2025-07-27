using Flyingdarts.Backend.Signalling.Api;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

// Get the service provider
var services = ServiceFactory.GetServiceProvider();
var mediator = services.GetRequiredService<IMediator>();

// Create and run the Lambda function using the new bootstrap pattern
var bootstrap = new SignallingApiBootstrap(mediator);

await bootstrap.RunAsync();

namespace Flyingdarts.Backend.Signalling.Api
{
    static class SignallingApiWebSocketMethods
    {
        public const string Connect = "$connect";
        public const string Default = "$default";
        public const string Disconnect = "$disconnect";
    }
}
