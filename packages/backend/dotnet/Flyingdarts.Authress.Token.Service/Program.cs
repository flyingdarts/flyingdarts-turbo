using Authress.SDK;
using Authress.SDK.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var accessKey =
    Environment.GetEnvironmentVariable("AUTHRESS_SERVICE_CLIENT_ACCESS_KEY")
    ?? throw new InvalidOperationException("Missing env AUTHRESS_SERVICE_CLIENT_ACCESS_KEY");

// Services
builder.Services.AddSingleton(new AuthressClientTokenProvider(accessKey));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Returns a service JWT minted via Service Client Provider
app.MapGet(
    "/token/service",
    async ([FromServices] AuthressClientTokenProvider provider) =>
    {
        // The provider issues a valid JWT for your service client.
        var jwt = await provider.GetBearerToken();
        return Results.Ok(new { token = jwt });
    }
);

app.Run();
