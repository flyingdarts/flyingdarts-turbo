# Version Checking Scripts

This directory contains a set of modular scripts for checking package versions from different sources.

## Scripts Overview

### Core Scripts

1. **`utils.sh`** - Shared utilities and common functions

   - Logging functions with colored output
   - Command existence checking
   - Version comparison utilities
   - Bash environment validation
   - **Verbose flag support** (`--verbose` or `-v`)

2. **`get-latest-package-version-from-registry.sh`** - Get latest version from npm registry

   - Uses `npm view <package> version` command
   - Returns the latest available version from npm registry
   - Supports `--verbose` flag for detailed logging

3. **`get-package-version-from-project.sh`** - Get version from current project

   - Uses `npm ls <package> --depth=0` command
   - Falls back to parsing package.json
   - Returns the version installed in the current project
   - Supports `--verbose` flag for detailed logging

4. **`get-package-version-from-global.sh`** - Get version from global installation
   - Uses `npm ls <package> --depth=0 -g` command
   - Falls back to command version flags (-v, --version, -V)
   - Returns the globally installed version
   - Supports `--verbose` flag for detailed logging

### Test Scripts

5. **`test-beachball-versions.sh`** - Main test script (refactored)

   - Uses all three version getter scripts
   - **Focuses on essential comparisons**: Project vs Registry, Global vs Registry
   - Additional comparisons and tests only in verbose mode
   - Supports `--verbose` flag for detailed output

6. **`test-individual-scripts.sh`** - Test individual scripts
   - Tests each version getter script independently
   - Useful for debugging individual components
   - Supports `--verbose` flag for detailed output

### Setup Scripts

7. **`setup-beachball.sh`** - Beachball setup script (updated)
   - Now uses modular version checking scripts
   - Cleaner, more maintainable code
   - Comprehensive beachball installation and setup

## Usage

### Basic Usage (Minimal Output)

```bash
# Test essential version comparisons only
bash scripts/test-beachball-versions.sh

# Test all version sources for a specific package
bash scripts/test-beachball-versions.sh typescript

# Test individual scripts with minimal output
bash scripts/test-individual-scripts.sh

# Get version from registry only (minimal output)
bash scripts/get-latest-package-version-from-registry.sh

# Get version from project only (minimal output)
bash scripts/get-package-version-from-project.sh

# Get version from global installation only (minimal output)
bash scripts/get-package-version-from-global.sh
```

### Verbose Usage (Detailed Output)

```bash
# Test with detailed logging
bash scripts/test-beachball-versions.sh --verbose

# Test individual scripts with detailed logging
bash scripts/test-individual-scripts.sh --verbose

# Get version with detailed logging
bash scripts/get-latest-package-version-from-registry.sh --verbose
bash scripts/get-package-version-from-project.sh --verbose
bash scripts/get-package-version-from-global.sh --verbose
```

### Setup Usage

```bash
# Setup beachball using modular scripts
bash scripts/setup-beachball.sh
```

### Individual Script Usage

Each script can be used independently and accepts an optional package name parameter:

```bash
# Default (beachball)
bash scripts/get-latest-package-version-from-registry.sh

# Specific package
bash scripts/get-latest-package-version-from-registry.sh typescript

# With verbose flag
bash scripts/get-latest-package-version-from-registry.sh typescript --verbose
```

## Commands Used

As requested, the scripts use these specific npm commands:

1. **Registry version**: `npm view <package> version`
2. **Project version**: `npm ls <package> --depth=0`
3. **Global version**: `npm ls <package> --depth=0 -g`

## Verbose Flag

The `--verbose` (or `-v`) flag controls logging output:

### Default Mode (Non-Verbose)

- Shows only essential information
- Version fetching results
- Essential comparisons (Project vs Registry, Global vs Registry)
- Success/error messages only

### Verbose Mode

- Shows detailed step-by-step progress
- Debug information
- All version comparisons
- Additional tests and validations
- Detailed summary

## Features

- ✅ **Modular Design**: Each script has a single responsibility
- ✅ **Error Handling**: Comprehensive error checking and logging
- ✅ **Colored Output**: Clear visual feedback with emojis and colors
- ✅ **Bash Validation**: Ensures scripts run with bash, not sh
- ✅ **Strict Mode**: Uses `set -euo pipefail` for robust error handling
- ✅ **Reusable**: Scripts can be sourced or executed independently
- ✅ **Clean Code**: Well-documented with descriptive variable names
- ✅ **Verbose Control**: `--verbose` flag for detailed logging
- ✅ **Essential Focus**: Default mode shows only essential comparisons

## Dependencies

- `bash` (not `sh`)
- `npm` (for version checking commands)
- Optional: `scripts/compare-semver.sh` (for semantic version comparison)

## Error Handling

All scripts include comprehensive error handling:

- Command existence validation
- Network connectivity checks (for registry)
- File existence validation (for package.json)
- Graceful fallbacks for missing dependencies

## Logging

The scripts use a consistent logging system with:

- ℹ️ INFO: General information
- ✅ SUCCESS: Successful operations
- ⚠️ WARNING: Non-critical issues
- ❌ ERROR: Critical failures
- 🔍 DEBUG: Detailed debugging information (verbose only)
- 📋 STEP: Step-by-step progress (verbose only)

## Integration

The `setup-beachball.sh` script now uses the modular version checking scripts, making it:

- More maintainable
- Consistent with other scripts
- Easier to debug
- More reliable
