#!/bin/bash

# Flutter Generated Files Cleanup Script
# This script removes all generated Dart files (.g.dart, .freezed.dart, etc.) 
# from all Flutter packages and apps in the workspace

set -e # Exit on any error

echo "🧹 Starting generated files cleanup..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Counters
removed_files=0
processed_packages=0

# Function to print colored output
print_info() {
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

# Function to check if we're in the workspace root
check_workspace_root() {
    if [ ! -f "pubspec.yaml" ] || ! grep -q "workspace:" pubspec.yaml; then
        print_error "This script must be run from the workspace root directory (where pubspec.yaml with workspace configuration is located)"
        exit 1
    fi
}

# Function to clean generated files in a directory
clean_generated_files_in_dir() {
    local dir="$1"
    local package_name=$(basename "$dir")
    
    print_info "Cleaning generated files in: $package_name"
    
    local count=0
    
    # Find and remove various types of generated files
    find "$dir" -name "*.g.dart" -type f 2>/dev/null | while read -r file; do
        print_info "  Removing: $(basename "$file")"
        rm -f "$file"
        ((count++))
    done
    
    find "$dir" -name "*.freezed.dart" -type f 2>/dev/null | while read -r file; do
        print_info "  Removing: $(basename "$file")"
        rm -f "$file"
        ((count++))
    done
    
    find "$dir" -name "*.chopper.dart" -type f 2>/dev/null | while read -r file; do
        print_info "  Removing: $(basename "$file")"
        rm -f "$file"
        ((count++))
    done
    
    find "$dir" -name "*.retrofit.dart" -type f 2>/dev/null | while read -r file; do
        print_info "  Removing: $(basename "$file")"
        rm -f "$file"
        ((count++))
    done
    
    find "$dir" -name "*.mocks.dart" -type f 2>/dev/null | while read -r file; do
        print_info "  Removing: $(basename "$file")"
        rm -f "$file"
        ((count++))
    done
    
    find "$dir" -name "*.config.dart" -type f 2>/dev/null | while read -r file; do
        print_info "  Removing: $(basename "$file")"
        rm -f "$file"
        ((count++))
    done
    
    find "$dir" -name "*.gr.dart" -type f 2>/dev/null | while read -r file; do
        print_info "  Removing: $(basename "$file")"
        rm -f "$file"
        ((count++))
    done
    
    # Count files that would be removed
    local file_count=0
    for pattern in "*.g.dart" "*.freezed.dart" "*.chopper.dart" "*.retrofit.dart" "*.mocks.dart" "*.config.dart" "*.gr.dart"; do
        local found_files=$(find "$dir" -name "$pattern" -type f 2>/dev/null | wc -l)
        file_count=$((file_count + found_files))
    done
    
    if [ $file_count -gt 0 ]; then
        print_success "  Cleaned $file_count generated files from $package_name"
        removed_files=$((removed_files + file_count))
    else
        print_info "  No generated files found in $package_name"
    fi
    
    processed_packages=$((processed_packages + 1))
}

# Function to clean all Flutter packages
clean_all_packages() {
    print_info "Scanning for Flutter packages and apps..."
    echo
    
    # Find all directories containing pubspec.yaml (Flutter packages/apps)
    local packages=($(find . -name "pubspec.yaml" -type f -exec dirname {} \; | grep -E "(packages/frontend/flutter|apps/frontend/flutter)" | sort))
    
    if [ ${#packages[@]} -eq 0 ]; then
        print_warning "No Flutter packages found in workspace"
        return
    fi
    
    print_info "Found ${#packages[@]} Flutter packages/apps"
    echo
    
    # Clean each package
    for package_dir in "${packages[@]}"; do
        if [ -d "$package_dir" ]; then
            clean_generated_files_in_dir "$package_dir"
            echo
        fi
    done
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Clean generated Dart files from all Flutter packages and apps in the workspace."
    echo ""
    echo "OPTIONS:"
    echo "  -h, --help     Show this help message"
    echo "  -v, --verbose  Enable verbose output"
    echo ""
    echo "Generated file types removed:"
    echo "  - *.g.dart      (json_annotation, injectable, etc.)"
    echo "  - *.freezed.dart (freezed)"
    echo "  - *.chopper.dart (chopper)"
    echo "  - *.retrofit.dart (retrofit)"
    echo "  - *.mocks.dart   (mockito)"
    echo "  - *.config.dart  (injectable config)"
    echo "  - *.gr.dart     (auto_route)"
    echo ""
    echo "This script must be run from the workspace root directory."
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_usage
            exit 0
            ;;
        -v|--verbose)
            set -x
            shift
            ;;
        *)
            print_error "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Main execution
main() {
    print_info "Flutter Generated Files Cleanup Script"
    echo
    
    # Check if we're in the right directory
    check_workspace_root
    
    # Clean all packages
    clean_all_packages
    
    # Summary
    echo "═══════════════════════════════════════"
    print_success "Cleanup completed!"
    print_info "Processed packages: $processed_packages"
    print_info "Removed generated files: $removed_files"
    echo
    
    if [ $removed_files -gt 0 ]; then
        print_info "You may want to run 'dart pub get' and then regenerate your files with:"
        print_info "  dart run build_runner build"
        print_info "or"
        print_info "  flutter packages pub run build_runner build"
    fi
}

# Run the main function
main 