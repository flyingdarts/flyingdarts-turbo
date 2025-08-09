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
        var dyteAuthorizationHeaderValue = Environment.GetEnvironmentVariable(
            "DyteAuthorizationHeaderValue"
        );

        if (string.IsNullOrEmpty(dyteAuthorizationHeaderValue))
        {
            throw new InvalidOperationException(
                "DyteAuthorizationHeaderValue environment variable must be set"
            );
        }

        request.Headers.Add("Authorization", dyteAuthorizationHeaderValue);

        await Task.CompletedTask;
    }
}
