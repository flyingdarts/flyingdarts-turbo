using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Optimized base page with lazy loading and element caching for improved performance
/// </summary>
public abstract class OptimizedBasePage : BasePage
{
    private readonly Dictionary<string, ILocator> _cachedLocators = new();
    private readonly Dictionary<string, object> _cachedElements = new();
    private readonly object _cacheLock = new();

    protected OptimizedBasePage(IPage page, string baseUrl = "https://staging.flyingdarts.net")
        : base(page, baseUrl) { }

    /// <summary>
    /// Get a cached locator with lazy initialization
    /// </summary>
    protected ILocator GetCachedLocator(string key, Func<ILocator> locatorFactory)
    {
        lock (_cacheLock)
        {
            if (!_cachedLocators.ContainsKey(key))
            {
                _cachedLocators[key] = locatorFactory();
            }
            return _cachedLocators[key];
        }
    }

    /// <summary>
    /// Get a cached element value
    /// </summary>
    protected T? GetCachedElement<T>(string key)
    {
        lock (_cacheLock)
        {
            if (_cachedElements.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }
    }

    /// <summary>
    /// Set a cached element value
    /// </summary>
    protected void SetCachedElement<T>(string key, T value)
    {
        lock (_cacheLock)
        {
            _cachedElements[key] = value!;
        }
    }

    /// <summary>
    /// Clear element cache
    /// </summary>
    protected void ClearElementCache()
    {
        lock (_cacheLock)
        {
            _cachedElements.Clear();
        }
    }

    /// <summary>
    /// Clear locator cache
    /// </summary>
    protected void ClearLocatorCache()
    {
        lock (_cacheLock)
        {
            _cachedLocators.Clear();
        }
    }

    /// <summary>
    /// Wait for element with smart retry logic
    /// </summary>
    protected async Task<ILocator> WaitForElementSmartAsync(
        Func<ILocator> locatorFactory,
        string cacheKey,
        int timeout = Constants.DefaultElementTimeout,
        int retryInterval = Constants.DefaultRetryInterval
    )
    {
        var startTime = DateTime.UtcNow;
        var locator = GetCachedLocator(cacheKey, locatorFactory);

        Console.WriteLine($"[TIMING] Starting WaitForElementSmartAsync for {cacheKey}");

        // First, try a quick check without waiting
        var quickCheckStart = DateTime.UtcNow;
        try
        {
            if (await locator.IsVisibleAsync())
            {
                var quickCheckTime = (DateTime.UtcNow - quickCheckStart).TotalMilliseconds;
                Console.WriteLine(
                    $"[TIMING] {cacheKey} immediately visible in {quickCheckTime:F0}ms"
                );
                return locator; // Element is immediately visible, return fast
            }
        }
        catch
        {
            // Element not ready yet, continue with retry logic
        }

        var quickCheckTimeTotal = (DateTime.UtcNow - quickCheckStart).TotalMilliseconds;
        Console.WriteLine(
            $"[TIMING] {cacheKey} not immediately visible, quick check took {quickCheckTimeTotal:F0}ms"
        );

        // If not immediately visible, use the retry logic with shorter timeout
        var retryTimeout = Math.Min(timeout, 1000); // Cap retry timeout at 1 second

        while (DateTime.UtcNow - startTime < TimeSpan.FromMilliseconds(retryTimeout))
        {
            try
            {
                if (await locator.IsVisibleAsync())
                {
                    var retryTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    Console.WriteLine(
                        $"[TIMING] {cacheKey} became visible after retries in {retryTime:F0}ms"
                    );
                    return locator;
                }
            }
            catch
            {
                // Element not ready yet, continue waiting
            }

            await Task.Delay(retryInterval);
        }

        // If we still can't see it after retries, try one more time with full timeout
        try
        {
            if (await locator.IsVisibleAsync())
            {
                var finalTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                Console.WriteLine(
                    $"[TIMING] {cacheKey} became visible on final attempt in {finalTime:F0}ms"
                );
                return locator;
            }
        }
        catch
        {
            // Final attempt failed
        }

        var finalTotalTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
        Console.WriteLine(
            $"[TIMING] {cacheKey} failed to become visible after {finalTotalTime:F0}ms"
        );
        throw new TimeoutException($"Element {cacheKey} not visible within {timeout}ms");
    }

    /// <summary>
    /// Click element with smart waiting and retry
    /// </summary>
    protected async Task ClickElementSmartAsync(
        Func<ILocator> locatorFactory,
        string cacheKey,
        int timeout = Constants.DefaultElementTimeout
    )
    {
        var locator = await WaitForElementSmartAsync(locatorFactory, cacheKey, timeout);

        try
        {
            await locator.ClickAsync();
        }
        catch (Exception ex) when (ex.Message.Contains("element is not attached"))
        {
            // Element detached, clear cache and retry once
            ClearLocatorCache();
            locator = await WaitForElementSmartAsync(locatorFactory, cacheKey, timeout);
            await locator.ClickAsync();
        }
    }

    /// <summary>
    /// Fill input with smart waiting and retry
    /// </summary>
    protected async Task FillInputSmartAsync(
        Func<ILocator> locatorFactory,
        string cacheKey,
        string value,
        int timeout = Constants.DefaultElementTimeout
    )
    {
        var locator = await WaitForElementSmartAsync(locatorFactory, cacheKey, timeout);

        try
        {
            await locator.FillAsync(value);
        }
        catch (Exception ex) when (ex.Message.Contains("element is not attached"))
        {
            // Element detached, clear cache and retry once
            ClearLocatorCache();
            locator = await WaitForElementSmartAsync(locatorFactory, cacheKey, timeout);
            await locator.FillAsync(value);
        }
    }

    /// <summary>
    /// Get text content with smart waiting
    /// </summary>
    protected async Task<string> GetTextSmartAsync(
        Func<ILocator> locatorFactory,
        string cacheKey,
        int timeout = Constants.DefaultElementTimeout
    )
    {
        var locator = await WaitForElementSmartAsync(locatorFactory, cacheKey, timeout);
        var text = await locator.TextContentAsync();
        return text?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Wait for page ready with performance optimization
    /// </summary>
    public override async Task WaitForPageReadyAsync()
    {
        // Wait for network idle with shorter timeout for better performance
        await Page.WaitForLoadStateAsync(
            LoadState.NetworkIdle,
            new() { Timeout = Constants.DefaultPageLoadTimeout }
        );

        // Additional wait for DOM stability
        await Task.Delay(Constants.ExtendedUiDelay);
    }

    /// <summary>
    /// Navigate with performance optimization
    /// </summary>
    public override async Task NavigateToAsync(
        string relativePath = "",
        LoadState waitForLoadState = LoadState.NetworkIdle
    )
    {
        var url = string.IsNullOrEmpty(relativePath)
            ? BaseUrl
            : $"{BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";

        if (Page.Url == url)
        {
            return;
        }

        // Clear caches before navigation
        ClearElementCache();
        ClearLocatorCache();

        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(
            waitForLoadState,
            new() { Timeout = Constants.DefaultPageLoadTimeout }
        );
    }

    /// <summary>
    /// Take performance-optimized screenshot
    /// </summary>
    protected async Task TakeOptimizedScreenshotAsync(string name)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = $"{name}_{timestamp}.png";

        // Use optimized screenshot options
        await Page.ScreenshotAsync(
            new()
            {
                Path = filename,
                FullPage = false, // Only capture viewport for faster processing
                Type = ScreenshotType.Png,
            }
        );
    }

    /// <summary>
    /// Get page performance metrics
    /// </summary>
    protected async Task<Dictionary<string, object>> GetPerformanceMetricsAsync()
    {
        try
        {
            var metrics = await Page.EvaluateAsync<Dictionary<string, object>>(
                @"
                () => {
                    const perf = performance;
                    return {
                        navigationStart: perf.timing.navigationStart,
                        loadEventEnd: perf.timing.loadEventEnd,
                        domContentLoaded: perf.timing.domContentLoadedEventEnd,
                        firstPaint: perf.getEntriesByType('paint').find(e => e.name === 'first-paint')?.startTime,
                        firstContentfulPaint: perf.getEntriesByType('paint').find(e => e.name === 'first-contentful-paint')?.startTime
                    };
                }
            "
            );
            return metrics;
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }
}
