using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Tests;
using Flyingdarts.E2E.Utilities;
using Microsoft.Playwright;

namespace Flyingdarts.E2E;

/// <summary>
/// Testing a game of darts between two players
/// In a logical order where who ever throws the first dart wins the game
/// This test throws perfect 9 darter legs each round.
/// </summary>
[Collection("SharedRunnerCollection")]
public class StarterAlwaysWins : MultiBrowserBaseTest
{
    /// <summary>
    /// Testing a game of darts between two players
    /// </summary>
    [Fact]
    public async Task StarterAlwaysWins_Scenario()
    {
        // Setup both users and navigate to home, applying settings if prompted
        await SetupAsync();
        var (player1Home, player2Home) =
            await InitializeBothUsersOnHomeAndApplySettingsIfPromptedAsync(3, 5);

        // Player 1 starts a new game
        await player1Home.StartNewGameAsync();

        // Wait for the game page to be ready
        await player1Home.WaitForGamePageReadyAsync();

        // Verify we're on a game page before extracting the ID
        var currentUrl = User1Page.Url;
        Console.WriteLine($"Current URL after starting game: {currentUrl}");

        if (!currentUrl.Contains(Constants.GameUrlSegment))
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
        await player2Home.NavigateToAsync($"{Constants.GameUrlSegment}{gameId}");

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
        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Init");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_1");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_2");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_3");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_4");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_5");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_6");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_7");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_8");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_9");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_10");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_11");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_12");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_13");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_14");

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

        await TakeBothUsersScreenshotsAsync("StarterAlwaysWins_Round_15");

        Console.WriteLine("🎯 Player 1 won the game!");

        await ValidatePlayer1WonGame();

        // Cleanup
        await TeardownAsync();
    }

    private async Task ValidatePlayer1WonLeg()
    {
        Console.WriteLine("🔍 Validating Player 1 won the leg...");

        // Wait for the winner popup to appear on both pages
        var winnerPopup1 = User1Page.Locator(Constants.WinnerPopupOverlaySelector);
        var winnerPopup2 = User2Page.Locator(Constants.WinnerPopupOverlaySelector);

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
        var winnerName = User1Page.Locator(Constants.WinnerNameSelector);
        var winnerText = User1Page.Locator(Constants.WinnerTextSelector);

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
        var closeButton1 = User1Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
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
        var winnerPopup1 = User1Page.Locator(Constants.WinnerPopupOverlaySelector);
        var winnerPopup2 = User2Page.Locator(Constants.WinnerPopupOverlaySelector);

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
        var winnerName = User1Page.Locator(Constants.WinnerNameSelector);
        var winnerText = User1Page.Locator(Constants.WinnerTextSelector);

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
        var closeButton1 = User1Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
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
        var winnerPopup1 = User1Page.Locator(Constants.WinnerPopupOverlaySelector);
        var winnerPopup2 = User2Page.Locator(Constants.WinnerPopupOverlaySelector);

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
        var winnerName = User1Page.Locator(Constants.WinnerNameSelector);
        var winnerText = User1Page.Locator(Constants.WinnerTextSelector);

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
        var closeButton1 = User1Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
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
        var winnerPopup1 = User1Page.Locator(Constants.WinnerPopupOverlaySelector);
        var winnerPopup2 = User2Page.Locator(Constants.WinnerPopupOverlaySelector);

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
        var winnerName = User2Page.Locator(Constants.WinnerNameSelector);
        var winnerText = User2Page.Locator(Constants.WinnerTextSelector);

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
        var closeButton1 = User1Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
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
        var winnerPopup1 = User1Page.Locator(Constants.WinnerPopupOverlaySelector);
        var winnerPopup2 = User2Page.Locator(Constants.WinnerPopupOverlaySelector);

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
        var winnerName = User2Page.Locator(Constants.WinnerNameSelector);
        var winnerText = User2Page.Locator(Constants.WinnerTextSelector);

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
        var closeButton1 = User1Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
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
        var winnerPopup1 = User1Page.Locator(Constants.WinnerPopupOverlaySelector);
        var winnerPopup2 = User2Page.Locator(Constants.WinnerPopupOverlaySelector);

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
        var winnerName = User2Page.Locator(Constants.WinnerNameSelector);
        var winnerText = User2Page.Locator(Constants.WinnerTextSelector);

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
        var closeButton1 = User1Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
        await closeButton1.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await closeButton1.ClickAsync();

        // Close the winner popup on Player 2's page
        var closeButton2 = User2Page.Locator(Constants.WinnerPopupPrimaryButtonSelector);
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
}
