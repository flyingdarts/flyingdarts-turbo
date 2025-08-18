using Microsoft.Playwright;

namespace Flyingdarts.E2E.Utilities;

/// <summary>
/// Manages parallel test execution with optimized resource usage
/// Provides performance monitoring and resource cleanup
/// </summary>
public class PerformanceTestRunner : IDisposable
{
    private readonly BrowserPool _browserPool;
    private readonly TokenCache _tokenCache;
    private readonly int _maxConcurrentTests;
    private readonly int _browserPoolSize;
    private readonly SemaphoreSlim _testSemaphore;
    private readonly List<TestExecutionInfo> _activeTests = new();
    private readonly object _lock = new();

    public PerformanceTestRunner(int maxConcurrentTests)
    {
        _maxConcurrentTests = maxConcurrentTests;
        _testSemaphore = new SemaphoreSlim(maxConcurrentTests, maxConcurrentTests);
        _browserPoolSize = Flyingdarts
            .E2E
            .Configuration
            .TestConfiguration
            .Performance
            .BrowserPoolSize;
        _browserPool = new BrowserPool(
            _browserPoolSize,
            headless: Flyingdarts.E2E.Configuration.TestConfiguration.Performance.HeadlessMode
        );
        _tokenCache = new TokenCache();
    }

    /// <summary>
    /// Initialize the test runner
    /// </summary>
    public async Task InitializeAsync()
    {
        await _browserPool.InitializeAsync();
        Console.WriteLine("🚀 Performance Test Runner initialized successfully!");
        Console.WriteLine($"📊 Max concurrent tests: {_maxConcurrentTests}");
        Console.WriteLine($"🌐 Browser pool size: {_browserPoolSize}");
    }

    /// <summary>
    /// Execute a test with performance monitoring
    /// </summary>
    public async Task<TestResult> ExecuteTestAsync(Func<Task> testAction, string testName)
    {
        var startTime = DateTime.UtcNow;
        var testId = Guid.NewGuid().ToString("N")[..8];

        Console.WriteLine($"🧪 Starting test: {testName} (ID: {testId})");

        await _testSemaphore.WaitAsync();

        try
        {
            lock (_lock)
            {
                _activeTests.Add(
                    new TestExecutionInfo
                    {
                        Id = testId,
                        Name = testName,
                        StartTime = startTime,
                        Status = TestStatus.Running,
                    }
                );
            }

            await testAction();

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            lock (_lock)
            {
                var testInfo = _activeTests.FirstOrDefault(t => t.Id == testId);
                if (testInfo != null)
                {
                    testInfo.Status = TestStatus.Completed;
                    testInfo.EndTime = endTime;
                    testInfo.Duration = duration;
                }
            }

            Console.WriteLine(
                $"✅ Test completed: {testName} (Duration: {duration.TotalSeconds:F2}s)"
            );

            return new TestResult
            {
                TestName = testName,
                TestId = testId,
                Success = true,
                Duration = duration,
                StartTime = startTime,
                EndTime = endTime,
            };
        }
        catch (Exception ex)
        {
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            lock (_lock)
            {
                var testInfo = _activeTests.FirstOrDefault(t => t.Id == testId);
                if (testInfo != null)
                {
                    testInfo.Status = TestStatus.Failed;
                    testInfo.EndTime = endTime;
                    testInfo.Duration = duration;
                    testInfo.Error = ex.Message;
                }
            }

            Console.WriteLine(
                $"❌ Test failed: {testName} (Duration: {duration.TotalSeconds:F2}s) - {ex.Message}"
            );

            return new TestResult
            {
                TestName = testName,
                TestId = testId,
                Success = false,
                Duration = duration,
                StartTime = startTime,
                EndTime = endTime,
                Error = ex.Message,
            };
        }
        finally
        {
            _testSemaphore.Release();
        }
    }

    /// <summary>
    /// Execute multiple tests in parallel
    /// </summary>
    public async Task<List<TestResult>> ExecuteTestsInParallelAsync(
        IEnumerable<(Func<Task> Action, string Name)> tests
    )
    {
        var testTasks = tests.Select(test => ExecuteTestAsync(test.Action, test.Name));
        var results = await Task.WhenAll(testTasks);
        return results.ToList();
    }

    /// <summary>
    /// Get current performance statistics
    /// </summary>
    public PerformanceStats GetPerformanceStats()
    {
        lock (_lock)
        {
            var completedTests = _activeTests
                .Where(t => t.Status == TestStatus.Completed || t.Status == TestStatus.Failed)
                .ToList();
            var runningTests = _activeTests.Where(t => t.Status == TestStatus.Running).ToList();

            var avgDuration = completedTests.Any()
                ? TimeSpan.FromTicks((long)completedTests.Average(t => t.Duration.Ticks))
                : TimeSpan.Zero;

            var successRate = completedTests.Any()
                ? (double)completedTests.Count(t => t.Status == TestStatus.Completed)
                    / completedTests.Count
                    * 100
                : 0;

            return new PerformanceStats
            {
                TotalTestsExecuted = completedTests.Count,
                CurrentlyRunning = runningTests.Count,
                SuccessRate = successRate,
                AverageDuration = avgDuration,
                BrowserPoolStats = _browserPool.GetPoolStats(),
                TokenCacheStats = _tokenCache.GetCacheStats(),
            };
        }
    }

    /// <summary>
    /// Get browser from pool
    /// </summary>
    public async Task<IBrowser> GetBrowserAsync()
    {
        return await _browserPool.GetBrowserAsync();
    }

    /// <summary>
    /// Return browser to pool
    /// </summary>
    public void ReturnBrowser(IBrowser browser)
    {
        _browserPool.ReturnBrowser(browser);
    }

    /// <summary>
    /// Get cached token
    /// </summary>
    public async Task<string> GetCachedTokenAsync(string key, Func<Task<string>> tokenFactory)
    {
        return await _tokenCache.GetOrCreateTokenAsync(key, tokenFactory);
    }

    /// <summary>
    /// Cleanup expired tokens
    /// </summary>
    public void CleanupExpiredTokens()
    {
        _tokenCache.CleanupExpiredTokens();
    }

    /// <summary>
    /// Print performance report
    /// </summary>
    public void PrintPerformanceReport()
    {
        var stats = GetPerformanceStats();

        Console.WriteLine("\n📊 PERFORMANCE REPORT 📊");
        Console.WriteLine("=========================");
        Console.WriteLine($"🧪 Total Tests Executed: {stats.TotalTestsExecuted}");
        Console.WriteLine($"🏃 Currently Running: {stats.CurrentlyRunning}");
        Console.WriteLine($"✅ Success Rate: {stats.SuccessRate:F1}%");
        Console.WriteLine($"⏱️  Average Duration: {stats.AverageDuration.TotalSeconds:F2}s");
        Console.WriteLine(
            $"🌐 Browser Pool: {stats.BrowserPoolStats.Available} available, {stats.BrowserPoolStats.InUse} in use"
        );
        Console.WriteLine(
            $"🔐 Token Cache: {stats.TokenCacheStats.Valid} valid, {stats.TokenCacheStats.Expired} expired"
        );
        Console.WriteLine("=========================\n");
    }

    public void Dispose()
    {
        _testSemaphore?.Dispose();
        _browserPool?.Dispose();
        _tokenCache?.Clear();
    }

    /// <summary>
    /// Test execution information
    /// </summary>
    private class TestExecutionInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TestStatus Status { get; set; }
        public string? Error { get; set; }
    }

    /// <summary>
    /// Test status enumeration
    /// </summary>
    private enum TestStatus
    {
        Running,
        Completed,
        Failed,
    }
}

/// <summary>
/// Test execution result
/// </summary>
public class TestResult
{
    public string TestName { get; set; } = string.Empty;
    public string TestId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Performance statistics
/// </summary>
public class PerformanceStats
{
    public int TotalTestsExecuted { get; set; }
    public int CurrentlyRunning { get; set; }
    public double SuccessRate { get; set; }
    public TimeSpan AverageDuration { get; set; }
    public (int Available, int InUse, int Total) BrowserPoolStats { get; set; }
    public (int Total, int Valid, int Expired) TokenCacheStats { get; set; }
}
