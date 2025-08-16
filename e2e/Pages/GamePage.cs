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
        GetCachedLocator("gameContainer", () => Page.Locator(Constants.GameContainerSelector));
    private ILocator PlayerName =>
        GetCachedLocator("playerName", () => Page.Locator(Constants.PlayerNameSelector));
    private ILocator UserDropdown =>
        GetCachedLocator("userDropdown", () => Page.Locator(Constants.UserDropdownSelector));
    private ILocator SettingsButton =>
        GetCachedLocator("settingsButton", () => Page.Locator(Constants.SettingsButtonSelector));

    public GamePage(IPage page, string baseUrl = Constants.DefaultBaseUrl)
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
    /// Get the current player score with optimized extraction
    /// </summary>
    public async Task<int> GetPlayerScoreAsync()
    {
        var element = await WaitForElementSmartAsync(
            () => Page.Locator(Constants.PlayerScoreSelector),
            "playerScore",
            Constants.OptimizedButtonTimeout
        );
        var text = await element.TextContentAsync();
        return int.Parse(text?.Trim() ?? throw new Exception("Player score not found"));
    }

    public async Task<string> GetRemainingScoreAsync()
    {
        var element = await WaitForElementSmartAsync(
            () => Page.Locator(Constants.PlayerScoreSelector),
            "playerScore",
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
        // Randomly select a number between 1-180 for realistic dart throwing
        var random = new Random();

        var dartScore = random.Next(1, 181);

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
    /// <param name="score">The score to throw (0-180)</param>
    public async Task ThrowDartWithScoreAsync(int score)
    {
        if (score < 0 || score > 180)
        {
            throw new ArgumentException("Score must be between 0 and 180", nameof(score));
        }

        // Click the individual digits for multi-digit numbers
        await ClickDigitsForNumberAsync(score);

        // Wait a moment for the input to register
        await Task.Delay(Constants.MinimalUiDelay);

        // Click OK to submit the score
        var okButton = Page.Locator(Constants.CalcButtonOkSelector);

        await okButton.ClickAsync();

        Console.WriteLine($"🎯 Threw dart with specific score: {score}");

        await Task.Delay(Constants.DartThrowDelay);
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
    /// Check if the checkout button is enabled
    /// </summary>
    public async Task<bool> CanCheckoutAsync()
    {
        try
        {
            var checkoutButton = Page.Locator(Constants.CalcButtonCheckSelector);

            // Wait for the button to be visible
            await checkoutButton.WaitForAsync(
                new()
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = Constants.DefaultVisibleWaitTimeout,
                }
            );

            // Check if it's enabled and not disabled
            var isEnabled = await checkoutButton.IsEnabledAsync();
            var classAttribute = await checkoutButton.GetAttributeAsync("class");
            var hasDisabledClass = classAttribute?.Contains("disabled") == true;

            var canCheckout = isEnabled && !hasDisabledClass;

            Console.WriteLine(
                $"🔍 Checkout button - Enabled: {isEnabled}, HasDisabledClass: {hasDisabledClass}, CanCheckout: {canCheckout}"
            );

            return canCheckout;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error checking checkout button: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Click the checkout button
    /// </summary>
    public async Task ClickCheckoutAsync()
    {
        var checkoutButton = Page.Locator(Constants.CalcButtonCheckSelector);

        // Wait for the button to be fully ready
        await checkoutButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        // Try to scroll the button into view and ensure it's clickable
        await checkoutButton.ScrollIntoViewIfNeededAsync();

        // Wait a bit for any animations or overlays to settle
        await Task.Delay(Constants.StandardUiDelay);

        try
        {
            // First try normal click
            await checkoutButton.ClickAsync();
            Console.WriteLine("✅ Checkout button clicked successfully");
        }
        catch (Exception ex)
            when (ex.Message.Contains("intercepts pointer events")
                || ex.Message.Contains("element is not stable")
            )
        {
            Console.WriteLine("⚠️ Normal click failed, trying force click...");

            // If normal click fails, try force click
            await checkoutButton.ClickAsync(new() { Force = true });
            Console.WriteLine("✅ Checkout button clicked with force");
        }
        finally
        {
            await Task.Delay(Constants.LegCompletionDelay);
        }
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
