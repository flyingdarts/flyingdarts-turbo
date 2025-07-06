#!/bin/bash

# 🧹 Melos Cleanup Script
# This script removes all melos-related content from the Flutter project
# Starting from apps/frontend/flutter and excluding dependencies directory

set -e # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if directory exists
check_directory() {
    if [ ! -d "$1" ]; then
        print_error "Directory $1 does not exist!"
        exit 1
    fi
}

# Function to backup a file before modification
backup_file() {
    local file="$1"
    if [ -f "$file" ]; then
        cp "$file" "${file}.backup.$(date +%Y%m%d_%H%M%S)"
        print_status "Backed up $file"
    fi
}

# Function to remove melos dependency from pubspec.yaml
remove_melos_from_pubspec() {
    local pubspec_file="$1"

    if [ -f "$pubspec_file" ]; then
        backup_file "$pubspec_file"

        # Remove melos dependency line
        sed -i.tmp '/^[[:space:]]*melos:/d' "$pubspec_file"
        sed -i.tmp '/^[[:space:]]*melos[[:space:]]*:/d' "$pubspec_file"

        # Clean up temporary file
        rm -f "${pubspec_file}.tmp"

        print_success "Removed melos dependency from $pubspec_file"
    fi
}

# Function to clean melos comments from pubspec_overrides.yaml
clean_melos_comments() {
    local overrides_file="$1"

    if [ -f "$overrides_file" ]; then
        backup_file "$overrides_file"

        # Remove lines containing melos_managed_dependency_overrides
        sed -i.tmp '/melos_managed_dependency_overrides/d' "$overrides_file"

        # Clean up temporary file
        rm -f "${overrides_file}.tmp"

        print_success "Cleaned melos comments from $overrides_file"
    fi
}

# Function to clean melos references from other files
clean_melos_references() {
    local file="$1"

    if [ -f "$file" ]; then
        backup_file "$file"

        # Remove melos commands and references
        sed -i.tmp '/melos exec/d' "$file"
        sed -i.tmp '/melos bootstrap/d' "$file"
        sed -i.tmp '/melos run/d' "$file"
        sed -i.tmp '/melos generate/d' "$file"

        # Clean up temporary file
        rm -f "${file}.tmp"

        print_success "Cleaned melos references from $file"
    fi
}

# Main cleanup function
clean_melos_recursive() {
    local start_dir="$1"
    local current_dir="$2"

    print_status "Scanning directory: $current_dir"

    # Skip dependencies directory
    if [[ "$current_dir" == *"/dependencies"* ]]; then
        print_warning "Skipping dependencies directory: $current_dir"
        return
    fi

    # Process files in current directory
    for file in "$current_dir"/*; do
        if [ -f "$file" ]; then
            case "$file" in
            */melos.yaml)
                print_status "Found melos.yaml: $file"
                rm -f "$file"
                print_success "Deleted $file"
                ;;
            */pubspec.yaml)
                print_status "Processing pubspec.yaml: $file"
                remove_melos_from_pubspec "$file"
                ;;
            */pubspec_overrides.yaml)
                print_status "Processing pubspec_overrides.yaml: $file"
                clean_melos_comments "$file"
                ;;
            *.md | *.txt | *.yaml | *.yml)
                if grep -q "melos" "$file" 2>/dev/null; then
                    print_status "Found melos references in: $file"
                    clean_melos_references "$file"
                fi
                ;;
            esac
        elif [ -d "$file" ]; then
            # Recursively process subdirectories
            clean_melos_recursive "$start_dir" "$file"
        fi
    done
}

# Main execution
main() {
    print_status "🧹 Starting Melos Cleanup Script"
    print_status "Target directory: apps/frontend/flutter"
    print_status "Excluding: dependencies directory"

    # Check if we're in the right directory
    if [ ! -d "apps/frontend/flutter" ]; then
        print_error "Please run this script from the project root directory!"
        print_error "Expected directory structure: apps/frontend/flutter"
        exit 1
    fi

    # Check if target directory exists
    check_directory "apps/frontend/flutter"

    # Create backup directory
    local backup_dir="melos_cleanup_backup_$(date +%Y%m%d_%H%M%S)"
    mkdir -p "$backup_dir"
    print_status "Backup directory created: $backup_dir"

    # Start recursive cleanup
    clean_melos_recursive "apps/frontend/flutter" "apps/frontend/flutter"

    print_success "🎉 Melos cleanup completed successfully!"
    print_status "Backup files created with .backup.* extension"
    print_status "You can review the changes and restore from backups if needed"

    # Summary
    echo
    print_status "📋 Summary of actions:"
    echo "  • Deleted melos.yaml files"
    echo "  • Removed melos dependencies from pubspec.yaml files"
    echo "  • Cleaned melos comments from pubspec_overrides.yaml files"
    echo "  • Removed melos references from documentation and config files"
    echo "  • Excluded dependencies directory from cleanup"
    echo "  • Created backups of all modified files"
}

# Check if user really wants to run this
echo -e "${YELLOW}⚠️  WARNING: This script will remove all melos-related content from your Flutter project!${NC}"
echo -e "${YELLOW}   This includes:${NC}"
echo -e "${YELLOW}   • melos.yaml configuration files${NC}"
echo -e "${YELLOW}   • melos dependencies in pubspec.yaml${NC}"
echo -e "${YELLOW}   • melos comments in pubspec_overrides.yaml${NC}"
echo -e "${YELLOW}   • melos references in other files${NC}"
echo
echo -e "${YELLOW}   The script will create backups of all modified files.${NC}"
echo -e "${YELLOW}   Target directory: apps/frontend/flutter${NC}"
echo -e "${YELLOW}   Excluded directory: dependencies${NC}"
echo
read -p "Are you sure you want to continue? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    print_warning "Script execution cancelled by user"
    exit 0
fi

# Run the main function
main "$@"
