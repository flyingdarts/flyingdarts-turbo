using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Flyingdarts.Workflows.Turbo
{
    public class WorkflowResult
{
    public string CurrentState { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    
    public static WorkflowResult Success(string state, Dictionary<string, object> data = null)
    {
        return new WorkflowResult
        {
            CurrentState = state,
            IsCompleted = false,
            IsError = false,
            Data = data ?? new Dictionary<string, object>()
        };
    }
    
    public static WorkflowResult Completed(string state, Dictionary<string, object> data = null)
    {
        return new WorkflowResult
        {
            CurrentState = state,
            IsCompleted = true,
            IsError = false,
            Data = data ?? new Dictionary<string, object>()
        };
    }
    
    public static WorkflowResult Error(string state, string message)
    {
        return new WorkflowResult
        {
            CurrentState = state,
            IsCompleted = true,
            IsError = true,
            ErrorMessage = message
        };
    }
}
    
    public abstract class StateMachineBase
{
    protected string _currentState;
    protected bool _isCompleted;
    protected readonly Dictionary<string, object> _stateData = new Dictionary<string, object>();
    
    protected async Task<WorkflowResult> TransitionToAsync(string newState)
    {
        _currentState = newState;
        _isCompleted = false;
        
        // Execute the state
        var methodName = $"Execute{newState}StateAsync";
        var method = GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method != null)
        {
            return await (Task<WorkflowResult>)method.Invoke(this, null);
        }
        
        return WorkflowResult.Success(newState);
    }
    
    protected void SetStateData(string key, object value)
    {
        _stateData[key] = value;
    }
    
    protected T GetStateData<T>(string key, T defaultValue = default(T))
    {
        return _stateData.TryGetValue(key, out var value) ? (T)value : defaultValue;
    }
}
    
    public interface ITurboSetupWorkflow
{
    Task<WorkflowResult> StartAsync();
    Task<WorkflowResult> HandleEventAsync(string eventName, object data = null);
    string CurrentState { get; }
    bool IsCompleted { get; }
}

public interface ITurboSetupWorkflowActions
{
    Task<bool> VerifyTurboInstallationAsync();
    Task<bool> VerifyNodeVersionAsync();
    Task<bool> InstallTurboPackageAsync();
    Task<WorkspaceAnalysis> AnalyzeWorkspaceStructureAsync();
    Task<string> DetectPackageManagerAsync();
    Task<ConfigValidationResult> VerifyTurboConfigAsync();
    Task<bool> ConfigureTurboTasksAsync();
    Task<bool> SetupCachingAsync();
    Task<bool> ConfigureDependenciesAsync();
    Task<CustomConfig> PromptCustomConfigAsync();
    Task<void> DisplayUsageExamplesAsync();
    Task<string> PromptUserAsync(string question);
}
    
    public class TurboSetupWorkflow : StateMachineBase, ITurboSetupWorkflow
{
    // State constants
    public const string Initial = "initial";
    public const string VerifyingRequirements = "verifying-requirements";
    public const string RequirementsMet = "requirements-met";
    public const string InstallingPackage = "installing-package";
    public const string UserPrompt = "user-prompt";
    public const string AnalyzingWorkspace = "analyzing-workspace";
    public const string CheckingConfig = "checking-config";
    public const string ConfigValid = "config-valid";
    public const string ConfiguringTasks = "configuring-tasks";
    public const string CustomConfigPrompt = "custom-config-prompt";
    public const string SetupComplete = "setup-complete";
    public const string Error = "error";
    
    // Dependencies
    private readonly ILogger<{class_name}> _ilogger;
    private readonly ITurboSetupWorkflowActions _iturbosetupworkflowactions;
    
    public TurboSetupWorkflow(ILogger<{class_name}>, ITurboSetupWorkflowActions)
    {
        _ilogger = ilogger;
        _iturbosetupworkflowactions = iturbosetupworkflowactions;
    }
    
    public async Task<WorkflowResult> StartAsync()
    {
        _logger.LogInformation("🚀 Starting TurboSetupWorkflow");
        return await TransitionToAsync(Initial);
    }
    
    public async Task<WorkflowResult> HandleEventAsync(string eventName, object data = null)
    {
        _logger.LogInformation($"📨 Handling event: {eventName}");
        
        switch (CurrentState)
        {
            case Initial:
                case "start":
                    return await TransitionToAsync(VerifyingRequirements);
                break;
            case VerifyingRequirements:
                case "RequirementsVerified":
                    return await TransitionToAsync(RequirementsMet);
                case "RequirementsVerified":
                    return await TransitionToAsync(InstallingPackage);
                case "ErrorOccurred":
                    return await TransitionToAsync(Error);
                break;
            case RequirementsMet:
                case "continue":
                    return await TransitionToAsync(AnalyzingWorkspace);
                break;
            case InstallingPackage:
                case "PackageInstalled":
                    return await TransitionToAsync(AnalyzingWorkspace);
                case "UserPrompted":
                    return await TransitionToAsync(UserPrompt);
                case "ErrorOccurred":
                    return await TransitionToAsync(Error);
                break;
            case UserPrompt:
                case "UserPrompted":
                    return await TransitionToAsync(InstallingPackage);
                case "UserPrompted":
                    return await TransitionToAsync(AnalyzingWorkspace);
                break;
            case AnalyzingWorkspace:
                case "WorkspaceAnalyzed":
                    return await TransitionToAsync(CheckingConfig);
                case "ErrorOccurred":
                    return await TransitionToAsync(Error);
                break;
            case CheckingConfig:
                case "ConfigVerified":
                    return await TransitionToAsync(ConfigValid);
                case "ConfigVerified":
                    return await TransitionToAsync(ConfiguringTasks);
                case "ErrorOccurred":
                    return await TransitionToAsync(Error);
                break;
            case ConfigValid:
                case "continue":
                    return await TransitionToAsync(SetupComplete);
                break;
            case ConfiguringTasks:
                case "TasksConfigured":
                    return await TransitionToAsync(SetupComplete);
                case "UserPrompted":
                    return await TransitionToAsync(CustomConfigPrompt);
                case "ErrorOccurred":
                    return await TransitionToAsync(Error);
                break;
            case CustomConfigPrompt:
                case "UserPrompted":
                    return await TransitionToAsync(ConfiguringTasks);
                case "UserPrompted":
                    return await TransitionToAsync(SetupComplete);
                break;
            default:
                throw new InvalidOperationException($"Unknown state: {CurrentState}");
        }
    }
    
    public string CurrentState => _currentState;
    public bool IsCompleted => _isCompleted;
    
    private async Task<WorkflowResult> ExecuteInitialStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: Initial");
        
        // No actions to execute
        
        return await TransitionToAsync(VerifyingRequirements);
    }
    private async Task<WorkflowResult> ExecuteVerifyingRequirementsStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: VerifyingRequirements");
        
        _logger.LogInfo("🚀 Setting up Turbo in $pwd");
        _logger.LogInfo("🔄 Checking Turbo requirements");
        var result = await _actions.VerifyTurboInstallationAsync();
        var result = await _actions.VerifyNodeVersionAsync();
        
        if (turbo_installed)
        {
            return await TransitionToAsync(RequirementsMet);
        }
        if (turbo_not_found)
        {
            return await TransitionToAsync(InstallingPackage);
        }
        return await TransitionToAsync(Error);
    }
    private async Task<WorkflowResult> ExecuteRequirementsMetStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: RequirementsMet");
        
        _logger.LogSuccess("✅ Turbo requirements verified");
        
        return await TransitionToAsync(AnalyzingWorkspace);
    }
    private async Task<WorkflowResult> ExecuteInstallingPackageStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: InstallingPackage");
        
        _logger.LogInfo("💡 Installing/upgrading Turbo package");
        var result = await _actions.InstallTurboPackageAsync();
        
        return await TransitionToAsync(AnalyzingWorkspace);
        return await TransitionToAsync(UserPrompt);
        return await TransitionToAsync(Error);
    }
    private async Task<WorkflowResult> ExecuteUserPromptStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: UserPrompt");
        
        _logger.LogWarning("⚠️ New version of Turbo is available. Do you want to upgrade to the latest version? (y/N):");
        var userResponse = await _actions.PromptUserAsync("upgrade_turbo");
        
        if (user_agreed)
        {
            return await TransitionToAsync(InstallingPackage);
        }
        if (user_declined)
        {
            return await TransitionToAsync(AnalyzingWorkspace);
        }
    }
    private async Task<WorkflowResult> ExecuteAnalyzingWorkspaceStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: AnalyzingWorkspace");
        
        _logger.LogInfo("🔍 Analyzing workspace structure");
        var result = await _actions.AnalyzeWorkspaceStructureAsync();
        var result = await _actions.DetectPackageManagerAsync();
        
        return await TransitionToAsync(CheckingConfig);
        return await TransitionToAsync(Error);
    }
    private async Task<WorkflowResult> ExecuteCheckingConfigStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: CheckingConfig");
        
        _logger.LogInfo("📋 Checking Turbo configuration");
        var result = await _actions.VerifyTurboConfigAsync();
        
        if (config_exists_and_valid)
        {
            return await TransitionToAsync(ConfigValid);
        }
        if (config_missing_or_invalid)
        {
            return await TransitionToAsync(ConfiguringTasks);
        }
        return await TransitionToAsync(Error);
    }
    private async Task<WorkflowResult> ExecuteConfigValidStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: ConfigValid");
        
        _logger.LogSuccess("✅ Turbo configuration is valid");
        
        return await TransitionToAsync(SetupComplete);
    }
    private async Task<WorkflowResult> ExecuteConfiguringTasksStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: ConfiguringTasks");
        
        _logger.LogInfo("⚙️ Configuring Turbo tasks");
        var result = await _actions.ConfigureTurboTasksAsync();
        var result = await _actions.SetupCachingAsync();
        var result = await _actions.ConfigureDependenciesAsync();
        
        return await TransitionToAsync(SetupComplete);
        return await TransitionToAsync(CustomConfigPrompt);
        return await TransitionToAsync(Error);
    }
    private async Task<WorkflowResult> ExecuteCustomConfigPromptStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: CustomConfigPrompt");
        
        _logger.LogInfo("🎛️ Customize Turbo configuration options");
        var result = await _actions.PromptCustomConfigAsync();
        
        if (user_provided_config)
        {
            return await TransitionToAsync(ConfiguringTasks);
        }
        if (user_skipped_config)
        {
            return await TransitionToAsync(SetupComplete);
        }
    }
    private async Task<WorkflowResult> ExecuteSetupCompleteStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: SetupComplete");
        
        _logger.LogSuccess("✅ Turbo setup completed successfully! 🎉");
        _logger.LogInfo("🚀 You can now use 'turbo run <task>' to execute tasks across your monorepo");
        var result = await _actions.DisplayUsageExamplesAsync();
        
        return WorkflowResult.Completed(CurrentState);
    }
    private async Task<WorkflowResult> ExecuteErrorStateAsyncAsync()
    {
        _logger.LogInformation("Entering state: Error");
        
        _logger.LogError("❌ Turbo setup failed. Please check the error messages above.");
        _logger.LogInfo("💡 Try running 'turbo --version' to check your installation");
        
        return WorkflowResult.Completed(CurrentState);
    }
}
}