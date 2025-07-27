#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

// List of all Flutter package.json files
const packageFiles = [
  'packages/frontend/flutter/authress-login/package.json',
  'packages/frontend/flutter/core/package.json', 
  'packages/frontend/flutter/features/splash/package.json',
  'packages/frontend/flutter/features/auth/package.json',
  'packages/frontend/flutter/features/dartboard/package.json',
  'packages/frontend/flutter/features/navigation/package.json',
  'packages/frontend/flutter/features/language/package.json',
  'packages/frontend/flutter/features/speech/package.json',
  'packages/frontend/flutter/features/keyboard/package.json',
  'packages/frontend/flutter/shared/ui/package.json',
  'packages/frontend/flutter/shared/configuration/preferences/package.json',
  'packages/frontend/flutter/shared/configuration/core/package.json',
  'packages/frontend/flutter/shared/configuration/secrets/package.json',
  'packages/frontend/flutter/shared/websocket/package.json',
  'packages/frontend/flutter/shared/internationalization/package.json',
  'packages/frontend/flutter/api/sdk/package.json',
  'apps/frontend/flutter/flyingdarts_mobile/package.json'
];

function addCleanScripts(packagePath) {
  try {
    // Read the package.json file
    const packageContent = fs.readFileSync(packagePath, 'utf8');
    const packageJson = JSON.parse(packageContent);
    
    // Check if scripts section exists
    if (!packageJson.scripts) {
      packageJson.scripts = {};
    }
    
    // Add clean scripts if they don't exist
    if (!packageJson.scripts.clean) {
      packageJson.scripts.clean = 'flutter clean && dart run build_runner clean';
    }
    
    if (!packageJson.scripts['clean:generated']) {
      packageJson.scripts['clean:generated'] = 'dart run build_runner clean';
    }
    
    // Write back to file with proper formatting
    fs.writeFileSync(packagePath, JSON.stringify(packageJson, null, 2) + '\n');
    console.log(`✅ Updated ${packagePath}`);
    
  } catch (error) {
    console.error(`❌ Error updating ${packagePath}:`, error.message);
  }
}

// Update all package files
console.log('🔧 Adding clean scripts to Flutter package.json files...\n');

let updated = 0;
let skipped = 0;

packageFiles.forEach(packagePath => {
  if (fs.existsSync(packagePath)) {
    addCleanScripts(packagePath);
    updated++;
  } else {
    console.log(`⚠️  File not found: ${packagePath}`);
    skipped++;
  }
});

console.log(`\n📊 Summary:`);
console.log(`   Updated: ${updated} files`);
console.log(`   Skipped: ${skipped} files`);
console.log('\n🎉 Done! All Flutter packages now have clean scripts.');
console.log('\nYou can now use:');
console.log('  npm run clean:flutter:generated  - Clean only generated files');
console.log('  npm run clean:flutter            - Full clean (flutter + generated)');
console.log('  turbo run clean:generated        - Clean generated files in all packages'); 