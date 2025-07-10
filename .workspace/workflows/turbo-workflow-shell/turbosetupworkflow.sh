#!/bin/bash
# TurboSetupWorkflow - Workflow for setting up and configuring Turbo in a monorepo project
# Generated State Machine Shell Script
# Version: 1.0.0
# Generated on: 2025-07-10 18:27:39

set -euo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# State constants
INITIAL="initial"
VERIFYING_REQUIREMENTS="verifying-requirements"
REQUIREMENTS_MET="requirements-met"
INSTALLING_PACKAGE="installing-package"
USER_PROMPT="user-prompt"
ANALYZING_WORKSPACE="analyzing-workspace"
CHECKING_CONFIG="checking-config"
CONFIG_VALID="config-valid"
CONFIGURING_TASKS="configuring-tasks"
CUSTOM_CONFIG_PROMPT="custom-config-prompt"
SETUP_COMPLETE="setup-complete"
ERROR="error"

# Global variables
CURRENT_STATE=""
IS_COMPLETED=false
ERROR_MESSAGE=""
STATE_DATA=()

# Helper functions
# Set state data
set_state_data() {
    local key="$1"
    local value="$2"
    STATE_DATA["$key"]="$value"
}

# Get state data
get_state_data() {
    local key="$1"
    echo "${STATE_DATA["$key"]:-}"
}

# Transition to a new state
transition_to() {
    local new_state="$1"
    echo -e "${BLUE}🔄 Transitioning to: $new_state${NC}"
    CURRENT_STATE="$new_state"
    IS_COMPLETED=false
}

# Complete the state machine
complete() {
    IS_COMPLETED=true
    echo -e "${GREEN}✅ State machine completed${NC}"
}

# Handle errors
handle_error() {
    local message="$1"
    ERROR_MESSAGE="$message"
    echo -e "${RED}❌ Error: $message${NC}"
    IS_COMPLETED=true
    exit 1
}

# State execution functions
# Execute Initial state
execute_initial_state() {
    echo "🔄 Entering state: Initial"
    
    # No actions to execute
    
    transition_to "verifying-requirements"
    return 0
}
# Execute VerifyingRequirements state
execute_verifying_requirements_state() {
    echo "🔄 Entering state: VerifyingRequirements"
    
    echo -e "${BLUE}ℹ️  🚀 Setting up Turbo in $(pwd)${NC}"
    echo -e "${BLUE}ℹ️  🔄 Checking Turbo requirements${NC}"
    # Execute custom action: verify-turbo-installation
    if [[ -f "actions/verify_turbo_installation.sh" ]]; then
        source "actions/verify_turbo_installation.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ verify-turbo-installation completed successfully${NC}"
        else
            echo -e "${RED}❌ verify-turbo-installation failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/verify_turbo_installation.sh${NC}"
    fi
    # Execute custom action: verify-node-version
    if [[ -f "actions/verify_node_version.sh" ]]; then
        source "actions/verify_node_version.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ verify-node-version completed successfully${NC}"
        else
            echo -e "${RED}❌ verify-node-version failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/verify_node_version.sh${NC}"
    fi
    
    if [[ "turbo_installed" == "true" ]]; then
        transition_to "requirements-met"
        return 0
    fi
    if [[ "turbo_not_found" == "true" ]]; then
        transition_to "installing-package"
        return 0
    fi
    transition_to "error"
    return 0
}
# Execute RequirementsMet state
execute_requirements_met_state() {
    echo "🔄 Entering state: RequirementsMet"
    
    echo -e "${GREEN}✅ ✅ Turbo requirements verified${NC}"
    
    transition_to "analyzing-workspace"
    return 0
}
# Execute InstallingPackage state
execute_installing_package_state() {
    echo "🔄 Entering state: InstallingPackage"
    
    echo -e "${BLUE}ℹ️  💡 Installing/upgrading Turbo package${NC}"
    # Execute custom action: install-turbo-package
    if [[ -f "actions/install_turbo_package.sh" ]]; then
        source "actions/install_turbo_package.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ install-turbo-package completed successfully${NC}"
        else
            echo -e "${RED}❌ install-turbo-package failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/install_turbo_package.sh${NC}"
    fi
    
    transition_to "analyzing-workspace"
    return 0
    transition_to "user-prompt"
    return 0
    transition_to "error"
    return 0
}
# Execute UserPrompt state
execute_user_prompt_state() {
    echo "🔄 Entering state: UserPrompt"
    
    echo -e "${YELLOW}⚠️  ⚠️ New version of Turbo is available. Do you want to upgrade to the latest version? (y/N):${NC}"
    read -p "⚠️  upgrade_turbo (y/N): " user_response
    if [[ "$user_response" =~ ^[Yy]$ ]]; then
        set_state_data "user_agreed" "true"
    else
        set_state_data "user_agreed" "false"
    fi
    
    if [[ "$(get_state_data "user_agreed")" == "true" ]]; then
        transition_to "installing-package"
        return 0
    fi
    if [[ "$(get_state_data "user_agreed")" == "false" ]]; then
        transition_to "analyzing-workspace"
        return 0
    fi
}
# Execute AnalyzingWorkspace state
execute_analyzing_workspace_state() {
    echo "🔄 Entering state: AnalyzingWorkspace"
    
    echo -e "${BLUE}ℹ️  🔍 Analyzing workspace structure${NC}"
    # Execute custom action: analyze-workspace-structure
    if [[ -f "actions/analyze_workspace_structure.sh" ]]; then
        source "actions/analyze_workspace_structure.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ analyze-workspace-structure completed successfully${NC}"
        else
            echo -e "${RED}❌ analyze-workspace-structure failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/analyze_workspace_structure.sh${NC}"
    fi
    # Execute custom action: detect-package-manager
    if [[ -f "actions/detect_package_manager.sh" ]]; then
        source "actions/detect_package_manager.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ detect-package-manager completed successfully${NC}"
        else
            echo -e "${RED}❌ detect-package-manager failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/detect_package_manager.sh${NC}"
    fi
    
    transition_to "checking-config"
    return 0
    transition_to "error"
    return 0
}
# Execute CheckingConfig state
execute_checking_config_state() {
    echo "🔄 Entering state: CheckingConfig"
    
    echo -e "${BLUE}ℹ️  📋 Checking Turbo configuration${NC}"
    # Execute custom action: verify-turbo-config
    if [[ -f "actions/verify_turbo_config.sh" ]]; then
        source "actions/verify_turbo_config.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ verify-turbo-config completed successfully${NC}"
        else
            echo -e "${RED}❌ verify-turbo-config failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/verify_turbo_config.sh${NC}"
    fi
    
    if [[ "config_exists_and_valid" == "true" ]]; then
        transition_to "config-valid"
        return 0
    fi
    if [[ "config_missing_or_invalid" == "true" ]]; then
        transition_to "configuring-tasks"
        return 0
    fi
    transition_to "error"
    return 0
}
# Execute ConfigValid state
execute_config_valid_state() {
    echo "🔄 Entering state: ConfigValid"
    
    echo -e "${GREEN}✅ ✅ Turbo configuration is valid${NC}"
    
    transition_to "setup-complete"
    return 0
}
# Execute ConfiguringTasks state
execute_configuring_tasks_state() {
    echo "🔄 Entering state: ConfiguringTasks"
    
    echo -e "${BLUE}ℹ️  ⚙️ Configuring Turbo tasks${NC}"
    # Execute custom action: configure-turbo-tasks
    if [[ -f "actions/configure_turbo_tasks.sh" ]]; then
        source "actions/configure_turbo_tasks.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ configure-turbo-tasks completed successfully${NC}"
        else
            echo -e "${RED}❌ configure-turbo-tasks failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/configure_turbo_tasks.sh${NC}"
    fi
    # Execute custom action: setup-caching
    if [[ -f "actions/setup_caching.sh" ]]; then
        source "actions/setup_caching.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ setup-caching completed successfully${NC}"
        else
            echo -e "${RED}❌ setup-caching failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/setup_caching.sh${NC}"
    fi
    # Execute custom action: configure-dependencies
    if [[ -f "actions/configure_dependencies.sh" ]]; then
        source "actions/configure_dependencies.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ configure-dependencies completed successfully${NC}"
        else
            echo -e "${RED}❌ configure-dependencies failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/configure_dependencies.sh${NC}"
    fi
    
    transition_to "setup-complete"
    return 0
    transition_to "custom-config-prompt"
    return 0
    transition_to "error"
    return 0
}
# Execute CustomConfigPrompt state
execute_custom_config_prompt_state() {
    echo "🔄 Entering state: CustomConfigPrompt"
    
    echo -e "${BLUE}ℹ️  🎛️ Customize Turbo configuration options${NC}"
    # Execute custom action: prompt-custom-config
    if [[ -f "actions/prompt_custom_config.sh" ]]; then
        source "actions/prompt_custom_config.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ prompt-custom-config completed successfully${NC}"
        else
            echo -e "${RED}❌ prompt-custom-config failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/prompt_custom_config.sh${NC}"
    fi
    
    if [[ "user_provided_config" == "true" ]]; then
        transition_to "configuring-tasks"
        return 0
    fi
    if [[ "user_skipped_config" == "true" ]]; then
        transition_to "setup-complete"
        return 0
    fi
}
# Execute SetupComplete state
execute_setup_complete_state() {
    echo "🔄 Entering state: SetupComplete"
    
    echo -e "${GREEN}✅ ✅ Turbo setup completed successfully! 🎉${NC}"
    echo -e "${BLUE}ℹ️  🚀 You can now use 'turbo run <task>' to execute tasks across your monorepo${NC}"
    # Execute custom action: display-usage-examples
    if [[ -f "actions/display_usage_examples.sh" ]]; then
        source "actions/display_usage_examples.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ display-usage-examples completed successfully${NC}"
        else
            echo -e "${RED}❌ display-usage-examples failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/display_usage_examples.sh${NC}"
    fi
    
    transition_to "$CURRENT_STATE"
}
# Execute Error state
execute_error_state() {
    echo "🔄 Entering state: Error"
    
    echo -e "${RED}❌ ❌ Turbo setup failed. Please check the error messages above.${NC}"
    echo -e "${BLUE}ℹ️  💡 Try running 'turbo --version' to check your installation${NC}"
    
    transition_to "$CURRENT_STATE"
}

# Main function
main() {
    echo -e "${BLUE}🚀 Starting TurboSetupWorkflow${NC}"
    
    # Initialize state machine
    transition_to "initial"
    
    # Main state machine loop
    while [[ "$IS_COMPLETED" != "true" ]]; do
        case "$CURRENT_STATE" in
            "initial")
                execute_initial_state
                ;;
            "verifying-requirements")
                execute_verifying_requirements_state
                ;;
            "requirements-met")
                execute_requirements_met_state
                ;;
            "installing-package")
                execute_installing_package_state
                ;;
            "user-prompt")
                execute_user_prompt_state
                ;;
            "analyzing-workspace")
                execute_analyzing_workspace_state
                ;;
            "checking-config")
                execute_checking_config_state
                ;;
            "config-valid")
                execute_config_valid_state
                ;;
            "configuring-tasks")
                execute_configuring_tasks_state
                ;;
            "custom-config-prompt")
                execute_custom_config_prompt_state
                ;;
            "setup-complete")
                execute_setup_complete_state
                ;;
            "error")
                execute_error_state
                ;;
            *)
                echo -e "${RED}❌ Unknown state: $CURRENT_STATE${NC}"
                exit 1
                ;;
        esac
    done
    
    echo -e "${GREEN}🎉 State machine completed successfully!${NC}"
}

# Script entry point
if [[ "${BASH_SOURCE[0]}" == "$0" ]]; then
    main "$@"
fi