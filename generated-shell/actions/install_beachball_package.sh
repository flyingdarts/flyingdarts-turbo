#!/bin/bash
# install-beachball-package - Install or update Beachball package
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

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to validate requirements
validate_requirements() {
    if ! command_exists npm; then
        log_error "npm is not installed"
        return 1
    fi

    if ! command_exists node; then
        log_error "node is not installed"
        return 1
    fi

    return 0
}

# Function to get target version from package.json
get_target_version() {
    if [ ! -f "package.json" ]; then
        echo "latest"
        return 0
    fi

    local version
    if version=$(node -e "
        try {
            const pkg = require('./package.json');
            const deps = { ...pkg.dependencies, ...pkg.devDependencies };
            console.log(deps.beachball || 'latest');
        } catch (e) {
            console.log('latest');
        }
    " 2>/dev/null); then
        echo "$version"
        return 0
    fi

    echo "latest"
    return 0
}

# Function to install beachball globally
install_global_beachball() {
    local target_version="$1"

    log_info "Installing beachball globally (version: $target_version)..."

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

    log_info "Installing beachball locally (version: $target_version)..."

    # Check if we're in a workspace
    local workspace_flag=""
    if [ -f "package.json" ] && node -e "
        try {
            const pkg = require('./package.json');
            if (pkg.workspaces) {
                process.exit(0);
            }
        } catch (e) {}
        process.exit(1);
    " 2>/dev/null; then
        workspace_flag="--no-workspaces"
    fi

    if npm install "beachball@$target_version" --save-dev $workspace_flag; then
        log_success "Local beachball installation completed"
        return 0
    else
        log_error "Local beachball installation failed"
        return 1
    fi
}

# Function to verify installation
verify_installation() {
    local installation_type="$1"

    log_info "Verifying $installation_type installation..."

    if [ "$installation_type" = "global" ]; then
        if command_exists beachball; then
            local version
            if version=$(beachball --version 2>/dev/null); then
                log_success "Global beachball verified: version '$version'"
                return 0
            fi
        fi
    else
        if [ -d "node_modules/beachball" ]; then
            local version
            if version=$(node -e "
                try {
                    const pkg = require('./node_modules/beachball/package.json');
                    console.log(pkg.version || '');
                } catch (e) {
                    console.log('');
                }
            " 2>/dev/null); then
                if [ -n "$version" ]; then
                    log_success "Local beachball verified: version '$version'"
                    return 0
                fi
            fi
        fi
    fi

    log_error "$installation_type installation verification failed"
    return 1
}

echo "🔧 Executing install-beachball-package..."

# Validate requirements
log_info "Validating system requirements..."
if ! validate_requirements; then
    log_error "System requirements not met"
    echo -e "${RED}❌ install-beachball-package failed${NC}"
    exit 1
fi

# Get target version
target_version=$(get_target_version)
log_info "Target beachball version: '$target_version'"

# Install globally
log_info "Installing beachball globally..."
if install_global_beachball "$target_version"; then
    if verify_installation "global"; then
        log_success "Global installation successful"
    else
        log_warning "Global installation verification failed"
    fi
else
    log_warning "Global installation failed, continuing with local installation"
fi

# Install locally (if package.json exists)
if [ -f "package.json" ]; then
    log_info "Installing beachball locally..."
    if install_local_beachball "$target_version"; then
        if verify_installation "local"; then
            log_success "Local installation successful"
        else
            log_warning "Local installation verification failed"
        fi
    else
        log_error "Local installation failed"
        echo -e "${RED}❌ install-beachball-package failed${NC}"
        exit 1
    fi
else
    log_info "No package.json found, skipping local installation"
fi

# Final verification
log_info "Performing final verification..."
if command_exists beachball || [ -d "node_modules/beachball" ]; then
    log_success "Beachball installation completed successfully"
    echo -e "${GREEN}✅ install-beachball-package completed successfully${NC}"
    exit 0
else
    log_error "Beachball installation verification failed"
    echo -e "${RED}❌ install-beachball-package failed${NC}"
    exit 1
fi
