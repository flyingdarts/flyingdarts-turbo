#!/bin/bash
# prompt-user - Prompt user for input and wait for response
# Generated action script for BeachballSetupWorkflow

set -euo pipefail

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Logging functions
log_info() {
    echo -e "${BLUE}ℹ️ INFO:${NC} $1"
}

log_success() {
    echo -e "${GREEN}✅ SUCCESS:${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}⚠️ WARNING:${NC} $1"
}

log_error() {
    echo -e "${RED}❌ ERROR:${NC} $1"
}

# Function to validate user input
validate_input() {
    local input="$1"
    local question_type="$2"

    case "$question_type" in
    "upgrade_beachball" | "yes_no")
        if [[ "$input" =~ ^[Yy]$ ]]; then
            echo "yes"
            return 0
        elif [[ "$input" =~ ^[Nn]$ ]] || [ -z "$input" ]; then
            echo "no"
            return 0
        else
            return 1
        fi
        ;;
    "version_choice")
        if [[ "$input" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]] || [ "$input" = "latest" ]; then
            echo "$input"
            return 0
        else
            return 1
        fi
        ;;
    "installation_type")
        if [[ "$input" =~ ^(global|local|both)$ ]]; then
            echo "$input"
            return 0
        else
            return 1
        fi
        ;;
    *)
        # For any other question type, accept any non-empty input
        if [ -n "$input" ]; then
            echo "$input"
            return 0
        else
            return 1
        fi
        ;;
    esac
}

# Function to get question text based on type
get_question_text() {
    local question_type="$1"

    case "$question_type" in
    "upgrade_beachball")
        echo "⚠️  New version of beachball is available. Do you want to upgrade to the latest version? (y/N):"
        ;;
    "yes_no")
        echo "Do you want to continue? (y/N):"
        ;;
    "version_choice")
        echo "Enter the beachball version to install (or 'latest'):"
        ;;
    "installation_type")
        echo "Choose installation type (global/local/both):"
        ;;
    *)
        echo "Please provide your input:"
        ;;
    esac
}

# Function to get default value based on question type
get_default_value() {
    local question_type="$1"

    case "$question_type" in
    "upgrade_beachball" | "yes_no")
        echo "N"
        ;;
    "version_choice")
        echo "latest"
        ;;
    "installation_type")
        echo "both"
        ;;
    *)
        echo ""
        ;;
    esac
}

# Function to prompt user with retry logic
prompt_user_with_retry() {
    local question_type="$1"
    local max_attempts="${2:-3}"
    local question_text="$3"

    local attempt=1
    local user_input=""
    local default_value

    default_value=$(get_default_value "$question_type")

    while [ $attempt -le $max_attempts ]; do
        if [ -n "$question_text" ]; then
            echo -e "${YELLOW}$question_text${NC}"
        else
            echo -e "${YELLOW}$(get_question_text "$question_type")${NC}"
        fi

        # Show default value if available
        if [ -n "$default_value" ]; then
            echo -e "${BLUE}Default: $default_value${NC}"
        fi

        # Read user input
        read -p "> " user_input

        # Use default if input is empty
        if [ -z "$user_input" ] && [ -n "$default_value" ]; then
            user_input="$default_value"
            log_info "Using default value: $default_value"
        fi

        # Validate input
        if validated_input=$(validate_input "$user_input" "$question_type"); then
            echo "$validated_input"
            return 0
        else
            log_warning "Invalid input: '$user_input'"
            if [ $attempt -lt $max_attempts ]; then
                log_info "Please try again (attempt $attempt/$max_attempts)"
                echo ""
            fi
        fi

        attempt=$((attempt + 1))
    done

    log_error "Maximum attempts reached. Using default value: $default_value"
    echo "$default_value"
    return 1
}

# Function to handle different question types
handle_question() {
    local question_type="$1"
    local custom_question="$2"

    case "$question_type" in
    "upgrade_beachball")
        log_info "Prompting user for beachball upgrade decision..."
        result=$(prompt_user_with_retry "$question_type" 3 "$custom_question")
        if [ "$result" = "yes" ]; then
            log_success "User agreed to upgrade beachball"
            echo "yes"
        else
            log_info "User declined beachball upgrade"
            echo "no"
        fi
        ;;
    "yes_no")
        log_info "Prompting user for yes/no decision..."
        result=$(prompt_user_with_retry "$question_type" 3 "$custom_question")
        if [ "$result" = "yes" ]; then
            log_success "User answered: yes"
            echo "yes"
        else
            log_info "User answered: no"
            echo "no"
        fi
        ;;
    "version_choice")
        log_info "Prompting user for version choice..."
        result=$(prompt_user_with_retry "$question_type" 3 "$custom_question")
        log_success "User selected version: $result"
        echo "$result"
        ;;
    "installation_type")
        log_info "Prompting user for installation type..."
        result=$(prompt_user_with_retry "$question_type" 3 "$custom_question")
        log_success "User selected installation type: $result"
        echo "$result"
        ;;
    *)
        log_info "Prompting user for custom input..."
        result=$(prompt_user_with_retry "$question_type" 3 "$custom_question")
        log_success "User provided input: $result"
        echo "$result"
        ;;
    esac
}

echo "🔧 Executing prompt-user..."

# Get question type from environment or use default
question_type="${QUESTION_TYPE:-upgrade_beachball}"
custom_question="${CUSTOM_QUESTION:-}"

log_info "Question type: $question_type"

# Handle the question
if result=$(handle_question "$question_type" "$custom_question"); then
    # Store result in a way that can be read by the state machine
    echo "USER_RESPONSE=$result" >>/tmp/user_response
    echo "QUESTION_TYPE=$question_type" >>/tmp/user_response

    log_success "User interaction completed successfully"
    echo -e "${GREEN}✅ prompt-user completed successfully${NC}"
    echo "Response: $result"
    exit 0
else
    log_error "User interaction failed"
    echo -e "${RED}❌ prompt-user failed${NC}"
    exit 1
fi
