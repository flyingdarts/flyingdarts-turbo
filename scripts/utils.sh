#!/bin/bash

# Shared utilities for version checking scripts

# Global verbose flag
VERBOSE=false

# Function to parse command line arguments
parse_args() {
    while [[ $# -gt 0 ]]; do
        case $1 in
        --verbose)
            VERBOSE=true
            shift
            ;;
        -v)
            VERBOSE=true
            shift
            ;;
        *)
            # Unknown option
            shift
            ;;
        esac
    done
}

# Check if running with bash
check_bash() {
    if [ -z "$BASH_VERSION" ]; then
        echo "❌ ERROR: This script must be run with bash, not sh"
        echo "   Please run: bash $0"
        exit 1
    fi
}

# Set strict error handling
set_strict_mode() {
    set -euo pipefail # Exit on error, undefined vars, pipe failures
}

# Color codes for better logging
readonly RED='\033[0;31m'
readonly GREEN='\033[0;32m'
readonly YELLOW='\033[1;33m'
readonly BLUE='\033[0;34m'
readonly PURPLE='\033[0;35m'
readonly CYAN='\033[0;36m'
readonly NC='\033[0m' # No Color

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

log_debug() {
    if [ "$VERBOSE" = true ]; then
        echo -e "${PURPLE}🔍 DEBUG:${NC} $1"
    fi
}

log_step() {
    if [ "$VERBOSE" = true ]; then
        echo -e "${CYAN}📋 STEP:${NC} $1"
    fi
}

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to test version comparison
test_version_comparison() {
    local version1="$1"
    local version2="$2"
    local description="$3"

    if [ "$VERBOSE" = true ]; then
        log_step "Testing: $description"
        log_info "Version 1: '$version1'"
        log_info "Version 2: '$version2'"
    fi

    if [ -f "scripts/compare-semver.sh" ]; then
        if bash scripts/compare-semver.sh "$version1" "$version2" >/dev/null 2>&1; then
            log_success "Versions are equal ✅"
        else
            log_warning "Versions are different ❌"
        fi
    else
        if [ "$VERBOSE" = true ]; then
            log_warning "compare-semver.sh not found, using simple comparison"
        fi
        if [ "$version1" = "$version2" ]; then
            log_success "Versions are equal ✅"
        else
            log_warning "Versions are different ❌"
        fi
    fi

    if [ "$VERBOSE" = true ]; then
        echo ""
    fi
}
