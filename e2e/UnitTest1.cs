using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Flyingdarts.E2E;

public class UnitTest1 : PageTest
{
    [Fact]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
    }

    [Fact]
    public async Task GetStartedLink()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new() { Name = "Get started" }).ClickAsync();

        // Expects page to have a heading with the name of Installation.
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" }))
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task GetToken()
    {
        var cookie = new Cookie
        {
            Name = "authorization",
            Value = $"{Environment.GetEnvironmentVariable("AUTHRESS_TOKEN")}",
            Domain = "flyingdarts.net",
            Path = "/",
        };

        await Page.Context.AddCookiesAsync(new List<Cookie> { cookie });
        await Page.GotoAsync("https://staging.flyingdarts.net");

        // Click the get started link.
        await Page.GetByText("create room", new() { Exact = false }).ClickAsync();
    }
}
