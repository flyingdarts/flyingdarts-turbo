#!/bin/bash

# Script to upgrade all Flutter packages in the workspace
# This script reads the workspace packages from root/pubspec.yaml and prompts before upgrading each

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Function to prompt user for confirmation
prompt_user() {
    local package_path="$1"
    echo
    print_info "About to run 'flutter pub upgrade' in: $package_path"
    echo -n "Do you want to continue? (y/N): "
    read -r response
    
    case "$response" in
        [yY][eE][sS]|[yY])
            return 0
            ;;
        *)
            return 1
            ;;
    esac
}

# Function to extract workspace packages from pubspec.yaml
extract_workspace_packages() {
    local pubspec_file="$1"
    local packages=()
    
    # Check if pubspec.yaml exists
    if [[ ! -f "$pubspec_file" ]]; then
        print_error "pubspec.yaml not found at: $pubspec_file"
        exit 1
    fi
    
    # Extract packages from workspace section
    # This regex looks for lines that start with spaces followed by a dash and a path
    while IFS= read -r line; do
        # Skip empty lines and comments
        if [[ -z "$line" || "$line" =~ ^[[:space:]]*# ]]; then
            continue
        fi
        
        # Check if line is in workspace section (starts with spaces, dash, and space)
        if [[ "$line" =~ ^[[:space:]]*-[[:space:]]+(.+)$ ]]; then
            package_path="${BASH_REMATCH[1]}"
            # Remove any trailing comments
            package_path=$(echo "$package_path" | sed 's/[[:space:]]*#.*$//')
            packages+=("$package_path")
        fi
    done < "$pubspec_file"
    
    echo "${packages[@]}"
}

# Main script
main() {
    print_info "Starting Flutter package upgrade script..."
    
    # Get the script directory and navigate to workspace root
    SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    WORKSPACE_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
    
    print_info "Workspace root: $WORKSPACE_ROOT"
    
    # Change to workspace root
    cd "$WORKSPACE_ROOT"
    
    # Extract workspace packages
    print_info "Reading workspace packages from pubspec.yaml..."
    packages=($(extract_workspace_packages "pubspec.yaml"))
    
    if [[ ${#packages[@]} -eq 0 ]]; then
        print_error "No packages found in workspace configuration"
        exit 1
    fi
    
    print_info "Found ${#packages[@]} packages in workspace:"
    for package in "${packages[@]}"; do
        echo "  - $package"
    done
    
    echo
    print_warning "This script will prompt you before running 'flutter pub upgrade' in each package."
    print_warning "You can skip any package by answering 'N' when prompted."
    echo
    
    # Counter for statistics
    upgraded_count=0
    skipped_count=0
    failed_count=0
    
    # Process each package
    for package in "${packages[@]}"; do
        package_path="$WORKSPACE_ROOT/$package"
        
        # Check if package directory exists
        if [[ ! -d "$package_path" ]]; then
            print_warning "Package directory does not exist: $package_path"
            ((skipped_count++))
            continue
        fi
        
        # Check if package has a pubspec.yaml
        if [[ ! -f "$package_path/pubspec.yaml" ]]; then
            print_warning "No pubspec.yaml found in: $package_path"
            ((skipped_count++))
            continue
        fi
        
        # Prompt user for confirmation
        if prompt_user "$package"; then
            print_info "Running 'flutter pub upgrade' in: $package"
            
            # Change to package directory and run flutter pub upgrade
            if cd "$package_path" && flutter pub upgrade; then
                print_success "Successfully upgraded packages in: $package"
                ((upgraded_count++))
            else
                print_error "Failed to upgrade packages in: $package"
                ((failed_count++))
            fi
            
            # Return to workspace root
            cd "$WORKSPACE_ROOT"
        else
            print_info "Skipped: $package"
            ((skipped_count++))
        fi
    done
    
    # Print summary
    echo
    print_info "=== Upgrade Summary ==="
    print_success "Successfully upgraded: $upgraded_count packages"
    print_warning "Skipped: $skipped_count packages"
    if [[ $failed_count -gt 0 ]]; then
        print_error "Failed: $failed_count packages"
    fi
    print_info "Total packages processed: ${#packages[@]}"
    
    if [[ $failed_count -gt 0 ]]; then
        exit 1
    fi
}

# Run main function
main "$@" 