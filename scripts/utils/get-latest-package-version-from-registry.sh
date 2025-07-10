#!/bin/bash

# Script to get the latest package version from npm registry
# Usage: bash scripts/get-latest-package-version-from-registry.sh [package_name] [--verbose]

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

# Function to get latest version from npm registry
get_latest_package_version_from_registry() {
    local package_name="$1"

    log_debug "Getting latest version for package: $package_name"

    # Check if npm is available
    if ! command_exists npm; then
        log_error "npm is not installed or not in PATH"
        return 1
    fi

    # Get version from npm registry
    local version_output
    if version_output=$(npm view "$package_name" version 2>/dev/null); then
        # Clean up the output (remove any extra whitespace or newlines)
        local clean_version=$(echo "$version_output" | tr -d '\r\n' | xargs)

        if [ -n "$clean_version" ]; then
            if [ "$VERBOSE" = true ]; then
                log_success "Latest version from registry: '$clean_version'"
            fi
            echo "$clean_version"
            return 0
        else
            log_error "Empty version output from npm view"
            return 1
        fi
    else
        log_error "Failed to get version from npm registry for package: $package_name"
        return 1
    fi
}

# Main function
main() {
    if [ "$VERBOSE" = true ]; then
        log_info "Getting latest package version from registry for: $PACKAGE_NAME"
    fi

    local version
    if version=$(get_latest_package_version_from_registry "$PACKAGE_NAME"); then
        echo "$version"
        exit 0
    else
        log_error "Failed to get latest version from registry"
        exit 1
    fi
}

# Run main function if script is executed directly
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
