using System.Text.Json;
using System.Text.RegularExpressions;
using Authress.SDK;
using Authress.SDK.Client;
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
        // Services
        var accessKey =
            Environment.GetEnvironmentVariable("AUTHRESS_SERVICE_CLIENT_ACCESS_KEY")
            ?? throw new InvalidOperationException(
                "Missing env AUTHRESS_SERVICE_CLIENT_ACCESS_KEY"
            );

        var provider = new AuthressClientTokenProvider(accessKey);
        var token = await provider.GetBearerToken();

        // Assert token is not null or empty
        Assert.False(string.IsNullOrEmpty(token), "Token should not be null or empty");

        // Add token to browser context
        var localStorageToken = JsonSerializer.Serialize(
            new
            {
                idToken = token,
                expiry = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeMilliseconds(),
                jsCookies = true,
            }
        );

        // var script =
        //     $@"
        //     () => {{
        //         window.localStorage.setItem('AuthenticationCredentialsStorage', '{localStorageToken}');
        //     }}
        // ";

        // await Context.AddInitScriptAsync(script);

        // Alternative approach - more robust for complex tokens
        // await Page.EvaluateAsync(@"
        //     (token) => {
        //         window.localStorage.setItem('AuthenticationCredentialsStorage', token);
        //     }
        // ", localStorageToken);

        var cookie = new Cookie
        {
            Name = "authorization",
            Value = token,
            Domain = ".flyingdarts.net",
            Path = "/",
        };

        await Page.Context.AddCookiesAsync(new List<Cookie> { cookie });

        await Page.GotoAsync("https://staging.flyingdarts.net");

        // Set local storage AFTER page loads
        await Page.EvaluateAsync(
            @"
            (token) => {
                console.log('Setting local storage token:', token);
                window.localStorage.setItem('AuthenticationCredentialsStorage', token);
                console.log('Local storage set. Current value:', window.localStorage.getItem('AuthenticationCredentialsStorage'));
            }
        ",
            localStorageToken
        );

        // Debug: Verify the token was stored
        var storedToken = await Page.EvaluateAsync<string>(
            "() => window.localStorage.getItem('AuthenticationCredentialsStorage')"
        );
        Console.WriteLine($"Stored token: {storedToken}");

        Assert.False(string.IsNullOrEmpty(storedToken), "Token should be stored in local storage");

        await Page.GetByText("Start Gaming").ClickAsync();
    }
}
