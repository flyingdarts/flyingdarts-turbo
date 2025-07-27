#!/bin/sh

# Pubspec Files Cleanup Script
# This script cleans up pubspec_overrides.yaml.* and pubspec.yaml.tmp files in the repository

set -e # Exit on any error

echo "🧹 Starting pubspec files cleanup..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    printf "${BLUE}[INFO]${NC} %s\n" "$1"
}

print_success() {
    printf "${GREEN}[SUCCESS]${NC} %s\n" "$1"
}

print_warning() {
    printf "${YELLOW}[WARNING]${NC} %s\n" "$1"
}

print_error() {
    printf "${RED}[ERROR]${NC} %s\n" "$1"
}

# Function to check if file exists and remove it
safe_remove_file() {
    if [ -f "$1" ]; then
        print_status "Removing $1"
        rm -f "$1"
        print_success "Removed $1"
    else
        print_warning "File $1 does not exist, skipping..."
    fi
}

# Get the script directory and determine working directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WORKSPACE_ROOT="$(cd "$SCRIPT_DIR/../../../.." && pwd)"

print_status "Working directory: $WORKSPACE_ROOT"

# Function to clean pubspec override files
clean_pubspec_override_files() {
    print_status "Cleaning pubspec_overrides.yaml.* files in workspace packages..."
    
    count=0
    # Search only in workspace packages
    find "$WORKSPACE_ROOT/apps/frontend/flutter" -name "pubspec_overrides.yaml*" -type f 2>/dev/null | while read -r file; do
        safe_remove_file "$file"
        count=$((count + 1))
    done
    
    find "$WORKSPACE_ROOT/packages/frontend/flutter" -name "pubspec_overrides.yaml*" -type f 2>/dev/null | while read -r file; do
        safe_remove_file "$file"
        count=$((count + 1))
    done
    
    if [ "$count" -eq 0 ]; then
        print_warning "No pubspec_overrides.yaml.* files found in workspace packages"
    else
        print_success "Cleaned $count pubspec_overrides.yaml.* files"
    fi
}

# Function to clean pubspec temporary files
clean_pubspec_temp_files() {
    print_status "Cleaning pubspec.yaml.tmp files in workspace packages..."
    
    count=0
    # Search only in workspace packages
    find "$WORKSPACE_ROOT/apps/frontend/flutter" -name "pubspec.yaml.tmp" -type f 2>/dev/null | while read -r file; do
        safe_remove_file "$file"
        count=$((count + 1))
    done
    
    find "$WORKSPACE_ROOT/packages/frontend/flutter" -name "pubspec.yaml.tmp" -type f 2>/dev/null | while read -r file; do
        safe_remove_file "$file"
        count=$((count + 1))
    done
    
    if [ "$count" -eq 0 ]; then
        print_warning "No pubspec.yaml.tmp files found in workspace packages"
    else
        print_success "Cleaned $count pubspec.yaml.tmp files"
    fi
}

# Function to show what files would be cleaned (dry run)
dry_run() {
    print_status "Performing dry run to show what would be cleaned in workspace packages..."
    
    echo
    print_status "pubspec_overrides.yaml.* files that would be removed:"
    count=0
    find "$WORKSPACE_ROOT/apps/frontend/flutter" -name "pubspec_overrides.yaml*" -type f 2>/dev/null | while read -r file; do
        echo "  - $file"
        count=$((count + 1))
    done
    
    find "$WORKSPACE_ROOT/packages/frontend/flutter" -name "pubspec_overrides.yaml*" -type f 2>/dev/null | while read -r file; do
        echo "  - $file"
        count=$((count + 1))
    done
    
    if [ "$count" -eq 0 ]; then
        print_warning "No pubspec_overrides.yaml.* files found in workspace packages"
    else
        print_status "Found $count pubspec_overrides.yaml.* files"
    fi
    
    echo
    print_status "pubspec.yaml.tmp files that would be removed:"
    count=0
    find "$WORKSPACE_ROOT/apps/frontend/flutter" -name "pubspec.yaml.tmp" -type f 2>/dev/null | while read -r file; do
        echo "  - $file"
        count=$((count + 1))
    done
    
    find "$WORKSPACE_ROOT/packages/frontend/flutter" -name "pubspec.yaml.tmp" -type f 2>/dev/null | while read -r file; do
        echo "  - $file"
        count=$((count + 1))
    done
    
    if [ "$count" -eq 0 ]; then
        print_warning "No pubspec.yaml.tmp files found in workspace packages"
    else
        print_status "Found $count pubspec.yaml.tmp files"
    fi
}

# Parse command line arguments
DRY_RUN=false

while [ $# -gt 0 ]; do
    case $1 in
        --dry-run)
            DRY_RUN=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo
            echo "Options:"
            echo "  --dry-run    Show what files would be cleaned without actually removing them"
            echo "  -h, --help   Show this help message"
            echo
            echo "This script cleans up:"
            echo "  - pubspec_overrides.yaml.* files (backup files created during workspace operations)"
            echo "  - pubspec.yaml.tmp files (temporary files created during pubspec modifications)"
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            echo "Use -h or --help for usage information"
            exit 1
            ;;
    esac
done

# Main execution
if [ "$DRY_RUN" = true ]; then
    dry_run
else
    # Confirm before proceeding
    echo
    print_warning "This will permanently delete pubspec_overrides.yaml.* and pubspec.yaml.tmp files."
    printf "Are you sure you want to continue? (y/N): "
    read -r reply
    echo
    
    case "$reply" in
        [Yy]*)
            clean_pubspec_override_files
            clean_pubspec_temp_files
            print_success "✅ Pubspec files cleanup completed!"
            ;;
        *)
            print_status "Cleanup cancelled by user"
            exit 0
            ;;
    esac
fi 