// @ts-check
/** @type {import('beachball').BeachballConfig} */

const config = {
  
  // Version bump strategy
  bumpDeps: true,
  generateChangelog: 'json',

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
