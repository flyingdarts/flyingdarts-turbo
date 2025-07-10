# TurboSetupWorkflow

Generated state machine for Workflow for setting up and configuring Turbo in a monorepo project

## Usage

```csharp
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<TurboSetupWorkflow>();
var actions = new TurboSetupWorkflowActions(); // Implement ITurboSetupWorkflowActions
var workflow = new TurboSetupWorkflow(logger, actions);

var result = await workflow.StartAsync();
```

## States

- **Initial**: Starting state of the workflow
- **VerifyingRequirements**: Checking if Turbo is installed and requirements are met
- **RequirementsMet**: Turbo requirements are satisfied
- **InstallingPackage**: Installing or updating Turbo package
- **UserPrompt**: Waiting for user input
- **AnalyzingWorkspace**: Analyzing workspace structure and package.json files
- **CheckingConfig**: Checking existing Turbo configuration
- **ConfigValid**: Turbo configuration is valid and up to date
- **ConfiguringTasks**: Configuring Turbo tasks and pipeline
- **CustomConfigPrompt**: Prompting user for custom configuration options
- **SetupComplete**: Turbo setup has been completed successfully
- **Error**: An error occurred during the setup process

## Events

- **start**: Start the workflow process
- **continue**: Continue to the next step
- **RequirementsVerified**: Turbo requirements have been verified
- **PackageInstalled**: Turbo package has been installed
- **ConfigVerified**: Turbo configuration has been verified
- **WorkspaceAnalyzed**: Workspace structure has been analyzed
- **TasksConfigured**: Turbo tasks have been configured
- **SetupCompleted**: Setup process has been completed
- **UserPrompted**: User has been prompted for input
- **ErrorOccurred**: An error occurred during the process

## Custom Actions

- **verify-turbo-installation**: Check if Turbo is installed globally or locally (returns bool)
- **verify-node-version**: Check if Node.js version is compatible with Turbo (returns bool)
- **install-turbo-package**: Install or update Turbo package (returns bool)
- **analyze-workspace-structure**: Analyze the workspace structure and detect packages (returns WorkspaceAnalysis)
- **detect-package-manager**: Detect the package manager being used (npm, yarn, pnpm) (returns string)
- **verify-turbo-config**: Verify that turbo.json exists and is valid (returns ConfigValidationResult)
- **configure-turbo-tasks**: Configure Turbo tasks based on workspace analysis (returns bool)
- **setup-caching**: Configure Turbo caching settings (returns bool)
- **configure-dependencies**: Configure task dependencies and pipeline (returns bool)
- **prompt-custom-config**: Prompt user for custom configuration options (returns CustomConfig)
- **display-usage-examples**: Display usage examples and next steps (returns void)
- **prompt-user**: Prompt user for input and wait for response (returns string)


Generated on: 2025-07-10 18:27:34
