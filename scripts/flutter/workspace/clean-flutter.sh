#!/bin/bash

# Flutter Cleanup Script
# This script cleans up Flutter build artifacts, cache files, and temporary files

set -e # Exit on any error

echo "🧹 Starting Flutter cleanup..."

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

# Function to check if directory exists and remove it
safe_remove_dir() {
    if [ -d "$1" ]; then
        print_status "Removing $1"
        rm -rf "$1"
        print_success "Removed $1"
    else
        print_warning "Directory $1 does not exist, skipping..."
    fi
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
CURRENT_DIR="$(pwd)"

# Check if we're in a Flutter project (has pubspec.yaml)
if [ -f "pubspec.yaml" ]; then
    # We're in a Flutter project directory
    PROJECT_ROOT="$CURRENT_DIR"
    print_status "Running cleanup in Flutter project: $PROJECT_ROOT"
    SINGLE_PROJECT=true
else
    # We're in workspace root, find all Flutter projects
    PROJECT_ROOT="$CURRENT_DIR"
    print_status "Running cleanup in workspace: $PROJECT_ROOT"
    SINGLE_PROJECT=false
fi

# Check if Flutter is installed
if ! command -v flutter &>/dev/null; then
    print_error "Flutter is not installed or not in PATH"
    exit 1
fi

print_status "Flutter version: $(flutter --version | head -n 1)"

# Function to clean a single Flutter project
clean_flutter_project() {
    local project_dir="$1"
    print_status "Cleaning Flutter project in: $project_dir"

    # Change to project directory
    cd "$project_dir"

    # Run flutter clean
    print_status "Running flutter clean..."
    flutter clean

    # Remove additional build artifacts
    safe_remove_dir "build"
    safe_remove_dir ".dart_tool"
    safe_remove_dir ".flutter-plugins"
    safe_remove_dir ".flutter-plugins-dependencies"
    safe_remove_file ".packages"
    safe_remove_file "pubspec.lock"

    # Remove iOS build artifacts
    if [ -d "ios" ]; then
        print_status "Cleaning iOS build artifacts..."
        safe_remove_dir "ios/build"
        safe_remove_dir "ios/Pods"
        safe_remove_file "ios/Podfile.lock"
        safe_remove_dir "ios/.symlinks"
        safe_remove_dir "ios/Flutter/Flutter.framework"
        safe_remove_dir "ios/Flutter/Flutter.podspec"
    fi

    # Remove Android build artifacts
    if [ -d "android" ]; then
        print_status "Cleaning Android build artifacts..."
        safe_remove_dir "android/build"
        safe_remove_dir "android/app/build"
        safe_remove_dir "android/.gradle"
        safe_remove_dir "android/local.properties"
    fi

    # Remove macOS build artifacts
    if [ -d "macos" ]; then
        print_status "Cleaning macOS build artifacts..."
        safe_remove_dir "macos/build"
        safe_remove_dir "macos/Pods"
        safe_remove_file "macos/Podfile.lock"
    fi

    # Remove Windows build artifacts
    if [ -d "windows" ]; then
        print_status "Cleaning Windows build artifacts..."
        safe_remove_dir "windows/build"
    fi

    # Remove Linux build artifacts
    if [ -d "linux" ]; then
        print_status "Cleaning Linux build artifacts..."
        safe_remove_dir "linux/build"
    fi

    # Remove web build artifacts
    if [ -d "web" ]; then
        print_status "Cleaning web build artifacts..."
        safe_remove_dir "web/build"
    fi

    print_success "✅ Flutter project cleaned: $project_dir"
}

# Main cleanup logic
if [ "$SINGLE_PROJECT" = true ]; then
    # Clean single project
    clean_flutter_project "$PROJECT_ROOT"
else
    # Find and clean all Flutter projects
    print_status "Finding all Flutter projects..."
    find . -name "pubspec.yaml" -type f | while read -r pubspec; do
        project_dir=$(dirname "$pubspec")
        clean_flutter_project "$project_dir"
        # Go back to project root
        cd "$PROJECT_ROOT"
    done
fi

# Clean global Flutter cache (optional)
read -p "Do you want to clean global Flutter cache? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    print_status "Cleaning global Flutter cache..."
    flutter pub cache clean
    print_success "Global Flutter cache cleaned"
fi

# Clean IDE-specific files
print_status "Cleaning IDE-specific files..."

# Remove VS Code files
safe_remove_dir ".vscode"

# Remove Android Studio files
safe_remove_dir ".idea"
safe_remove_dir "*.iml"

# Remove Xcode files
find . -name "*.xcworkspace" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "*.xcodeproj" -type d -exec rm -rf {} + 2>/dev/null || true

# Clean temporary files
print_status "Cleaning temporary files..."

# Remove .DS_Store files (macOS)
find . -name ".DS_Store" -type f -delete 2>/dev/null || true

# Remove Thumbs.db files (Windows)
find . -name "Thumbs.db" -type f -delete 2>/dev/null || true

# Remove temporary files
find . -name "*.tmp" -type f -delete 2>/dev/null || true
find . -name "*.temp" -type f -delete 2>/dev/null || true

# Clean log files
print_status "Cleaning log files..."
find . -name "*.log" -type f -delete 2>/dev/null || true

print_success "🎉 Flutter cleanup completed successfully!"
