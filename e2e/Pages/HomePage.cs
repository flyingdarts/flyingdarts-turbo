using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Page object for the Home page
/// </summary>
public class HomePage : BasePage
{
    // Locators for home page elements
    private readonly ILocator _homeContainer;
    private readonly ILocator _gameSettingsButton;
    private readonly ILocator _startGameButton;

    public HomePage(IPage page, string baseUrl = "https://staging.flyingdarts.net")
        : base(page, baseUrl)
    {
        // Initialize locators
        _homeContainer = Page.Locator("#homeContainer");
        _gameSettingsButton = Page.Locator("//*[@id='homeContainer']/div/app-home/div/i");
        _startGameButton = Page.GetByText("create room");
    }

    /// <summary>
    /// Wait for the home page to be fully loaded
    /// </summary>
    public override async Task WaitForPageReadyAsync()
    {
        await base.WaitForPageReadyAsync();

        // Wait for home container to be visible
        await WaitForElementVisibleAsync(_homeContainer);
    }

    /// <summary>
    /// Navigate to the home page
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
        return Page.Url.EndsWith("/settings");
    }

    /// <summary>
    /// Navigate to game settings
    /// </summary>
    public async Task NavigateToGameSettingsAsync()
    {
        if (IsOnSettingsPage())
        {
            // Already on settings page
            return;
        }

        await WaitForElementVisibleAsync(_gameSettingsButton);
        await ClickWithRetryAsync(_gameSettingsButton);

        // Wait for navigation to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public async Task StartNewGameAsync()
    {
        await WaitForElementVisibleAsync(_startGameButton);
        await ClickWithRetryAsync(_startGameButton);

        // Wait for game to start
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Wait for the game page to be ready after starting a new game
    /// </summary>
    public async Task WaitForGamePageReadyAsync()
    {
        // Wait for navigation to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait for the URL to change to a game page
        await Page.WaitForURLAsync("**/game/**");

        // Additional wait to ensure the page is fully loaded
        await Task.Delay(500);
    }

    public string GetGameId()
    {
        var url = Page.Url;

        // Verify we're on a game page
        if (!url.Contains("/game/"))
        {
            throw new InvalidOperationException(
                $"Cannot extract game ID from URL: {url}. Expected URL to contain '/game/'"
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
    /// Check if the home page is fully loaded
    /// </summary>
    public async Task<bool> IsHomePageLoadedAsync()
    {
        try
        {
            return await IsElementVisibleAsync(_homeContainer)
                && await IsElementVisibleAsync(_startGameButton);
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
    /// Check if a specific button is visible
    /// </summary>
    /// <param name="buttonName">Name of the button to check</param>
    public async Task<bool> IsButtonVisibleAsync(string buttonName)
    {
        var button = Page.GetByRole(AriaRole.Button, new() { Name = buttonName });
        return await IsElementVisibleAsync(button);
    }
}
