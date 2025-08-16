using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Utilities;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Flyingdarts.E2E.Tests;

/// <summary>
/// Base class for E2E tests that require multiple browser contexts (multiple users).
/// Creates separate browser instances for each user to ensure complete isolation.
/// </summary>
public abstract class MultiBrowserBaseTest
{
    protected string BaseUrl { get; set; } = "https://staging.flyingdarts.net";
    protected AuthressHelper? AuthressHelper { get; private set; }

    // Multiple browser contexts for different users
    protected IBrowser BrowserUser1 { get; private set; } = null!;
    protected IBrowser BrowserUser2 { get; private set; } = null!;
    protected IBrowserContext User1Context { get; private set; } = null!;
    protected IBrowserContext User2Context { get; private set; } = null!;
    protected IPage User1Page { get; private set; } = null!;
    protected IPage User2Page { get; private set; } = null!;

    /// <summary>
    /// Setup method that runs before each test - creates multiple browser contexts
    /// </summary>
    protected virtual async Task SetupAsync()
    {
        // Initialize Authress helper if needed
        AuthressHelper = new AuthressHelper();

        // Create separate browser instances for complete isolation
        // We'll use Playwright's built-in browser management
        var playwright = await Playwright.CreateAsync();

        // Launch separate browsers for each user
        BrowserUser1 = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false, // Show browsers during testing
            }
        );

        BrowserUser2 = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false, // Show browsers during testing
            }
        );

        // Create two separate browser contexts for different users
        User1Context = await BrowserUser1.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
            }
        );

        User2Context = await BrowserUser2.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
            }
        );

        // Create pages for each context
        User1Page = await User1Context.NewPageAsync();
        User2Page = await User2Context.NewPageAsync();

        // Set default timeouts for both pages
        User1Page.SetDefaultTimeout(30000);
        User1Page.SetDefaultNavigationTimeout(30000);
        User2Page.SetDefaultTimeout(30000);
        User2Page.SetDefaultNavigationTimeout(30000);
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

        // Close browsers
        if (BrowserUser1 != null)
        {
            await BrowserUser1.CloseAsync();
        }

        if (BrowserUser2 != null)
        {
            await BrowserUser2.CloseAsync();
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
    protected async Task SetAuthTokenAsync(string token, IPage page)
    {
        if (AuthressHelper != null)
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
    }

    /// <summary>
    /// Set authentication token for User 2
    /// </summary>
    protected async Task SetUser2AuthTokenAsync()
    {
        if (AuthressHelper != null)
        {
            var token = await AuthressHelper.GetBearerTokenAsync();
            var cookie = new Cookie
            {
                Name = "custom-jwt-token-override",
                Value = token,
                Domain = ".flyingdarts.net",
                Path = "/",
            };

            await User2Context.AddCookiesAsync(new List<Cookie> { cookie });
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
