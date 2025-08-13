using Authress.SDK;
using Authress.SDK.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Config via env vars
var authressApiUrl =
    Environment.GetEnvironmentVariable("AUTHRESS_API_URL")
    ?? throw new InvalidOperationException(
        "Missing env AUTHRESS_API_URL (e.g. https://yourdomain.api.authress.io)"
    );
var accessKey =
    Environment.GetEnvironmentVariable("AUTHRESS_SERVICE_CLIENT_ACCESS_KEY")
    ?? throw new InvalidOperationException("Missing env AUTHRESS_SERVICE_CLIENT_ACCESS_KEY");

// Services
builder.Services.AddSingleton(new AuthressSettings { AuthressApiUrl = authressApiUrl });
builder.Services.AddSingleton(new AuthressClientTokenProvider(accessKey));

// If you need an AuthressClient for other calls, register it too
builder.Services.AddSingleton<AuthressClient>(sp =>
{
    var settings = sp.GetRequiredService<AuthressSettings>();
    var provider = sp.GetRequiredService<AuthressClientTokenProvider>();
    return new AuthressClient(provider, settings);
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Returns a service JWT minted via Service Client Provider
app.MapGet(
    "/token/service",
    ([FromServices] AuthressClientTokenProvider provider) =>
    {
        // The provider issues a valid JWT for your service client.
        // If the SDK version exposes async, switch to an async handler and await accordingly.
        var jwt = provider.GetBearerToken();
        return Results.Ok(new { token = jwt });
    }
);

app.Run();
