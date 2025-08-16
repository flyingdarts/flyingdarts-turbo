using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Utilities;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Flyingdarts.E2E.Tests;

/// <summary>
/// Optimized base class for E2E tests that require multiple browser contexts (multiple users).
/// Uses browser pooling and token caching for improved performance.
/// </summary>
public abstract class MultiBrowserBaseTest : IDisposable
{
    protected string BaseUrl { get; set; } = "https://staging.flyingdarts.net";
    protected AuthressHelper? AuthressHelperUser1 { get; private set; }
    protected AuthressHelper? AuthressHelperUser2 { get; private set; }
    protected PerformanceTestRunner? TestRunner { get; private set; }

    // Multiple browser contexts for different users
    protected IBrowser BrowserUser1 { get; private set; } = null!;
    protected IBrowser BrowserUser2 { get; private set; } = null!;
    protected IBrowserContext User1Context { get; private set; } = null!;
    protected IBrowserContext User2Context { get; private set; } = null!;
    protected IPage User1Page { get; private set; } = null!;
    protected IPage User2Page { get; private set; } = null!;

    // Performance tracking
    private readonly DateTime _testStartTime = DateTime.UtcNow;
    private bool _disposed = false;

    /// <summary>
    /// Setup method that runs before each test - creates multiple browser contexts
    /// </summary>
    protected virtual async Task SetupAsync()
    {
        // Initialize performance test runner
        TestRunner = new PerformanceTestRunner(maxConcurrentTests: 2);
        await TestRunner.InitializeAsync();

        // Get browsers from pool for better performance
        BrowserUser1 = await TestRunner.GetBrowserAsync();
        BrowserUser2 = await TestRunner.GetBrowserAsync();

        // Initialize Authress helper with caching
        AuthressHelperUser1 = new AuthressHelper(
            Environment.GetEnvironmentVariable("AUTHRESS_SERVICE_CLIENT_ACCESS_KEY")
                ?? throw new InvalidOperationException(
                    "Missing env AUTHRESS_SERVICE_CLIENT_ACCESS_KEY"
                )
        );

        AuthressHelperUser2 = new AuthressHelper(
            Environment.GetEnvironmentVariable("AUTHRESS_SERVICE_CLIENT_ACCESS_KEY_OPPONENT")
                ?? throw new InvalidOperationException(
                    "Missing env AUTHRESS_SERVICE_CLIENT_ACCESS_KEY_OPPONENT"
                )
        );

        // Create two separate browser contexts for different users
        User1Context = await BrowserUser1.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                // Performance optimizations
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept-Encoding"] = "gzip, deflate, br",
                },
            }
        );

        User2Context = await BrowserUser2.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
                // Performance optimizations
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept-Encoding"] = "gzip, deflate, br",
                },
            }
        );

        // Create pages for each context
        User1Page = await User1Context.NewPageAsync();
        User2Page = await User2Context.NewPageAsync();

        // Set optimized timeouts for both pages
        User1Page.SetDefaultTimeout(15000); // Reduced from 30s to 15s
        User1Page.SetDefaultNavigationTimeout(15000);
        User2Page.SetDefaultTimeout(15000);
        User2Page.SetDefaultNavigationTimeout(15000);

        // Enable performance monitoring
        await User1Page.Context.RouteAsync(
            "**/*",
            async route =>
            {
                // Block unnecessary resources for faster loading
                if (ShouldBlockResource(route.Request.Url))
                {
                    await route.AbortAsync();
                    return;
                }
                await route.ContinueAsync();
            }
        );

        await User2Page.Context.RouteAsync(
            "**/*",
            async route =>
            {
                // Block unnecessary resources for faster loading
                if (ShouldBlockResource(route.Request.Url))
                {
                    await route.AbortAsync();
                    return;
                }
                await route.ContinueAsync();
            }
        );

        Console.WriteLine(
            $"🚀 Test setup completed in {(DateTime.UtcNow - _testStartTime).TotalMilliseconds:F0}ms"
        );
    }

    /// <summary>
    /// Determine if a resource should be blocked for performance
    /// </summary>
    private static bool ShouldBlockResource(string url)
    {
        var lowerUrl = url.ToLowerInvariant();

        // Block unnecessary resources
        return lowerUrl.Contains("analytics")
            || lowerUrl.Contains("tracking")
            || lowerUrl.Contains("ads")
            || lowerUrl.Contains("doubleclick")
            || lowerUrl.Contains("facebook.com")
            || lowerUrl.Contains("google-analytics")
            || lowerUrl.Contains("googletagmanager")
            || lowerUrl.Contains("hotjar")
            || lowerUrl.Contains("mixpanel")
            || lowerUrl.Contains("optimizely")
            || lowerUrl.Contains("segment")
            || lowerUrl.Contains("amplitude")
            || lowerUrl.Contains("intercom")
            || lowerUrl.Contains("zendesk")
            || lowerUrl.Contains("livechat")
            || lowerUrl.Contains("chatbot")
            || lowerUrl.Contains("widget")
            || lowerUrl.Contains("pixel")
            || lowerUrl.Contains("beacon")
            || lowerUrl.Contains("telemetry");
    }

    /// <summary>
    /// Teardown method that runs after each test - cleans up browser contexts
    /// </summary>
    protected virtual async Task TeardownAsync()
    {
        // Close pages and contexts
        if (User1Page != null)
        {
            await User1Page.CloseAsync();
        }

        if (User2Page != null)
        {
            await User2Page.CloseAsync();
        }

        if (User1Context != null)
        {
            await User1Context.CloseAsync();
        }

        if (User2Context != null)
        {
            await User2Context.CloseAsync();
        }

        // Return browsers to pool instead of closing them
        if (TestRunner != null)
        {
            if (BrowserUser1 != null)
            {
                TestRunner.ReturnBrowser(BrowserUser1);
            }
            if (BrowserUser2 != null)
            {
                TestRunner.ReturnBrowser(BrowserUser2);
            }
        }

        // Print performance stats
        if (TestRunner != null)
        {
            TestRunner.PrintPerformanceReport();
        }
    }

    /// <summary>
    /// Dispose method for cleanup
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            TestRunner?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Navigate User 1 to a specific path relative to base URL
    /// </summary>
    /// <param name="relativePath">Relative path to navigate to</param>
    protected async Task NavigateUser1ToAsync(string relativePath)
    {
        var url = $"{BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        await User1Page.GotoAsync(url);
        await User1Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Navigate User 2 to a specific path relative to base URL
    /// </summary>
    /// <param name="relativePath">Relative path to navigate to</param>
    protected async Task NavigateUser2ToAsync(string relativePath)
    {
        var url = $"{BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        await User2Page.GotoAsync(url);
        await User2Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Set authentication token for User 1
    /// </summary>
    protected async Task SetUser1AuthTokenAsync()
    {
        if (AuthressHelperUser1 != null)
        {
            var token = await AuthressHelperUser1.GetBearerTokenAsync();
            var cookie = new Cookie
            {
                Name = "custom-jwt-token-override",
                Value = token,
                Domain = ".flyingdarts.net",
                Path = "/",
            };

            await User1Context.AddCookiesAsync(new List<Cookie> { cookie });
            Console.WriteLine("🔐 User 1 authentication token set successfully");
        }
    }

    /// <summary>
    /// Set authentication token for a specific page (legacy method)
    /// </summary>
    protected async Task SetAuthTokenAsync(string token, IPage page)
    {
        var cookie = new Cookie
        {
            Name = "custom-jwt-token-override",
            Value = token,
            Domain = ".flyingdarts.net",
            Path = "/",
        };

        await page.Context.AddCookiesAsync(new List<Cookie> { cookie });
    }

    /// <summary>
    /// Set authentication token for User 2
    /// </summary>
    protected async Task SetUser2AuthTokenAsync()
    {
        if (AuthressHelperUser2 != null)
        {
            var token = await AuthressHelperUser2.GetBearerTokenAsync();
            var cookie = new Cookie
            {
                Name = "custom-jwt-token-override",
                Value = token,
                Domain = ".flyingdarts.net",
                Path = "/",
            };

            await User2Context.AddCookiesAsync(new List<Cookie> { cookie });
            Console.WriteLine("🔐 User 2 authentication token set successfully");
        }
    }

    /// <summary>
    /// Wait for a specific condition with timeout on User 1 page
    /// </summary>
    /// <param name="condition">Condition to wait for</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    protected async Task WaitForUser1ConditionAsync(Func<Task<bool>> condition, int timeout = 30000)
    {
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeout))
        {
            if (await condition())
                return;

            await Task.Delay(100);
        }

        throw new TimeoutException($"Condition not met within {timeout}ms for User 1");
    }

    /// <summary>
    /// Wait for a specific condition with timeout on User 2 page
    /// </summary>
    /// <param name="condition">Condition to wait for</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    protected async Task WaitForUser2ConditionAsync(Func<Task<bool>> condition, int timeout = 30000)
    {
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeout))
        {
            if (await condition())
                return;

            await Task.Delay(100);
        }

        throw new TimeoutException($"Condition not met within {timeout}ms for User 2");
    }

    /// <summary>
    /// Take a screenshot for debugging from User 1's perspective
    /// </summary>
    /// <param name="name">Screenshot name</param>
    protected async Task TakeUser1ScreenshotAsync(string name)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = $"User1_{name}_{timestamp}.png";
        await User1Page.ScreenshotAsync(new() { Path = filename });
    }

    /// <summary>
    /// Take a screenshot for debugging from User 2's perspective
    /// </summary>
    /// <param name="name">Screenshot name</param>
    protected async Task TakeUser2ScreenshotAsync(string name)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = $"User2_{name}_{timestamp}.png";
        await User2Page.ScreenshotAsync(new() { Path = filename });
    }

    /// <summary>
    /// Take screenshots from both users' perspectives
    /// </summary>
    /// <param name="name">Screenshot name</param>
    protected async Task TakeBothUsersScreenshotsAsync(string name)
    {
        await TakeUser1ScreenshotAsync(name);
        await TakeUser2ScreenshotAsync(name);
    }

    /// <summary>
    /// Get a page object instance for User 1
    /// </summary>
    /// <typeparam name="T">Type of page object</typeparam>
    /// <returns>Page object instance for User 1</returns>
    protected T GetUser1Page<T>()
        where T : BasePage
    {
        return (T)Activator.CreateInstance(typeof(T), User1Page, BaseUrl)!;
    }

    /// <summary>
    /// Get a page object instance for User 2
    /// </summary>
    /// <typeparam name="T">Type of page object</typeparam>
    /// <returns>Page object instance for User 2</returns>
    protected T GetUser2Page<T>()
        where T : BasePage
    {
        return (T)Activator.CreateInstance(typeof(T), User2Page, BaseUrl)!;
    }

    /// <summary>
    /// Assert that an element is visible on User 1's page
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="message">Assertion message</param>
    protected async Task AssertUser1ElementVisibleAsync(
        ILocator locator,
        string message = "Element should be visible for User 1"
    )
    {
        // Use Playwright's built-in assertion
        await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }

    /// <summary>
    /// Assert that an element is visible on User 2's page
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="message">Assertion message</param>
    protected async Task AssertUser2ElementVisibleAsync(
        ILocator locator,
        string message = "Element should be visible for User 2"
    )
    {
        // Use Playwright's built-in assertion
        await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }

    /// <summary>
    /// Assert that an element contains specific text on User 1's page
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="expectedText">Expected text</param>
    /// <param name="message">Assertion message</param>
    protected async Task AssertUser1ElementContainsTextAsync(
        ILocator locator,
        string expectedText,
        string message = "Element should contain expected text for User 1"
    )
    {
        // Use Playwright's built-in assertion
        var text = await locator.TextContentAsync();
        if (text == null || !text.Contains(expectedText))
        {
            throw new Exception($"{message}. Expected: '{expectedText}', Actual: '{text}'");
        }
    }

    /// <summary>
    /// Assert that an element contains specific text on User 2's page
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="expectedText">Expected text</param>
    /// <param name="message">Assertion message</param>
    protected async Task AssertUser2ElementContainsTextAsync(
        ILocator locator,
        string expectedText,
        string message = "Element should contain expected text for User 2"
    )
    {
        // Use Playwright's built-in assertion
        var text = await locator.TextContentAsync();
        if (text == null || !text.Contains(expectedText))
        {
            throw new Exception($"{message}. Expected: '{expectedText}', Actual: '{text}'");
        }
    }

    /// <summary>
    /// Wait for both users to be on the same page/URL
    /// </summary>
    /// <param name="expectedUrl">Expected URL (can be partial)</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    protected async Task WaitForBothUsersOnSamePageAsync(string expectedUrl, int timeout = 30000)
    {
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeout))
        {
            var user1Url = User1Page.Url;
            var user2Url = User2Page.Url;

            if (user1Url.Contains(expectedUrl) && user2Url.Contains(expectedUrl))
                return;

            await Task.Delay(100);
        }

        throw new TimeoutException(
            $"Both users not on expected page '{expectedUrl}' within {timeout}ms. User1: {User1Page.Url}, User2: {User2Page.Url}"
        );
    }

    /// <summary>
    /// Verify that users have different authentication tokens
    /// </summary>
    protected async Task VerifyUniqueUserTokensAsync()
    {
        var user1Cookies = await User1Context.CookiesAsync();
        var user2Cookies = await User2Context.CookiesAsync();

        var user1Token = user1Cookies
            .FirstOrDefault(c => c.Name == "custom-jwt-token-override")
            ?.Value;
        var user2Token = user2Cookies
            .FirstOrDefault(c => c.Name == "custom-jwt-token-override")
            ?.Value;

        if (string.IsNullOrEmpty(user1Token) || string.IsNullOrEmpty(user2Token))
        {
            throw new InvalidOperationException("One or both users missing authentication tokens");
        }

        if (user1Token == user2Token)
        {
            throw new InvalidOperationException(
                "Both users have identical authentication tokens - this should not happen!"
            );
        }

        // Log the tokens for debugging (first 20 chars only for security)
        Console.WriteLine(
            $"User1 Token: {user1Token.Substring(0, Math.Min(20, user1Token.Length))}..."
        );
        Console.WriteLine(
            $"User2 Token: {user2Token.Substring(0, Math.Min(20, user2Token.Length))}..."
        );
        Console.WriteLine("✅ Users have unique authentication tokens");
    }
}
