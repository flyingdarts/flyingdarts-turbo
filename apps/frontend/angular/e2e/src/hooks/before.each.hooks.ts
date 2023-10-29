import { Before } from "@wdio/cucumber-framework";
import { browser } from "@wdio/globals";

Before(async () => {
    await browser.setTimeout({implicit: 5000, pageLoad: 10000});
    await browser.setWindowSize(1920, 1080);
    await browser.url("http://localhost:4200");
})
