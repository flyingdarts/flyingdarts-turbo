import { browser } from '@wdio/globals'

export abstract class BasePage {
    protected abstract getPartialUrl(): string

    private getBaseUrl(): string {
        let baseUrl = "http://localhost:4200"

        if (baseUrl) {
            return String(baseUrl)
        }

        throw new Error('Unable to get base URL property from ')
    }

    public async open(): Promise<void> {
        // Perform client-side navigation
        await browser.url(this.getBaseUrl() + this.getPartialUrl())
    }

    public async verify(): Promise<void> {
        await browser.getUrl().then((currentUrl: string) => {
            if (!currentUrl.endsWith(this.getPartialUrl())) {
                throw Error('Current URL ' + currentUrl + ' does not end with ' + this.getPartialUrl());
            }
        })
    }

    public async wait(miliseconds: number): Promise<void> {
        await wait(miliseconds);
    }

}

function wait(ms: number): Promise<void> {
    return new Promise<void>(resolve => {
        setTimeout(resolve, ms);
    });
}