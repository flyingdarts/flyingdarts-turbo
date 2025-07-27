#!/bin/bash

# Script to get package version from current project
# Usage: bash scripts/get-package-version-from-project.sh [package_name] [--verbose]

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

# Function to get package version from package.json
get_package_version_from_package_json() {
    local package_name="$1"

    if [ ! -f "package.json" ]; then
        log_error "package.json not found in current directory"
        return 1
    fi

    log_debug "Searching for $package_name version in package.json..."

    # Try to find package version in devDependencies first, then dependencies
    local version=""

    # Check devDependencies first (most common location)
    log_debug "Searching in devDependencies..."
    version=$(grep -A 20 '"devDependencies"' package.json | grep "\"$package_name\"" | sed -E "s/.*\"$package_name\":\\s*\"([^\"]*)\".*/\\1/")

    if [ -n "$version" ] && [ "$version" != "$package_name" ]; then
        if [ "$VERBOSE" = true ]; then
            log_success "Found $package_name version in devDependencies: '$version'"
        fi
        echo "$version"
        return 0
    fi

    # If not found in devDependencies, check dependencies
    log_debug "Searching in dependencies..."
    version=$(grep -A 20 '"dependencies"' package.json | grep "\"$package_name\"" | sed -E "s/.*\"$package_name\":\\s*\"([^\"]*)\".*/\\1/")
    if [ -n "$version" ] && [ "$version" != "$package_name" ]; then
        if [ "$VERBOSE" = true ]; then
            log_success "Found $package_name version in dependencies: '$version'"
        fi
        echo "$version"
        return 0
    fi

    log_warning "No $package_name version found in package.json"
    return 1
}

# Function to get package version from npm list
get_package_version_from_npm_list() {
    local package_name="$1"

    log_debug "Getting $package_name version from npm list..."

    # Check if npm is available
    if ! command_exists npm; then
        log_error "npm is not installed or not in PATH"
        return 1
    fi

    # Check if package.json exists
    if [ ! -f "package.json" ]; then
        log_error "package.json not found in current directory"
        return 1
    fi

    # Get version from npm list
    local npm_list_output
    if npm_list_output=$(npm ls "$package_name" --depth=0 2>/dev/null | grep "$package_name" | head -n1); then
        # Extract version from npm list output
        # Format is typically: └── package_name@version
        local version=$(echo "$npm_list_output" | sed -E "s/.*$package_name@([^[:space:]]*).*/\\1/")

        if [ -n "$version" ] && [ "$version" != "$package_name" ]; then
            if [ "$VERBOSE" = true ]; then
                log_success "Found $package_name version from npm list: '$version'"
            fi
            echo "$version"
            return 0
        else
            log_warning "Could not extract version from npm list output: '$npm_list_output'"
            return 1
        fi
    else
        log_warning "Failed to get $package_name version from npm list"
        return 1
    fi
}

# Function to get package version from project
get_package_version_from_project() {
    local package_name="$1"

    if [ "$VERBOSE" = true ]; then
        log_info "Getting $package_name version from project..."
    fi

    # Try npm list first (more accurate for installed packages)
    local version
    if version=$(get_package_version_from_npm_list "$package_name"); then
        echo "$version"
        return 0
    fi

    # Fallback to package.json parsing
    if version=$(get_package_version_from_package_json "$package_name"); then
        echo "$version"
        return 0
    fi

    log_error "Could not find $package_name version in project"
    return 1
}

# Main function
main() {
    if [ "$VERBOSE" = true ]; then
        log_info "Getting package version from project for: $PACKAGE_NAME"
    fi

    local version
    if version=$(get_package_version_from_project "$PACKAGE_NAME"); then
        echo "$version"
        exit 0
    else
        log_error "Failed to get package version from project"
        exit 1
    fi
}

# Run main function if script is executed directly
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
