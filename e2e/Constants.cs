namespace Flyingdarts.E2E;

/// <summary>
/// Centralized constants for the E2E test suite
/// Consolidates all magic numbers, timeouts, and delays for better maintainability
/// </summary>
public static class Constants
{
    #region Timeouts

    /// <summary>
    /// Default timeout for element operations (15 seconds)
    /// </summary>
    public const int DefaultElementTimeout = 15000;

    /// <summary>
    /// Default timeout for waiting for element visibility (5 seconds)
    /// </summary>
    public const int DefaultVisibleWaitTimeout = 5000;

    /// <summary>
    /// Default timeout for page load operations (10 seconds)
    /// </summary>
    public const int DefaultPageLoadTimeout = 10000;

    /// <summary>
    /// Default timeout for URL navigation operations (10 seconds)
    /// </summary>
    public const int DefaultUrlNavigationTimeout = 30000;

    /// <summary>
    /// Default timeout for settings page operations (5 seconds)
    /// </summary>
    public const int DefaultSettingsTimeout = 5000;

    /// <summary>
    /// Default timeout for game page operations (5 seconds)
    /// </summary>
    public const int DefaultGamePageTimeout = 5000;

    /// <summary>
    /// Default timeout for home page operations (5 seconds)
    /// </summary>
    public const int DefaultHomePageTimeout = 5000;

    /// <summary>
    /// Default timeout for element value change detection (1 second)
    /// </summary>
    public const int DefaultElementValueChangeTimeout = 1000;

    /// <summary>
    /// Optimized timeout for element value change detection (500ms) - for faster UI updates
    /// </summary>
    public const int OptimizedElementValueChangeTimeout = 500;

    /// <summary>
    /// Default timeout for save operations (3 seconds)
    /// </summary>
    public const int DefaultSaveTimeout = 3000;

    /// <summary>
    /// Default timeout for button operations (3 seconds)
    /// </summary>
    public const int DefaultButtonTimeout = 3000;

    /// <summary>
    /// Optimized timeout for button operations (1 second) - for elements that should be immediately ready
    /// </summary>
    public const int OptimizedButtonTimeout = 1000;

    #endregion

    #region Delays

    /// <summary>
    /// Minimal delay for UI stability between operations (50ms)
    /// Used when changing settings values sequentially
    /// </summary>
    public const int MinimalUiDelay = 50;

    /// <summary>
    /// Standard delay for UI updates (100ms)
    /// Used after navigation and page state changes
    /// </summary>
    public const int StandardUiDelay = 100;

    /// <summary>
    /// Extended delay for complex UI operations (200ms)
    /// Used for network idle and load state operations
    /// </summary>
    public const int ExtendedUiDelay = 200;

    /// <summary>
    /// Delay between individual dart throws (500ms)
    /// Used to ensure proper backend processing between dart inputs
    /// </summary>
    public const int DartThrowDelay = 500;

    /// <summary>
    /// Extended delay for game completion (1000ms)
    /// Used after final dart throws to ensure game state is fully updated
    /// </summary>
    public const int LegCompletionDelay = 1000;

    /// <summary>
    /// Polling interval for element value change detection (50ms)
    /// How often to check if an element value has changed
    /// </summary>
    public const int ElementValueChangePollingInterval = 50;

    #endregion

    #region Retry Intervals

    /// <summary>
    /// Default retry interval for smart waiting operations (100ms)
    /// How often to retry element operations
    /// </summary>
    public const int DefaultRetryInterval = 100;

    #endregion

    #region URL Patterns

    /// <summary>
    /// Pattern for game URLs
    /// </summary>
    public const string GameUrlPattern = "**/game/**";
    public const string GameUrlSegment = "/game/";

    /// <summary>
    /// Pattern for settings URLs
    /// </summary>
    public const string SettingsUrlPattern = "**/settings";
    public const string SettingsUrlSegment = "/settings";

    /// <summary>
    /// Default base URL for the application under test
    /// </summary>
    public const string DefaultBaseUrl = "https://staging.flyingdarts.net";

    #endregion

    #region Element Selectors

    /// <summary>
    /// Home container selector
    /// </summary>
    public const string HomeContainerSelector = "#homeContainer";

    /// <summary>
    /// Game container selector
    /// </summary>
    public const string GameContainerSelector = "#gameContainer";

    /// <summary>
    /// Player name selector
    /// </summary>
    public const string PlayerNameSelector = "#playerName";

    /// <summary>
    /// Player score selector
    /// </summary>
    public const string PlayerScoreSelector = "#playerScore";

    /// <summary>
    /// Save button text
    /// </summary>
    public const string SaveButtonText = "save";

    /// <summary>
    /// Create room button text
    /// </summary>
    public const string CreateRoomButtonText = "create room";

    /// <summary>
    /// Game settings button selector
    /// </summary>
    public const string GameSettingsButtonSelector = "#gameSettings";

    /// <summary>
    /// Start new game button selector
    /// </summary>
    public const string StartNewGameButtonSelector = "#startNewGameButton";

    /// <summary>
    /// Settings button selector
    /// </summary>
    public const string SettingsButtonSelector = "#settingsButton";

    /// <summary>
    /// User dropdown selector
    /// </summary>
    public const string UserDropdownSelector = "#userDropdown";

    /// <summary>
    /// Winner overlay selector
    /// </summary>
    public const string WinnerPopupOverlaySelector = "#winnerPopupOverlay";

    /// <summary>
    /// Winner overlay primary button selector
    /// </summary>
    public const string WinnerPopupPrimaryButtonSelector = "#winnerPopupOverlay .btn.btn-primary";

    /// <summary>
    /// Winner name selector
    /// </summary>
    public const string WinnerNameSelector = ".winner-name";

    /// <summary>
    /// Winner text selector
    /// </summary>
    public const string WinnerTextSelector = ".winner-text";

    #endregion

    #region Game Keyboard Selectors

    /// <summary>
    /// Clear button selector
    /// </summary>
    public const string CalcButtonClearSelector = "#calcButtonClear";

    /// <summary>
    /// Check button selector
    /// </summary>
    public const string CalcButtonCheckSelector = "#calcButtonCheck";

    /// <summary>
    /// OK button selector
    /// </summary>
    public const string CalcButtonOkSelector = "#calcButtonOK";

    /// <summary>
    /// No Score button selector
    /// </summary>
    public const string CalcButtonNoScoreSelector = "#calcButtonNOSCORE";

    /// <summary>
    /// Input field selector
    /// </summary>
    public const string CalcInputFieldSelector = "#calcInputField";

    /// <summary>
    /// Hidden input field selector
    /// </summary>
    public const string CalcInputFieldHiddenSelector = "#calcInputFieldHidden";

    /// <summary>
    /// Number button selectors (0-9)
    /// </summary>
    public static readonly Dictionary<int, string> NumberButtonSelectors = new()
    {
        { 0, "#calcButton0" },
        { 1, "#calcButton1" },
        { 2, "#calcButton2" },
        { 3, "#calcButton3" },
        { 4, "#calcButton4" },
        { 5, "#calcButton5" },
        { 6, "#calcButton6" },
        { 7, "#calcButton7" },
        { 8, "#calcButton8" },
        { 9, "#calcButton9" },
    };

    /// <summary>
    /// Quick score button selectors
    /// </summary>
    public static readonly Dictionary<int, string> QuickScoreButtonSelectors = new()
    {
        { 26, "#calcButton26" },
        { 41, "#calcButton41" },
        { 45, "#calcButton45" },
        { 60, "#calcButton60" },
        { 85, "#calcButton85" },
        { 100, "#calcButton100" },
    };

    #endregion

    #region Performance Settings

    /// <summary>
    /// Maximum concurrent tests for performance optimization
    /// </summary>
    public const int MaxConcurrentTests = 2;

    /// <summary>
    /// Browser pool size for performance optimization
    /// </summary>
    public const int BrowserPoolSize = 4;

    /// <summary>
    /// Auth cookie name used to inject token for E2E
    /// </summary>
    public const string AuthCookieName = "custom-jwt-token-override";

    #endregion
}
