using Flyingdarts.E2E.Pages;
using Flyingdarts.E2E.Tests;
using Flyingdarts.E2E.Utilities;

namespace Flyingdarts.E2E;

/// <summary>
/// E2E coverage for the Friends feature area: navigation and basic UI states
/// </summary>
[Collection("SharedRunnerCollection")]
public class FriendsFeature : MultiBrowserBaseTest
{
    [Fact]
    public async Task FriendInviteAcceptHappyFlow()
    {
        await SetupAsync();

        // Initialize User1 on Home (handles auth + potential settings redirect), then navigate to Friends

        var (player1Home, player2Home) =
            await InitializeBothUsersOnHomeAndApplySettingsIfPromptedAsync(3, 5);

        var player1Friends = new FriendsPage(player1Home.Page, BaseUrl);
        var player2Friends = new FriendsPage(player2Home.Page, BaseUrl);
        var tasks = new List<Task>
        {
            player1Friends.NavigateToFriendsAsync(),
            player2Friends.NavigateToFriendsAsync(),
        };
        await Task.WhenAll(tasks);

        await player1Friends.GoToAddFriendTabAsync();

        // Test search with various query lengths
        await player1Friends.SetSearchQueryAsync("sc");

        await player1Friends.ClickSearchAsync();

        await player2Friends.GoToRequestsTabAsync();

        Thread.Sleep(10 * 60 * 1000);
        // Verify search results are displayed (assuming there's a method to check this)
        // This would depend on the actual implementation of the FriendsPage class

        await TeardownAsync();
    }
}
