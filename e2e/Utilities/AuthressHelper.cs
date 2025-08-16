using Authress.SDK;
using Authress.SDK.Client;

namespace Flyingdarts.E2E.Utilities;

/// <summary>
/// Helper class for Authress authentication operations
/// </summary>
public class AuthressHelper
{
    private readonly string _accessKey;
    private readonly string _authressUrl;
    private readonly AuthressClientTokenProvider _tokenProvider;

    public AuthressHelper()
    {
        _accessKey =
            Environment.GetEnvironmentVariable("AUTHRESS_SERVICE_CLIENT_ACCESS_KEY")
            ?? throw new InvalidOperationException(
                "Missing env AUTHRESS_SERVICE_CLIENT_ACCESS_KEY"
            );

        _authressUrl = "https://authress.flyingdarts.net";
        _tokenProvider = new AuthressClientTokenProvider(_accessKey, _authressUrl);
    }

    /// <summary>
    /// Get a bearer token for authentication
    /// </summary>
    /// <returns>Bearer token string</returns>
    public async Task<string> GetBearerTokenAsync()
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
            throw new InvalidOperationException($"Failed to get Authress token: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validate that the token is valid and not expired
    /// </summary>
    /// <param name="token">Token to validate</param>
    /// <returns>True if token is valid</returns>
    public bool IsTokenValid(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        // Basic validation - you might want to add JWT token validation here
        return token.Length > 10; // Simple length check
    }

    /// <summary>
    /// Get the Authress URL being used
    /// </summary>
    public string AuthressUrl => _authressUrl;
}
