#!/bin/bash

# Script to get package version from global installation
# Usage: bash scripts/get-package-version-from-global.sh [package_name] [--verbose]

# Source shared utilities
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/utils.sh"

# Initialize script
check_bash
set_strict_mode

# Parse command line arguments
parse_args "$@"

# Default package name (first argument after flags)
PACKAGE_NAME="${1:-beachball}"

# Function to get global package version from npm list
get_global_package_version_from_npm_list() {
    local package_name="$1"

    log_debug "Getting global $package_name version from npm list..."

    # Check if npm is available
    if ! command_exists npm; then
        log_error "npm is not installed or not in PATH"
        return 1
    fi

    # Get global version from npm list
    local npm_list_output
    if npm_list_output=$(npm ls "$package_name" --depth=0 -g 2>/dev/null | grep "$package_name" | head -n1); then
        # Extract version from npm list output
        # Format is typically: └── package_name@version
        local version=$(echo "$npm_list_output" | sed -E "s/.*$package_name@([^[:space:]]*).*/\\1/")

        if [ -n "$version" ] && [ "$version" != "$package_name" ]; then
            if [ "$VERBOSE" = true ]; then
                log_success "Found global $package_name version from npm list: '$version'"
            fi
            echo "$version"
            return 0
        else
            log_warning "Could not extract version from npm list output: '$npm_list_output'"
            return 1
        fi
    else
        log_warning "Failed to get global $package_name version from npm list"
        return 1
    fi
}

# Function to get global package version from command
get_global_package_version_from_command() {
    local package_name="$1"

    log_debug "Getting global $package_name version from command..."

    # Check if the package command exists
    if ! command_exists "$package_name"; then
        log_error "$package_name is not installed globally or not in PATH"
        return 1
    fi

    # Try different version flags
    local version_output=""

    # Try -v flag first
    if version_output=$("$package_name" -v 2>/dev/null | head -n1); then
        local clean_version=$(echo "$version_output" | tr -d '\r\n' | xargs)
        if [ -n "$clean_version" ]; then
            if [ "$VERBOSE" = true ]; then
                log_success "Found global $package_name version from command (-v): '$clean_version'"
            fi
            echo "$clean_version"
            return 0
        fi
    fi

    # Try --version flag
    if version_output=$("$package_name" --version 2>/dev/null | head -n1); then
        local clean_version=$(echo "$version_output" | tr -d '\r\n' | xargs)
        if [ -n "$clean_version" ]; then
            if [ "$VERBOSE" = true ]; then
                log_success "Found global $package_name version from command (--version): '$clean_version'"
            fi
            echo "$clean_version"
            return 0
        fi
    fi

    # Try -V flag (some packages use this)
    if version_output=$("$package_name" -V 2>/dev/null | head -n1); then
        local clean_version=$(echo "$version_output" | tr -d '\r\n' | xargs)
        if [ -n "$clean_version" ]; then
            if [ "$VERBOSE" = true ]; then
                log_success "Found global $package_name version from command (-V): '$clean_version'"
            fi
            echo "$clean_version"
            return 0
        fi
    fi

    log_warning "Could not get version from $package_name command"
    return 1
}

# Function to get global package version
get_package_version_from_global() {
    local package_name="$1"

    if [ "$VERBOSE" = true ]; then
        log_info "Getting $package_name version from global installation..."
    fi

    # Try npm list first (most reliable for npm packages)
    local version
    if version=$(get_global_package_version_from_npm_list "$package_name"); then
        echo "$version"
        return 0
    fi

    # Fallback to command version
    if version=$(get_global_package_version_from_command "$package_name"); then
        echo "$version"
        return 0
    fi

    log_error "Could not find global $package_name version"
    return 1
}

# Main function
main() {
    if [ "$VERBOSE" = true ]; then
        log_info "Getting package version from global installation for: $PACKAGE_NAME"
    fi

    local version
    if version=$(get_package_version_from_global "$PACKAGE_NAME"); then
        echo "$version"
        exit 0
    else
        log_error "Failed to get package version from global installation"
        exit 1
    fi
}

# Run main function if script is executed directly
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
