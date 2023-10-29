import { Given, When, Then } from '@wdio/cucumber-framework';
import { OnboardingPage } from '../pages/onboarding.page';

let onboardingPage: OnboardingPage;

Given("the onboarding profile page is loaded", async () => {
    onboardingPage = new OnboardingPage();

    await onboardingPage.open();
    await onboardingPage.verify();
})

Given(/^I can fill in (.*) as my Nickname$/, async (name: string) => {
    await onboardingPage.inputNickname(name);
})

Given(/^I can fill in (.*) as my Email$/, async (email: string) => {
    await onboardingPage.inputEmail(email);
})

Given(/^I can select (.*) as my Country$/, async (country: string) => {
    await onboardingPage.inputCountry(country);
})