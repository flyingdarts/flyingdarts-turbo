#!/bin/bash

# Script to run flutter pub upgrade on all Flutter packages in the workspace
# This script will:
# 1. Find all Flutter packages from the workspace pubspec.yaml
# 2. Run `flutter pub upgrade` in each package

set -e

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

# Function to upgrade a package
upgrade_package() {
    local package_dir="$1"
    local package_name=$(basename "$package_dir")
    
    print_status "Upgrading $package_name..."
    
    # Change to package directory
    cd "$package_dir"
    
    # Run flutter pub upgrade
    if flutter pub upgrade; then
        print_success "Successfully upgraded $package_name"
        return 0
    else
        print_error "Failed to upgrade $package_name"
        return 1
    fi
}

# Main script
main() {
    print_status "Starting Flutter package upgrade process..."
    
    # Get the script directory
    local script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    local workspace_root="$(cd "$script_dir/../../.." && pwd)"
    
    # Change to workspace root
    cd "$workspace_root"
    
    # Check if pubspec.yaml exists
    if [ ! -f "pubspec.yaml" ]; then
        print_error "pubspec.yaml not found in workspace root"
        exit 1
    fi
    
    # Extract package paths from workspace pubspec.yaml
    local packages=()
    while IFS= read -r line; do
        # Extract package paths from workspace section
        if [[ $line =~ ^[[:space:]]*-[[:space:]]*(.+)$ ]]; then
            local package_path="${BASH_REMATCH[1]}"
            # Remove any trailing comments
            package_path=$(echo "$package_path" | sed 's/[[:space:]]*#.*$//')
            if [ -d "$package_path" ] && [ -f "$package_path/pubspec.yaml" ]; then
                packages+=("$package_path")
            fi
        fi
    done < <(awk '/^workspace:/{flag=1; next} /^[^[:space:]]/{flag=0} flag' pubspec.yaml)
    
    if [ ${#packages[@]} -eq 0 ]; then
        print_error "No Flutter packages found in workspace configuration"
        exit 1
    fi
    
    print_status "Found ${#packages[@]} Flutter packages in workspace"
    
    # Display packages that will be upgraded
    echo
    print_status "Packages to upgrade:"
    for package in "${packages[@]}"; do
        print_status "  - $package"
    done
    
    local upgrade_success=()
    local upgrade_failed=()
    
    # Upgrade each package
    echo
    print_status "Starting package upgrades..."
    
    for package in "${packages[@]}"; do
        if upgrade_package "$package"; then
            upgrade_success+=("$package")
        else
            upgrade_failed+=("$package")
        fi
        
        # Return to workspace root
        cd "$workspace_root"
    done
    
    # Final summary
    echo
    print_status "Upgrade Summary:"
    print_success "Successfully upgraded: ${#upgrade_success[@]} packages"
    
    if [ ${#upgrade_failed[@]} -gt 0 ]; then
        print_error "Failed to upgrade: ${#upgrade_failed[@]} packages"
        for package in "${upgrade_failed[@]}"; do
            print_error "  - $package"
        done
        exit 1
    fi
    
    if [ ${#upgrade_success[@]} -gt 0 ]; then
        print_success "Successfully upgraded packages:"
        for package in "${upgrade_success[@]}"; do
            print_success "  - $package"
        done
    fi
    
    print_success "All Flutter packages have been upgraded successfully!"
}

# Run main function
main "$@" 