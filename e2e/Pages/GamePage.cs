using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Optimized page object for the Game page with performance improvements
/// Eliminates waiting pauses and uses smart waiting strategies
/// </summary>
public class GamePage : OptimizedBasePage
{
    // Cached locators for better performance
    private ILocator GameContainer =>
        GetCachedLocator("gameContainer", () => Page.Locator("#gameContainer"));
    private ILocator PlayerName =>
        GetCachedLocator("playerName", () => Page.Locator("#playerName"));
    private ILocator UserDropdown =>
        GetCachedLocator("userDropdown", () => Page.Locator("#userDropdown"));
    private ILocator SettingsButton =>
        GetCachedLocator("settingsButton", () => Page.Locator("#settingsButton"));

    public GamePage(IPage page, string baseUrl = "https://staging.flyingdarts.net")
        : base(page, baseUrl) { }

    /// <summary>
    /// Wait for the game page to be fully loaded with optimized waiting
    /// </summary>
    public override async Task WaitForPageReadyAsync()
    {
        // Use optimized base page waiting
        await base.WaitForPageReadyAsync();

        // Wait for key elements in parallel for faster loading
        var waitTasks = new[]
        {
            WaitForElementSmartAsync(
                () => Page.Locator(Constants.GameContainerSelector),
                "gameContainer",
                Constants.DefaultGamePageTimeout
            ),
            WaitForElementSmartAsync(
                () => Page.Locator(Constants.PlayerNameSelector),
                "playerName",
                Constants.DefaultGamePageTimeout
            ),
        };

        await Task.WhenAll(waitTasks);
    }

    /// <summary>
    /// Check if the game page is loaded with optimized checking
    /// </summary>
    public async Task<bool> IsGamePageLoadedAsync()
    {
        try
        {
            var containerTask = IsElementVisibleAsync(GameContainer);
            var playerNameTask = IsElementVisibleAsync(PlayerName);

            await Task.WhenAll(containerTask, playerNameTask);

            return await containerTask && await playerNameTask;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get the current player name with optimized extraction
    /// </summary>
    public async Task<string> GetPlayerNameAsync()
    {
        var element = await WaitForElementSmartAsync(
            () => Page.Locator(Constants.PlayerNameSelector),
            "playerName",
            Constants.OptimizedButtonTimeout
        );
        var text = await element.TextContentAsync();
        return text?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Check if the user dropdown is visible
    /// </summary>
    public async Task<bool> IsUserDropdownVisibleAsync()
    {
        try
        {
            return await IsElementVisibleAsync(UserDropdown);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if the settings button is visible
    /// </summary>
    public async Task<bool> IsSettingsButtonVisibleAsync()
    {
        try
        {
            return await IsElementVisibleAsync(SettingsButton);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Wait for game page to be ready with optimized waiting
    /// </summary>
    public async Task WaitForGamePageReadyAsync()
    {
        // Wait for URL to contain game pattern
        await Page.WaitForURLAsync(
            Constants.GameUrlPattern,
            new() { Timeout = Constants.DefaultUrlNavigationTimeout }
        );

        // Wait for page to be ready
        await WaitForPageReadyAsync();

        // Brief wait for page stability
        await Task.Delay(Constants.StandardUiDelay);
    }

    /// <summary>
    /// Throw a dart by clicking a random number button (1-20) and then OK
    /// </summary>
    public async Task ThrowDartAsync()
    {
        // Randomly select a number between 1-20 for realistic dart throwing
        var random = new Random();
        var dartScore = random.Next(1, 21);

        // Click the individual digits for multi-digit numbers
        await ClickDigitsForNumberAsync(dartScore);

        // Wait a moment for the input to register
        await Task.Delay(Constants.MinimalUiDelay);

        // Click OK to submit the score
        var okButton = Page.Locator(Constants.CalcButtonOkSelector);
        await okButton.ClickAsync();

        Console.WriteLine($"🎯 Threw dart with score: {dartScore}");
    }

    /// <summary>
    /// Throw a dart with a specific score
    /// </summary>
    /// <param name="score">The score to throw (0-20)</param>
    public async Task ThrowDartWithScoreAsync(int score)
    {
        if (score < 0 || score > 20)
        {
            throw new ArgumentException("Score must be between 0 and 20", nameof(score));
        }

        // Click the individual digits for multi-digit numbers
        await ClickDigitsForNumberAsync(score);

        // Wait a moment for the input to register
        await Task.Delay(Constants.MinimalUiDelay);

        // Click OK to submit the score
        var okButton = Page.Locator(Constants.CalcButtonOkSelector);
        await okButton.ClickAsync();

        Console.WriteLine($"🎯 Threw dart with specific score: {score}");
    }

    /// <summary>
    /// Use a quick score button (26, 41, 45, 60, 85, 100)
    /// </summary>
    /// <param name="quickScore">The quick score to use</param>
    public async Task UseQuickScoreAsync(int quickScore)
    {
        if (!Constants.QuickScoreButtonSelectors.ContainsKey(quickScore))
        {
            throw new ArgumentException(
                $"Quick score {quickScore} is not valid. Valid scores: {string.Join(", ", Constants.QuickScoreButtonSelectors.Keys)}",
                nameof(quickScore)
            );
        }

        // Click the quick score button
        var quickScoreButton = Page.Locator(Constants.QuickScoreButtonSelectors[quickScore]);
        await quickScoreButton.ClickAsync();

        // Wait a moment for the input to register
        await Task.Delay(Constants.MinimalUiDelay);

        // Click OK to submit the score
        var okButton = Page.Locator(Constants.CalcButtonOkSelector);
        await okButton.ClickAsync();

        Console.WriteLine($"🎯 Used quick score: {quickScore}");
    }

    /// <summary>
    /// Clear the current score input
    /// </summary>
    public async Task ClearScoreAsync()
    {
        var clearButton = Page.Locator(Constants.CalcButtonClearSelector);
        await clearButton.ClickAsync();
        Console.WriteLine("🧹 Cleared score input");
    }

    /// <summary>
    /// Submit no score for the current turn
    /// </summary>
    public async Task SubmitNoScoreAsync()
    {
        var noScoreButton = Page.Locator(Constants.CalcButtonNoScoreSelector);
        await noScoreButton.ClickAsync();
        Console.WriteLine("❌ Submitted no score");
    }

    /// <summary>
    /// Input any score by clicking individual digits (useful for scores like 15, 23, etc.)
    /// </summary>
    /// <param name="score">The score to input (0-180)</param>
    public async Task InputScoreAsync(int score)
    {
        if (score < 0 || score > 180)
        {
            throw new ArgumentException("Score must be between 0 and 180", nameof(score));
        }

        // Click the individual digits for the score
        await ClickDigitsForNumberAsync(score);

        Console.WriteLine($"⌨️ Input score: {score}");
    }

    /// <summary>
    /// Input a score and submit it (complete flow)
    /// </summary>
    /// <param name="score">The score to input and submit (0-180)</param>
    public async Task InputAndSubmitScoreAsync(int score)
    {
        if (score < 0 || score > 180)
        {
            throw new ArgumentException("Score must be between 0 and 180", nameof(score));
        }

        // Input the score
        await InputScoreAsync(score);

        // Wait a moment for the input to register
        await Task.Delay(Constants.MinimalUiDelay);

        // Click OK to submit the score
        var okButton = Page.Locator(Constants.CalcButtonOkSelector);
        await okButton.ClickAsync();

        Console.WriteLine($"🎯 Input and submitted score: {score}");
    }

    /// <summary>
    /// Helper method to click individual digit buttons for multi-digit numbers
    /// </summary>
    /// <param name="number">The number to input (0-180)</param>
    private async Task ClickDigitsForNumberAsync(int number)
    {
        var digits = number.ToString().ToCharArray();

        foreach (var digit in digits)
        {
            var digitValue = int.Parse(digit.ToString());

            if (!Constants.NumberButtonSelectors.ContainsKey(digitValue))
            {
                throw new InvalidOperationException(
                    $"Digit {digitValue} is not available in the number button selectors"
                );
            }

            var digitButton = Page.Locator(Constants.NumberButtonSelectors[digitValue]);
            await digitButton.ClickAsync();

            // Small delay between digit clicks for stability
            await Task.Delay(Constants.MinimalUiDelay);
        }
    }
}
