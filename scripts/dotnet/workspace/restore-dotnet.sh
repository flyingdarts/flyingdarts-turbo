#!/bin/bash

# .NET Solution and Projects Restoration Script
# This script restores the .NET solution and all projects within the workspace

set -e  # Exit on any error

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

# Function to check if .NET SDK is installed
check_dotnet_sdk() {
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK is not installed or not in PATH"
        print_status "Please install .NET SDK from https://dotnet.microsoft.com/download"
        exit 1
    fi
    
    DOTNET_VERSION=$(dotnet --version)
    print_status "Using .NET SDK version: $DOTNET_VERSION"
}

# Function to get the workspace root directory
get_workspace_root() {
    # Navigate to the script directory and then to workspace root
    SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    WORKSPACE_ROOT="$(cd "$SCRIPT_DIR/../../.." && pwd)"
    echo "$WORKSPACE_ROOT"
}

# Function to clean previous build artifacts
clean_build_artifacts() {
    print_status "Cleaning previous build artifacts..."
    
    # Remove bin and obj directories
    find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} + 2>/dev/null || true
    
    print_success "Build artifacts cleaned"
}

# Function to restore the main solution
restore_solution() {
    local solution_file="$1"
    
    if [[ -f "$solution_file" ]]; then
        print_status "Restoring solution: $solution_file"
        dotnet restore "$solution_file" --verbosity normal
        print_success "Solution restored successfully"
    else
        print_error "Solution file not found: $solution_file"
        return 1
    fi
}

# Function to restore individual projects
restore_projects() {
    print_status "Restoring individual projects..."
    
    # Find all .csproj files and restore them
    local project_count=0
    while IFS= read -r project_file; do
        if [[ -n "$project_file" ]]; then
            print_status "Restoring project: $(basename "$project_file")"
            dotnet restore "$project_file" --verbosity normal
            ((project_count++))
        fi
    done < <(find . -name "*.csproj" -type f)
    
    print_success "Restored $project_count projects"
}

# Function to verify restoration
verify_restoration() {
    print_status "Verifying restoration..."
    
    # Check if packages are restored by looking for obj/project.assets.json files
    local restored_count=0
    while IFS= read -r assets_file; do
        if [[ -n "$assets_file" ]]; then
            ((restored_count++))
        fi
    done < <(find . -name "project.assets.json" -type f)
    
    if [[ $restored_count -gt 0 ]]; then
        print_success "Verification complete: $restored_count projects have restored packages"
    else
        print_warning "No project.assets.json files found. Restoration may have failed."
        return 1
    fi
}

# Function to build the solution to ensure everything works
build_solution() {
    local solution_file="$1"
    
    print_status "Building solution to verify restoration..."
    dotnet build "$solution_file" --no-restore --verbosity normal
    print_success "Solution built successfully"
}

# Main execution
main() {
    print_status "Starting .NET solution and projects restoration..."
    
    # Check .NET SDK
    check_dotnet_sdk
    
    # Get workspace root
    WORKSPACE_ROOT=$(get_workspace_root)
    print_status "Workspace root: $WORKSPACE_ROOT"
    
    # Change to workspace root
    cd "$WORKSPACE_ROOT"
    
    # Clean previous build artifacts
    clean_build_artifacts
    
    # Main solution file
    SOLUTION_FILE="flyingdarts-turbo.sln"
    
    # Restore the main solution
    if restore_solution "$SOLUTION_FILE"; then
        print_success "Main solution restoration completed"
    else
        print_error "Failed to restore main solution"
        exit 1
    fi
    
    # Restore individual projects (as a backup/verification)
    restore_projects
    
    # Verify restoration
    if verify_restoration; then
        print_success "Restoration verification passed"
    else
        print_warning "Restoration verification failed"
    fi
    
    # Build solution to ensure everything works
    if build_solution "$SOLUTION_FILE"; then
        print_success "Build verification passed"
    else
        print_error "Build verification failed"
        exit 1
    fi
    
    print_success "All .NET solutions and projects have been restored successfully!"
}

# Run main function
main "$@"
