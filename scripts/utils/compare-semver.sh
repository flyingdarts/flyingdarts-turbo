#!/bin/bash

# Semver Comparison Script
# Takes two version parameters, sanitizes them, extracts semver, and returns 0 if equal

# Check if running with bash
if [ -z "$BASH_VERSION" ]; then
    echo "❌ ERROR: This script must be run with bash, not sh"
    echo "   Please run: bash scripts/compare-semver.sh"
    exit 1
fi

set -euo pipefail # Exit on error, undefined vars, pipe failures

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
    echo -e "${PURPLE}🔍 DEBUG:${NC} $1"
}

# Function to extract semver from version string
extract_semver() {
    local version_string="$1"
    local original_input="$version_string"

    # Remove all whitespace and the word 'beachball' (case-insensitive)
    version_string=$(echo "$version_string" | tr -d '[:space:]' | sed -E 's/(beachball)//Ig')

    # Handle empty or null input
    if [ -z "$version_string" ] || [ "$version_string" = "null" ]; then
        echo ""
        return 1
    fi

    # Handle "latest" or special keywords
    if [[ "$version_string" =~ ^(latest|next|canary|beta|alpha)$ ]]; then
        echo "$version_string"
        return 0
    fi

    # Handle semver ranges (like ^2.54.0, ~2.54.0, >=2.54.0)
    if [[ "$version_string" == ^* ]] || [[ "$version_string" == ~* ]] || [[ "$version_string" == ">=*" ]] || [[ "$version_string" == "<=*" ]] || [[ "$version_string" == ">*" ]] || [[ "$version_string" == "<*" ]]; then
        local clean_version=$(echo "$version_string" | sed -E 's/^[\^~><=]+//')
        version_string="$clean_version"
    fi

    # Extract X.Y.Z pattern (major.minor.patch)
    if [[ "$version_string" =~ ([0-9]+)\.([0-9]+)\.([0-9]+) ]]; then
        local extracted_version="${BASH_REMATCH[1]}.${BASH_REMATCH[2]}.${BASH_REMATCH[3]}"
        echo "$extracted_version"
        return 0
    fi

    # Handle X.Y pattern (major.minor)
    if [[ "$version_string" =~ ([0-9]+)\.([0-9]+)$ ]]; then
        local extracted_version="${BASH_REMATCH[1]}.${BASH_REMATCH[2]}.0"
        echo "$extracted_version"
        return 0
    fi

    # Handle single number (major)
    if [[ "$version_string" =~ ^([0-9]+)$ ]]; then
        local extracted_version="${BASH_REMATCH[1]}.0.0"
        echo "$extracted_version"
        return 0
    fi

    # If no pattern matches, return the original string
    echo "$version_string"
    return 1
}

# Function to compare versions
compare_versions() {
    local version1="$1"
    local version2="$2"

    # Handle empty versions
    if [ -z "$version1" ] || [ -z "$version2" ]; then
        return 1
    fi

    # Handle special cases
    if [ "$version1" = "latest" ] || [ "$version2" = "latest" ]; then
        return 1
    fi

    # Extract clean semver versions
    local clean_version1=$(extract_semver "$version1")
    local clean_version2=$(extract_semver "$version2")

    # Check if extraction failed
    if [ -z "$clean_version1" ] || [ -z "$clean_version2" ]; then
        return 1
    fi

    if [ "$clean_version1" = "$clean_version2" ]; then
        return 0
    else
        return 1
    fi
}

# Main function
main() {
    # Check if we have exactly 2 arguments
    if [ $# -ne 2 ]; then
        log_error "Usage: $0 <version1> <version2>"
        log_info "Example: $0 '2.54.0' '^2.54.0'"
        exit 1
    fi

    local version1="$1"
    local version2="$2"

    log_info "Comparing versions: '$version1' and '$version2'"

    # Compare versions
    if compare_versions "$version1" "$version2"; then
        log_success "Versions are equal"
        exit 0
    else
        log_warning "Versions are different"
        exit 1
    fi
}

# Run main function
main "$@"
