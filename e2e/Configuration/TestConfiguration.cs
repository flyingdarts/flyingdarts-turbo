namespace Flyingdarts.E2E.Configuration;

/// <summary>
/// Configuration for test execution with performance optimizations
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Performance-related configuration
    /// </summary>
    public static class Performance
    {
        /// <summary>
        /// Maximum number of concurrent tests
        /// </summary>
        public static int MaxConcurrentTests =>
            int.TryParse(
                Environment.GetEnvironmentVariable("E2E_MAX_CONCURRENT_TESTS"),
                out var value
            )
                ? value
                : 4;

        /// <summary>
        /// Browser pool size
        /// </summary>
        public static int BrowserPoolSize =>
            int.TryParse(Environment.GetEnvironmentVariable("E2E_BROWSER_POOL_SIZE"), out var value)
                ? value
                : 8;

        /// <summary>
        /// Whether to run browsers in headless mode
        /// </summary>
        public static bool HeadlessMode =>
            bool.TryParse(Environment.GetEnvironmentVariable("E2E_HEADLESS_MODE"), out var value)
            && value;

        /// <summary>
        /// Default timeout for element operations (ms)
        /// </summary>
        public static int DefaultElementTimeout =>
            int.TryParse(Environment.GetEnvironmentVariable("E2E_ELEMENT_TIMEOUT"), out var value)
                ? value
                : 15000;

        /// <summary>
        /// Default timeout for navigation (ms)
        /// </summary>
        public static int DefaultNavigationTimeout =>
            int.TryParse(
                Environment.GetEnvironmentVariable("E2E_NAVIGATION_TIMEOUT"),
                out var value
            )
                ? value
                : 15000;

        /// <summary>
        /// Whether to enable resource blocking
        /// </summary>
        public static bool EnableResourceBlocking =>
            bool.TryParse(
                Environment.GetEnvironmentVariable("E2E_ENABLE_RESOURCE_BLOCKING"),
                out var value
            )
                ? value
                : true;

        /// <summary>
        /// Whether to enable token caching
        /// </summary>
        public static bool EnableTokenCaching =>
            bool.TryParse(
                Environment.GetEnvironmentVariable("E2E_ENABLE_TOKEN_CACHING"),
                out var value
            )
                ? value
                : true;

        /// <summary>
        /// Token cache expiration time (minutes)
        /// </summary>
        public static int TokenCacheExpirationMinutes =>
            int.TryParse(
                Environment.GetEnvironmentVariable("E2E_TOKEN_CACHE_EXPIRATION"),
                out var value
            )
                ? value
                : 55;
    }

    /// <summary>
    /// Browser-related configuration
    /// </summary>
    public static class Browser
    {
        /// <summary>
        /// Default viewport width
        /// </summary>
        public static int DefaultViewportWidth =>
            int.TryParse(Environment.GetEnvironmentVariable("E2E_VIEWPORT_WIDTH"), out var value)
                ? value
                : 1600;

        /// <summary>
        /// Default viewport height
        /// </summary>
        public static int DefaultViewportHeight =>
            int.TryParse(Environment.GetEnvironmentVariable("E2E_VIEWPORT_HEIGHT"), out var value)
                ? value
                : 900;

        /// <summary>
        /// Whether to enable browser performance monitoring
        /// </summary>
        public static bool EnablePerformanceMonitoring =>
            bool.TryParse(
                Environment.GetEnvironmentVariable("E2E_ENABLE_PERFORMANCE_MONITORING"),
                out var value
            )
                ? value
                : true;
    }

    /// <summary>
    /// Test execution configuration
    /// </summary>
    public static class Execution
    {
        /// <summary>
        /// Whether to enable parallel test execution
        /// </summary>
        public static bool EnableParallelExecution =>
            bool.TryParse(Environment.GetEnvironmentVariable("E2E_ENABLE_PARALLEL"), out var value)
                ? value
                : true;

        /// <summary>
        /// Whether to enable test retry on failure
        /// </summary>
        public static bool EnableTestRetry =>
            bool.TryParse(Environment.GetEnvironmentVariable("E2E_ENABLE_RETRY"), out var value)
                ? value
                : true;

        /// <summary>
        /// Maximum number of test retries
        /// </summary>
        public static int MaxTestRetries =>
            int.TryParse(Environment.GetEnvironmentVariable("E2E_MAX_RETRIES"), out var value)
                ? value
                : 2;

        /// <summary>
        /// Whether to enable performance reporting
        /// </summary>
        public static bool EnablePerformanceReporting =>
            bool.TryParse(
                Environment.GetEnvironmentVariable("E2E_ENABLE_PERFORMANCE_REPORTING"),
                out var value
            )
                ? value
                : true;
    }

    /// <summary>
    /// Resource blocking configuration
    /// </summary>
    public static class ResourceBlocking
    {
        /// <summary>
        /// Whether to block images for faster loading
        /// </summary>
        public static bool BlockImages =>
            bool.TryParse(Environment.GetEnvironmentVariable("E2E_BLOCK_IMAGES"), out var value)
                ? value
                : false;

        /// <summary>
        /// Whether to block CSS for faster loading
        /// </summary>
        public static bool BlockCSS =>
            bool.TryParse(Environment.GetEnvironmentVariable("E2E_BLOCK_CSS"), out var value)
                ? value
                : false;
    }

    /// <summary>
    /// Get all configuration as a dictionary for logging
    /// </summary>
    public static Dictionary<string, object> GetAllConfiguration()
    {
        return new Dictionary<string, object>
        {
            ["Performance.MaxConcurrentTests"] = Performance.MaxConcurrentTests,
            ["Performance.BrowserPoolSize"] = Performance.BrowserPoolSize,
            ["Performance.HeadlessMode"] = Performance.HeadlessMode,
            ["Performance.DefaultElementTimeout"] = Performance.DefaultElementTimeout,
            ["Performance.DefaultNavigationTimeout"] = Performance.DefaultNavigationTimeout,
            ["Performance.EnableResourceBlocking"] = Performance.EnableResourceBlocking,
            ["Performance.EnableTokenCaching"] = Performance.EnableTokenCaching,
            ["Performance.TokenCacheExpirationMinutes"] = Performance.TokenCacheExpirationMinutes,
            ["Browser.DefaultViewportWidth"] = Browser.DefaultViewportWidth,
            ["Browser.DefaultViewportHeight"] = Browser.DefaultViewportHeight,
            ["Browser.EnablePerformanceMonitoring"] = Browser.EnablePerformanceMonitoring,
            ["Execution.EnableParallelExecution"] = Execution.EnableParallelExecution,
            ["Execution.EnableTestRetry"] = Execution.EnableTestRetry,
            ["Execution.MaxTestRetries"] = Execution.MaxTestRetries,
            ["Execution.EnablePerformanceReporting"] = Execution.EnablePerformanceReporting,
            ["ResourceBlocking.BlockImages"] = ResourceBlocking.BlockImages,
            ["ResourceBlocking.BlockCSS"] = ResourceBlocking.BlockCSS,
        };
    }
}
