# Flutter Workspace Scripts

This directory contains scripts for managing the Flutter workspace.

## Scripts

### update-build-commands.sh

Updates the build command in every `package.json` file that's next to a `pubspec.yaml` file to include the build runner build command.

**What it does:**
- Finds all `pubspec.yaml` files in the workspace
- Checks if there's a `package.json` file in the same directory
- Updates the `build` script in each `package.json` to include the build runner command
- Creates backups of the original files before making changes

**Usage:**
```bash
# Run from the workspace root directory
bash scripts/flutter/workspace/update-build-commands.sh
```

**Before:**
```json
{
  "scripts": {
    "build": "flutter build"
  }
}
```

**After:**
```json
{
  "scripts": {
    "build": " dart run build_runner build && flutter build"
  }
}
```

**Features:**
- Cross-platform compatibility (works on both macOS and Linux)
- Creates backups of original files
- Colored output for better visibility
- Error handling and validation
- Progress reporting

**Requirements:**
- Must be run from the workspace root directory (where `turbo.json` is located)
- Requires bash shell
- Uses `sed` for file modifications (available on most Unix-like systems)

**Safety:**
- Creates `.backup` files before making changes
- Validates that the script is run from the correct directory
- Provides detailed output of what files are being updated 