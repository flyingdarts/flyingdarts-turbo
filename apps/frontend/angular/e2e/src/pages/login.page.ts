import { BasePage } from './base.page';
import { $ } from '@wdio/globals'

export class LoginPage extends BasePage {
    readonly partialUrl: string = "/login"

    protected override getPartialUrl(): string {
        return this.partialUrl;
    }

    public get checkBox () { return $('#flexCheckChecked'); }
    public get loginButton () { return $('#loginButton'); }
    public get authenticator () { return $('#amplifyLogin'); }

    authenticatorLoaded(): boolean {
        return this.authenticator != null;
    }

    async clickCheckbox() {
        await this.checkBox.click();
    }

    async clickLoginButton() {
        await this.loginButton.click();
    }
    
    async buttonEnabled(): Promise<boolean> {
        return await this.loginButton.isEnabled();
    }

}