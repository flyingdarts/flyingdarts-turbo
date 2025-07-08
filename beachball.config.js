// @ts-check
/** @type {import('beachball').BeachballConfig} */

const config = {
  branch: "main",

  // Version bump strategy
  bumpDeps: true,
  generateChangelog: true,

  // Publishing configuration
  publish: false,
  access: "restricted",

  // Git configuration
  gitTags: true,
  push: false,

  // Tag configuration
  tag: "latest",
};

module.exports = config;
