import { After, Before } from "@wdio/cucumber-framework";
import { browser } from "@wdio/globals";
require('dotenv').config({path: __dirname + '/.env'})

Before({ tags: "@requires-logged-in-user", timeout: 10 * 1000 }, async () => {
    await browser.setTimeout({ implicit: 5000, pageLoad: 10000 });
    await browser.setWindowSize(1920, 1080);
    await browser.url("http://localhost:4200");

    const pool = process.env.POOL;
    const user = process.env.USER;
    const signin = process.env.SIGNIN;
    const redirect = process.env.REDIRECT;
    const accessToken = process.env.ACCESS_TOKEN;
    const idToken = process.env.ID_TOKEN;
    const clockDrift = process.env.CLOCK_DRIFT;
    const refreshToken = process.env.REFRESH_TOKEN;
    const jsonString = process.env.USER_DATA;
    const userData = JSON.parse(JSON.parse(jsonString!))

    await browser.executeScript(`localStorage.setItem("amplify-signin-with-hostedUI", "${signin}");`, [])
    await browser.executeScript(`localStorage.setItem("amplify-redirected-from-hosted-ui", "${redirect}");`, [])
    await browser.executeScript(`localStorage.setItem("CognitoIdentityServiceProvider.${pool}.LastAuthUser","${user}");`, [])
    await browser.executeScript(`localStorage.setItem("CognitoIdentityServiceProvider.${pool}.${user}.accessToken","${accessToken}");`, [])
    await browser.executeScript(`localStorage.setItem("CognitoIdentityServiceProvider.${pool}.${user}.idToken","${idToken}");`, [])
    await browser.executeScript(`localStorage.setItem("CognitoIdentityServiceProvider.${pool}.${user}.clockDrift","${clockDrift}");`, [])
    await browser.executeScript(`localStorage.setItem("CognitoIdentityServiceProvider.${pool}.${user}.refreshToken","${refreshToken}");`, [])
    await browser.executeScript(`localStorage.setItem("CognitoIdentityServiceProvider.${pool}.${user}.userData",${JSON.stringify(userData)});`, [])
})

After({ tags: "@requires-logged-in-user" }, async () => {
    await browser.executeScript('localStorage.clear();', []);
})