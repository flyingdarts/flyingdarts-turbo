using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Page object for the Darts Game page
/// </summary>
public class GamePage : BasePage
{
    // Game information and stats (placeholders)
    private readonly ILocator _gameTitle;
    private readonly ILocator _bestOfDisplay;
    private readonly ILocator _playerWonSets;
    private readonly ILocator _playerWonLegs;
    private readonly ILocator _playerScore;
    private readonly ILocator _playerName;

    // User controls
    private readonly ILocator _userDropdown;
    private readonly ILocator _settingsButton;

    // Input display
    private readonly ILocator _inputDisplay;

    // Control buttons
    private readonly ILocator _clearButton;
    private readonly ILocator _checkButton;
    private readonly ILocator _noScoreButton;
    private readonly ILocator _okButton;

    // Numerical/Score keypad buttons
    private readonly ILocator _button26;
    private readonly ILocator _button1;
    private readonly ILocator _button2;
    private readonly ILocator _button3;
    private readonly ILocator _button41;
    private readonly ILocator _button45;
    private readonly ILocator _button4;
    private readonly ILocator _button5;
    private readonly ILocator _button6;
    private readonly ILocator _button60;
    private readonly ILocator _button85;
    private readonly ILocator _button7;
    private readonly ILocator _button8;
    private readonly ILocator _button9;
    private readonly ILocator _button100;
    private readonly ILocator _button0;

    public GamePage(IPage page, string baseUrl = "https://staging.flyingdarts.net")
        : base(page, baseUrl)
    {
        // Initialize game information locators (placeholders)
        _gameTitle = Page.GetByText("Flyingdarts");
        _bestOfDisplay = Page.GetByText("Best Of 3/3");

        _playerWonSets = Page.Locator(
            "/html/body/app-root/div/div/div/app-game/app-game-ui/div/div[1]/div/app-game-stats/app-game-stats-ui/div/table/tbody/tr[1]/td[2]"
        );
        _playerWonLegs = Page.Locator(
            "/html/body/app-root/div/div/div/app-game/app-game-ui/div/div[1]/div/app-game-stats/app-game-stats-ui/div/table/tbody/tr[1]/td[3]"
        );
        _playerScore = Page.Locator(
            "/html/body/app-root/div/div/div/app-game/app-game-ui/div/div[1]/div/app-game-stats/app-game-stats-ui/div/table/tbody/tr[1]/td[4]"
        );

        // Initialize input display (placeholder)
        _inputDisplay = Page.Locator("#calcInputFieldHidden");

        // Initialize control button locators
        _clearButton = Page.GetByText("CLEAR");
        _checkButton = Page.GetByText("CHECK");
        _noScoreButton = Page.GetByText("NO SCORE");
        _okButton = Page.GetByText("OK");

        // Initialize numerical/score keypad locators
        _button26 = Page.GetByText("26");
        _button1 = Page.GetByText("1");
        _button2 = Page.GetByText("2");
        _button3 = Page.GetByText("3");
        _button41 = Page.GetByText("41");
        _button45 = Page.GetByText("45");
        _button4 = Page.GetByText("4");
        _button5 = Page.GetByText("5");
        _button6 = Page.GetByText("6");
        _button60 = Page.GetByText("60");
        _button85 = Page.GetByText("85");
        _button7 = Page.GetByText("7");
        _button8 = Page.GetByText("8");
        _button9 = Page.GetByText("9");
        _button100 = Page.GetByText("100");
        _button0 = Page.GetByText("0");
    }

    /// <summary>
    /// Wait for the game page to be fully loaded
    /// </summary>
    public override async Task WaitForPageReadyAsync()
    {
        await base.WaitForPageReadyAsync();

        // Wait for key game elements to be visible
        await WaitForElementVisibleAsync(_gameTitle);
        await WaitForElementVisibleAsync(_bestOfDisplay);
        await WaitForElementVisibleAsync(_clearButton);
        await WaitForElementVisibleAsync(_checkButton);
    }

    #region Game Information

    /// <summary>
    /// Get the current game title
    /// </summary>
    public async Task<string> GetGameTitleAsync()
    {
        return await GetTextSafelyAsync(_gameTitle);
    }

    /// <summary>
    /// Get the "Best Of" display text
    /// </summary>
    public async Task<string> GetBestOfDisplayAsync()
    {
        return await GetTextSafelyAsync(_bestOfDisplay);
    }

    /// <summary>
    /// Get the current sets score
    /// </summary>
    public async Task<int> GetCurrentSetsAsync()
    {
        var text = await GetTextSafelyAsync(_playerWonSets);
        return int.TryParse(text, out var result) ? result : 0;
    }

    /// <summary>
    /// Get the current legs score
    /// </summary>
    public async Task<int> GetCurrentLegsAsync()
    {
        var text = await GetTextSafelyAsync(_playerWonLegs);
        return int.TryParse(text, out var result) ? result : 0;
    }

    /// <summary>
    /// Get the current game score
    /// </summary>
    public async Task<int> GetCurrentScoreAsync()
    {
        var text = await GetTextSafelyAsync(_playerScore);
        return int.TryParse(text, out var result) ? result : 501;
    }

    #endregion

    #region Input Display

    /// <summary>
    /// Get the current input display value
    /// </summary>
    public async Task<string> GetInputDisplayValueAsync()
    {
        return await GetTextSafelyAsync(_inputDisplay);
    }

    /// <summary>
    /// Check if input display is empty
    /// </summary>
    public async Task<bool> IsInputDisplayEmptyAsync()
    {
        var value = await GetInputDisplayValueAsync();
        return string.IsNullOrEmpty(value);
    }

    #endregion

    #region Control Buttons

    /// <summary>
    /// Clear the current input
    /// </summary>
    public async Task ClearInputAsync()
    {
        await WaitForElementVisibleAsync(_clearButton);
        await ClickWithRetryAsync(_clearButton);
    }

    /// <summary>
    /// Check the current input
    /// </summary>
    public async Task CheckInputAsync()
    {
        await WaitForElementVisibleAsync(_checkButton);
        await ClickWithRetryAsync(_checkButton);
    }

    /// <summary>
    /// Mark no score for the current throw
    /// </summary>
    public async Task MarkNoScoreAsync()
    {
        await WaitForElementVisibleAsync(_noScoreButton);
        await ClickWithRetryAsync(_noScoreButton);
    }

    /// <summary>
    /// Confirm the current input
    /// </summary>
    public async Task ConfirmInputAsync()
    {
        await WaitForElementVisibleAsync(_okButton);
        await ClickWithRetryAsync(_okButton);
    }

    #endregion

    #region Numerical Keypad

    /// <summary>
    /// Press a number button
    /// </summary>
    /// <param name="number">Number to press (0-9)</param>
    public async Task PressNumberAsync(int number)
    {
        var button = number switch
        {
            0 => _button0,
            1 => _button1,
            2 => _button2,
            3 => _button3,
            4 => _button4,
            5 => _button5,
            6 => _button6,
            7 => _button7,
            8 => _button8,
            9 => _button9,
            _ => throw new ArgumentException($"Invalid number: {number}. Must be 0-9."),
        };

        await WaitForElementVisibleAsync(button);
        await ClickWithRetryAsync(button);
    }

    #endregion

    #region Score Buttons

    /// <summary>
    /// Press a specific score button
    /// </summary>
    /// <param name="score">Score value to press</param>
    public async Task PressScoreAsync(int score)
    {
        var button = score switch
        {
            26 => _button26,
            41 => _button41,
            45 => _button45,
            60 => _button60,
            85 => _button85,
            100 => _button100,
            _ => throw new ArgumentException(
                $"Invalid score: {score}. Valid scores are: 26, 41, 45, 60, 85, 100."
            ),
        };

        await WaitForElementVisibleAsync(button);
        await ClickWithRetryAsync(button);
    }

    #endregion

    #region Combined Operations

    /// <summary>
    /// Enter a score using the keypad
    /// </summary>
    /// <param name="score">Score to enter</param>
    public async Task EnterScoreAsync(int score)
    {
        // Clear any existing input first
        await ClearInputAsync();

        // Convert score to string and press each digit
        var scoreString = score.ToString();
        foreach (var digit in scoreString)
        {
            await PressNumberAsync(int.Parse(digit.ToString()));
        }
    }

    /// <summary>
    /// Enter a score and confirm it
    /// </summary>
    /// <param name="score">Score to enter</param>
    public async Task EnterAndConfirmScoreAsync(int score)
    {
        await EnterScoreAsync(score);
        await ConfirmInputAsync();
    }

    /// <summary>
    /// Enter a score and check it
    /// </summary>
    /// <param name="score">Score to enter</param>
    public async Task EnterAndCheckScoreAsync(int score)
    {
        await EnterScoreAsync(score);
        await CheckInputAsync();
    }

    #endregion

    #region User Controls

    /// <summary>
    /// Navigate to settings
    /// </summary>
    public async Task NavigateToSettingsAsync()
    {
        await WaitForElementVisibleAsync(_settingsButton);
        await ClickWithRetryAsync(_settingsButton);

        // Wait for navigation to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Get the current user name
    /// </summary>
    public async Task<string> GetUserNameAsync()
    {
        return await GetTextSafelyAsync(_playerName);
    }

    /// <summary>
    /// Open user dropdown menu
    /// </summary>
    public async Task OpenUserDropdownAsync()
    {
        await WaitForElementVisibleAsync(_userDropdown);
        await ClickWithRetryAsync(_userDropdown);
    }

    #endregion

    #region Game State Validation

    /// <summary>
    /// Check if the game page is fully loaded
    /// </summary>
    public async Task<bool> IsGamePageLoadedAsync()
    {
        try
        {
            return await IsElementVisibleAsync(_gameTitle)
                && await IsElementVisibleAsync(_bestOfDisplay)
                && await IsElementVisibleAsync(_clearButton)
                && await IsElementVisibleAsync(_checkButton);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if the game is in progress
    /// </summary>
    public async Task<bool> IsGameInProgressAsync()
    {
        var currentScore = await GetCurrentScoreAsync();
        return currentScore > 0 && currentScore <= 501;
    }

    #endregion
}
