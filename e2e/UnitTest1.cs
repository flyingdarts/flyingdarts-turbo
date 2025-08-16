using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Tests;

namespace Flyingdarts.E2E;

/// <summary>
/// Testing a game of darts between two players
/// </summary>
public class UnitTest1 : MultiBrowserBaseTest
{
    /// <summary>
    /// Testing a game of darts between two players
    /// </summary>
    [Fact]
    public async Task GameOfDarts()
    {
        // Setup both users
        await SetupAsync();
        var tasks = new List<Task> { SetupPlayer(), SetupOpponent() };
        await Task.WhenAll(tasks);

        // Create optimized home pages for both users
        var player1Home = new HomePage(User1Page, BaseUrl);
        var player2Home = new HomePage(User2Page, BaseUrl);

        // Navigate both users to home simultaneously
        var player1NavigateTask = Task.Run(async () =>
        {
            await player1Home.NavigateToHomeAsync();
        });

        var player2NavigateTask = Task.Run(async () =>
        {
            await player2Home.NavigateToHomeAsync();
        });

        var navigateTasks = new List<Task> { player1NavigateTask, player2NavigateTask };
        await Task.WhenAll(navigateTasks);

        // Handle settings for both users simultaneously with original settings page
        var player1SettingsTask = Task.Run(async () =>
        {
            if (player1Home.IsOnSettingsPage())
            {
                var settingsPage = new SettingsPage(User1Page, BaseUrl);
                await settingsPage.WaitForPageReadyAsync();
                await settingsPage.SetGameSettingsAsync(3, 5);
                await settingsPage.SaveSettingsAsync();
            }
        });

        var player2SettingsTask = Task.Run(async () =>
        {
            if (player2Home.IsOnSettingsPage())
            {
                var settingsPage = new SettingsPage(User2Page, BaseUrl);
                await settingsPage.WaitForPageReadyAsync();
                await settingsPage.SetGameSettingsAsync(3, 5);
                await settingsPage.SaveSettingsAsync();
            }
        });

        var settingsTasks = new List<Task> { player1SettingsTask, player2SettingsTask };
        await Task.WhenAll(settingsTasks);

        // Player 1 starts a new game
        await player1Home.StartNewGameAsync();

        // Wait for the game page to be ready
        await player1Home.WaitForGamePageReadyAsync();

        // Verify we're on a game page before extracting the ID
        var currentUrl = User1Page.Url;
        Console.WriteLine($"Current URL after starting game: {currentUrl}");

        if (!currentUrl.Contains("/game/"))
        {
            throw new Exception($"Expected to be on game page, but current URL is: {currentUrl}");
        }

        var gameId = player1Home.GetGameId();
        Console.WriteLine($"Extracted game ID: {gameId}");

        if (string.IsNullOrEmpty(gameId))
        {
            throw new Exception($"Game ID is null. Current URL: {currentUrl}");
        }

        // Player 2 joins the game
        await player2Home.NavigateToAsync($"/game/{gameId}");

        // Create optimized game page objects for both users
        var player1Game = new GamePage(User1Page, BaseUrl);
        var player2Game = new GamePage(User2Page, BaseUrl);

        // Wait for both game pages to be ready
        await player1Game.WaitForPageReadyAsync();
        await player2Game.WaitForPageReadyAsync();

        // Verify both users are on the game page
        await player1Game.IsGamePageLoadedAsync();
        await player2Game.IsGamePageLoadedAsync();

        // Take screenshots from both perspectives
        await TakeBothUsersScreenshotsAsync("game_settings_migrated");

        // Player 1 throws a dart
        await player1Game.ThrowDartAsync();

        // Player 2 throws a dart
        await player2Game.ThrowDartAsync();

        // Player 1 throws a dart
        await player1Game.ThrowDartAsync();

        // Player 2 throws a dart
        await player2Game.ThrowDartAsync();

        // Player 1 throws a dart
        await player1Game.ThrowDartAsync();

        // Player 2 throws a dart
        await player2Game.ThrowDartAsync();

        // Player 1 throws a dart
        await player1Game.ThrowDartAsync();

        // Player 2 throws a dart
        await player2Game.ThrowDartAsync();

        // Player 1 throws a dart
        await player1Game.ThrowDartAsync();

        // Player 2 throws a dart
        await player2Game.ThrowDartAsync();

        // Player 1 throws a dart
        await player1Game.ThrowDartAsync();

        // Player 2 throws a dart
        await player2Game.ThrowDartAsync();

        // Player 1 throws a dart
        await player1Game.ThrowDartAsync();

        // Player 2 throws a dart
        await player2Game.ThrowDartAsync();

        // Cleanup
        await TeardownAsync();
    }

    private async Task SetupPlayer()
    {
        var token = await AuthressHelperUser1?.GetBearerTokenAsync();
        if (token is null)
        {
            throw new Exception("Token is null");
        }

        await SetAuthTokenAsync(token, User1Page);
    }

    private async Task SetupOpponent()
    {
        var token = await AuthressHelperUser2?.GetBearerTokenAsync();
        if (token is null)
        {
            throw new Exception("Token is null");
        }

        await SetAuthTokenAsync(token, User2Page);
    }
}
