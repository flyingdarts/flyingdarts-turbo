import { $, browser } from '@wdio/globals'

import { BasePage } from "./base.page";
import { expect } from 'chai';

export class GamePage extends BasePage {
    readonly partialUrl = "/x01/638286987074415329";

    protected override getPartialUrl(): string {
        return this.partialUrl;
    }

    public get inputField() { return $('#calcInputField'); }
    public get playerNameField() { return $('#playerNameField'); }
    public get playerSetsField() { return $('#playerSetsField'); }
    public get playerLegsField() { return $('#playerLegsField'); }
    public get playerScoreField() { return $('#playerScoreField'); }
    public get playerHistoryField() { return $('#playerHistoryField'); }
    public get opponentNameField() { return $('#opponentNameField'); }
    public get opponentSetsField() { return $('#opponentSetsField'); }
    public get opponentLegsField() { return $('#opponentLegsField'); }
    public get opponentScoreField() { return $('#opponentScoreField'); }
    public get opponentHistoryField() { return $('#opponentHistoryField'); }
    public get calcButton_1() { return $('#calcButton1'); }
    public get calcButton_26() { return $('#calcButton26'); }
    public get calcButton_2() { return $('#calcButton2'); }
    public get calcButton_3() { return $('#calcButton3'); }
    public get calcButton_41() { return $('#calcButton41'); }
    public get calcButton_45() { return $('#calcButton45'); }
    public get calcButton_4() { return $('#calcButton4'); }
    public get calcButton_5() { return $('#calcButton5'); }
    public get calcButton_6() { return $('#calcButton6'); }
    public get calcButton_60() { return $('#calcButton60'); }
    public get calcButton_85() { return $('#calcButton85'); }
    public get calcButton_7() { return $('#calcButton7'); }
    public get calcButton_8() { return $('#calcButton8'); }
    public get calcButton_9() { return $('#calcButton9'); }
    public get calcButton_100() { return $('#calcButton100'); }
    public get calcButton_NOSCORE() { return $('#calcButtonNOSCORE'); }
    public get calcButton_0() { return $('#calcButton0'); }
    public get calcButton_OK() { return $('#calcButtonOK'); }
    public get calcButton_CLEAR() { return $('#calcButtonClear'); }
    public get calcButton_CHECK() { return $('#calcButtonCheck'); }

    async validateInputField(input: string) {
        await browser.waitUntil(async () => {
            return (await this.inputField.getText() == input)
        })
    }

    async validateInputFieldsAreDisabled() {
        await browser.waitUntil(async () => {
            return (
                !this.calcButton_1.isEnabled() &&
                !this.calcButton_26.isEnabled() &&
                !this.calcButton_2.isEnabled() &&
                !this.calcButton_3.isEnabled() &&
                !this.calcButton_41.isEnabled() &&
                !this.calcButton_45.isEnabled() &&
                !this.calcButton_4.isEnabled() &&
                !this.calcButton_5.isEnabled() &&
                !this.calcButton_6.isEnabled() &&
                !this.calcButton_60.isEnabled() &&
                !this.calcButton_85.isEnabled() &&
                !this.calcButton_7.isEnabled() &&
                !this.calcButton_8.isEnabled() &&
                !this.calcButton_9.isEnabled() &&
                !this.calcButton_100.isEnabled() &&
                !this.calcButton_NOSCORE.isEnabled() &&
                !this.calcButton_0.isEnabled() &&
                !this.calcButton_OK.isEnabled() &&
                !this.calcButton_CLEAR.isEnabled() &&
                !this.calcButton_CHECK.isEnabled())
        })
    }

    async validateShortcutsAreDisabled() {
        await browser.waitUntil(async () => {
            return (
                !this.calcButton_26.isEnabled() &&
                !this.calcButton_41.isEnabled() &&
                !this.calcButton_45.isEnabled() &&
                !this.calcButton_60.isEnabled() &&
                !this.calcButton_85.isEnabled() &&
                !this.calcButton_100.isEnabled());
        })
    }

    // Player fields
    async validatePlayerNameField(input: string) {
        await browser.waitUntil(async () => {
            return (await this.playerNameField.getText() == input);
        });
    }
    async validatePlayerSetsField(input: string) {
        await browser.waitUntil(async () => {
            return (await this.playerSetsField.getText() == input);
        });
    }
    async validatePlayerLegsField(input: string) {
        await browser.waitUntil(async () => {
            return (await this.playerLegsField.getText() == input);
        });
    }
    async validatePlayerScoreField(input: string) {
        await browser.waitUntil(async () => {
            return (await this.playerScoreField.getText() == input)
        })
    }
    async validatePlayerHistoryField(input: string) {
        await browser.waitUntil(async () => {
            return (await this.playerHistoryField.getText() == input);
        });
    }

    // Opponent fields
    async validateOpponentNameField(input: string) {
        var result = await this.opponentNameField.getText();
        expect(result).to.be.equal(input);
    }
    async validateOpponentSetsField(input: string) {
        var result = await this.opponentSetsField.getText();
        expect(result).to.be.equal(input);
    }
    async validateOpponentLegsField(input: string) {
        var result = await this.opponentLegsField.getText();
        expect(result).to.be.equal(input);
    }
    async validateOpponentScoreField(input: string) {
        var result = await this.opponentScoreField.getText();
        expect(result).to.be.equal(input);
    }
    async validateOpponentHistoryField(input: string) {
        var result = await this.opponentHistoryField.getText();
        expect(result).to.be.equal(input);
    }

    // Calculator buttons
    async clickCalcButton_26() {
        return await this.calcButton_26.click();
    }
    async clickCalcButton_1() {
        return await this.calcButton_1.click();
    }
    async clickCalcButton_2() {
        return await this.calcButton_2.click();
    }
    async clickCalcButton_3() {
        return await this.calcButton_3.click();
    }
    async clickCalcButton_41() {
        return await this.calcButton_41.click();
    }
    async clickCalcButton_45() {
        return await this.calcButton_45.click();
    }
    async clickCalcButton_4() {
        return await this.calcButton_4.click();
    }
    async clickCalcButton_5() {
        return await this.calcButton_5.click();
    }
    async clickCalcButton_6() {
        return await this.calcButton_6.click();
    }
    async clickCalcButton_60() {
        return await this.calcButton_60.click();
    }
    async clickCalcButton_85() {
        return await this.calcButton_85.click();
    }
    async clickCalcButton_7() {
        return await this.calcButton_7.click();
    }
    async clickCalcButton_8() {
        return await this.calcButton_8.click();
    }
    async clickCalcButton_9() {
        return await this.calcButton_9.click();
    }
    async clickCalcButton_100() {
        return await this.calcButton_100.click();
    }
    async clickCalcButton_NOSCORE() {
        return await this.calcButton_NOSCORE.click();
    }
    async clickCalcButton_0() {
        return await this.calcButton_0.click();
    }
    async clickCalcButton_OK() {
        return await this.calcButton_OK.click();
    }
    async clickCalcButton_CLEAR() {
        return await this.calcButton_CLEAR.click();
    }
    async clickCalcButton_CHECK() {
        return await this.calcButton_CHECK.click();
    }
}
