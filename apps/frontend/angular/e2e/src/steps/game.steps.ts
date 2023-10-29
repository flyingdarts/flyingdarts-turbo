import { Given, When, Then } from '@wdio/cucumber-framework';
import { GamePage } from "../pages/game.page";

let gamePage: GamePage;

Given(/^the game page is loaded$/, { timeout: 10000 }, async () => {
    gamePage = new GamePage();

    await gamePage.open();
    // await gamePage.wait(5000);
    await gamePage.verify();
});

When(/^I press the shortcut '(.*)'$/, { timeout: 10000 }, async (value: string) => {
    switch (value) {
        case "26":
            await gamePage.clickCalcButton_26();
            break;
        case "41":
            await gamePage.clickCalcButton_41();
            break;
        case "45":
            await gamePage.clickCalcButton_45();
            break;
        case "60":
            await gamePage.clickCalcButton_60();
            break;
        case "85":
            await gamePage.clickCalcButton_85();
            break;
        case "100":
            await gamePage.clickCalcButton_100();
            break;
    }
    await gamePage.wait(400);
});

When(/^I press the number '(.*)'$/, async (value: string) => {
    switch (Number(value)) {
        case 1:
            await gamePage.clickCalcButton_1();
            break;
        case 2:
            await gamePage.clickCalcButton_2();
            break;
        case 3:
            await gamePage.clickCalcButton_3();
            break;
        case 4:
            await gamePage.clickCalcButton_4();
            break;
        case 5:
            await gamePage.clickCalcButton_5();
            break;
        case 6:
            await gamePage.clickCalcButton_6();
            break;
        case 7:
            await gamePage.clickCalcButton_7();
            break;
        case 8:
            await gamePage.clickCalcButton_8();
            break;
        case 9:
            await gamePage.clickCalcButton_9();
            break;
        case 0:
            await gamePage.clickCalcButton_0();
            break;
    }
});

When(/^I press OK$/, async () => {
    await gamePage.clickCalcButton_OK();
});

When(/^I press CLEAR$/, async () => {
    await gamePage.clickCalcButton_CLEAR();
});

Then(/^the input is '(.*)'$/, async (value: string) => {
    await gamePage.validateInputField(value);
});

Then(/^all input fields are disabled$/, async () => {
    await gamePage.validateInputFieldsAreDisabled()
});

Then(/^all shortcuts are disabled$/, async () => {
    await gamePage.validateShortcutsAreDisabled()
});

Then(/^the player name is '(.*)'$/, async (value: string) => {
    await gamePage.validatePlayerNameField(value);
});
Then(/^the player sets is '(.*)'$/, async (value: string) => {
    await gamePage.validatePlayerSetsField(value);
});
Then(/^the player legs is '(.*)'$/, async (value: string) => {
    await gamePage.validatePlayerLegsField(value);
});
Then(/^the player score is '(.*)'$/, async (value: string) => {
    await gamePage.validatePlayerScoreField(value);
});
Then(/^the player history is '(.*)'$/, async (value: string) => {
    await gamePage.validatePlayerHistoryField(value);
});

Then(/^the opponent name is '(.*)'$/, async (value: string) => {
    await gamePage.validateOpponentNameField(value);
});
Then(/^the opponent sets is '(.*)'$/, async (value: string) => {
    await gamePage.validateOpponentSetsField(value);
});
Then(/^the opponent legs is '(.*)'$/, async (value: string) => {
    await gamePage.validateOpponentLegsField(value);
});
Then(/^the opponent score is '(.*)'$/, async (value: string) => {
    await gamePage.validateOpponentScoreField(value);
});
Then(/^the opponent history is '(.*)'$/, async (value: string) => {
    await gamePage.validateOpponentHistoryField(value);
});