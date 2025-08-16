using System.Collections.Concurrent;

namespace Flyingdarts.E2E.Utilities;

/// <summary>
/// Caches authentication tokens to reduce API calls and improve test performance
/// Automatically handles token expiration and refresh
/// </summary>
public class TokenCache
{
    private readonly ConcurrentDictionary<string, CachedToken> _tokenCache = new();
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(55); // 5 min buffer before actual expiry

    /// <summary>
    /// Get or create a cached token
    /// </summary>
    public async Task<string> GetOrCreateTokenAsync(string key, Func<Task<string>> tokenFactory)
    {
        if (_tokenCache.TryGetValue(key, out var cachedToken) && !IsTokenExpired(cachedToken))
        {
            return cachedToken.Token;
        }

        // Token expired or doesn't exist, refresh it
        return await RefreshTokenAsync(key, tokenFactory);
    }

    /// <summary>
    /// Refresh a token using the provided factory
    /// </summary>
    private async Task<string> RefreshTokenAsync(string key, Func<Task<string>> tokenFactory)
    {
        await _refreshLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (
                _tokenCache.TryGetValue(key, out var existingToken)
                && !IsTokenExpired(existingToken)
            )
            {
                return existingToken.Token;
            }

            // Create new token
            var newToken = await tokenFactory();
            var cachedToken = new CachedToken
            {
                Token = newToken,
                ExpiresAt = DateTime.UtcNow.Add(_defaultExpiry),
            };

            _tokenCache.AddOrUpdate(key, cachedToken, (_, _) => cachedToken);
            return newToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    /// <summary>
    /// Check if a token is expired
    /// </summary>
    private static bool IsTokenExpired(CachedToken cachedToken)
    {
        return DateTime.UtcNow >= cachedToken.ExpiresAt;
    }

    /// <summary>
    /// Clear expired tokens from cache
    /// </summary>
    public void CleanupExpiredTokens()
    {
        var expiredKeys = _tokenCache
            .Where(kvp => IsTokenExpired(kvp.Value))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _tokenCache.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Clear all cached tokens
    /// </summary>
    public void Clear()
    {
        _tokenCache.Clear();
    }

    /// <summary>
    /// Get cache statistics
    /// </summary>
    public (int Total, int Valid, int Expired) GetCacheStats()
    {
        var now = DateTime.UtcNow;
        var total = _tokenCache.Count;
        var valid = _tokenCache.Count(kvp => !IsTokenExpired(kvp.Value));
        var expired = total - valid;

        return (total, valid, expired);
    }

    /// <summary>
    /// Represents a cached token with expiration
    /// </summary>
    private class CachedToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
