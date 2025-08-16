using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Utilities;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Flyingdarts.E2E.Tests;

/// <summary>
/// Base class for all E2E tests in the POM architecture.
/// Provides common setup, teardown, and utility methods.
/// </summary>
public abstract class BaseTest : PageTest
{
    protected string BaseUrl { get; set; } = "https://staging.flyingdarts.net";
    protected AuthressHelper? AuthressHelper { get; private set; }

    /// <summary>
    /// Setup method that runs before each test
    /// </summary>
    protected virtual async Task SetupAsync()
    {
        // Initialize Authress helper if needed
        AuthressHelper = new AuthressHelper();

        // Set default timeout
        Page.SetDefaultTimeout(30000);

        // Set default navigation timeout
        Page.SetDefaultNavigationTimeout(30000);
    }

    /// <summary>
    /// Teardown method that runs after each test
    /// </summary>
    protected virtual async Task TeardownAsync()
    {
        // Take screenshot on test failure (if implemented)
        // Clean up any test data if needed
    }

    /// <summary>
    /// Navigate to a specific path relative to base URL
    /// </summary>
    /// <param name="relativePath">Relative path to navigate to</param>
    protected async Task NavigateToAsync(string relativePath)
    {
        var url = $"{BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Set authentication token for the test
    /// </summary>
    protected async Task SetAuthTokenAsync()
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

            await Page.Context.AddCookiesAsync(new List<Cookie> { cookie });
        }
    }

    /// <summary>
    /// Wait for a specific condition with timeout
    /// </summary>
    /// <param name="condition">Condition to wait for</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    protected async Task WaitForConditionAsync(Func<Task<bool>> condition, int timeout = 30000)
    {
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeout))
        {
            if (await condition())
                return;

            await Task.Delay(100);
        }

        throw new TimeoutException($"Condition not met within {timeout}ms");
    }

    /// <summary>
    /// Take a screenshot for debugging
    /// </summary>
    /// <param name="name">Screenshot name</param>
    protected async Task TakeScreenshotAsync(string name)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = $"{name}_{timestamp}.png";
        await Page.ScreenshotAsync(new() { Path = filename });
    }

    /// <summary>
    /// Get a page object instance
    /// </summary>
    /// <typeparam name="T">Type of page object</typeparam>
    /// <returns>Page object instance</returns>
    protected T GetPage<T>()
        where T : BasePage
    {
        return (T)Activator.CreateInstance(typeof(T), Page, BaseUrl)!;
    }

    /// <summary>
    /// Assert that an element is visible
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="message">Assertion message</param>
    protected async Task AssertElementVisibleAsync(
        ILocator locator,
        string message = "Element should be visible"
    )
    {
        await Expect(locator).ToBeVisibleAsync();
    }

    /// <summary>
    /// Assert that an element contains specific text
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="expectedText">Expected text</param>
    /// <param name="message">Assertion message</param>
    protected async Task AssertElementContainsTextAsync(
        ILocator locator,
        string expectedText,
        string message = "Element should contain expected text"
    )
    {
        await Expect(locator).ToContainTextAsync(expectedText);
    }
}
