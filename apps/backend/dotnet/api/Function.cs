using Flyingdarts.Backend.Api;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

// Get the service provider
var services = ServiceFactory.GetServiceProvider();
var mediator = services.GetRequiredService<IMediator>();

// Create and run the Lambda function using the new bootstrap pattern
var bootstrap = new BackendApi(mediator);

await bootstrap.RunAsync();

namespace Flyingdarts.Backend.Api
{
    static class X01ApiWebSocketMethods
    {
        // TODO: Add signalling methods here (e.g. connect, disconnect, and default) which are used in the signalling api
        // This will result in the gaming endpoint already being available immediately after the user connected.
        // Thus eliminating the cold boot when low amount of / no users are connected.
        public const string Create = "games/x01/create";
        public const string Join = "games/x01/join";
        public const string Score = "games/x01/score";
    }
}
