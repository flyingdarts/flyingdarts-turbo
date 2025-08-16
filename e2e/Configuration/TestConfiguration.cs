namespace Flyingdarts.E2E.Configuration;

/// <summary>
/// Configuration settings for E2E tests
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Base URLs for different environments
    /// </summary>
    public static class Urls
    {
        public const string Staging = "https://staging.flyingdarts.net";
        public const string Production = "https://flyingdarts.net";
        public const string Development = "https://dev.flyingdarts.net";
        public const string Local = "http://localhost:4200";
    }

    /// <summary>
    /// Authress configuration
    /// </summary>
    public static class Authress
    {
        public const string Url = "https://authress.flyingdarts.net";
        public const string TokenCookieName = "custom-jwt-token-override";
        public const string TokenCookieDomain = ".flyingdarts.net";
        public const string TokenCookiePath = "/";
    }

    /// <summary>
    /// Timeout settings
    /// </summary>
    public static class Timeouts
    {
        public const int DefaultTimeout = 30000; // 30 seconds
        public const int NavigationTimeout = 30000; // 30 seconds
        public const int ElementWaitTimeout = 10000; // 10 seconds
        public const int RetryDelay = 1000; // 1 second
        public const int MaxRetries = 3;
    }

    /// <summary>
    /// Screenshot settings
    /// </summary>
    public static class Screenshots
    {
        public const string Directory = "screenshots";
        public const string Format = "png";
        public const string DateTimeFormat = "yyyyMMdd_HHmmss";
    }

    /// <summary>
    /// Test data settings
    /// </summary>
    public static class TestData
    {
        public const int MinSets = 1;
        public const int MaxSets = 10;
        public const int MinLegs = 1;
        public const int MaxLegs = 15;
        public const int DefaultSets = 3;
        public const int DefaultLegs = 5;
    }

    /// <summary>
    /// Get the base URL based on environment variable or default to staging
    /// </summary>
    /// <returns>Base URL for tests</returns>
    public static string GetBaseUrl()
    {
        var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT")?.ToLower();

        return environment switch
        {
            "production" => Urls.Production,
            "development" => Urls.Development,
            "local" => Urls.Local,
            _ => Urls.Staging, // Default to staging
        };
    }

    /// <summary>
    /// Get timeout value based on environment variable or default
    /// </summary>
    /// <param name="defaultTimeout">Default timeout value</param>
    /// <returns>Timeout value in milliseconds</returns>
    public static int GetTimeout(int defaultTimeout = Timeouts.DefaultTimeout)
    {
        var timeoutEnv = Environment.GetEnvironmentVariable("TEST_TIMEOUT");
        if (int.TryParse(timeoutEnv, out var timeout))
        {
            return timeout;
        }
        return defaultTimeout;
    }

    /// <summary>
    /// Check if tests should run in headless mode
    /// </summary>
    /// <returns>True if headless mode is enabled</returns>
    public static bool IsHeadlessMode()
    {
        var headlessEnv = Environment.GetEnvironmentVariable("TEST_HEADLESS");
        return string.IsNullOrEmpty(headlessEnv) || headlessEnv.ToLower() == "true";
    }

    /// <summary>
    /// Check if screenshots should be taken
    /// </summary>
    /// <returns>True if screenshots are enabled</returns>
    public static bool AreScreenshotsEnabled()
    {
        var screenshotsEnv = Environment.GetEnvironmentVariable("TEST_SCREENSHOTS");
        return string.IsNullOrEmpty(screenshotsEnv) || screenshotsEnv.ToLower() == "true";
    }

    /// <summary>
    /// Get the screenshot directory path
    /// </summary>
    /// <returns>Screenshot directory path</returns>
    public static string GetScreenshotDirectory()
    {
        var screenshotsDir = Environment.GetEnvironmentVariable("TEST_SCREENSHOTS_DIR");
        return string.IsNullOrEmpty(screenshotsDir) ? Screenshots.Directory : screenshotsDir;
    }
}
