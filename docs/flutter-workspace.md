# Flutter Workspace Setup

This repository uses Dart pub workspaces to manage the Flutter monorepo structure. This provides several benefits:

- **Single dependency resolution**: All packages share the same dependency versions
- **Improved performance**: Reduced memory usage for analysis
- **Easier management**: Centralized dependency management
- **Better IDE support**: Single analysis context for the entire workspace

## Workspace Structure

The workspace is configured in the root `pubspec.yaml` file and includes:

### Main App
- `apps/frontend/flutter/flyingdarts_mobile` - The main Flutter application

### Core Packages
- `packages/frontend/flutter/flyingdarts_frontend_flutter_core` - Core functionality

### Feature Packages
- `packages/frontend/flutter/features/flyingdarts_frontend_flutter_features_*` - Feature-specific packages

### Shared Packages
- `packages/frontend/flutter/shared/flyingdarts_frontend_flutter_shared_*` - Shared utilities and components

### Configuration Packages
- `packages/frontend/flutter/shared/configuration/flyingdarts_frontend_flutter_shared_configuration_*` - Configuration management

### API Packages
- `packages/frontend/flutter/api/sdk/flyingdarts_frontend_flutter_api_sdk` - API SDK

## Getting Started

### Prerequisites

- Dart SDK 3.6.0 or higher
- Flutter SDK 1.17.0 or higher

### Initial Setup

1. **Navigate to the workspace root** (repository root):
   ```bash
   cd /path/to/flyingdarts-turbo
   ```

2. **Get dependencies for all packages**:
   ```bash
   dart pub get
   ```

   This will:
   - Create a single `pubspec.lock` file at the root
   - Create a shared `.dart_tool/package_config.json`
   - Remove any existing `pubspec.lock` and `.dart_tool` files from individual packages

### Using the Workspace Management Script

We provide a convenient script to manage the workspace:

```bash
# Make sure you're in the workspace root
./scripts/flutter/workspace/workspace.sh <command>
```

#### Available Commands

- **`list`** - List all workspace packages
- **`get`** - Get dependencies for all packages
- **`clean`** - Clean all packages (remove build artifacts)
- **`analyze`** - Run `dart analyze` on all packages
- **`test`** - Run tests on all packages
- **`format`** - Format code in all packages
- **`status`** - Show workspace status
- **`run <cmd>`** - Run a command in all packages

#### Examples

```bash
# List all packages
./scripts/flutter/workspace/workspace.sh list

# Get dependencies
./scripts/flutter/workspace/workspace.sh get

# Analyze all packages
./scripts/flutter/workspace/workspace.sh analyze

# Run a custom command in all packages
./scripts/flutter/workspace/workspace.sh run 'flutter pub get'
```

## Working with Packages

### Adding Dependencies

To add a dependency to a specific package:

```bash
# Navigate to the package directory
cd packages/frontend/flutter/features/flyingdarts_frontend_flutter_features_auth

# Add the dependency
dart pub add package_name

# Or use the workspace script
cd /path/to/flyingdarts-turbo
dart pub -C packages/frontend/flutter/features/flyingdarts_frontend_flutter_features_auth add package_name
```

### Inter-package Dependencies

When one workspace package depends on another, it will automatically resolve to the local version:

```yaml
# In packages/frontend/flutter/features/flyingdarts_frontend_flutter_features_auth/pubspec.yaml
dependencies:
  flyingdarts_frontend_flutter_core: ^1.0.0
```

The workspace will automatically use the local version of `flyingdarts_frontend_flutter_core` instead of trying to fetch it from pub.dev.

### Publishing Packages

To publish a specific package:

```bash
# Navigate to the package directory
cd packages/frontend/flutter/features/flyingdarts_frontend_flutter_features_auth

# Publish the package
dart pub publish

# Or use the workspace script
cd /path/to/flyingdarts-turbo
dart pub -C packages/frontend/flutter/features/flyingdarts_frontend_flutter_features_auth publish
```

## Troubleshooting

### Dependency Conflicts

If you encounter dependency conflicts, the workspace will help you identify them early. You'll need to resolve version conflicts by updating the conflicting packages to compatible versions.

### Stray Files

When migrating to workspaces, existing `pubspec.lock` and `.dart_tool` files in package directories will be automatically removed. This is expected behavior.

### IDE Issues

If your IDE doesn't recognize the workspace:

1. Make sure you're opening the workspace root directory
2. Run `dart pub get` from the workspace root
3. Restart your IDE
4. Check that the `.dart_tool/package_config.json` file exists at the workspace root

### Temporary Independent Resolution

If you need to resolve a package independently (e.g., for testing):

1. Create a `pubspec_overrides.yaml` file in the package directory:
   ```yaml
   resolution:
   ```

2. Run `dart pub get` in that package directory
3. Remove the `pubspec_overrides.yaml` file when done

## Benefits of This Setup

1. **Consistent Dependencies**: All packages use the same versions of shared dependencies
2. **Faster Development**: Single dependency resolution reduces setup time
3. **Better IDE Performance**: Reduced memory usage and faster analysis
4. **Easier Maintenance**: Centralized dependency management
5. **Early Conflict Detection**: Dependency conflicts are caught early in development

## Migration Notes

If you're migrating from the old relative path setup:

1. The main app's `pubspec.yaml` has been updated to remove relative paths
2. All packages are now referenced by name only
3. The workspace automatically resolves local package references
4. Existing `pubspec.lock` and `.dart_tool` files in package directories will be removed

## References

- [Dart Pub Workspaces Documentation](https://dart.dev/tools/pub/workspaces)
- [Flutter Package Management](https://docs.flutter.dev/packages-and-plugins/using-packages) 