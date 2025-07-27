#!/bin/bash

# Script to check for outdated Flutter packages and upgrade them if updates are available
# This script will:
# 1. Find all Flutter packages (directories containing pubspec.yaml)
# 2. Run `flutter pub outdated` in each package
# 3. Check if there are any outdated packages
# 4. Run `flutter pub upgrade` if updates are available

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

# Function to check if a package has outdated dependencies
check_package_outdated() {
    local package_dir="$1"
    local package_name=$(basename "$package_dir")
    
    print_status "Checking $package_name for outdated packages..."
    
    # Change to package directory
    cd "$package_dir"
    
    # Run flutter pub outdated and capture output
    local outdated_output
    if outdated_output=$(flutter pub outdated 2>&1); then
        # Check if there are any outdated packages
        if echo "$outdated_output" | grep -q "dependencies are constrained to versions that are older than a resolvable version"; then
            print_warning "$package_name has outdated packages that can be upgraded"
            echo "$outdated_output"
            return 0  # Has outdated packages
        elif echo "$outdated_output" | grep -q "Showing outdated packages"; then
            # Check if there are any packages that can be upgraded
            if echo "$outdated_output" | grep -q "The following packages are newer than the versions listed in pubspec.lock:"; then
                print_warning "$package_name has outdated packages that can be upgraded"
                echo "$outdated_output"
                return 0  # Has outdated packages
            else
                print_success "$package_name is up to date"
                return 1  # No outdated packages
            fi
        else
            print_success "$package_name is up to date"
            return 1  # No outdated packages
        fi
    else
        print_error "Failed to run flutter pub outdated in $package_name"
        echo "$outdated_output"
        return 2  # Error
    fi
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
    
    # Find all Flutter packages (directories containing pubspec.yaml)
    local packages=($(find . -name "pubspec.yaml" -type f -exec dirname {} \; | sort))
    
    if [ ${#packages[@]} -eq 0 ]; then
        print_error "No Flutter packages found"
        exit 1
    fi
    
    print_status "Found ${#packages[@]} Flutter packages"
    
    local packages_with_updates=()
    local failed_packages=()
    
    # Check each package for outdated dependencies
    for package in "${packages[@]}"; do
        local package_name=$(basename "$package")
        print_status "Processing package: $package_name"
        
        case $(check_package_outdated "$package") in
            0)  # Has outdated packages
                packages_with_updates+=("$package")
                ;;
            1)  # No outdated packages
                # Do nothing, package is up to date
                ;;
            2)  # Error
                failed_packages+=("$package")
                ;;
        esac
        
        # Return to workspace root
        cd "$workspace_root"
    done
    
    # Summary of findings
    echo
    print_status "Summary:"
    print_status "Total packages checked: ${#packages[@]}"
    print_status "Packages with updates available: ${#packages_with_updates[@]}"
    print_status "Packages with errors: ${#failed_packages[@]}"
    
    if [ ${#failed_packages[@]} -gt 0 ]; then
        echo
        print_warning "The following packages had errors:"
        for package in "${failed_packages[@]}"; do
            print_warning "  - $(basename "$package")"
        done
    fi
    
    if [ ${#packages_with_updates[@]} -eq 0 ]; then
        echo
        print_success "All packages are up to date!"
        exit 0
    fi
    
    # Ask user if they want to proceed with upgrades
    echo
    print_warning "The following packages have updates available:"
    for package in "${packages_with_updates[@]}"; do
        print_warning "  - $(basename "$package")"
    done
    
    echo
    read -p "Do you want to upgrade these packages? (y/N): " -n 1 -r
    echo
    
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_status "Upgrade cancelled by user"
        exit 0
    fi
    
    # Upgrade packages
    echo
    print_status "Starting package upgrades..."
    
    local upgrade_success=()
    local upgrade_failed=()
    
    for package in "${packages_with_updates[@]}"; do
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
            print_error "  - $(basename "$package")"
        done
    fi
    
    if [ ${#upgrade_success[@]} -gt 0 ]; then
        print_success "Successfully upgraded packages:"
        for package in "${upgrade_success[@]}"; do
            print_success "  - $(basename "$package")"
        done
    fi
    
    print_status "Flutter package upgrade process completed!"
}

# Run main function
main "$@" 