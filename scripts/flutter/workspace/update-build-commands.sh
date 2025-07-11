#!/bin/bash

# Script to update build commands in package.json files next to pubspec.yaml files
# This script adds the build runner build command to the existing flutter build command

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Function to update a single package.json file
update_package_json() {
    local package_json="$1"
    local temp_file=$(mktemp)
    
    print_info "Updating $package_json"
    
    # Create a backup
    cp "$package_json" "${package_json}.backup"
    
    # Use sed to replace the build command (works on both Linux and macOS)
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS version
        sed -i '' 's/"build": "flutter build"/"build": "flutter packages pub run build_runner build --delete-conflicting-outputs \&\& flutter build"/g' "$package_json"
    else
        # Linux version
        sed -i 's/"build": "flutter build"/"build": "flutter packages pub run build_runner build --delete-conflicting-outputs \&\& flutter build"/g' "$package_json"
    fi
    
    print_success "Updated $package_json"
}

# Main script
main() {
    print_info "🔧 Updating build commands in package.json files"
    
    # Check if we're in the workspace root
    if [ ! -f "turbo.json" ]; then
        print_error "This script must be run from the workspace root directory (where turbo.json is located)"
        exit 1
    fi
    
    # Find all pubspec.yaml files and check for package.json in the same directory
    local updated_count=0
    local total_count=0
    
    while IFS= read -r -d '' pubspec; do
        local dir=$(dirname "$pubspec")
        local package_json="$dir/package.json"
        
        if [ -f "$package_json" ]; then
            total_count=$((total_count + 1))
            if update_package_json "$package_json"; then
                updated_count=$((updated_count + 1))
            fi
        fi
    done < <(find . -name "pubspec.yaml" -type f -print0)
    
    print_success "Updated $updated_count out of $total_count package.json files"
    
    if [ $updated_count -eq $total_count ]; then
        print_success "All package.json files have been successfully updated!"
    else
        print_warning "Some package.json files may not have been updated. Check the output above for errors."
    fi
}

# Run the main function
main "$@" 