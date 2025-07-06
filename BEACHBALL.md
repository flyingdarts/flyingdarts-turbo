# Beachball Version Management

This document explains how to use Beachball for version management in the Flyingdarts monorepo.

## What is Beachball?

Beachball is a tool for managing versioning and publishing in monorepos. It helps with:
- Automatic version bumping across multiple packages
- Changelog generation
- Coordinated publishing
- Release management

## Configuration

The Beachball configuration is in `beachball.config.js` and includes:

- **Packages**: All packages in `packages/`, `apps/backend/`, `apps/frontend/`, and `apps/tools/`
- **Exclusions**: Flutter apps, test files, build artifacts, and node_modules
- **Changelog Groups**: Separate changelogs for packages, backend, frontend, and tools
- **Hooks**: Pre/post build and test commands
- **Publishing**: Currently disabled (set `publish: true` when ready)

## Available Scripts

### Basic Beachball Commands
```bash
# Check for pending changes
npm run beachball:check

# Create a change file for a package
npm run beachball:change

# Bump versions based on change files
npm run beachball:bump

# Publish packages (when enabled)
npm run beachball:publish

# Sync dependencies between packages
npm run beachball:sync
```

### Release Commands
```bash
# Patch release (bug fixes)
npm run release:patch

# Minor release (new features)
npm run release:minor

# Major release (breaking changes)
npm run release:major

# Prerelease (beta versions)
npm run release:prerelease
```

## Workflow

### 1. Making Changes
When you make changes to packages:

1. **Create a change file**:
   ```bash
   # For individual packages
   npm run beachball:change
   ```
   This will prompt you to select:
   - Which packages changed (or group them automatically)
   - Type of change (patch, minor, major, prerelease)
   - Description of changes

2. **Commit your changes**:
   ```bash
   git add .
   git commit -m "feat: add new authentication feature"
   ```

### 2. Checking Status
Before releasing, check what changes are pending:
```bash
npm run beachball:check
```

### 3. Bumping Versions
When ready to release:
```bash
npm run beachball:bump
```
This will:
- Update version numbers in package.json files
- Generate changelogs
- Create git tags
- Run pre/post hooks (build, test)

### 4. Publishing (When Enabled)
To publish packages to npm:
```bash
npm run beachball:publish
```

## Change Types

- **patch**: Bug fixes (1.0.0 → 1.0.1)
- **minor**: New features (1.0.0 → 1.1.0)
- **major**: Breaking changes (1.0.0 → 2.0.0)
- **prerelease**: Beta/alpha versions (1.0.0 → 1.0.0-beta.1)

## Changelog Structure

Beachball generates separate changelogs for different areas:
- `packages/changelog.md` - Shared packages
- `apps/backend/changelog.md` - Backend services
- `apps/frontend/changelog.md` - Frontend applications
- `apps/tools/changelog.md` - Development tools

## Configuration Options

### Key Settings in beachball.config.js

```javascript
{
  branch: 'main',                    // Default branch
  packages: ['packages/**', ...],    // Packages to version
  ignore: ['apps/frontend/flutter/**'], // Packages to exclude
  bumpDeps: true,                    // Update dependencies
  generateChangelog: true,           // Generate changelogs
  publish: false,                    // Publishing enabled/disabled
  gitTags: true,                     // Create git tags
  push: true,                        // Push to remote
  hooks: {                           // Pre/post hooks
    prebump: 'npm run build-apis-prod',
    postbump: 'npm run test'
  }
}
```

## Best Practices

1. **Always create change files** for significant changes
2. **Use semantic versioning** appropriately
3. **Review changelogs** before publishing
4. **Test thoroughly** before releasing
5. **Coordinate releases** across teams

## Troubleshooting

### Common Issues

1. **Change files not found**: Run `npm run beachball:change` to create them
2. **Version conflicts**: Use `npm run beachball:sync` to resolve
3. **Build failures**: Check that all packages build before bumping
4. **Publishing errors**: Ensure you have npm permissions and `publish: true`

### Getting Help

- [Beachball Documentation](https://microsoft.github.io/beachball/)
- [GitHub Repository](https://github.com/microsoft/beachball)
- Check the generated changelogs for examples 