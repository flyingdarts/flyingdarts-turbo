import { BasePage } from "./base.page";
import { $, browser } from '@wdio/globals'

export class LobbyPage extends BasePage {
    readonly partialUrl: string = "/lobby"

    protected override getPartialUrl(): string {
        return this.partialUrl
    }

    public get userNameField () { return $('#loggedInUserName'); }
    public get profileButtonField () { return $('#profileButton'); }
    public get settingsButtonField () { return $('#settingsButton'); }
    public get gameWithFriendsButton () { return $('#gameWithFriendsButton'); }

    async getNickname() {
        return await this.userNameField.getText();
    }

    async clickProfileButton() {
        return await this.profileButtonField.click();
    }

    async clickSettingsButton() {
        return await this.settingsButtonField.click();
    }

    async getUrl() {
        return await browser.getUrl();
    }

    async clickGameWithFriends() {
        return await this.gameWithFriendsButton.click();
    }
}