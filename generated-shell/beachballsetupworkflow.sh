#!/bin/bash
# BeachballSetupWorkflow - Workflow for setting up and configuring Beachball in a project
# Generated State Machine Shell Script
# Version: 1.0.0
# Generated on: 2025-07-06 18:50:09

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
CHECKING_CHANGELOG="checking-changelog"
SETUP_COMPLETE="setup-complete"
ERROR="error"

# Global variables
CURRENT_STATE=""
IS_COMPLETED=false
ERROR_MESSAGE=""
STATE_DATA=()

# Helper functions
# Set state data
set_state_data() {{
    local key="$1"
    local value="$2"
    STATE_DATA["$key"]="$value"
}}

# Get state data
get_state_data() {{
    local key="$1"
    echo "${{STATE_DATA["$key"]:-}}"
}}

# Transition to a new state
transition_to() {{
    local new_state="$1"
    echo -e "${{BLUE}}🔄 Transitioning to: $new_state${{NC}}"
    CURRENT_STATE="$new_state"
    IS_COMPLETED=false
}}

# Complete the state machine
complete() {{
    IS_COMPLETED=true
    echo -e "${{GREEN}}✅ State machine completed${{NC}}"
}}

# Handle errors
handle_error() {{
    local message="$1"
    ERROR_MESSAGE="$message"
    echo -e "${{RED}}❌ Error: $message${{NC}}"
    IS_COMPLETED=true
    exit 1
}}

# State execution functions
# Execute VerifyingRequirements state
execute_verifying_requirements_state() {
    echo "🔄 Entering state: VerifyingRequirements"
    
    echo -e "${BLUE}ℹ️  🚀 Setting up Beachball in $pwd${NC}"
    echo -e "${BLUE}ℹ️  🔄 Checking beachball requirements${NC}"
    # Execute custom action: verify-beachball-installation
    if [[ -f "actions/verify_beachball_installation.sh" ]]; then
        source "actions/verify_beachball_installation.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ verify-beachball-installation completed successfully${NC}"
        else
            echo -e "${RED}❌ verify-beachball-installation failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/verify_beachball_installation.sh${NC}"
    fi
    
    if [[ -n "$(command -v beachball)" ]]; then
        transition_to "requirements-met"
        return 0
    fi
    if [[ -z "$(command -v beachball)" ]]; then
        transition_to "installing-package"
        return 0
    fi
    transition_to "error"
    return 0
}

# Execute RequirementsMet state
execute_requirements_met_state() {
    echo "🔄 Entering state: RequirementsMet"
    
    echo -e "${GREEN}✅ ✅ Package requirements verified${NC}"
    
    transition_to "checking-changelog"
    return 0
}

# Execute InstallingPackage state
execute_installing_package_state() {
    echo "🔄 Entering state: InstallingPackage"
    
    echo -e "${BLUE}ℹ️  💡 Upgrading beachball package in (project/global)${NC}"
    # Execute custom action: install-beachball-package
    if [[ -f "actions/install_beachball_package.sh" ]]; then
        source "actions/install_beachball_package.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ install-beachball-package completed successfully${NC}"
        else
            echo -e "${RED}❌ install-beachball-package failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/install_beachball_package.sh${NC}"
    fi
    
    transition_to "checking-changelog"
    return 0
    transition_to "user-prompt"
    return 0
    transition_to "error"
    return 0
}
# Execute UserPrompt state
execute_user_prompt_state() {
    echo "🔄 Entering state: UserPrompt"
    
    echo -e "${YELLOW}⚠️  ⚠️ New version of beachball is available. Do you want to upgrade to the latest version? (y/N):${NC}"
    read -p "⚠️  upgrade_beachball (y/N): " user_response
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
        transition_to "checking-changelog"
        return 0
    fi
}
# Execute CheckingChangelog state
execute_checking_changelog_state() {
    echo "🔄 Entering state: CheckingChangelog"
    
    echo -e "${BLUE}ℹ️  🔄 Checking package installations${NC}"
    # Execute custom action: verify-changelog-files
    if [[ -f "actions/verify_changelog_files.sh" ]]; then
        source "actions/verify_changelog_files.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${GREEN}✅ verify-changelog-files completed successfully${NC}"
        else
            echo -e "${RED}❌ verify-changelog-files failed${NC}"
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Action script not found: actions/verify_changelog_files.sh${NC}"
    fi
    
    transition_to "setup-complete"
    return 0
    transition_to "error"
    return 0
}
# Execute SetupComplete state
execute_setup_complete_state() {
    echo "🔄 Entering state: SetupComplete"
    
    echo -e "${GREEN}✅ ✅ Setup completed successfully! 🎉${NC}"
    echo -e "${GREEN}✅ ✅ Verifying requirements are met${NC}"
    
    transition_to "$CURRENT_STATE"
}
# Execute Error state
execute_error_state() {
    echo "🔄 Entering state: Error"
    
    echo -e "${RED}❌ ❌ Setup failed. Please check the error messages above.${NC}"
    
    transition_to "$CURRENT_STATE"
}

# Main function
main() {
    echo -e "${BLUE}🚀 Starting BeachballSetupWorkflow${NC}"
    
    # Initialize state machine
    transition_to "initial"
    
    # Main state machine loop
    while [[ "$IS_COMPLETED" != "true" ]]; do
        case "$CURRENT_STATE" in
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
            "checking-changelog")
                execute_checking_changelog_state
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