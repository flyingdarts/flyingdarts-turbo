using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Optimized page object for the Game Settings page with performance improvements
/// Eliminates pauses and uses smart waiting strategies
/// </summary>
public class SettingsPage : OptimizedBasePage
{
    // Cached locators for better performance
    private ILocator SetMinusButton =>
        GetCachedLocator("setMinus", () => Page.Locator(SettingsXpathSelectors.SetMinusXpath));
    private ILocator SetPlusButton =>
        GetCachedLocator("setPlus", () => Page.Locator(SettingsXpathSelectors.SetPlusXpath));
    private ILocator SetCountDisplay =>
        GetCachedLocator("setCount", () => Page.Locator(SettingsXpathSelectors.SetCount));
    private ILocator LegMinusButton =>
        GetCachedLocator("legMinus", () => Page.Locator(SettingsXpathSelectors.LegMinusXpath));
    private ILocator LegPlusButton =>
        GetCachedLocator("legPlus", () => Page.Locator(SettingsXpathSelectors.LegPlusXpath));
    private ILocator LegCountDisplay =>
        GetCachedLocator("legCount", () => Page.Locator(SettingsXpathSelectors.LegCount));
    private ILocator SaveButton => GetCachedLocator("save", () => Page.GetByText("save"));

    public SettingsPage(IPage page, string baseUrl = "https://staging.flyingdarts.net")
        : base(page, baseUrl) { }

    /// <summary>
    /// Wait for the settings page to be fully loaded with optimized waiting
    /// </summary>
    public override async Task WaitForPageReadyAsync()
    {
        // Use optimized base page waiting
        await base.WaitForPageReadyAsync();

        // Wait for key elements in parallel for faster loading
        var waitTasks = new[]
        {
            WaitForElementSmartAsync(
                () => Page.Locator(SettingsXpathSelectors.SetCount),
                "setCount",
                Constants.DefaultSettingsTimeout
            ),
            WaitForElementSmartAsync(
                () => Page.Locator(SettingsXpathSelectors.LegCount),
                "legCount",
                Constants.DefaultSettingsTimeout
            ),
            WaitForElementSmartAsync(
                () => Page.GetByText(Constants.SaveButtonText),
                "save",
                Constants.DefaultSettingsTimeout
            ),
        };

        await Task.WhenAll(waitTasks);
    }

    #region Optimized Set Controls

    /// <summary>
    /// Decrease the number of sets with smart waiting
    /// </summary>
    public async Task DecreaseSetsAsync()
    {
        var button = await WaitForElementSmartAsync(
            () => Page.Locator(SettingsXpathSelectors.SetMinusXpath),
            "setMinus",
            Constants.OptimizedButtonTimeout
        );
        await button.ClickAsync();

        // Smart wait for UI update instead of fixed delay
        await WaitForElementValueChangeAsync(
            SetCountDisplay,
            Constants.OptimizedElementValueChangeTimeout
        );
    }

    /// <summary>
    /// Increase the number of sets with smart waiting
    /// </summary>
    public async Task IncreaseSetsAsync()
    {
        Console.WriteLine($"[TIMING] Starting IncreaseSetsAsync");
        var startTime = DateTime.UtcNow;

        var button = await WaitForElementSmartAsync(
            () => Page.Locator(SettingsXpathSelectors.SetPlusXpath),
            "setPlus",
            Constants.OptimizedButtonTimeout
        );

        var buttonWaitTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
        Console.WriteLine($"[TIMING] Button wait completed in {buttonWaitTime:F0}ms");

        await button.ClickAsync();
        Console.WriteLine($"[TIMING] Button clicked");

        // Smart wait for UI update instead of fixed delay
        var valueChangeStart = DateTime.UtcNow;
        await WaitForElementValueChangeAsync(
            SetCountDisplay,
            Constants.OptimizedElementValueChangeTimeout
        );

        var valueChangeTime = (DateTime.UtcNow - valueChangeStart).TotalMilliseconds;
        Console.WriteLine($"[TIMING] Value change wait completed in {valueChangeTime:F0}ms");

        var totalTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
        Console.WriteLine($"[TIMING] IncreaseSetsAsync total time: {totalTime:F0}ms");
    }

    /// <summary>
    /// Get the current number of sets with caching
    /// </summary>
    public async Task<int> GetSetCountAsync()
    {
        var display = await WaitForElementSmartAsync(
            () => Page.Locator(SettingsXpathSelectors.SetCount),
            "setCount",
            Constants.OptimizedButtonTimeout
        );
        var text = await display.TextContentAsync();

        // Extract number from text like "Sets: 3"
        var match = System.Text.RegularExpressions.Regex.Match(text ?? "", @"\d+");
        return match.Success ? int.Parse(match.Value) : 1;
    }

    #endregion

    #region Optimized Leg Controls

    /// <summary>
    /// Decrease the number of legs with smart waiting
    /// </summary>
    public async Task DecreaseLegsAsync()
    {
        var button = await WaitForElementSmartAsync(
            () => Page.Locator(SettingsXpathSelectors.LegMinusXpath),
            "legMinus",
            Constants.OptimizedButtonTimeout
        );
        await button.ClickAsync();

        // Smart wait for UI update instead of fixed delay
        await WaitForElementValueChangeAsync(
            LegCountDisplay,
            Constants.OptimizedElementValueChangeTimeout
        );
    }

    /// <summary>
    /// Increase the number of legs with smart waiting
    /// </summary>
    public async Task IncreaseLegsAsync()
    {
        var button = await WaitForElementSmartAsync(
            () => Page.Locator(SettingsXpathSelectors.LegPlusXpath),
            "legPlus",
            Constants.OptimizedButtonTimeout
        );
        await button.ClickAsync();

        // Smart wait for UI update instead of fixed delay
        await WaitForElementValueChangeAsync(
            LegCountDisplay,
            Constants.OptimizedElementValueChangeTimeout
        );
    }

    /// <summary>
    /// Get the current number of legs with caching
    /// </summary>
    public async Task<int> GetLegCountAsync()
    {
        var display = await WaitForElementSmartAsync(
            () => Page.Locator(SettingsXpathSelectors.LegCount),
            "legCount",
            Constants.OptimizedButtonTimeout
        );
        var text = await display.TextContentAsync();

        // Extract number from text like "Legs: 5"
        var match = System.Text.RegularExpressions.Regex.Match(text ?? "", @"\d+");
        return match.Success ? int.Parse(match.Value) : 1;
    }

    #endregion

    #region Optimized Save Operations

    /// <summary>
    /// Save the current settings with optimized waiting
    /// </summary>
    public async Task SaveSettingsAsync()
    {
        var button = await WaitForElementSmartAsync(
            () => Page.GetByText(Constants.SaveButtonText),
            "save",
            Constants.DefaultSaveTimeout
        );
        await button.ClickAsync();

        // Wait for save operation with shorter timeout and smart detection
        await WaitForSaveCompletionAsync();
    }

    #endregion

    #region High-Performance Combined Operations

    /// <summary>
    /// Set the game settings to specific values with parallel operations and smart waiting
    /// </summary>
    /// <param name="sets">Number of sets</param>
    /// <param name="legs">Number of legs</param>
    public async Task SetGameSettingsAsync(int sets, int legs)
    {
        // Validate input parameters
        if (sets < 1 || legs < 1)
            throw new ArgumentException("Sets and legs must be at least 1");

        // Get current values in parallel
        var (currentSets, currentLegs) = await GetCurrentSettingsAsync();

        // Calculate required changes for both directions
        var setsToAdd = Math.Max(0, sets - currentSets);
        var setsToRemove = Math.Max(0, currentSets - sets);
        var legsToAdd = Math.Max(0, legs - currentLegs);
        var legsToRemove = Math.Max(0, currentLegs - legs);

        // Execute changes sequentially to avoid race conditions
        if (setsToAdd > 0)
        {
            await IncreaseSetsByAmountAsync(setsToAdd);
        }

        if (setsToRemove > 0)
        {
            await DecreaseSetsByAmountAsync(setsToRemove);
        }

        if (legsToAdd > 0)
        {
            await IncreaseLegsByAmountAsync(legsToAdd);
        }

        if (legsToRemove > 0)
        {
            await DecreaseLegsByAmountAsync(legsToRemove);
        }

        // Verify settings were applied correctly
        await VerifySettingsAsync(sets, legs);
    }

    /// <summary>
    /// Reset settings to minimum values with parallel operations
    /// </summary>
    public async Task ResetToMinimumAsync()
    {
        // Get current values in parallel
        var (currentSets, currentLegs) = await GetCurrentSettingsAsync();

        // Calculate required changes
        var setsToRemove = Math.Max(0, currentSets - 1);
        var legsToRemove = Math.Max(0, currentLegs - 1);

        // Execute changes in parallel if possible
        var tasks = new List<Task>();

        if (setsToRemove > 0)
        {
            tasks.Add(DecreaseSetsByAmountAsync(setsToRemove));
        }

        if (legsToRemove > 0)
        {
            tasks.Add(DecreaseLegsByAmountAsync(legsToRemove));
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
        }
    }

    #endregion

    #region Performance-Optimized Utility Methods

    /// <summary>
    /// Get current settings in parallel for better performance
    /// </summary>
    private async Task<(int Sets, int Legs)> GetCurrentSettingsAsync()
    {
        var setsTask = GetSetCountAsync();
        var legsTask = GetLegCountAsync();

        await Task.WhenAll(setsTask, legsTask);

        return (await setsTask, await legsTask);
    }

    /// <summary>
    /// Increase sets by a specific amount with optimized waiting
    /// </summary>
    private async Task IncreaseSetsByAmountAsync(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            await IncreaseSetsAsync();

            // Brief wait only if more changes are needed
            if (i < amount - 1)
            {
                await Task.Delay(Constants.MinimalUiDelay); // Minimal delay for UI stability
            }
        }
    }

    /// <summary>
    /// Increase legs by a specific amount with optimized waiting
    /// </summary>
    private async Task IncreaseLegsByAmountAsync(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            await IncreaseLegsAsync();

            // Brief wait only if more changes are needed
            if (i < amount - 1)
            {
                await Task.Delay(Constants.MinimalUiDelay); // Minimal delay for UI stability
            }
        }
    }

    /// <summary>
    /// Decrease sets by a specific amount with optimized waiting
    /// </summary>
    private async Task DecreaseSetsByAmountAsync(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            await DecreaseSetsAsync();

            // Brief wait only if more changes are needed
            if (i < amount - 1)
            {
                await Task.Delay(Constants.MinimalUiDelay); // Minimal delay for UI stability
            }
        }
    }

    /// <summary>
    /// Decrease legs by a specific amount with optimized waiting
    /// </summary>
    private async Task DecreaseLegsByAmountAsync(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            await DecreaseLegsAsync();

            // Brief wait only if more changes are needed
            if (i < amount - 1)
            {
                await Task.Delay(Constants.MinimalUiDelay); // Minimal delay for UI stability
            }
        }
    }

    /// <summary>
    /// Smart wait for element value change instead of fixed delays
    /// </summary>
    private async Task WaitForElementValueChangeAsync(
        ILocator element,
        int timeoutMs = Constants.DefaultElementValueChangeTimeout
    )
    {
        var startTime = DateTime.UtcNow;
        var initialValue = await element.TextContentAsync();

        Console.WriteLine($"[TIMING] Waiting for value change from '{initialValue}'");

        // First, try immediate check (UI might update instantly)
        await Task.Delay(10); // Minimal 10ms wait for immediate updates
        var currentValue = await element.TextContentAsync();
        if (currentValue != initialValue)
        {
            var immediateTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Console.WriteLine($"[TIMING] Value changed immediately in {immediateTime:F0}ms");
            return; // Value changed immediately
        }

        // If not immediate, use faster polling
        var fastPollingInterval = 20; // Check every 20ms instead of 50ms

        while (DateTime.UtcNow - startTime < TimeSpan.FromMilliseconds(timeoutMs))
        {
            await Task.Delay(fastPollingInterval);

            var currentValue2 = await element.TextContentAsync();
            if (currentValue2 != initialValue)
            {
                var totalTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                Console.WriteLine($"[TIMING] Value changed after {totalTime:F0}ms");
                return; // Value changed, we're done
            }
        }

        // If timeout reached, check one more time
        var finalValue = await element.TextContentAsync();
        if (finalValue != initialValue)
        {
            var totalTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Console.WriteLine($"[TIMING] Value changed on final check after {totalTime:F0}ms");
            return;
        }

        var timeoutTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
        Console.WriteLine($"[TIMING] Value change timeout after {timeoutTime:F0}ms");
        // If timeout reached, continue anyway - the change might have happened
    }

    /// <summary>
    /// Smart wait for save completion instead of fixed network idle wait
    /// </summary>
    private async Task WaitForSaveCompletionAsync()
    {
        // Wait for any network activity to settle (shorter timeout)
        await Page.WaitForLoadStateAsync(
            LoadState.NetworkIdle,
            new() { Timeout = Constants.DefaultSaveTimeout }
        );

        // Additional brief wait for UI to update
        await Task.Delay(Constants.StandardUiDelay);
    }

    /// <summary>
    /// Verify settings were applied correctly
    /// </summary>
    private async Task VerifySettingsAsync(int expectedSets, int expectedLegs)
    {
        var (actualSets, actualLegs) = await GetCurrentSettingsAsync();

        if (actualSets != expectedSets || actualLegs != expectedLegs)
        {
            throw new InvalidOperationException(
                $"Settings verification failed. Expected: {expectedSets} sets, {expectedLegs} legs. "
                    + $"Actual: {actualSets} sets, {actualLegs} legs."
            );
        }
    }

    /// <summary>
    /// Check if the current settings match the expected values
    /// </summary>
    public async Task<bool> AreSettingsCorrectAsync(int expectedSets, int expectedLegs)
    {
        var (currentSets, currentLegs) = await GetCurrentSettingsAsync();
        return currentSets == expectedSets && currentLegs == expectedLegs;
    }

    #endregion
}
