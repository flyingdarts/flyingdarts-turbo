using System.Text;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Flyingdarts.Meetings.Service.Factories;

public class DyteApiClientFactory
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly HttpClient _httpClient;

    public DyteApiClientFactory(HttpClient httpClient)
    {
        _authenticationProvider = new DyteAuthenticationProvider();
        _httpClient = httpClient;
    }

    public DyteApiClient GetClient()
    {
        return new DyteApiClient(
            new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient)
        );
    }
}

public class DyteAuthenticationProvider : IAuthenticationProvider
{
    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default
    )
    {
        var dyteOrganizationId = Environment.GetEnvironmentVariable("DyteOrganizationId");
        var dyteApiKey = Environment.GetEnvironmentVariable("DyteApiKey");

        if (string.IsNullOrEmpty(dyteOrganizationId) || string.IsNullOrEmpty(dyteApiKey))
        {
            throw new InvalidOperationException(
                "DyteOrganizationId and DyteApiKey environment variables must be set"
            );
        }

        var authHeaderValue = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{dyteOrganizationId}:{dyteApiKey}")
        );
        request.Headers.Add("Authorization", $"Basic {authHeaderValue}");
        await Task.CompletedTask;
    }
}
