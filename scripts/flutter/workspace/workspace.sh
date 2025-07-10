#!/bin/bash

# Flyingdarts Flutter Workspace Management Script
# This script helps manage the Dart pub workspace for the Flutter monorepo

set -e

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

# Function to check if we're in the workspace root
check_workspace_root() {
    if [ ! -f "pubspec.yaml" ] || ! grep -q "workspace:" pubspec.yaml; then
        print_error "This script must be run from the workspace root directory (where pubspec.yaml with workspace configuration is located)"
        exit 1
    fi
}

# Function to list all workspace packages
list_packages() {
    print_info "Listing all workspace packages:"
    echo
    dart pub workspace list
}

# Function to get dependencies for all packages
get_dependencies() {
    print_info "Getting dependencies for all workspace packages..."
    dart pub get
    print_success "Dependencies resolved successfully!"
}

# Function to clean all packages
clean_packages() {
    print_info "Cleaning all workspace packages..."
    
    # Remove build artifacts
    find . -name "build" -type d -exec rm -rf {} + 2>/dev/null || true
    find . -name ".dart_tool" -type d -exec rm -rf {} + 2>/dev/null || true
    find . -name "pubspec.lock" -type f -delete 2>/dev/null || true
    
    print_success "Clean completed!"
}

# Function to run a command in all packages
run_in_all_packages() {
    local command="$1"
    local packages=($(dart pub workspace list | tail -n +2 | awk '{print $2}'))
    
    print_info "Running '$command' in all packages..."
    echo
    
    for package in "${packages[@]}"; do
        if [ -d "$package" ]; then
            print_info "Running in $package"
            (cd "$package" && eval "$command") || print_warning "Command failed in $package"
            echo
        fi
    done
}

# Function to analyze all packages
analyze_packages() {
    print_info "Analyzing all workspace packages..."
    run_in_all_packages "dart analyze"
    print_success "Analysis completed!"
}

# Function to test all packages
test_packages() {
    print_info "Testing all workspace packages..."
    run_in_all_packages "dart test"
    print_success "Testing completed!"
}

# Function to format all packages
format_packages() {
    print_info "Formatting all workspace packages..."
    run_in_all_packages "dart format ."
    print_success "Formatting completed!"
}

# Function to show workspace status
show_status() {
    print_info "Workspace Status:"
    echo
    print_info "Workspace packages:"
    dart pub workspace list
    echo
    print_info "Dependencies status:"
    if [ -f "pubspec.lock" ]; then
        print_success "✓ pubspec.lock exists"
    else
        print_warning "⚠ pubspec.lock missing - run 'get' to resolve dependencies"
    fi
    
    if [ -d ".dart_tool" ]; then
        print_success "✓ .dart_tool directory exists"
    else
        print_warning "⚠ .dart_tool directory missing - run 'get' to resolve dependencies"
    fi
}

# Function to show help
show_help() {
    echo "Flyingdarts Flutter Workspace Management Script"
    echo
    echo "Usage: $0 <command>"
    echo
    echo "Commands:"
    echo "  list          List all workspace packages"
    echo "  get           Get dependencies for all packages"
    echo "  clean         Clean all packages (remove build artifacts)"
    echo "  analyze       Run dart analyze on all packages"
    echo "  test          Run tests on all packages"
    echo "  format        Format code in all packages"
    echo "  status        Show workspace status"
    echo "  run <cmd>     Run a command in all packages"
    echo "  help          Show this help message"
    echo
    echo "Examples:"
    echo "  $0 get"
    echo "  $0 analyze"
    echo "  $0 run 'flutter pub get'"
}

# Main script logic
main() {
    check_workspace_root
    
    case "${1:-help}" in
        "list")
            list_packages
            ;;
        "get")
            get_dependencies
            ;;
        "clean")
            clean_packages
            ;;
        "analyze")
            analyze_packages
            ;;
        "test")
            test_packages
            ;;
        "format")
            format_packages
            ;;
        "status")
            show_status
            ;;
        "run")
            if [ -z "$2" ]; then
                print_error "No command specified for 'run'"
                echo "Usage: $0 run '<command>'"
                exit 1
            fi
            run_in_all_packages "$2"
            ;;
        "help"|"-h"|"--help")
            show_help
            ;;
        *)
            print_error "Unknown command: $1"
            echo
            show_help
            exit 1
            ;;
    esac
}

# Run main function with all arguments
main "$@" 