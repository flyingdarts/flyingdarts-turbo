# BeachballSetupWorkflow

Generated state machine for Workflow for setting up and configuring Beachball in a project

## Usage

```bash
# Make the script executable
chmod +x beachballsetupworkflow.sh

# Run the state machine
./beachballsetupworkflow.sh

# Or with custom parameters
./beachballsetupworkflow.sh --debug --verbose
```

## States

- **Initial**: Starting state of the workflow
- **VerifyingRequirements**: Checking if Beachball is installed and requirements are met
- **RequirementsMet**: Beachball requirements are satisfied
- **InstallingPackage**: Installing or updating Beachball package
- **UserPrompt**: Waiting for user input
- **CheckingChangelog**: Verifying changelog files in workspace
- **SetupComplete**: Beachball setup has been completed successfully
- **Error**: An error occurred during the setup process

## Events

- **RequirementsVerified**: Beachball requirements have been verified
- **PackageInstalled**: Beachball package has been installed
- **ChangelogVerified**: Changelog files have been verified
- **SetupCompleted**: Setup process has been completed
- **UserPrompted**: User has been prompted for input
- **ErrorOccurred**: An error occurred during the process

## Custom Actions

- **verify-beachball-installation**: Check if Beachball is installed globally or locally (returns bool)
- **install-beachball-package**: Install or update Beachball package (returns bool)
- **verify-changelog-files**: Verify that changelog files exist and are valid (returns bool)
- **prompt-user**: Prompt user for input and wait for response (returns string)

## Action Scripts

The following action scripts are generated in the `actions/` directory:

- `actions/verify_beachball_installation.sh` - Check if Beachball is installed globally or locally
- `actions/install_beachball_package.sh` - Install or update Beachball package
- `actions/verify_changelog_files.sh` - Verify that changelog files exist and are valid
- `actions/prompt_user.sh` - Prompt user for input and wait for response

You can customize these scripts to implement your specific logic.

Generated on: 2025-07-06 18:50:09
