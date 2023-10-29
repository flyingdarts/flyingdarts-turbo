module.exports = function (config) {
  config.set({
    basePath: "",
    frameworks: ["jasmine", "@angular-devkit/build-angular"],
    plugins: [
      require("karma-jasmine"),
      require("karma-chrome-launcher"),
      require("karma-jasmine-html-reporter"),
      require("karma-webpack"),
      require("@angular-devkit/build-angular/plugins/karma"),
    ],
    client: {
      clearContext: false,
      zone: "ProxyZone"
    },
    reporters: ["progress"],
    logLevel: config.LOG_INFO,
    browsers: ["ChromeHeadless"],
    singleRun: true,
  });
};
