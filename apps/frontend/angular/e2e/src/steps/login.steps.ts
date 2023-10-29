import { Given, When, Then } from '@wdio/cucumber-framework';
import { LoginPage } from '../pages/login.page';
import { browser } from "@wdio/globals";
import { expect } from 'chai';

let loginPage: LoginPage;

Given("the login page is loaded", async () => {
    loginPage = new LoginPage();
    
    await loginPage.open();
    await loginPage.verify();
});

When("I select the checkbox", async () => {
    await loginPage.clickCheckbox();
});

Then("I can login", async () => {
    expect(await loginPage.buttonEnabled()).to.be.true;
});

Then("I cannot login", async () => {
    expect(await loginPage.buttonEnabled()).to.be.false;
});

Given("I click the login with facebook button", async () => {
    await loginPage.clickLoginButton();
})

Given("The AWS Oauth window opens", async () => {
    var currentUrl = await browser.getUrl();
    expect(currentUrl).to.contain("amazoncognito.com")
})