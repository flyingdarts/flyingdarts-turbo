using Authress.SDK;
using Authress.SDK.Client;

namespace Flyingdarts.E2E.Utilities;

/// <summary>
/// Helper class for Authress authentication operations with performance optimizations
/// </summary>
public class AuthressHelper
{
    private readonly string _accessKey;
    private readonly string _authressUrl;
    private readonly AuthressClientTokenProvider _tokenProvider;
    private readonly TokenCache _tokenCache;

    public AuthressHelper(string accessKey)
    {
        _accessKey = accessKey;
        _authressUrl = "https://authress.flyingdarts.net";
        _tokenProvider = new AuthressClientTokenProvider(_accessKey, _authressUrl);
        _tokenCache = new TokenCache();
    }

    /// <summary>
    /// Get a bearer token for authentication with caching
    /// </summary>
    /// <returns>Bearer token string</returns>
    public async Task<string> GetBearerTokenAsync()
    {
        return await _tokenCache.GetOrCreateTokenAsync(
            "default",
            async () =>
            {
                try
                {
                    var token = await _tokenProvider.GetBearerToken();

                    if (string.IsNullOrEmpty(token))
                    {
                        throw new InvalidOperationException("Received empty token from Authress");
                    }

                    return token;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to get Authress token: {ex.Message}",
                        ex
                    );
                }
            }
        );
    }
}
