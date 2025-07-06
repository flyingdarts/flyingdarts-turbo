#!/bin/bash

# Beachball Setup Script for Flyingdarts Monorepo
# Enhanced version with better logging, error handling, and modular version checking

# Check if running with bash
if [ -z "$BASH_VERSION" ]; then
    echo "❌ ERROR: This script must be run with bash, not sh"
    echo "   Please run: bash scripts/setup-beachball.sh"
    exit 1
fi

set -euo pipefail # Exit on error, undefined vars, pipe failures

# Source shared utilities
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/utils.sh"

# Script configuration
readonly SCRIPT_NAME="setup-beachball.sh"
readonly SCRIPT_VERSION="2.0.0"
readonly REQUIRED_TOOLS=("npm" "node")

# Function to validate required tools
validate_requirements() {
    log_step "Validating system requirements..."

    local missing_tools=()

    for tool in "${REQUIRED_TOOLS[@]}"; do
        if ! command_exists "$tool"; then
            missing_tools+=("$tool")
        else
            local version=$($tool --version 2>/dev/null | head -n1)
            log_debug "Found $tool: $version"
        fi
    done

    if [ ${#missing_tools[@]} -gt 0 ]; then
        log_error "Missing required tools: ${missing_tools[*]}"
        log_info "Please install Node.js and npm first: https://nodejs.org/"
        exit 1
    fi

    log_success "All required tools are available"
}

# Function to get beachball version from package.json using modular script
get_beachball_version_from_package_json() {
    log_info "🔍 Searching for beachball version in package.json..."

    if [ ! -f "package.json" ]; then
        log_warning "package.json not found in current directory"
        return 1
    fi

    # Use the modular script to get project version
    local version
    if version=$(bash "$SCRIPT_DIR/get-package-version-from-project.sh" "beachball" 2>/dev/null); then
        log_success "Found beachball version in package.json: '$version'"
        echo "$version"
        return 0
    else
        log_warning "No beachball version found in package.json"
        return 1
    fi
}

# Function to extract semver from version string (using external script)
extract_semver() {
    local version_string="$1"

    # Use the external semver comparison script to extract semver
    if [ -f "scripts/compare-semver.sh" ]; then
        # We'll use a temporary approach to extract semver by comparing with itself
        # and capturing the debug output
        local temp_output
        temp_output=$(bash scripts/compare-semver.sh "$version_string" "$version_string" 2>&1 || true)

        # Extract the clean version from debug output
        local clean_version=$(echo "$temp_output" | grep "→ Result: extracted semver" | sed 's/.*→ Result: extracted semver '\''\([^'\'']*\)'\''.*/\1/')

        if [ -n "$clean_version" ]; then
            echo "$clean_version"
            return 0
        fi
    fi

    # Fallback: simple extraction for basic cases
    version_string=$(echo "$version_string" | tr -d '[:space:]' | sed -E 's/(beachball)//Ig')

    if [ -z "$version_string" ] || [ "$version_string" = "null" ]; then
        echo ""
        return 1
    fi

    # Handle semver ranges
    if [[ "$version_string" == ^* || "$version_string" == ~* ]]; then
        local clean_version=$(echo "$version_string" | sed -E 's/^[\^~]//')
        echo "$clean_version"
        return 0
    fi
    if [[ "$version_string" == '>'* || "$version_string" == '<'* || "$version_string" == '='* ]]; then
        local clean_version=$(echo "$version_string" | sed -E 's/^[><=]+//')
        echo "$clean_version"
        return 0
    fi

    # Handle clean semver
    if [[ "$version_string" =~ ^([0-9]+\.[0-9]+\.[0-9]+) ]]; then
        echo "${BASH_REMATCH[1]}"
        return 0
    fi

    echo "$version_string"
    return 1
}

# Function to check if beachball is installed globally using modular script
check_global_beachball_installation() {
    log_info "🔍 Checking global beachball installation..."

    if ! command_exists beachball; then
        log_warning "Global beachball not found"
        return 1
    fi

    # Use the modular script to get global version
    local version_string
    if version_string=$(bash "$SCRIPT_DIR/get-package-version-from-global.sh" "beachball" 2>/dev/null); then
        log_success "Global beachball found: version '$version_string'"
        echo "$version_string"
        return 0
    else
        log_warning "Failed to get global beachball version"
        return 1
    fi
}

# Function to check if beachball is installed locally using modular script
check_local_beachball_installation() {
    log_info "🔍 Checking local beachball installation..."

    if [ ! -f "package.json" ]; then
        log_warning "package.json not found, cannot check local installation"
        return 1
    fi

    # Use the modular script to get project version
    local version_string
    if version_string=$(bash "$SCRIPT_DIR/get-package-version-from-project.sh" "beachball" 2>/dev/null); then
        log_success "Local beachball version from package.json: '$version_string'"
        echo "$version_string"
        return 0
    else
        log_warning "No beachball version found in package.json"
        return 1
    fi
}

# Function to compare versions using external script
compare_versions() {
    local version1="$1"
    local version2="$2"

    log_debug "⚖️  Comparing versions: '$version1' vs '$version2'"

    # Use external semver comparison script if available
    if [ -f "scripts/compare-semver.sh" ]; then
        if bash scripts/compare-semver.sh "$version1" "$version2" >/dev/null 2>&1; then
            log_debug "   → Result: versions are equal ✅"
            return 0
        else
            log_debug "   → Result: versions are different ❌"
            return 1
        fi
    fi

    # Fallback: simple string comparison
    if [ "$version1" = "$version2" ]; then
        log_debug "   → Result: versions are equal ✅"
        return 0
    else
        log_debug "   → Result: versions are different ❌"
        return 1
    fi
}

# Function to prompt user for upgrade
prompt_for_upgrade() {
    local installation_type="$1"
    local current_version="$2"
    local target_version="$3"

    echo ""
    log_warning "$installation_type beachball is already installed (version: $current_version)"
    log_info "Target version: $target_version"
    echo ""
    read -p "Do you want to upgrade to version $target_version? (y/N): " -n 1 -r
    echo ""

    if [[ $REPLY =~ ^[Yy]$ ]]; then
        return 0
    else
        return 1
    fi
}

# Function to install beachball globally
install_global_beachball() {
    local target_version="$1"

    log_step "Installing beachball globally (version: $target_version)..."

    if npm install -g "beachball@$target_version"; then
        log_success "Global beachball installation completed"
        return 0
    else
        log_error "Global beachball installation failed"
        return 1
    fi
}

# Function to install beachball locally
install_local_beachball() {
    local target_version="$1"

    log_step "Installing beachball locally (version: $target_version)..."

    if npm install "beachball@$target_version" --save-dev --no-workspaces; then
        log_success "Local beachball installation completed"
        return 0
    else
        log_error "Local beachball installation failed"
        return 1
    fi
}

# Function to create changelog files using external script
create_changelog_files() {
    log_step "Creating changelog files for .csproj projects..."

    if [ -f "scripts/create-changelog-files.sh" ]; then
        log_info "Using external changelog creation script..."
        if bash scripts/create-changelog-files.sh; then
            log_success "Changelog files creation completed"
            return 0
        fi
    else
        log_warning "create-changelog-files.sh not found, skipping changelog creation"
        return 0
    fi
}

# Function to display final summary
display_summary() {
    echo ""
    log_success "Beachball setup completed successfully! 🎉"
    echo ""
    log_info "Next steps:"
    echo "1. Run 'beachball check' to see current status"
    echo "2. Run 'beachball change' to create change files"
    echo "3. Run 'beachball bump' to bump versions"
    echo "4. Check BEACHBALL.md for detailed usage instructions"
    echo ""
    log_info "Available npm scripts:"
    echo "npm run beachball:check - Check for pending changes"
    echo "npm run beachball:change - Create change file"
    echo "npm run beachball:bump - Bump versions"
    echo "npm run release:patch - Patch release"
    echo "npm run release:minor - Minor release"
    echo "npm run release:major - Major release"
    echo ""
}

# Main execution function
main() {
    echo "🚀 $SCRIPT_NAME v$SCRIPT_VERSION - Setting up Beachball for Flyingdarts..."
    echo ""

    # Validate requirements
    validate_requirements

    # Get target beachball version from local package.json FIRST
    local target_beachball_version
    if target_beachball_version=$(get_beachball_version_from_package_json); then
        log_info "📦 Using beachball version from local package.json: '$target_beachball_version'"
    else
        log_info "📦 No beachball version found in local package.json - will install latest version"
        target_beachball_version="latest"
    fi

    # Handle local installation
    echo ""
    log_step "Checking local beachball installation..."
    local current_local_version
    if current_local_version=$(check_local_beachball_installation); then
        if compare_versions "$current_local_version" "$target_beachball_version"; then
            log_success "Local beachball is already up to date (version: '$current_local_version')"
        else
            if prompt_for_upgrade "Local" "$current_local_version" "$target_beachball_version"; then
                install_local_beachball "$target_beachball_version"
            else
                log_info "Skipping local beachball upgrade"
            fi
        fi
    else
        log_info "Installing local beachball (version: '$target_beachball_version')..."
        install_local_beachball "$target_beachball_version"
    fi

    # Handle global installation
    log_step "Checking global beachball installation..."
    local current_global_version
    if current_global_version=$(check_global_beachball_installation); then
        if compare_versions "$current_global_version" "$target_beachball_version"; then
            log_success "Global beachball is already up to date (version: '$current_global_version')"
        else
            if prompt_for_upgrade "Global" "$current_global_version" "$target_beachball_version"; then
                install_global_beachball "$target_beachball_version"
            else
                log_info "Skipping global beachball upgrade"
            fi
        fi
    else
        log_info "Installing global beachball (version: '$target_beachball_version')..."
        install_global_beachball "$target_beachball_version"
    fi

    # Create changelog files
    create_changelog_files

    # Display summary
    display_summary
}

# Run main function
main "$@"
