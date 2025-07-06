#!/bin/bash

# Test script for individual version getter scripts
# Usage: bash scripts/test-individual-scripts.sh [package_name] [--verbose]

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

# Test function
test_script() {
    local script_name="$1"
    local description="$2"

    if [ "$VERBOSE" = true ]; then
        log_step "Testing $description"
    fi

    if [ -f "$SCRIPT_DIR/$script_name" ]; then
        if output=$(bash "$SCRIPT_DIR/$script_name" "$PACKAGE_NAME" 2>/dev/null); then
            log_success "$description: '$output'"
            return 0
        else
            log_warning "$description: Failed"
            return 1
        fi
    else
        log_error "Script not found: $script_name"
        return 1
    fi
}

# Main function
main() {
    if [ "$VERBOSE" = true ]; then
        echo "🧪 Testing Individual Version Getter Scripts"
        echo "==========================================="
        echo ""
        log_info "Testing package: $PACKAGE_NAME"
        echo ""
    fi

    # Test each script
    test_script "get-latest-package-version-from-registry.sh" "Registry Version"
    test_script "get-package-version-from-project.sh" "Project Version"
    test_script "get-package-version-from-global.sh" "Global Version"

    if [ "$VERBOSE" = true ]; then
        echo ""
    fi
    log_success "Individual script testing completed! 🎉"
}

# Run main function
main "$@"
