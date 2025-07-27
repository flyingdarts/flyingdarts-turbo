#!/usr/bin/env node

const fs = require('fs');

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

const correctCleanGenerated = "find lib/ -name '*.g.dart' -delete 2>/dev/null || find lib/ -name '*.freezed.dart' -delete 2>/dev/null || find lib/ -name '*.mocks.dart' -delete 2>/dev/null || find lib/ -name '*.config.dart' -delete 2>/dev/null || true";

function fixCleanScript(packagePath) {
  try {
    const packageContent = fs.readFileSync(packagePath, 'utf8');
    const packageJson = JSON.parse(packageContent);
    
    if (packageJson.scripts && packageJson.scripts['clean:generated']) {
      packageJson.scripts['clean:generated'] = correctCleanGenerated;
      fs.writeFileSync(packagePath, JSON.stringify(packageJson, null, 2) + '\n');
      console.log(`✅ Fixed ${packagePath}`);
      return true;
    } else {
      console.log(`⚠️  No clean:generated script found in ${packagePath}`);
      return false;
    }
    
  } catch (error) {
    console.error(`❌ Error fixing ${packagePath}:`, error.message);
    return false;
  }
}

console.log('🔧 Fixing clean:generated scripts to actually delete generated files...\n');

let fixed = 0;
packageFiles.forEach(packagePath => {
  if (fs.existsSync(packagePath) && fixCleanScript(packagePath)) {
    fixed++;
  }
});

console.log(`\n📊 Fixed ${fixed} package.json files`);
console.log('\n🎉 Now clean:generated will actually remove .g.dart, .freezed.dart, .mocks.dart, and .config.dart files!'); 