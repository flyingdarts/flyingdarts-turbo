# Flying Darts Scripts Collection

## Overview

The Flying Darts Scripts Collection is a comprehensive set of utility scripts designed to manage, maintain, and automate various aspects of the Flying Darts monorepo development workflow. This collection provides tools for workspace management, build system maintenance, package management, and development automation across multiple technology stacks including .NET, Flutter, and general workspace utilities.

The scripts are organized into logical categories and provide automated solutions for common development tasks such as cleaning build artifacts, managing dependencies, fixing workspace configurations, and maintaining code quality. Each script is designed to be cross-platform compatible and includes comprehensive error handling, user feedback, and safety measures.

## Script Categories

### 🧹 **Workspace Management**
- **clean-build-folders.sh**: Comprehensive workspace cleanup across all technology stacks
- **remove-csproj-bak-files.sh**: Remove backup files from .NET projects

### 🔧 **Package Management**
- **add-clean-scripts-to-package-json-files.js**: Add standardized clean scripts to Flutter package.json files

### 🎯 **Technology-Specific Scripts**

#### .NET Scripts (`dotnet/`)
- **Workspace Management**: Clean, restore, and fix .NET solutions
- **Code Generation**: Generate API clients using Microsoft Kiota
- **Build System**: Manage .NET build artifacts and dependencies

#### Flutter Scripts (`flutter/`)
- **Workspace Management**: Clean, upgrade, and manage Flutter packages
- **Package Management**: Upgrade dependencies and manage pubspec files
- **Build System**: Manage Flutter build artifacts and generated files
- **Fixes**: Fix package names, folder structures, and configurations

#### Utility Scripts (`utils/`)
- **Version Management**: Compare, extract, and manage package versions
- **Git Management**: Manage .gitignore files and repository structure

## Features

- **Cross-Platform Compatibility**: Works on macOS, Linux, and Windows (with WSL)
- **Comprehensive Error Handling**: Robust error handling with user-friendly messages
- **Safety Measures**: Confirmation prompts and backup creation for destructive operations
- **Progress Feedback**: Real-time progress indicators and colored output
- **Modular Design**: Organized by technology stack and functionality
- **Automation**: Streamlined workflows for common development tasks
- **Validation**: Input validation and environment checks
- **Documentation**: Comprehensive inline documentation and usage examples

## Prerequisites

### System Requirements
- **Bash Shell**: Most scripts require bash or POSIX-compliant shell
- **Node.js**: Required for JavaScript-based scripts (v14 or higher)
- **Git**: Required for version control operations
- **Cross-Platform Tools**: sed, find, grep, awk (available on most Unix-like systems)

### Technology-Specific Requirements

#### .NET Scripts
- **.NET SDK**: Version 6.0 or higher
- **Microsoft Kiota**: For API client generation
- **NuGet**: For package management

#### Flutter Scripts
- **Flutter SDK**: Version 3.26.0 or higher
- **Dart SDK**: Version 3.8.1 or higher
- **build_runner**: For code generation

#### Utility Scripts
- **npm/yarn**: For package version management
- **Git**: For repository operations

## Installation

### From Repository
1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd flyingdarts-turbo
   ```

2. **Make scripts executable**:
   ```bash
   chmod +x scripts/*.sh
   chmod +x scripts/*/*.sh
   chmod +x scripts/*/*/*.sh
   ```

3. **Install Node.js dependencies** (for JavaScript scripts):
   ```bash
   npm install
   ```

### Script Permissions
Ensure all scripts have execute permissions:
```bash
find scripts/ -name "*.sh" -exec chmod +x {} \;
find scripts/ -name "*.js" -exec chmod +x {} \;
```

## Usage

### Quick Start

#### Clean Workspace
```bash
# Clean all build artifacts across the entire workspace
./scripts/clean-build-folders.sh
```

#### Manage .NET Projects
```bash
# Clean .NET build artifacts
./scripts/dotnet/workspace/clean-dotnet.sh

# Restore .NET dependencies
./scripts/dotnet/workspace/restore-dotnet.sh

# Fix .NET solution structure
./scripts/dotnet/fixes/fix-dotnet-solution.sh
```

#### Manage Flutter Projects
```bash
# Clean Flutter build artifacts
./scripts/flutter/workspace/clean-flutter.sh

# Upgrade all Flutter packages
./scripts/flutter/upgrade-all-packages.sh

# Fix Flutter package names and folders
./scripts/flutter/fixes/fix-flutter-package-names-and-folders.sh
```

#### Generate API Clients
```bash
# Generate Dyte API client
./scripts/dotnet/generate-dyte-client.sh
```

### Advanced Usage

#### Batch Operations
```bash
# Clean everything and restore dependencies
./scripts/clean-build-folders.sh
./scripts/dotnet/workspace/restore-dotnet.sh
./scripts/flutter/workspace/clean-flutter.sh
```

#### Selective Operations
```bash
# Clean only .NET projects
./scripts/dotnet/workspace/clean-dotnet.sh --force

# Clean only Flutter projects
./scripts/flutter/workspace/clean-flutter.sh

# Remove only backup files
./scripts/remove-csproj-bak-files.sh
```

## Script Documentation

### 🧹 Workspace Management Scripts

#### clean-build-folders.sh
**Purpose**: Comprehensive workspace cleanup across all technology stacks

**Features**:
- Removes build artifacts from .NET, Flutter, Angular, and Rust projects
- Cleans package manager caches (npm, NuGet)
- Removes temporary and generated files
- Cross-platform compatibility

**Usage**:
```bash
./scripts/clean-build-folders.sh
```

**What it cleans**:
- `node_modules`, `dist`, `.turbo`, `.angular` (Node.js/Angular)
- `bin`, `obj` (.NET)
- `.dart_tool` (Flutter)
- `package-lock.json`, `pubspec.lock`, `Podfile.lock`
- Generated files and caches

#### remove-csproj-bak-files.sh
**Purpose**: Remove backup files from .NET projects

**Features**:
- Simple one-liner for removing `.csproj.bak` files
- Safe operation with no confirmation required

**Usage**:
```bash
./scripts/remove-csproj-bak-files.sh
```

### 🔧 Package Management Scripts

#### add-clean-scripts-to-package-json-files.js
**Purpose**: Add standardized clean scripts to Flutter package.json files

**Features**:
- Automatically adds clean scripts to all Flutter packages
- Creates consistent build commands across the workspace
- Supports build_runner integration

**Usage**:
```bash
node scripts/add-clean-scripts-to-package-json-files.js
```

**Added Scripts**:
- `clean`: Full clean (flutter clean + build_runner clean)
- `clean:generated`: Clean only generated files

### 🎯 .NET Scripts (`dotnet/`)

#### Workspace Management

##### clean-dotnet.sh
**Purpose**: Clean .NET build artifacts across the workspace

**Features**:
- Removes `bin` and `obj` folders from all .NET projects
- Workspace-aware (respects package.json workspaces)
- Confirmation prompts for safety
- Force mode for automation

**Usage**:
```bash
./scripts/dotnet/workspace/clean-dotnet.sh
./scripts/dotnet/workspace/clean-dotnet.sh --force
```

##### restore-dotnet.sh
**Purpose**: Restore .NET dependencies and rebuild solution

**Features**:
- Restores NuGet packages for all projects
- Rebuilds solution structure
- Comprehensive error handling
- Progress reporting

**Usage**:
```bash
./scripts/dotnet/workspace/restore-dotnet.sh
```

#### Code Generation

##### generate-dyte-client.sh
**Purpose**: Generate Dyte API client using Microsoft Kiota

**Features**:
- Generates type-safe API client from OpenAPI specification
- Configurable output path and namespace
- Automatic Kiota installation
- Clean output directory management

**Usage**:
```bash
./scripts/dotnet/generate-dyte-client.sh [spec-path] [output-path] [namespace] [class-name]
```

**Parameters**:
- `spec-path`: OpenAPI specification file (default: specs/dyte-api-spec-fixed.yaml)
- `output-path`: Output directory (default: packages/Flyingdarts.Meetings.Service/Generated/Dyte)
- `namespace`: C# namespace (default: Flyingdarts.Meetings.Service.Generated.Dyte)
- `class-name`: Generated class name (default: DyteApiClient)

#### Fixes

##### fix-dotnet-solution.sh
**Purpose**: Rebuild .NET solution file with all projects

**Features**:
- Automatically discovers all .csproj files in workspaces
- Creates new solution file with all projects
- Confirmation prompts for safety
- Workspace-aware project discovery

**Usage**:
```bash
./scripts/dotnet/fixes/fix-dotnet-solution.sh
```

### 🎯 Flutter Scripts (`flutter/`)

#### Workspace Management

##### clean-flutter.sh
**Purpose**: Clean Flutter build artifacts and generated files

**Features**:
- Removes build artifacts from all Flutter packages
- Cleans generated files (build_runner)
- Workspace-aware operation
- Comprehensive cleanup

**Usage**:
```bash
./scripts/flutter/workspace/clean-flutter.sh
```

##### upgrade-all-packages.sh
**Purpose**: Upgrade all Flutter packages in the workspace

**Features**:
- Interactive package upgrade with confirmation
- Workspace-aware package discovery
- Progress reporting and error handling
- Safe upgrade process

**Usage**:
```bash
./scripts/flutter/upgrade-all-packages.sh
```

#### Package Management

##### clean-pubspec-files.sh
**Purpose**: Clean pubspec files and manage dependencies

**Features**:
- Removes generated pubspec files
- Manages dependency overrides
- Workspace-aware operation
- Safe cleanup process

**Usage**:
```bash
./scripts/flutter/workspace/clean-pubspec-files.sh
```

##### update-pubspecs.sh
**Purpose**: Update pubspec files across the workspace

**Features**:
- Batch update of pubspec.yaml files
- Dependency version management
- Workspace-aware operation
- Backup creation

**Usage**:
```bash
./scripts/flutter/workspace/update-pubspecs.sh
```

#### Build System

##### clean-generated-files.sh
**Purpose**: Clean generated files from Flutter projects

**Features**:
- Removes build_runner generated files
- Cleans .dart_tool directories
- Workspace-aware operation
- Safe cleanup process

**Usage**:
```bash
./scripts/flutter/workspace/clean-generated-files.sh
```

##### update-build-commands.sh
**Purpose**: Update build commands in package.json files

**Features**:
- Adds build_runner commands to package.json
- Creates backups before modification
- Cross-platform compatibility
- Progress reporting

**Usage**:
```bash
./scripts/flutter/workspace/update-build-commands.sh
```

#### Fixes

##### fix-flutter-package-names-and-folders.sh
**Purpose**: Fix Flutter package names and folder structures

**Features**:
- Synchronizes package.json names with pubspec.yaml
- Fixes folder naming conventions
- Dry-run mode for preview
- Comprehensive validation

**Usage**:
```bash
./scripts/flutter/fixes/fix-flutter-package-names-and-folders.sh
./scripts/flutter/fixes/fix-flutter-package-names-and-folders.sh --dry-run
```

### 🛠️ Utility Scripts (`utils/`)

#### Version Management

##### compare-semver.sh
**Purpose**: Compare semantic versions

**Features**:
- Semantic version comparison
- Multiple version formats support
- Exit codes for automation
- Detailed comparison output

**Usage**:
```bash
./scripts/utils/compare-semver.sh version1 version2
```

##### get-package-version-from-global.sh
**Purpose**: Get package version from global installation

**Features**:
- Extracts version from global packages
- Multiple package manager support
- Error handling and validation
- Cross-platform compatibility

**Usage**:
```bash
./scripts/utils/get-package-version-from-global.sh package-name
```

##### get-package-version-from-project.sh
**Purpose**: Get package version from project dependencies

**Features**:
- Extracts version from project files
- Multiple project format support
- Workspace-aware operation
- Detailed version information

**Usage**:
```bash
./scripts/utils/get-package-version-from-project.sh package-name [project-path]
```

##### get-latest-package-version-from-registry.sh
**Purpose**: Get latest package version from registry

**Features**:
- Fetches latest version from npm registry
- Multiple registry support
- Caching for performance
- Error handling

**Usage**:
```bash
./scripts/utils/get-latest-package-version-from-registry.sh package-name
```

#### Git Management

##### merge-gitignores.sh
**Purpose**: Merge multiple .gitignore files

**Features**:
- Combines multiple .gitignore files
- Removes duplicates
- Maintains file structure
- Backup creation

**Usage**:
```bash
./scripts/utils/git/merge-gitignores.sh source1 source2 ... target
```

##### delete-extra-gitignores.sh
**Purpose**: Remove extra .gitignore files

**Features**:
- Removes duplicate .gitignore files
- Keeps only root .gitignore
- Safe operation with confirmation
- Workspace-aware operation

**Usage**:
```bash
./scripts/utils/git/delete-extra-gitignores.sh
```

## Configuration

### Environment Variables
Some scripts support environment variables for configuration:

```bash
# .NET SDK version
export DOTNET_VERSION="8.0"

# Flutter SDK path
export FLUTTER_ROOT="/path/to/flutter"

# Workspace root
export WORKSPACE_ROOT="/path/to/flyingdarts-turbo"
```

### Script Configuration
Scripts can be configured through command-line arguments:

```bash
# Force mode (skip confirmations)
./scripts/dotnet/workspace/clean-dotnet.sh --force

# Dry run mode (preview changes)
./scripts/flutter/fixes/fix-flutter-package-names-and-folders.sh --dry-run

# Verbose output
./scripts/clean-build-folders.sh --verbose
```

## Safety Features

### Confirmation Prompts
Destructive operations require user confirmation:
```bash
⚠️  This will permanently delete all bin and obj folders.
Are you sure you want to DELETE all .NET build folders? This cannot be undone! (yes/NO):
```

### Backup Creation
Scripts create backups before modifying files:
```bash
✅ Created backup: package.json.backup
✅ Updated package.json
```

### Dry Run Mode
Preview changes without applying them:
```bash
🔍 [DRY RUN] Would update: packages/frontend/flutter/ui/pubspec.yaml
🔍 [DRY RUN] Would change name from: ui to: flyingdarts_ui
```

### Error Handling
Comprehensive error handling with user-friendly messages:
```bash
❌ Error: .NET SDK not found
ℹ️  Please install .NET SDK from https://dotnet.microsoft.com/download
```

## Troubleshooting

### Common Issues

#### Permission Denied
```bash
# Make scripts executable
chmod +x scripts/*.sh
chmod +x scripts/*/*.sh
chmod +x scripts/*/*/*.sh
```

#### Script Not Found
```bash
# Ensure you're in the workspace root
cd /path/to/flyingdarts-turbo

# Run script with relative path
./scripts/clean-build-folders.sh
```

#### .NET SDK Not Found
```bash
# Install .NET SDK
# macOS: brew install dotnet
# Ubuntu: sudo apt-get install dotnet-sdk-8.0
# Windows: Download from https://dotnet.microsoft.com/download
```

#### Flutter SDK Not Found
```bash
# Install Flutter SDK
# macOS: brew install flutter
# Linux: Download from https://flutter.dev/docs/get-started/install/linux
# Windows: Download from https://flutter.dev/docs/get-started/install/windows
```

#### Node.js Not Found
```bash
# Install Node.js
# macOS: brew install node
# Ubuntu: sudo apt-get install nodejs npm
# Windows: Download from https://nodejs.org/
```

### Debug Mode
Enable debug output for troubleshooting:
```bash
# Set debug environment variable
export DEBUG=true

# Run script with debug output
./scripts/clean-build-folders.sh
```

### Log Files
Some scripts create log files for debugging:
```bash
# Check for log files
find . -name "*.log" -type f

# View recent logs
tail -f scripts.log
```

## Contributing

### Adding New Scripts
1. **Follow Naming Convention**: Use descriptive names with `.sh` or `.js` extension
2. **Add Documentation**: Include comprehensive inline documentation
3. **Error Handling**: Implement robust error handling and user feedback
4. **Safety Measures**: Add confirmation prompts for destructive operations
5. **Cross-Platform**: Ensure compatibility across different operating systems
6. **Testing**: Test scripts on different environments and scenarios

### Script Template
```bash
#!/bin/bash

# Script Name: description.sh
# Purpose: Brief description of what the script does
# Author: Your Name
# Date: YYYY-MM-DD

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Main script logic here
main() {
    print_info "Starting script..."
    
    # Your script logic here
    
    print_success "Script completed successfully!"
}

# Run main function
main "$@"
```

### Testing Guidelines
1. **Test on Multiple Platforms**: macOS, Linux, Windows (WSL)
2. **Test Error Scenarios**: Missing dependencies, invalid inputs, network issues
3. **Test Edge Cases**: Empty directories, large files, special characters
4. **Performance Testing**: Large workspaces, many files
5. **Integration Testing**: Script interactions and dependencies

## License

This scripts collection is part of the Flying Darts platform and is subject to the project's licensing terms.

## Support

For issues, questions, or contributions:
1. Check the troubleshooting section above
2. Review script documentation and inline comments
3. Test with debug mode enabled
4. Create an issue with detailed information about the problem
5. Include environment details and error messages