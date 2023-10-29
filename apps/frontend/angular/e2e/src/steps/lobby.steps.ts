import { Given, When, Then } from '@wdio/cucumber-framework';
import { LobbyPage } from '../pages/lobby.page';
import { expect } from 'chai';
import { browser } from "@wdio/globals";

let lobbyPage: LobbyPage;

Given(/^the lobby page is loaded$/, async () => {
    lobbyPage = new LobbyPage();

    await lobbyPage.open();
    await lobbyPage.verify();
})

Given(/^I can see (.*) in the nav-bar$/, async (userName: string) => {
    var currentNickname = await lobbyPage.getNickname();
    expect(currentNickname).to.be.equal(userName);
})

When("I click on the profile button", async () => {
    await lobbyPage.clickProfileButton();
    await lobbyPage.wait(2000);
})

When("I click on the settings button", async () => {
    await lobbyPage.clickSettingsButton();
})

When("I click on the play with friends button", async () => {
    await lobbyPage.clickGameWithFriends();
})

Then("the account/profile page is loaded", async () => {
    await browser.waitUntil(async () => {
        return expect(await browser.getUrl()).to.contain("profile")
    });
})

Then("the account/settings page is loaded", async () => {
    await browser.waitUntil(async() => {
        return expect(await browser.getUrl()).to.contain("settings")
    })
})