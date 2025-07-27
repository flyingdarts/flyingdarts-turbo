# .NET Solution Management

This document explains how to manage the `flyingdarts-turbo.sln` file in this monorepo.

## Overview

The solution file is automatically generated based on the workspace structure defined in `package.json` and follows the schema outlined in `fd-v2.yml`. This approach ensures consistency and makes it easy to reorganize projects without manually managing solution files.

## Schema Structure

The project follows this directory structure as defined in `fd-v2.yml`:

```
flyingdarts-turbo/
├── apps/
│   ├── backend/
│   │   └── dotnet/          # .NET backend applications
│   │       ├── auth/        # Authentication services
│   │       ├── games/       # Game-related services
│   │       │   └── x01/     # X01 game services
│   │       ├── signalling/  # WebSocket signalling services
│   │       └── user/        # User management services
│   ├── frontend/
│   │   ├── angular/         # Angular frontend applications
│   │   └── flutter/         # Flutter mobile applications
│   └── tools/
│       └── dotnet/          # .NET tooling applications
│           └── infrastructure # Infrastructure as Code
├── packages/
│   ├── backend/
│   │   └── dotnet/          # .NET backend packages
│   │       ├── Flyingdarts.CDK.Constructs
│   │       ├── Flyingdarts.Meetings.Service
│   │       ├── Flyingdarts.Metadata.Services
│   │       └── Flyingdarts.Persistence
│   ├── frontend/
│   │   ├── angular/         # Angular frontend packages
│   │   └── flutter/         # Flutter packages
│   └── tools/
│       └── dotnet/          # .NET tooling packages
└── scripts/
    └── dotnet/              # .NET build and management scripts
```

## Workspace Configuration

The solution file is generated based on the workspaces defined in `package.json`:

```json
{
  "workspaces": [
    "packages/**/*",
    "apps/backend/**/*",
    "apps/frontend/**/*",
    "apps/tools/**/*"
  ]
}
```

## Managing the Solution File

### Automatic Generation

The `scripts/dotnet/restore-sln.sh` script automatically:

1. **Scans Workspaces**: Reads the workspace configuration from `package.json`
2. **Finds Projects**: Locates all `.csproj` files within the workspace directories
3. **Creates Solution**: Generates a new `flyingdarts-turbo.sln` file
4. **Adds Projects**: Automatically adds all found projects to the solution

### Reorganizing Projects

To reorganize your .NET projects:

1. **Organize on Disk**: Move your `.csproj` files to follow the schema in `fd-v2.yml`

   - Backend applications → `apps/backend/dotnet/`
   - Frontend applications → `apps/frontend/` (Angular/Flutter)
   - Tooling applications → `apps/tools/dotnet/`
   - Shared packages → `packages/backend/dotnet/` or `packages/frontend/`

2. **Delete Existing Solution**: Remove the current `flyingdarts-turbo.sln` file

3. **Regenerate**: Run the restore script from the root directory:
   ```bash
   sh ./scripts/dotnet/restore-sln.sh
   ```

### Example Workflow

```bash
# 1. Organize your projects according to the schema
mv MyOldProject/ apps/backend/dotnet/my-new-project/

# 2. Delete the existing solution
rm flyingdarts-turbo.sln

# 3. Regenerate the solution
sh ./scripts/dotnet/restore-sln.sh
```

## Benefits

- **Consistency**: All projects follow the same organizational structure
- **Automation**: No manual solution file management required
- **Scalability**: Easy to add new projects by placing them in the correct directory
- **Maintainability**: Clear separation between apps, packages, and tools
- **Version Alignment**: Each technology stack has its own version specification

## Script Features

The restore script includes:

- ✅ **POSIX Compatibility**: Works with any standard shell
- 🔍 **Workspace Scanning**: Only includes projects from configured workspaces
- ⚠️ **Safety Checks**: Confirms before deleting existing solution files
- 📊 **Progress Reporting**: Shows which projects are being added
- 🎯 **Error Handling**: Graceful failure with clear error messages

## Troubleshooting

### No .csproj Files Found

- Ensure your projects are placed within the workspace directories
- Check that `package.json` contains the correct workspace configuration

### Permission Issues

- Make sure the script is executable: `chmod +x scripts/dotnet/restore-sln.sh`
- Run from the repository root directory

### Duplicate Projects

- The script automatically removes duplicates
- Ensure projects are not nested within other project directories
