exports.config = {
    user: process.env.BROWSERSTACK_USERNAME,
    key: process.env.BROWSERSTACK_ACCESS_KEY,
    hostname: 'hub.browserstack.com',
    services: [
      [
        'browserstack',
        { browserstackLocal: true, opts: { forcelocal: false } },
      ],
    ],
    // add path to the test file
    specs: ['./e2e/src/features/lobby.feature'],
    framework: 'cucumber',
    reporters: ['spec'],

    //
    // If you are using Cucumber you need to specify the location of your step definitions.
    cucumberOpts: {
        // <string[]> (file/dir) require files before executing features
        require: ['./e2e/out/steps/**/*.steps.js', './e2e/out/hooks/*.hooks.js'],
        // <boolean> show full backtrace for errors
        backtrace: false,
        // <string[]> ("extension:module") require files with the given EXTENSION after requiring MODULE (repeatable)
        requireModule: [],
        // <boolean> invoke formatters without executing steps
        dryRun: false,
        // <boolean> abort the run on first failure
        failFast: false,
        // <boolean> hide step definition snippets for pending steps
        snippets: true,
        // <boolean> hide source uris
        source: true,
        // <boolean> fail if there are any undefined or pending steps
        strict: false,
        // <string> (expression) only execute the features or scenarios with tags matching the expression
        tagExpression: '',
        // <number> timeout for step definitions
        timeout: 60000,
        // <boolean> Enable this config to treat undefined definitions as warnings.
        ignoreUndefinedDefinitions: false
    },
    capabilities: [
      {
        browserName: 'Chrome',
        'bstack:options': {
          browserVersion: '103.0',
          os: 'Windows',
          osVersion: '11'
        }
      },
      {
        browserName: 'Firefox',
        'bstack:options': {
          browserVersion: '102.0',
          os: 'Windows',
          osVersion: '10'
        }
      },
      {
        browserName: 'Safari',
        'bstack:options': {
          browserVersion: '14.1',
          os: 'OS X',
          osVersion: 'Big Sur'
        }
      }
    ],
    commonCapabilities: {
      'bstack:options': {
        buildName: "bstack-demo",
        buildIdentifier: "${BUILD_NUMBER}",
        projectName: "Flyingdarts",
        debug: "true",
        networkLogs: "true",
        consoleLogs: "info"
      }
    },
    // rest of your config goes here...
  };
  exports.config.capabilities.forEach(function (caps) {
    for (let i in exports.config.commonCapabilities)
      caps[i] = { ...caps[i], ...exports.config.commonCapabilities[i]};
  });