using Microsoft.Playwright;

namespace Flyingdarts.E2E.Pages;

/// <summary>
/// Page object for the Friends feature area (list, requests, add)
/// </summary>
public class FriendsPage : OptimizedBasePage
{
    private ILocator FriendsContainer =>
        GetCachedLocator(
            "friendsContainer",
            () => Page.Locator(Constants.FriendsContainerSelector)
        );

    private ILocator NavMyFriends =>
        GetCachedLocator(
            "navMyFriends",
            () => Page.GetByRole(AriaRole.Link, new() { Name = "My Friends" })
        );

    private ILocator NavRequests =>
        GetCachedLocator(
            "navRequests",
            () => Page.GetByRole(AriaRole.Link, new() { Name = "Requests" })
        );

    private ILocator NavAddFriend =>
        GetCachedLocator(
            "navAddFriend",
            () => Page.GetByRole(AriaRole.Link, new() { Name = "Add Friend" })
        );

    private ILocator AddFriendCtaButton =>
        GetCachedLocator(
            "addFriendCta",
            () => Page.GetByRole(AriaRole.Button, new() { Name = "Add Friend" })
        );

    private ILocator SearchInput =>
        GetCachedLocator(
            "searchInput",
            () => Page.GetByPlaceholder("Search for users by username or email address...")
        );

    private ILocator SearchButton =>
        GetCachedLocator(
            "searchButton",
            () => Page.GetByRole(AriaRole.Button, new() { Name = "Search" })
        );

    public FriendsPage(IPage page, string baseUrl = Constants.DefaultBaseUrl)
        : base(page, baseUrl) { }

    public override async Task WaitForPageReadyAsync()
    {
        await base.WaitForPageReadyAsync();
        await WaitForElementSmartAsync(
            () => Page.Locator(Constants.FriendsContainerSelector),
            "friendsContainer",
            Constants.DefaultHomePageTimeout
        );
    }

    public async Task NavigateToFriendsAsync()
    {
        await NavigateToAsync("friends");
        await WaitForPageReadyAsync();
    }

    public async Task GoToMyFriendsTabAsync()
    {
        await ClickElementSmartAsync(() => NavMyFriends, "navMyFriends");
    }

    public async Task GoToRequestsTabAsync()
    {
        await ClickElementSmartAsync(() => NavRequests, "navRequests");
    }

    public async Task GoToAddFriendTabAsync()
    {
        await ClickElementSmartAsync(() => NavAddFriend, "navAddFriend");
    }

    public async Task ClickAddFriendCtaAsync()
    {
        await ClickElementSmartAsync(() => AddFriendCtaButton, "addFriendCta");
    }

    public async Task<bool> IsOnFriendsPageAsync()
    {
        return await IsElementVisibleAsync(FriendsContainer);
    }

    public async Task<bool> IsMyFriendsHeaderVisibleAsync()
    {
        var header = Page.GetByRole(AriaRole.Heading, new() { Name = "My Friends" });
        return await IsElementVisibleAsync(header);
    }

    public async Task<bool> IsFriendRequestsHeaderVisibleAsync()
    {
        var header = Page.GetByRole(AriaRole.Heading, new() { Name = "Friend Requests" });
        return await IsElementVisibleAsync(header);
    }

    public async Task<bool> IsAddFriendHeaderVisibleAsync()
    {
        var header = Page.GetByRole(AriaRole.Heading, new() { Name = "Add Friend" });
        return await IsElementVisibleAsync(header);
    }

    public async Task SetSearchQueryAsync(string query)
    {
        await FillInputSmartAsync(() => SearchInput, "searchInput", query);
    }

    public async Task<bool> IsSearchButtonDisabledAsync()
    {
        return !(await SearchButton.IsEnabledAsync());
    }

    public async Task ClickSearchAsync()
    {
        await ClickElementSmartAsync(() => SearchButton, "searchButton");
    }
}
