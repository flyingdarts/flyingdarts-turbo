#!/bin/bash

# Script to update all pubspec.yaml files for workspace compatibility
# This script ensures all pubspec.yaml files have:
# - environment.sdk: ^3.6.0
# - resolution: workspace

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Function to update a single pubspec.yaml file
update_pubspec() {
    local file="$1"
    local temp_file="${file}.tmp"
    
    print_info "Updating $file"
    
    # Check if file already has correct configuration
    if grep -q "sdk: \^3.6.0" "$file" && grep -q "resolution: workspace" "$file"; then
        print_success "✓ $file already has correct configuration"
        return 0
    fi
    
    # Create backup
    cp "$file" "${file}.backup"
    
    # Update the file
    # Replace SDK constraint
    sed -i.tmp 's/sdk: ">=3\.[0-9]\+\.[0-9]\+ <4\.0\.0"/sdk: ^3.6.0/g' "$file"
    sed -i.tmp 's/sdk: ">=3\.[0-9]\+\.[0-9]\+"/sdk: ^3.6.0/g' "$file"
    
    # Ensure resolution: workspace is present and in correct location
    if ! grep -q "resolution: workspace" "$file"; then
        # Add resolution after environment block
        awk '/^environment:/{print; print "resolution: workspace"; next} 1' "$file" > "$temp_file"
        mv "$temp_file" "$file"
    fi
    
    # Verify the changes
    if grep -q "sdk: \^3.6.0" "$file" && grep -q "resolution: workspace" "$file"; then
        print_success "✓ Updated $file"
        rm -f "${file}.backup"
    else
        print_error "✗ Failed to update $file"
        mv "${file}.backup" "$file"
        return 1
    fi
}

# Main script
main() {
    print_info "Updating all pubspec.yaml files for workspace compatibility"
    echo
    
    # Find all pubspec.yaml files (excluding the root workspace pubspec.yaml)
    local files=($(find packages/frontend/flutter/ apps/frontend/flutter/ -name "pubspec.yaml" -type f))
    
    local updated_count=0
    local error_count=0
    
    for file in "${files[@]}"; do
        if update_pubspec "$file"; then
            ((updated_count++))
        else
            ((error_count++))
        fi
    done
    
    echo
    print_info "Summary:"
    print_success "✓ Successfully updated: $updated_count files"
    if [ $error_count -gt 0 ]; then
        print_error "✗ Errors: $error_count files"
    fi
    
    if [ $error_count -eq 0 ]; then
        print_success "All pubspec.yaml files are now workspace compatible!"
    fi
}

# Run the script
main "$@" 