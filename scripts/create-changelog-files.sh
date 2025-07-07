#!/bin/bash

# Changelog Files Creation Script
# Creates CHANGELOG.md and CHANGELOG.json files for .csproj projects

# Check if running with bash
if [ -z "$BASH_VERSION" ]; then
    echo "❌ ERROR: This script must be run with bash, not sh"
    echo "   Please run: bash scripts/create-changelog-files.sh"
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

log_step() {
    echo -e "${CYAN}📋 STEP:${NC} $1"
}

# Function to create CHANGELOG.md
create_changelog_md() {
    local project_dir="$1"
    local project_name="$2"
    local changelog_path="$project_dir/CHANGELOG.md"

    if [ -f "$changelog_path" ]; then
        log_debug "CHANGELOG.md already exists for $project_name, skipping..."
        return 0
    fi

    log_debug "Creating CHANGELOG.md for $project_name..."

    if cat >"$changelog_path" <<EOF; then
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial changelog setup

### Changed

### Deprecated

### Removed

### Fixed

### Security

EOF
        log_debug "Successfully created CHANGELOG.md for $project_name"
        return 0
    else
        log_error "Failed to create CHANGELOG.md for $project_name"
        return 1
    fi
}

# Function to create CHANGELOG.json
create_changelog_json() {
    local project_dir="$1"
    local project_name="$2"
    local changelog_path="$project_dir/CHANGELOG.json"

    if [ -f "$changelog_path" ]; then
        log_debug "CHANGELOG.json already exists for $project_name, skipping..."
        return 0
    fi

    log_debug "Creating CHANGELOG.json for $project_name..."

    if cat >"$changelog_path" <<EOF; then
{
  "name": "$project_name",
  "version": "0.1.0",
  "changes": [
    {
      "type": "added",
      "description": "Initial changelog setup",
      "version": "0.1.0"
    }
  ]
}
EOF
        log_debug "Successfully created CHANGELOG.json for $project_name"
        return 0
    else
        log_error "Failed to create CHANGELOG.json for $project_name"
        return 1
    fi
}

# Function to create changelog files
create_changelog_files() {
    log_step "Creating changelog files for .csproj projects..."

    local csproj_files
    csproj_files=$(find . -name "*.csproj" -type f 2>/dev/null || true)

    if [ -z "$csproj_files" ]; then
        log_warning "No .csproj files found in the project"
        return 0
    fi

    local total_projects=$(echo "$csproj_files" | wc -l | tr -d ' ')
    log_info "Found $total_projects .csproj projects"

    local created_count=0
    local skipped_count=0
    local error_count=0

    while IFS= read -r csproj_file; do
        local project_dir=$(dirname "$csproj_file")
        local project_name=$(basename "$csproj_file" .csproj)

        log_debug "Processing project: $project_name in $project_dir"

        # Create CHANGELOG.md
        if create_changelog_md "$project_dir" "$project_name"; then
            created_count=$((created_count + 1))
        else
            error_count=$((error_count + 1))
        fi

        # Create CHANGELOG.json
        if create_changelog_json "$project_dir" "$project_name"; then
            created_count=$((created_count + 1))
        else
            error_count=$((error_count + 1))
        fi

    done <<<"$csproj_files"

    # Summary
    echo ""
    log_info "Changelog creation summary:"
    echo "✅ Created: $created_count files"
    echo "⏭️ Skipped: $skipped_count files (already existed)"
    echo "❌ Errors: $error_count files"
    echo "📁 Total projects processed: $total_projects"
}

# Main function
main() {
    echo "📝 Changelog Files Creation Script"
    echo "================================="
    echo ""

    create_changelog_files

    echo ""
    log_success "Changelog files creation completed! 🎉"
}

# Run main function
main "$@"
