import { BasePage } from "./base.page";

export class AccountProfilePage extends BasePage {
    readonly partialUrl = "/account/(account-outlet:profile)"
    protected override getPartialUrl(): string {
        return this.partialUrl;
    }
}