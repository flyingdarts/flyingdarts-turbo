#!/bin/bash
# verify-changelog-files - Verify that changelog files exist and are valid
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

# Function to find .csproj files
find_csproj_files() {
    find . -name "*.csproj" -type f 2>/dev/null | head -20
}

# Function to check if a file is a valid changelog
is_valid_changelog() {
    local file="$1"

    if [ ! -f "$file" ]; then
        return 1
    fi

    # Check if file has content
    if [ ! -s "$file" ]; then
        return 1
    fi

    # Check if file contains changelog-like content
    if grep -q -i "changelog\|version\|release" "$file" 2>/dev/null; then
        return 0
    fi

    # Check if file has markdown structure
    if grep -q "^#" "$file" 2>/dev/null; then
        return 0
    fi

    return 1
}

# Function to verify changelog for a specific project
verify_project_changelog() {
    local project_dir="$1"
    local project_name="$2"

    log_info "Checking changelog for project: $project_name"

    # Look for common changelog file names
    local changelog_files=(
        "CHANGELOG.md"
        "CHANGELOG.txt"
        "CHANGELOG"
        "CHANGES.md"
        "CHANGES.txt"
        "RELEASE_NOTES.md"
        "RELEASE_NOTES.txt"
    )

    local found_changelog=""

    for changelog_file in "${changelog_files[@]}"; do
        local full_path="$project_dir/$changelog_file"
        if [ -f "$full_path" ]; then
            if is_valid_changelog "$full_path"; then
                found_changelog="$full_path"
                break
            else
                log_warning "Found changelog file but it appears to be empty or invalid: $full_path"
            fi
        fi
    done

    if [ -n "$found_changelog" ]; then
        log_success "Valid changelog found: $found_changelog"
        echo "$found_changelog"
        return 0
    else
        log_warning "No valid changelog found for project: $project_name"
        return 1
    fi
}

# Function to create changelog file
create_changelog_file() {
    local project_dir="$1"
    local project_name="$2"
    local changelog_file="$project_dir/CHANGELOG.md"

    log_info "Creating changelog file: $changelog_file"

    cat >"$changelog_file" <<EOF
# Changelog

All notable changes to the $project_name project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial project setup

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [1.0.0] - $(date +%Y-%m-%d)

### Added
- Initial release
EOF

    if [ -f "$changelog_file" ]; then
        log_success "Created changelog file: $changelog_file"
        return 0
    else
        log_error "Failed to create changelog file: $changelog_file"
        return 1
    fi
}

# Function to check if we should create changelog files
should_create_changelog() {
    # Check if we're in a .NET project context
    if [ -f "*.sln" ] || [ -f "*.csproj" ] || find . -name "*.csproj" -type f | head -1 | grep -q .; then
        return 0
    fi

    # Check if we have a package.json (Node.js project)
    if [ -f "package.json" ]; then
        return 0
    fi

    return 1
}

echo "🔧 Executing verify-changelog-files..."

# Check if we're in a project that needs changelog files
if ! should_create_changelog; then
    log_info "No project files detected, skipping changelog verification"
    echo -e "${GREEN}✅ verify-changelog-files completed successfully${NC}"
    exit 0
fi

# Initialize counters
total_projects=0
projects_with_changelog=0
projects_created_changelog=0

# Check for .csproj files (C# projects)
log_info "Checking for .NET projects..."
csproj_files=$(find_csproj_files)

if [ -n "$csproj_files" ]; then
    log_info "Found .NET projects, checking changelog files..."

    while IFS= read -r csproj_file; do
        if [ -n "$csproj_file" ]; then
            project_dir=$(dirname "$csproj_file")
            project_name=$(basename "$csproj_file" .csproj)

            total_projects=$((total_projects + 1))

            if verify_project_changelog "$project_dir" "$project_name" >/dev/null; then
                projects_with_changelog=$((projects_with_changelog + 1))
            else
                # Try to create changelog file
                if create_changelog_file "$project_dir" "$project_name"; then
                    projects_created_changelog=$((projects_created_changelog + 1))
                    projects_with_changelog=$((projects_with_changelog + 1))
                fi
            fi
        fi
    done <<<"$csproj_files"
fi

# Check for package.json (Node.js project)
if [ -f "package.json" ]; then
    log_info "Checking for Node.js project changelog..."
    total_projects=$((total_projects + 1))

    project_name=$(node -e "
        try {
            const pkg = require('./package.json');
            console.log(pkg.name || 'project');
        } catch (e) {
            console.log('project');
        }
    " 2>/dev/null)

    if verify_project_changelog "." "$project_name" >/dev/null; then
        projects_with_changelog=$((projects_with_changelog + 1))
    else
        # Try to create changelog file
        if create_changelog_file "." "$project_name"; then
            projects_created_changelog=$((projects_created_changelog + 1))
            projects_with_changelog=$((projects_with_changelog + 1))
        fi
    fi
fi

# Summary
log_info "Changelog verification summary:"
log_info "  Total projects: $total_projects"
log_info "  Projects with changelog: $projects_with_changelog"
if [ $projects_created_changelog -gt 0 ]; then
    log_info "  Changelog files created: $projects_created_changelog"
fi

# Determine success
if [ $total_projects -eq 0 ]; then
    log_info "No projects found, skipping changelog verification"
    echo -e "${GREEN}✅ verify-changelog-files completed successfully${NC}"
    exit 0
elif [ $projects_with_changelog -eq $total_projects ]; then
    log_success "All projects have valid changelog files"
    echo -e "${GREEN}✅ verify-changelog-files completed successfully${NC}"
    exit 0
else
    log_warning "Some projects are missing changelog files"
    echo -e "${YELLOW}⚠️ Some projects missing changelog files${NC}"
    exit 1
fi
