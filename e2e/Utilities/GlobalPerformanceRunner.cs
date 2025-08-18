using Flyingdarts.E2E.Configuration;

namespace Flyingdarts.E2E.Utilities;

/// <summary>
/// Provides a single shared instance of PerformanceTestRunner (and thus BrowserPool)
/// across the entire test suite to enforce a global browser cap.
/// </summary>
public static class GlobalPerformanceRunner
{
    private static readonly object _lock = new();
    private static PerformanceTestRunner? _runner;
    private static bool _initialized;

    public static async Task<PerformanceTestRunner> GetRunnerAsync()
    {
        if (_runner != null && _initialized)
        {
            return _runner;
        }

        lock (_lock)
        {
            if (_runner == null)
            {
                var maxConcurrent = TestConfiguration.Performance.MaxConcurrentTests;
                _runner = new PerformanceTestRunner(maxConcurrent);
            }
        }

        if (!_initialized)
        {
            await _runner!.InitializeAsync();
            _initialized = true;
        }

        return _runner!;
    }

    public static PerformanceTestRunner Runner =>
        _runner
        ?? throw new InvalidOperationException(
            "Global runner not initialized. Call GetRunnerAsync() first."
        );

    public static void DisposeRunner()
    {
        lock (_lock)
        {
            if (_runner != null)
            {
                _runner.Dispose();
                _runner = null;
                _initialized = false;
            }
        }
    }
}
