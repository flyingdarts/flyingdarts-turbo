import { After } from '@wdio/cucumber-framework';

import { browser } from "@wdio/globals";
import path from 'path';

After(async (scenario) => {
    // Access the scenario name
    var screenshotName = scenario.pickle.name;

    // Modify scenario name
    screenshotName = screenshotName.replace(/[\/ ]/g, '_').toLowerCase();

    // Define path
    const screenshotPath = path.join(__dirname, '../../screenshots', `${screenshotName}.png`);
    
    await browser.saveScreenshot(screenshotPath);
})