import { BasePage } from "./base.page";

export class AccountSettingsPage extends BasePage {
    readonly partialUrl = "/account/(account-outlet:settings)"
    protected override getPartialUrl(): string {
        return this.partialUrl;
    }
}