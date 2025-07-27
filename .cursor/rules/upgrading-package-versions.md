# Package Version Upgrade Rules

## Overview
When upgrading or downgrading package versions across the monorepo, follow these systematic steps to ensure consistency and avoid breaking changes.

## Pre-Upgrade Checklist

1. **Research the Package**
   - Check the package's changelog/release notes
   - Verify licensing changes (e.g., MediatR 13.0.0+ is licensed)
   - Review breaking changes and migration guides
   - Check compatibility with other packages in the solution

2. **Identify All Projects**
   - Search for all `.csproj` files containing the package reference
   - Use `grep_search` with pattern: `PackageReference Include="PackageName"`
   - Document all affected projects before making changes

## Upgrade Process

### Step 1: Test with One Project First
- Choose a representative project (preferably one with good test coverage)
- Update the package version in that project only
- Run `dotnet restore` and `dotnet build`
- Execute unit tests to verify functionality
- If successful, proceed to step 2

### Step 2: Systematic Update
- Update all identified projects with the new version
- Use `search_replace` tool for consistency
- Maintain proper indentation and formatting
- Update one project at a time and verify each change

### Step 3: Solution-Wide Verification
- Run `dotnet restore` at the solution root
- Build the entire solution: `dotnet build`
- Run tests across all projects
- Check for any compilation errors or warnings

## Special Considerations

### .NET Backend Projects
- Focus on `.csproj` files in `apps/backend/dotnet/` and `packages/backend/dotnet/`
- Pay attention to shared packages that may affect multiple projects
- Check for global usings that might reference the package

### Flutter Frontend Projects
- Update `pubspec.yaml` files in `apps/frontend/flutter/` and `packages/frontend/flutter/`
- Run `flutter pub get` after updates
- Check for breaking changes in Dart packages

### Package-Specific Rules

#### MediatR
- **Version 13.0.0+**: Licensed version - avoid unless explicitly required
- **Version 12.5.0**: Last free version - recommended for new projects
- Check for changes in handler registration patterns
- Verify `AddMediatR()` configuration still works

#### FluentValidation
- Check for changes in validation syntax
- Verify dependency injection extensions still work
- Test custom validators for breaking changes

#### Microsoft.Extensions.*
- Ensure all packages in the same family are compatible
- Check for changes in configuration patterns
- Verify dependency injection registrations

## Rollback Plan

If issues arise after upgrading:
1. Revert to the previous version in all projects
2. Run `dotnet restore` to restore previous packages
3. Document the specific issues encountered
4. Research alternative solutions or compatible versions

## Documentation

After successful upgrades:
- Update this rule with any new findings
- Document any breaking changes encountered
- Note any special configuration requirements
- Update project documentation if needed

## Commands Reference

```bash
# Find all projects using a specific package
grep_search "PackageReference Include=\"PackageName\"" --include="*.csproj"

# Restore packages after updates
dotnet restore

# Build entire solution
dotnet build

# Clean and rebuild if needed
dotnet clean && dotnet build
```

## Example: MediatR Downgrade (2024)
- **From**: Version 13.0.0 (licensed)
- **To**: Version 12.5.0 (free)
- **Reason**: Avoid licensing requirements
- **Projects Updated**: 8 .NET projects
- **Result**: Successful downgrade with no breaking changes