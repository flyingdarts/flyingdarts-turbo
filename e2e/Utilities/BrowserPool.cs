using Microsoft.Playwright;

namespace Flyingdarts.E2E.Utilities;

/// <summary>
/// Manages a pool of browser instances for efficient test execution
/// Reuses browsers across tests to reduce startup overhead
/// </summary>
public class BrowserPool : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Queue<IBrowser> _availableBrowsers;
    private readonly HashSet<IBrowser> _inUseBrowsers;
    private readonly object _lock = new();
    private readonly int _maxPoolSize;
    private readonly bool _headless;
    private IPlaywright? _playwright;

    public BrowserPool(int maxPoolSize = 4, bool headless = false)
    {
        _maxPoolSize = maxPoolSize;
        _headless = headless;
        _semaphore = new SemaphoreSlim(maxPoolSize, maxPoolSize);
        _availableBrowsers = new Queue<IBrowser>();
        _inUseBrowsers = new HashSet<IBrowser>();
    }

    /// <summary>
    /// Initialize the browser pool
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_playwright == null)
        {
            _playwright = await Playwright.CreateAsync();
        }

        // Pre-warm the pool with initial browsers
        var initialBrowsers = Math.Min(2, _maxPoolSize);
        for (int i = 0; i < initialBrowsers; i++)
        {
            var browser = await CreateBrowserAsync();
            _availableBrowsers.Enqueue(browser);
        }
    }

    /// <summary>
    /// Get a browser from the pool
    /// </summary>
    public async Task<IBrowser> GetBrowserAsync()
    {
        await _semaphore.WaitAsync();

        lock (_lock)
        {
            if (_availableBrowsers.Count > 0)
            {
                var browser = _availableBrowsers.Dequeue();
                _inUseBrowsers.Add(browser);
                return browser;
            }
        }

        // Create new browser if pool is empty
        var newBrowser = await CreateBrowserAsync();
        lock (_lock)
        {
            _inUseBrowsers.Add(newBrowser);
        }
        return newBrowser;
    }

    /// <summary>
    /// Return a browser to the pool
    /// </summary>
    public void ReturnBrowser(IBrowser browser)
    {
        if (browser == null)
            return;

        lock (_lock)
        {
            if (_inUseBrowsers.Remove(browser))
            {
                if (_availableBrowsers.Count < _maxPoolSize)
                {
                    _availableBrowsers.Enqueue(browser);
                }
                else
                {
                    // Pool is full, dispose the browser
                    browser.DisposeAsync();
                }
            }
        }

        _semaphore.Release();
    }

    /// <summary>
    /// Create a new browser instance
    /// </summary>
    private async Task<IBrowser> CreateBrowserAsync()
    {
        if (_playwright == null)
        {
            throw new InvalidOperationException(
                "BrowserPool not initialized. Call InitializeAsync() first."
            );
        }

        return await _playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = _headless,
                Args = new[]
                {
                    "--disable-dev-shm-usage",
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-gpu",
                    "--disable-web-security",
                    "--disable-features=VizDisplayCompositor",
                },
            }
        );
    }

    /// <summary>
    /// Get pool statistics
    /// </summary>
    public (int Available, int InUse, int Total) GetPoolStats()
    {
        lock (_lock)
        {
            return (
                _availableBrowsers.Count,
                _inUseBrowsers.Count,
                _availableBrowsers.Count + _inUseBrowsers.Count
            );
        }
    }

    /// <summary>
    /// Clean up all browsers and dispose the pool
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        lock (_lock)
        {
            foreach (var browser in _availableBrowsers)
            {
                browser.DisposeAsync();
            }
            _availableBrowsers.Clear();

            foreach (var browser in _inUseBrowsers)
            {
                browser.DisposeAsync();
            }
            _inUseBrowsers.Clear();
        }

        _semaphore?.Dispose();
        _playwright?.Dispose();
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }
}
