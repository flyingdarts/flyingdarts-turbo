#!/bin/bash
# verify-beachball-installation - Check if Beachball is installed globally or locally
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

# Function to get beachball version from package.json
get_beachball_version_from_package_json() {
    if [ ! -f "package.json" ]; then
        return 1
    fi

    # Extract beachball version from package.json
    local version
    if version=$(node -e "
        try {
            const pkg = require('./package.json');
            const deps = { ...pkg.dependencies, ...pkg.devDependencies };
            console.log(deps.beachball || '');
        } catch (e) {
            console.log('');
        }
    " 2>/dev/null); then
        if [ -n "$version" ] && [ "$version" != "undefined" ]; then
            echo "$version"
            return 0
        fi
    fi

    return 1
}

# Function to check global beachball installation
check_global_beachball() {
    if ! command_exists beachball; then
        return 1
    fi

    local version
    if version=$(beachball --version 2>/dev/null); then
        echo "$version"
        return 0
    fi

    return 1
}

# Function to check local beachball installation
check_local_beachball() {
    if [ ! -f "package.json" ]; then
        return 1
    fi

    # Check if beachball is in node_modules
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
                echo "$version"
                return 0
            fi
        fi
    fi

    return 1
}

echo "🔧 Executing verify-beachball-installation..."

# Check global installation
log_info "Checking global beachball installation..."
if global_version=$(check_global_beachball); then
    log_success "Global beachball found: version '$global_version'"
    echo "GLOBAL_INSTALLED=true" >>/tmp/beachball_status
    echo "GLOBAL_VERSION=$global_version" >>/tmp/beachball_status
else
    log_warning "Global beachball not found"
    echo "GLOBAL_INSTALLED=false" >>/tmp/beachball_status
fi

# Check local installation
log_info "Checking local beachball installation..."
if local_version=$(check_local_beachball); then
    log_success "Local beachball found: version '$local_version'"
    echo "LOCAL_INSTALLED=true" >>/tmp/beachball_status
    echo "LOCAL_VERSION=$local_version" >>/tmp/beachball_status
else
    log_warning "Local beachball not found"
    echo "LOCAL_INSTALLED=false" >>/tmp/beachball_status
fi

# Check package.json for target version
log_info "Checking package.json for target beachball version..."
if target_version=$(get_beachball_version_from_package_json); then
    log_success "Target beachball version from package.json: '$target_version'"
    echo "TARGET_VERSION=$target_version" >>/tmp/beachball_status
else
    log_info "No beachball version specified in package.json - will use latest"
    echo "TARGET_VERSION=latest" >>/tmp/beachball_status
fi

# Determine overall status
if [ -f /tmp/beachball_status ]; then
    source /tmp/beachball_status

    if [ "$GLOBAL_INSTALLED" = "true" ] || [ "$LOCAL_INSTALLED" = "true" ]; then
        log_success "Beachball installation verified"
        echo -e "${GREEN}✅ verify-beachball-installation completed successfully${NC}"
        rm -f /tmp/beachball_status
        exit 0
    else
        log_warning "No beachball installation found"
        echo -e "${YELLOW}⚠️ No beachball installation found${NC}"
        rm -f /tmp/beachball_status
        exit 1
    fi
else
    log_error "Failed to verify beachball installation"
    echo -e "${RED}❌ verify-beachball-installation failed${NC}"
    exit 1
fi
