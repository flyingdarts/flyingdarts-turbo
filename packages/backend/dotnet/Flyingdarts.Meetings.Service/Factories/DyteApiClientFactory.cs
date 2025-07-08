using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Flyingdarts.Meetings.Service.Factories;

public class DyteApiClientFactory
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly HttpClient _httpClient;

    public DyteApiClientFactory(HttpClient httpClient)
    {
        _authenticationProvider = new AnonymousAuthenticationProvider();
        _httpClient = httpClient;
    }

    public DyteApiClient GetClient()
    {
        return new DyteApiClient(
            new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient)
        );
    }
}
