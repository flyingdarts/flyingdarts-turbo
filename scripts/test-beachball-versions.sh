#!/bin/bash

# Test Script for Beachball Version Validation
# Tests the differences between local and global beachball versions vs registry
# Usage: bash scripts/test-beachball-versions.sh [package_name] [--verbose]

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

# Main test function
main() {
    if [ "$VERBOSE" = true ]; then
        echo "🧪 Beachball Version Testing Script"
        echo "=================================="
        echo ""
    fi

    # Check if beachball is available
    if ! command_exists beachball; then
        log_error "Beachball is not installed globally"
        exit 1
    fi

    # Get versions from different sources
    if [ "$VERBOSE" = true ]; then
        log_step "Getting beachball versions from different sources..."
    fi

    # Get registry version
    local registry_version=""
    if registry_version=$(bash "$SCRIPT_DIR/get-latest-package-version-from-registry.sh" "$PACKAGE_NAME" 2>/dev/null); then
        log_success "Registry version: '$registry_version'"
    else
        log_error "Failed to get registry version"
        exit 1
    fi

    # Get project version
    local project_version=""
    if project_version=$(bash "$SCRIPT_DIR/get-package-version-from-project.sh" "$PACKAGE_NAME" 2>/dev/null); then
        log_success "Project version: '$project_version'"
    else
        log_warning "Failed to get project version"
    fi

    # Get global version
    local global_version=""
    if global_version=$(bash "$SCRIPT_DIR/get-package-version-from-global.sh" "$PACKAGE_NAME" 2>/dev/null); then
        log_success "Global version: '$global_version'"
    else
        log_error "Failed to get global version"
        exit 1
    fi

    echo ""

    # Compare versions (only the essential comparisons)
    if [ "$VERBOSE" = true ]; then
        log_step "Comparing versions..."
    fi

    # Essential comparison 1: Project vs Registry
    if [ -n "$project_version" ] && [ -n "$registry_version" ]; then
        test_version_comparison "$project_version" "$registry_version" "Project vs Registry"
    fi

    # Essential comparison 2: Global vs Registry
    if [ -n "$global_version" ] && [ -n "$registry_version" ]; then
        test_version_comparison "$global_version" "$registry_version" "Global vs Registry"
    fi

    # Only show additional comparisons in verbose mode
    if [ "$VERBOSE" = true ]; then
        # Additional comparison: Project vs Global
        if [ -n "$project_version" ] && [ -n "$global_version" ]; then
            test_version_comparison "$project_version" "$global_version" "Project vs Global"
        fi

        # Test different version formats
        log_step "Testing different version formats..."
        test_version_comparison "2.54.0" "^2.54.0" "Exact vs Range (^)"
        test_version_comparison "2.54.0" "~2.54.0" "Exact vs Range (~)"
        test_version_comparison "2.54.0" ">=2.54.0" "Exact vs Range (>=)"
        test_version_comparison "2.54.0" "2.54.0" "Exact vs Exact"
        test_version_comparison "2.54.0" "2.55.0" "Different versions"

        # Test with beachball command output
        log_step "Testing with actual beachball command outputs..."

        # Test beachball -v (local)
        local beachball_v_output=""
        if beachball_v_output=$(beachball -v 2>/dev/null | head -n1); then
            log_info "beachball -v output: '$beachball_v_output'"

            if [ -n "$global_version" ]; then
                test_version_comparison "$beachball_v_output" "$global_version" "beachball -v vs Global"
            fi
        else
            log_warning "Failed to get beachball -v output"
        fi

        # Test beachball --version
        local beachball_version_output=""
        if beachball_version_output=$(beachball --version 2>/dev/null | head -n1); then
            log_info "beachball --version output: '$beachball_version_output'"

            if [ -n "$global_version" ]; then
                test_version_comparison "$beachball_version_output" "$global_version" "beachball --version vs Global"
            fi
        else
            log_warning "Failed to get beachball --version output"
        fi

        echo ""
        log_success "Version testing completed! 🎉"
        echo ""
        log_info "Summary:"
        echo "- Registry version: ${registry_version:-"Not found"}"
        echo "- Project version: ${project_version:-"Not found"}"
        echo "- Global version: ${global_version:-"Not found"}"
        echo "- beachball -v: ${beachball_v_output:-"Not found"}"
        echo "- beachball --version: ${beachball_version_output:-"Not found"}"
    else
        log_success "Version testing completed! 🎉"
    fi
}

# Run main function
main "$@"
