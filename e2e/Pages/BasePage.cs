using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Base class for all page objects in the POM architecture.
/// Provides common functionality and utilities for page interactions.
/// </summary>
public abstract class BasePage
{
    public readonly IPage Page;
    protected readonly string BaseUrl;

    protected BasePage(IPage page, string baseUrl = "https://staging.flyingdarts.net")
    {
        Page = page;
        BaseUrl = baseUrl;
    }

    /// <summary>
    /// Navigate to a specific URL relative to the base URL
    /// </summary>
    /// <param name="relativePath">Relative path to navigate to</param>
    /// <param name="waitForLoadState">Wait for specific load state</param>
    public virtual async Task NavigateToAsync(
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

        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(waitForLoadState);
    }

    /// <summary>
    /// Wait for an element to be visible
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    protected async Task WaitForElementVisibleAsync(ILocator locator, int timeout = 30000)
    {
        await locator.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = timeout }
        );
    }

    /// <summary>
    /// Wait for an element to be attached to DOM
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    protected async Task WaitForElementAttachedAsync(ILocator locator, int timeout = 30000)
    {
        await locator.WaitForAsync(
            new() { State = WaitForSelectorState.Attached, Timeout = timeout }
        );
    }

    /// <summary>
    /// Click an element with retry logic
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="maxRetries">Maximum number of retries</param>
    protected async Task ClickWithRetryAsync(ILocator locator, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await locator.ClickAsync();
                return;
            }
            catch (Exception) when (i < maxRetries - 1)
            {
                await Task.Delay(1000); // Wait 1 second before retry
            }
        }
        throw new Exception($"Failed to click element after {maxRetries} retries");
    }

    /// <summary>
    /// Fill input field with retry logic
    /// </summary>
    /// <param name="locator">Input locator</param>
    /// <param name="value">Value to fill</param>
    /// <param name="maxRetries">Maximum number of retries</param>
    protected async Task FillWithRetryAsync(ILocator locator, string value, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await locator.FillAsync(value);
                return;
            }
            catch (Exception) when (i < maxRetries - 1)
            {
                await Task.Delay(1000); // Wait 1 second before retry
            }
        }
        throw new Exception($"Failed to fill element after {maxRetries} retries");
    }

    /// <summary>
    /// Get text content safely with fallback
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="fallback">Fallback value if text is null</param>
    protected async Task<string> GetTextSafelyAsync(ILocator locator, string fallback = "")
    {
        var text = await locator.TextContentAsync();
        return text?.Trim() ?? fallback;
    }

    /// <summary>
    /// Check if element is visible
    /// </summary>
    /// <param name="locator">Element locator</param>
    protected async Task<bool> IsElementVisibleAsync(ILocator locator)
    {
        try
        {
            return await locator.IsVisibleAsync();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Wait for page to be ready (customizable per page)
    /// </summary>
    public virtual async Task WaitForPageReadyAsync()
    {
        // Default implementation - can be overridden by specific pages
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
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
}
