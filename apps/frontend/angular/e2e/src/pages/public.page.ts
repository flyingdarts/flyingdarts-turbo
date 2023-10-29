import { BasePage } from "./base.page";
import { $ } from '@wdio/globals'

export class PublicPage extends BasePage {
    readonly partialUrl: string = ""

    protected override getPartialUrl(): string {
        return this.partialUrl;
    }

    public get tosField () { return $('#tos-link');}
    public get ppField () { return $('#pp-link');}

    async clickTos() {
        await this.tosField.click();
    }

    async clickPp() {
        await this.ppField.click();
    }
}