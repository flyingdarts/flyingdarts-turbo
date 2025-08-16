using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Page object for the Game Settings page
/// </summary>
public class SettingsPage : BasePage
{
    // Locators using the existing XPath selectors
    private readonly ILocator _setMinusButton;
    private readonly ILocator _setPlusButton;
    private readonly ILocator _setCountDisplay;
    private readonly ILocator _legMinusButton;
    private readonly ILocator _legPlusButton;
    private readonly ILocator _legCountDisplay;
    private readonly ILocator _saveButton;

    public SettingsPage(IPage page, string baseUrl = "https://staging.flyingdarts.net")
        : base(page, baseUrl)
    {
        // Initialize locators using the existing XPath selectors
        _setMinusButton = Page.Locator(SettingsXpathSelectors.SetMinusXpath);
        _setPlusButton = Page.Locator(SettingsXpathSelectors.SetPlusXpath);
        _setCountDisplay = Page.Locator(SettingsXpathSelectors.SetCount);
        _legMinusButton = Page.Locator(SettingsXpathSelectors.LegMinusXpath);
        _legPlusButton = Page.Locator(SettingsXpathSelectors.LegPlusXpath);
        _legCountDisplay = Page.Locator(SettingsXpathSelectors.LegCount);
        _saveButton = Page.GetByText("save");
    }

    /// <summary>
    /// Wait for the settings page to be fully loaded
    /// </summary>
    public override async Task WaitForPageReadyAsync()
    {
        await base.WaitForPageReadyAsync();

        // Wait for key elements to be visible
        await WaitForElementVisibleAsync(_setCountDisplay);
        await WaitForElementVisibleAsync(_legCountDisplay);
        await WaitForElementVisibleAsync(_saveButton);
    }

    #region Set Controls

    /// <summary>
    /// Decrease the number of sets
    /// </summary>
    public async Task DecreaseSetsAsync()
    {
        await WaitForElementVisibleAsync(_setMinusButton);
        await ClickWithRetryAsync(_setMinusButton);
        await WaitForSetCountUpdateAsync();
    }

    /// <summary>
    /// Increase the number of sets
    /// </summary>
    public async Task IncreaseSetsAsync()
    {
        await WaitForElementVisibleAsync(_setPlusButton);
        await ClickWithRetryAsync(_setPlusButton);
        await WaitForSetCountUpdateAsync();
    }

    /// <summary>
    /// Get the current number of sets
    /// </summary>
    public async Task<int> GetSetCountAsync()
    {
        await WaitForElementVisibleAsync(_setCountDisplay);
        var text = await GetTextSafelyAsync(_setCountDisplay);

        // Extract number from text like "Sets: 3"
        var match = System.Text.RegularExpressions.Regex.Match(text, @"\d+");
        return match.Success ? int.Parse(match.Value) : 1;
    }

    #endregion

    #region Leg Controls

    /// <summary>
    /// Decrease the number of legs
    /// </summary>
    public async Task DecreaseLegsAsync()
    {
        await WaitForElementVisibleAsync(_legMinusButton);
        await ClickWithRetryAsync(_legMinusButton);
        await WaitForLegCountUpdateAsync();
    }

    /// <summary>
    /// Increase the number of legs
    /// </summary>
    public async Task IncreaseLegsAsync()
    {
        await WaitForElementVisibleAsync(_legPlusButton);
        await ClickWithRetryAsync(_legPlusButton);
        await WaitForLegCountUpdateAsync();
    }

    /// <summary>
    /// Get the current number of legs
    /// </summary>
    public async Task<int> GetLegCountAsync()
    {
        await WaitForElementVisibleAsync(_legCountDisplay);
        var text = await GetTextSafelyAsync(_legCountDisplay);

        // Extract number from text like "Legs: 5"
        var match = System.Text.RegularExpressions.Regex.Match(text, @"\d+");
        return match.Success ? int.Parse(match.Value) : 1;
    }

    #endregion

    #region Save Operations

    /// <summary>
    /// Save the current settings
    /// </summary>
    public async Task SaveSettingsAsync()
    {
        await WaitForElementVisibleAsync(_saveButton);
        await ClickWithRetryAsync(_saveButton);

        // Wait for save operation to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    #endregion

    #region Combined Operations

    /// <summary>
    /// Set the game settings to specific values
    /// </summary>
    /// <param name="sets">Number of sets</param>
    /// <param name="legs">Number of legs</param>
    public async Task SetGameSettingsAsync(int sets, int legs)
    {
        // Validate input parameters
        if (sets < 1 || legs < 1)
            throw new ArgumentException("Sets and legs must be at least 1");

        // Reset to minimum values first
        await ResetToMinimumAsync();

        // Set the desired number of sets
        var currentSets = await GetSetCountAsync();
        while (currentSets < sets)
        {
            await IncreaseSetsAsync();
            currentSets = await GetSetCountAsync();
        }

        // Set the desired number of legs
        var currentLegs = await GetLegCountAsync();
        while (currentLegs < legs)
        {
            await IncreaseLegsAsync();
            currentLegs = await GetLegCountAsync();
        }
    }

    /// <summary>
    /// Reset settings to minimum values
    /// </summary>
    public async Task ResetToMinimumAsync()
    {
        // Reset sets to minimum (1)
        var currentSets = await GetSetCountAsync();
        while (currentSets > 1)
        {
            await DecreaseSetsAsync();
            currentSets = await GetSetCountAsync();
        }

        // Reset legs to minimum (1)
        var currentLegs = await GetLegCountAsync();
        while (currentLegs > 1)
        {
            await DecreaseLegsAsync();
            currentLegs = await GetLegCountAsync();
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Wait for set count to update after a change
    /// </summary>
    private async Task WaitForSetCountUpdateAsync()
    {
        await Task.Delay(500); // Brief delay for UI update
    }

    /// <summary>
    /// Wait for leg count to update after a change
    /// </summary>
    private async Task WaitForLegCountUpdateAsync()
    {
        await Task.Delay(500); // Brief delay for UI update
    }

    /// <summary>
    /// Check if the current settings match the expected values
    /// </summary>
    /// <param name="expectedSets">Expected number of sets</param>
    /// <param name="expectedLegs">Expected number of legs</param>
    public async Task<bool> AreSettingsCorrectAsync(int expectedSets, int expectedLegs)
    {
        var currentSets = await GetSetCountAsync();
        var currentLegs = await GetLegCountAsync();

        return currentSets == expectedSets && currentLegs == expectedLegs;
    }

    #endregion
}

/// <summary>
/// XPath selectors for the settings page - using the existing selectors
/// </summary>
public static class SettingsXpathSelectors
{
    public const string SetMinusXpath = "//*[@id='homeContainer']/div/app-home/div[2]/button[1]";
    public const string SetPlusXpath = "//*[@id='homeContainer']/div/app-home/div[2]/button[2]";
    public const string SetCount = "//*[@id='homeContainer']/div/app-home/div[2]/p[2]";

    public const string LegMinusXpath = "//*[@id='homeContainer']/div/app-home/div[3]/button[1]";
    public const string LegPlusXpath = "//*[@id='homeContainer']/div/app-home/div[3]/button[2]";
    public const string LegCount = "//*[@id='homeContainer']/div/app-home/div[3]/p[2]";
}
