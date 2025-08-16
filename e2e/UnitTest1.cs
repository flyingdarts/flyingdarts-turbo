using System.Text.Json;
using System.Text.RegularExpressions;
using Authress.SDK;
using Authress.SDK.Client;
using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Tests;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Flyingdarts.E2E;

/// <summary>
/// Legacy test class - demonstrates migration to POM architecture
/// This class shows how existing tests can be gradually migrated
/// </summary>
public class UnitTest1 : BaseTest
{
    /// <summary>
    /// Example of how to migrate to the new POM architecture
    /// This test demonstrates the new approach
    /// </summary>
    [Fact]
    public async Task MigratedToPOM_ShouldSetGameSettings()
    {
        await SetupAsync();
        await SetAuthTokenAsync();

        var homePage = new HomePage(Page);
        await homePage.NavigateToHomeAsync();

        if (homePage.IsOnSettingsPage())
        {
            var settingsPage = new SettingsPage(Page);
            await settingsPage.WaitForPageReadyAsync();
            await settingsPage.SetGameSettingsAsync(3, 5);
            await settingsPage.SaveSettingsAsync();
        }

        await homePage.StartNewGameAsync();

        var gamePage = new GamePage(Page);

        await gamePage.WaitForPageReadyAsync();
    }
}
