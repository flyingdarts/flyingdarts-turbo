using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Tests;
using Microsoft.Playwright;

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

        // 🎯 Hardcoded 9-dart leg instead of random darts
        Console.WriteLine("🎯 Starting hardcoded 9-dart leg sequence...");

        // Round 1: Player 1 starts a new leg
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 1: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 1: Player 1: Next 3 darts: 180 (3x60) to get to 141
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 1: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 1: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync(); // Triple 20

        Console.WriteLine("🎯 Player 1 won the leg!");

        await ValidatePlayer1WonLeg();

        // Round 2: Player 2 starts a new leg: 180 (3x60) to get to 321
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 2: Player 1: 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 2: Player 2: Next 3 darts: 180 (3x60) to get to 141
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 2: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 2: Player 2: Final 3 darts: 141 checkout (T20, T19, D12)
        await player2Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 2 won the leg!");

        await ValidatePlayer2WonLeg();

        // Round 3: Player 1 starts a new leg
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 3: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 3: Player 1: Next 3 darts: 180 (3x60) to get to 141
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 3: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 3: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 1 won the leg!");

        await ValidatePlayer1WonLeg();

        // Round 4: Player 2 starts a new leg
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 4: Player 1: 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 4: Player 2: Next 3 darts: 180 (3x60) to get to 141
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 4: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 4: Player 2: Final 3 darts: 141 checkout (T20, T19, D12)
        await player2Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 2 won the leg!");

        await ValidatePlayer2WonLeg();

        // Round 5: Player 1 starts a new leg
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 5: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 5: Player 1: Next 3 darts: 180 (3x60) to get to 141
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 5: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 5: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync(); // Triple 20

        Console.WriteLine("🎯 Player 1 won the set!");

        // validate player 1 won the set
        await ValidatePlayer1WonSet();

        // Round 6: Player 2 starts a new set
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 6: Player 1: 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 6: Player 2: Next 3 darts: 180 (3x60) to get to 141
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 6: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 6: Player 2: Final 3 darts: 141 checkout (T20, T19, D12)
        await player2Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 2 won the set!");

        // validate player 2 won the set
        await ValidatePlayer2WonLeg();

        // Round 7: Player 1 starts a new round
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 7: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 7: Player 1: Next 3 darts: 180 (3x60) to get to 141
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 7: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 7: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 1 won the round!");

        await ValidatePlayer1WonLeg();

        // Round 8: Player 2 starts a new round
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 8: Player 1: 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 8: Player 2: Next 3 darts: 180 (3x60) to get to 141
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 8: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 8: Player 2: Final 3 darts: 141 checkout (T20, T19, D12)
        await player2Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 2 won the round!");

        await ValidatePlayer2WonLeg();

        // Round 9: Player 1 starts a new round
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 9: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 9: Player 1: Next 3 darts: 180 (3x60) to get to 141
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 9: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 9: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 1 won the round!");

        await ValidatePlayer1WonLeg();

        // Round 10: Player 2 starts a new round
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 10: Player 1: 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 10: Player 2: Next 3 darts: 180 (3x60) to get to 141
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 10: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 10: Player 2: Final 3 darts: 141 checkout (T20, T19, D12)
        await player2Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 2 won the set!");

        await ValidatePlayer2WonSet();

        // Round 11: Player 1 starts a new game
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 11: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 11: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 11: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 11: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 1 won the leg!");

        await ValidatePlayer1WonLeg();

        // Round 12: Player 2 starts a new leg
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 12: Player 1: 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 12: Player 2: Next 3 darts: 180 (3x60) to get to 141
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 12: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 12: Player 2: Final 3 darts: 141 checkout (T20, T19, D12)
        await player2Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 2 won the leg!");

        await ValidatePlayer2WonLeg();

        // Round 13: Player 1 starts a new round
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 13: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 13: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 13: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 13: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 1 won the round!");

        await ValidatePlayer1WonLeg();

        // Round 14: Player 2 starts a new round
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 14: Player 1: 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 14: Player 2: Next 3 darts: 180 (3x60) to get to 141
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 14: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 14: Player 2: Final 3 darts: 141 checkout (T20, T19, D12)
        await player2Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 2 won the round!");

        await ValidatePlayer2WonLeg();

        // Round 15: Player 1 starts a new round
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 15: Player 2: 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 15: Player 1: Next 3 darts
        await player1Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 15: Player 2: Next 3 darts
        await player2Game.ThrowDartWithScoreAsync(180); // Triple 20

        // Round 15: Player 1: Final 3 darts: 141 checkout (T20, T19, D12)
        await player1Game.ClickCheckoutAsync();

        Console.WriteLine("🎯 Player 1 won the round!");

        await ValidatePlayer1WonGame();

        // Cleanup
        await TeardownAsync();
    }

    private async Task ValidatePlayer1WonLeg()
    {
        Console.WriteLine("🔍 Validating Player 1 won the leg...");

        // Wait for the winner popup to appear on both pages
        var winnerPopup1 = User1Page.Locator("#winnerPopupOverlay");
        var winnerPopup2 = User2Page.Locator("#winnerPopupOverlay");

        await winnerPopup1.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );
        await winnerPopup2.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );

        // Verify the popup is visible on both pages
        var isVisible1 = await winnerPopup1.IsVisibleAsync();
        var isVisible2 = await winnerPopup2.IsVisibleAsync();

        if (!isVisible1 || !isVisible2)
        {
            throw new Exception(
                "Winner popup is not visible on both pages after Player 1 won the leg"
            );
        }

        // Check the winner name and text on Player 1's page
        var winnerName = User1Page.Locator(".winner-name");
        var winnerText = User1Page.Locator(".winner-text");

        await winnerName.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await winnerText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var nameText = await winnerName.TextContentAsync();
        var textContent = await winnerText.TextContentAsync();

        Console.WriteLine($"🏆 Winner popup shows: {nameText} - {textContent}");

        // Verify it shows "Wins the leg!" text
        if (textContent?.Trim() != "Wins the leg!")
        {
            throw new Exception($"Expected 'Wins the leg!' but got '{textContent}'");
        }

        Console.WriteLine("✅ Player 1 leg win validation successful!");

        // Close the winner popup on Player 1's page
        var closeButton1 = User1Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton2.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton2.ClickAsync();

        // Wait for popups to disappear on both pages
        await Task.Delay(500); // Brief delay for animation
        var isHidden1 = await winnerPopup1.IsHiddenAsync();
        var isHidden2 = await winnerPopup2.IsHiddenAsync();

        if (!isHidden1 || !isHidden2)
        {
            throw new Exception(
                "Winner popup did not close on both pages after clicking close buttons"
            );
        }
        Console.WriteLine("🚪 Winner popup closed successfully on both pages!");
    }

    private async Task ValidatePlayer1WonSet()
    {
        Console.WriteLine("🔍 Validating Player 1 won the set...");

        // Wait for the winner popup to appear on both pages
        var winnerPopup1 = User1Page.Locator("#winnerPopupOverlay");
        var winnerPopup2 = User2Page.Locator("#winnerPopupOverlay");

        await winnerPopup1.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );
        await winnerPopup2.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );

        // Verify the popup is visible on both pages
        var isVisible1 = await winnerPopup1.IsVisibleAsync();
        var isVisible2 = await winnerPopup2.IsVisibleAsync();

        if (!isVisible1 || !isVisible2)
        {
            throw new Exception(
                "Winner popup is not visible on both pages after Player 1 won the set"
            );
        }

        // Check the winner name and text on Player 1's page
        var winnerName = User1Page.Locator(".winner-name");
        var winnerText = User1Page.Locator(".winner-text");

        await winnerName.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await winnerText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var nameText = await winnerName.TextContentAsync();
        var textContent = await winnerText.TextContentAsync();

        Console.WriteLine($"🏆 Winner popup shows: {nameText} - {textContent}");

        // Verify it shows "Wins the set!" text
        if (textContent?.Trim() != "Wins the set!")
        {
            throw new Exception($"Expected 'Wins the set!' but got '{textContent}'");
        }

        Console.WriteLine("✅ Player 1 set win validation successful!");

        // Close the winner popup on Player 1's page
        var closeButton1 = User1Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton2.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton2.ClickAsync();

        // Wait for popups to disappear on both pages
        await Task.Delay(500); // Brief delay for animation
        var isHidden1 = await winnerPopup1.IsHiddenAsync();
        var isHidden2 = await winnerPopup2.IsHiddenAsync();

        if (!isHidden1 || !isHidden2)
        {
            throw new Exception(
                "Winner popup did not close on both pages after clicking close buttons"
            );
        }
        Console.WriteLine("🚪 Winner popup closed successfully on both pages!");
    }

    private async Task ValidatePlayer1WonGame()
    {
        Console.WriteLine("🔍 Validating Player 1 won the game...");

        // Wait for the winner popup to appear on both pages
        var winnerPopup1 = User1Page.Locator("#winnerPopupOverlay");
        var winnerPopup2 = User2Page.Locator("#winnerPopupOverlay");

        await winnerPopup1.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );
        await winnerPopup2.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );

        // Verify the popup is visible on both pages
        var isVisible1 = await winnerPopup1.IsVisibleAsync();
        var isVisible2 = await winnerPopup2.IsVisibleAsync();

        if (!isVisible1 || !isVisible2)
        {
            throw new Exception(
                "Winner popup is not visible on both pages after Player 1 won the game"
            );
        }

        // Check the winner name and text on Player 1's page
        var winnerName = User1Page.Locator(".winner-name");
        var winnerText = User1Page.Locator(".winner-text");

        await winnerName.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await winnerText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var nameText = await winnerName.TextContentAsync();
        var textContent = await winnerText.TextContentAsync();

        Console.WriteLine($"🏆 Winner popup shows: {nameText} - {textContent}");

        // Verify it shows "Wins the game!" text
        if (textContent?.Trim() != "Wins the game!")
        {
            throw new Exception($"Expected 'Wins the game!' but got '{textContent}'");
        }

        Console.WriteLine("✅ Player 1 game win validation successful!");

        // Close the winner popup on Player 1's page
        var closeButton1 = User1Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton2.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton2.ClickAsync();

        // Wait for popups to disappear on both pages
        await Task.Delay(500); // Brief delay for animation
        var isHidden1 = await winnerPopup1.IsHiddenAsync();
        var isHidden2 = await winnerPopup2.IsHiddenAsync();

        if (!isHidden1 || !isHidden2)
        {
            throw new Exception(
                "Winner popup did not close on both pages after clicking close buttons"
            );
        }
        Console.WriteLine("🚪 Winner popup closed successfully on both pages!");
    }

    private async Task ValidatePlayer2WonLeg()
    {
        Console.WriteLine("🔍 Validating Player 2 won the leg...");

        // Wait for the winner popup to appear on both pages
        var winnerPopup1 = User1Page.Locator("#winnerPopupOverlay");
        var winnerPopup2 = User2Page.Locator("#winnerPopupOverlay");

        await winnerPopup1.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );
        await winnerPopup2.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );

        // Verify the popup is visible on both pages
        var isVisible1 = await winnerPopup1.IsVisibleAsync();
        var isVisible2 = await winnerPopup2.IsVisibleAsync();

        if (!isVisible1 || !isVisible2)
        {
            throw new Exception(
                "Winner popup is not visible on both pages after Player 2 won the leg"
            );
        }

        // Check the winner name and text on Player 2's page
        var winnerName = User2Page.Locator(".winner-name");
        var winnerText = User2Page.Locator(".winner-text");

        await winnerName.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await winnerText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var nameText = await winnerName.TextContentAsync();
        var textContent = await winnerText.TextContentAsync();

        Console.WriteLine($"🏆 Winner popup shows: {nameText} - {textContent}");

        // Verify it shows "Wins the leg!" text
        if (textContent?.Trim() != "Wins the leg!")
        {
            throw new Exception($"Expected 'Wins the leg!' but got '{textContent}'");
        }

        Console.WriteLine("✅ Player 2 leg win validation successful!");

        // Close the winner popup on Player 1's page
        var closeButton1 = User1Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton2.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton2.ClickAsync();

        // Wait for popups to disappear on both pages
        await Task.Delay(500); // Brief delay for animation
        var isHidden1 = await winnerPopup1.IsHiddenAsync();
        var isHidden2 = await winnerPopup2.IsHiddenAsync();

        if (!isHidden1 || !isHidden2)
        {
            throw new Exception(
                "Winner popup did not close on both pages after clicking close buttons"
            );
        }
        Console.WriteLine("🚪 Winner popup closed successfully on both pages!");
    }

    private async Task ValidatePlayer2WonSet()
    {
        Console.WriteLine("🔍 Validating Player 2 won the set...");

        // Wait for the winner popup to appear on both pages
        var winnerPopup1 = User1Page.Locator("#winnerPopupOverlay");
        var winnerPopup2 = User2Page.Locator("#winnerPopupOverlay");

        await winnerPopup1.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );
        await winnerPopup2.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );

        // Verify the popup is visible on both pages
        var isVisible1 = await winnerPopup1.IsVisibleAsync();
        var isVisible2 = await winnerPopup2.IsVisibleAsync();

        if (!isVisible1 || !isVisible2)
        {
            throw new Exception(
                "Winner popup is not visible on both pages after Player 2 won the set"
            );
        }

        // Check the winner name and text on Player 2's page
        var winnerName = User2Page.Locator(".winner-name");
        var winnerText = User2Page.Locator(".winner-text");

        await winnerName.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await winnerText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var nameText = await winnerName.TextContentAsync();
        var textContent = await winnerText.TextContentAsync();

        Console.WriteLine($"🏆 Winner popup shows: {nameText} - {textContent}");

        // Verify it shows "Wins the set!" text
        if (textContent?.Trim() != "Wins the set!")
        {
            throw new Exception($"Expected 'Wins the set!' but got '{textContent}'");
        }

        Console.WriteLine("✅ Player 2 set win validation successful!");

        // Close the winner popup on Player 1's page
        var closeButton1 = User1Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton2.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton2.ClickAsync();

        // Wait for popups to disappear on both pages
        await Task.Delay(500); // Brief delay for animation
        var isHidden1 = await winnerPopup1.IsHiddenAsync();
        var isHidden2 = await winnerPopup2.IsHiddenAsync();

        if (!isHidden1 || !isHidden2)
        {
            throw new Exception(
                "Winner popup did not close on both pages after clicking close buttons"
            );
        }
        Console.WriteLine("🚪 Winner popup closed successfully on both pages!");
    }

    private async Task ValidatePlayer2WonGame()
    {
        Console.WriteLine("🔍 Validating Player 2 won the game...");

        // Wait for the winner popup to appear on both pages
        var winnerPopup1 = User1Page.Locator("#winnerPopupOverlay");
        var winnerPopup2 = User2Page.Locator("#winnerPopupOverlay");

        await winnerPopup1.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );
        await winnerPopup2.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 10000 }
        );

        // Verify the popup is visible on both pages
        var isVisible1 = await winnerPopup1.IsVisibleAsync();
        var isVisible2 = await winnerPopup2.IsVisibleAsync();

        if (!isVisible1 || !isVisible2)
        {
            throw new Exception(
                "Winner popup is not visible on both pages after Player 2 won the game"
            );
        }

        // Check the winner name and text on Player 2's page
        var winnerName = User2Page.Locator(".winner-name");
        var winnerText = User2Page.Locator(".winner-text");

        await winnerName.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await winnerText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var nameText = await winnerName.TextContentAsync();
        var textContent = await winnerText.TextContentAsync();

        Console.WriteLine($"🏆 Winner popup shows: {nameText} - {textContent}");

        // Verify it shows "Wins the game!" text
        if (textContent?.Trim() != "Wins the game!")
        {
            throw new Exception($"Expected 'Wins the game!' but got '{textContent}'");
        }

        Console.WriteLine("✅ Player 2 game win validation successful!");

        // Close the winner popup on Player 1's page
        var closeButton1 = User1Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator("#winnerPopupOverlay .btn.btn-primary");
        await closeButton2.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton2.ClickAsync();

        // Wait for popups to disappear on both pages
        await Task.Delay(500); // Brief delay for animation
        var isHidden1 = await winnerPopup1.IsHiddenAsync();
        var isHidden2 = await winnerPopup2.IsHiddenAsync();

        if (!isHidden1 || !isHidden2)
        {
            throw new Exception(
                "Winner popup did not close on both pages after clicking close buttons"
            );
        }
        Console.WriteLine("🚪 Winner popup closed successfully on both pages!");
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
