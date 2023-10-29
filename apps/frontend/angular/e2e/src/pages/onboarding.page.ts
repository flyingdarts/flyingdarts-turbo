import { BasePage } from "./base.page";
import { $ } from '@wdio/globals'

export class OnboardingPage extends BasePage {
    readonly partialUrl: string = "/onboarding/(onboarding-outlet:profile)"

    protected override getPartialUrl(): string {
        return this.partialUrl
    }

    public get nickNameField () { return $('#profileFormNickname'); }
    public get emailField () { return $('#profileFormEmailAddress'); }
    public get countryField () { return $('#profileFormCountry'); }


    async inputNickname(value: string) {
        await this.nickNameField.addValue(value);
    }

    async getNickname() {
        return await this.nickNameField.getText();
    }

    async inputEmail(value: string) {
        await this.emailField.addValue(value);
    }

    async getEmail() {
        return await this.emailField.getText();
    }

    async inputCountry(value: string) {
        // element(by.cssContainingText('option', value)).click();
    }

    async getCountry() {
        return await this.countryField.getText();
    }
}