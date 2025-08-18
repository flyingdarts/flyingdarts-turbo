using Xunit;

namespace Flyingdarts.E2E.Utilities;

/// <summary>
/// xUnit collection fixture that ensures a single shared PerformanceTestRunner
/// lifecycle across the entire test run.
/// </summary>
[CollectionDefinition("SharedRunnerCollection")]
public class SharedRunnerCollection : ICollectionFixture<SharedRunnerFixture> { }

public class SharedRunnerFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await GlobalPerformanceRunner.GetRunnerAsync();
    }

    public Task DisposeAsync()
    {
        GlobalPerformanceRunner.DisposeRunner();
        return Task.CompletedTask;
    }
}
