# .NET Workspace Scripts

This directory contains scripts for managing .NET solutions and projects within the Flyingdarts workspace.

## Scripts

### `restore-dotnetsh`

A comprehensive script that restores the .NET solution and all projects within the workspace.

#### Features

- **Automatic .NET SDK detection**: Checks if .NET SDK is installed and reports the version
- **Solution restoration**: Restores the main `flyingdarts-turbo.sln` solution
- **Project restoration**: Restores all individual `.csproj` files as a backup/verification
- **Build artifact cleanup**: Removes previous `bin` and `obj` directories before restoration
- **Verification**: Checks that packages were restored correctly
- **Build verification**: Builds the solution to ensure everything works after restoration
- **Colored output**: Provides clear, colored status messages

#### Usage

```bash
# From the workspace root
./scripts/dotnet/workspace/restore-dotnetsh

# Or from any directory (script will navigate to workspace root)
/path/to/workspace/scripts/dotnet/workspace/restore-dotnetsh
```

#### What it does

1. **Checks prerequisites**: Verifies .NET SDK is installed
2. **Cleans artifacts**: Removes existing `bin` and `obj` directories
3. **Restores solution**: Runs `dotnet restore` on the main solution file
4. **Restores projects**: Runs `dotnet restore` on all individual projects
5. **Verifies restoration**: Checks for `project.assets.json` files
6. **Builds solution**: Runs `dotnet build` to ensure everything compiles

#### Requirements

- .NET SDK (version compatible with the projects)
- Bash shell
- Sufficient disk space for NuGet packages

#### Error Handling

The script will exit with error code 1 if:
- .NET SDK is not installed
- Solution file is not found
- Solution restoration fails
- Build verification fails

#### Output

The script provides colored output with different levels:
- 🔵 **INFO**: General information and progress
- 🟢 **SUCCESS**: Successful operations
- 🟡 **WARNING**: Non-critical issues
- 🔴 **ERROR**: Critical errors that cause script failure

## Project Structure

The workspace contains the following .NET projects:

### Apps (Backend)
- **Auth**: `apps/backend/dotnet/auth/`
- **Games**: `apps/backend/dotnet/games/`
- **Signalling**: `apps/backend/dotnet/signalling/`
- **User**: `apps/backend/dotnet/user/`

### Tools
- **CDK**: `apps/tools/dotnet/cdk/`

### Packages (Backend)
- **Connection Services**: `packages/backend/dotnet/Flyingdarts.Connection.Services/`
- **Meetings Service**: `packages/backend/dotnet/Flyingdarts.Meetings.Service/`
- **Metadata Services**: `packages/backend/dotnet/Flyingdarts.Metadata.Services/`
- **Persistence**: `packages/backend/dotnet/Flyingdarts.Persistence/`

### Packages (Tools)
- **CDK Constructs**: `packages/tools/dotnet/Flyingdarts.CDK.Constructs/`

## Central Package Management

The workspace uses central package version management via `Directory.Build.props` in the root directory. This ensures consistent package versions across all projects.

TODO: It really doesn't this needs another look ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^


## Troubleshooting

### Common Issues

1. **"dotnet command not found"**
   - Install .NET SDK from https://dotnet.microsoft.com/download
   - Ensure it's added to your PATH

2. **Restoration fails**
   - Check internet connection for NuGet package downloads
   - Verify NuGet sources are accessible
   - Try running `dotnet restore` manually on specific projects

3. **Build fails after restoration**
   - Check for missing dependencies
   - Verify target framework compatibility
   - Review build errors for specific project issues

### Manual Restoration

If the script fails, you can manually restore:

```bash
# Restore main solution
dotnet restore

# Restore specific project
dotnet restore path/to/project.csproj
``` 