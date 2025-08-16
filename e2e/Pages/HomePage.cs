using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Optimized page object for the Home page with performance improvements
/// Eliminates navigation pauses and uses smart waiting strategies
/// </summary>
public class HomePage : OptimizedBasePage
{
    // Cached locators for better performance
    private ILocator HomeContainer =>
        GetCachedLocator("homeContainer", () => Page.Locator(Constants.HomeContainerSelector));
    private ILocator GameSettingsButton =>
        GetCachedLocator("gameSettings", () => Page.Locator(Constants.GameSettingsButtonSelector));
    private ILocator StartGameButton =>
        GetCachedLocator("startGame", () => Page.GetByText(Constants.CreateRoomButtonText));

    public HomePage(IPage page, string baseUrl = "https:/ /staging.flyingdarts.net") // get from consts
        : base(page, baseUrl) { }

    /// <summary>
    /// Wait for the home page to be fully loaded with optimized waiting
    /// </summary>
    public override async Task WaitForPageReadyAsync()
    {
        // Use optimized base page waiting
        await base.WaitForPageReadyAsync();

        // Wait for home container with smart waiting
        await WaitForElementSmartAsync(
            () => Page.Locator(Constants.HomeContainerSelector),
            "homeContainer",
            Constants.DefaultHomePageTimeout
        );
    }

    /// <summary>
    /// Navigate to the home page with optimized loading
    /// </summary>
    public async Task NavigateToHomeAsync()
    {
        await NavigateToAsync("");
        await WaitForPageReadyAsync();
    }

    /// <summary>
    /// Check if the user is on the settings page
    /// </summary>
    public bool IsOnSettingsPage()
    {
        return Page.Url.EndsWith(Constants.SettingsUrlSegment);
    }

    /// <summary>
    /// Navigate to game settings with optimized waiting
    /// </summary>
    public async Task NavigateToGameSettingsAsync()
    {
        if (IsOnSettingsPage())
        {
            // Already on settings page
            return;
        }

        var button = await WaitForElementSmartAsync(
            () => Page.Locator(Constants.GameSettingsButtonSelector),
            "gameSettings",
            Constants.OptimizedButtonTimeout
        );
        await button.ClickAsync();

        // Smart wait for navigation instead of fixed network idle wait
        await WaitForNavigationToSettingsAsync();
    }

    /// <summary>
    /// Start a new game with optimized waiting
    /// </summary>
    public async Task StartNewGameAsync()
    {
        var button = await WaitForElementSmartAsync(
            () => Page.GetByText(Constants.CreateRoomButtonText),
            "startGame",
            Constants.OptimizedButtonTimeout
        );
        await button.ClickAsync();

        // Smart wait for game to start instead of fixed network idle wait
        await WaitForGameStartAsync();
    }

    /// <summary>
    /// Wait for the game page to be ready after starting a new game with optimized waiting
    /// </summary>
    public async Task WaitForGamePageReadyAsync()
    {
        // Wait for URL change with shorter timeout
        await Page.WaitForURLAsync(
            Constants.GameUrlPattern,
            new() { Timeout = Constants.DefaultUrlNavigationTimeout }
        );

        // Brief wait for page stability instead of fixed delay
        await Task.Delay(Constants.StandardUiDelay);
    }

    /// <summary>
    /// Get game ID with optimized extraction
    /// </summary>
    public string GetGameId()
    {
        var url = Page.Url;

        // Verify we're on a game page
        if (!url.Contains(Constants.GameUrlSegment))
        {
            throw new InvalidOperationException(
                $"Cannot extract game ID from URL: {url}. Expected URL to contain '{Constants.GameUrlPattern.Replace("**", "")}'"
            );
        }

        var urlParts = url.Split("/");
        var gameId = urlParts.Last();

        // Verify the game ID is not empty and looks valid
        if (string.IsNullOrEmpty(gameId) || gameId == "game")
        {
            throw new InvalidOperationException(
                $"Invalid game ID extracted from URL: {url}. Game ID: '{gameId}'"
            );
        }

        return gameId;
    }

    /// <summary>
    /// Check if the home page is fully loaded with optimized checking
    /// </summary>
    public async Task<bool> IsHomePageLoadedAsync()
    {
        try
        {
            var containerTask = IsElementVisibleAsync(HomeContainer);
            var buttonTask = IsElementVisibleAsync(StartGameButton);

            await Task.WhenAll(containerTask, buttonTask);

            return await containerTask && await buttonTask;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get the current page title
    /// </summary>
    public async Task<string> GetPageTitleAsync()
    {
        return await Page.TitleAsync() ?? string.Empty;
    }

    /// <summary>
    /// Check if a specific button is visible with optimized checking
    /// </summary>
    /// <param name="buttonName">Name of the button to check</param>
    public async Task<bool> IsButtonVisibleAsync(string buttonName)
    {
        var button = Page.GetByRole(AriaRole.Button, new() { Name = buttonName });
        return await IsElementVisibleAsync(button);
    }

    #region Performance-Optimized Navigation Methods

    /// <summary>
    /// Smart wait for navigation to settings page
    /// </summary>
    private async Task WaitForNavigationToSettingsAsync()
    {
        // Wait for URL change with shorter timeout
        await Page.WaitForURLAsync(
            Constants.SettingsUrlPattern,
            new() { Timeout = Constants.DefaultSettingsTimeout }
        );

        // Brief wait for page stability
        await Task.Delay(Constants.StandardUiDelay);
    }

    /// <summary>
    /// Smart wait for game start
    /// </summary>
    private async Task WaitForGameStartAsync()
    {
        // Wait for URL change with shorter timeout
        await Page.WaitForURLAsync(
            Constants.GameUrlPattern,
            new() { Timeout = Constants.DefaultUrlNavigationTimeout }
        );

        // Brief wait for page stability
        await Task.Delay(Constants.StandardUiDelay);
    }

    #endregion
}
