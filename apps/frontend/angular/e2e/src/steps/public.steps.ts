import { Given, When, Then } from '@wdio/cucumber-framework';
import { PublicPage } from "../pages/public.page";
import { browser } from "@wdio/globals";
import { assert, expect } from 'chai'; // Import expect from the chai library or the appropriate Jasmine library.

let publicPage: PublicPage = new PublicPage();

When("I click on Privacy Policy", async () => {
    await publicPage.clickPp();
})

When("I click on Terms of Service", async () => {
    await publicPage.clickTos();
})

Given("The Privacy Policy page is opened", async () => {
    expect(await browser.getUrl()).to.be.equal('https://localhost:4200/privacy-policy');
})

Given("The Terms of Service page is opened", async () => {
    expect(await browser.getUrl()).to.be.equal('https://localhost:4200/terms-of-service');
})