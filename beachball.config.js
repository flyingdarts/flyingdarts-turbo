// @ts-check
/** @type {import('beachball').BeachballConfig} */

const config = {
  // Version bump strategy
  bumpDeps: true,
  generateChangelog: true,

  // Publishing configuration
  publish: false,
  access: "restricted",

  // Git configuration
  gitTags: true,
  push: false,

  // Message configuration
  message: "chore: bump version to %s [skip ci]",

  // Tag configuration
  tag: "latest",
};

module.exports = config;
