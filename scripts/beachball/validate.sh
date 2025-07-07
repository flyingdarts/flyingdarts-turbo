#!/bin/bash

# Workflow Validation Script
# This script validates workflow YAML files against the JSON schema

set -e # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Default values
YAML_FILE="${SCRIPT_DIR}/states.yml"
SCHEMA_FILE="${SCRIPT_DIR}/workflow-schema.json"
VALIDATOR_SCRIPT="${SCRIPT_DIR}/validate-workflow.py"

# Function to print colored output
print_status() {
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

# Function to check if Python is available
check_python() {
    if ! command -v python3 &>/dev/null; then
        print_error "Python 3 is not installed or not in PATH"
        exit 1
    fi

    PYTHON_VERSION=$(python3 --version 2>&1 | cut -d' ' -f2)
    print_success "Found Python $PYTHON_VERSION"
}

# Function to install dependencies
install_dependencies() {
    print_status "Checking Python dependencies..."

    if ! python3 -c "import yaml, jsonschema" 2>/dev/null; then
        print_warning "Required Python packages not found. Installing..."

        if [ -f "${SCRIPT_DIR}/requirements.txt" ]; then
            python3 -m pip install -r "${SCRIPT_DIR}/requirements.txt"
            print_success "Dependencies installed successfully"
        else
            print_error "requirements.txt not found"
            exit 1
        fi
    else
        print_success "All dependencies are already installed"
    fi
}

# Function to validate file existence
validate_files() {
    if [ ! -f "$YAML_FILE" ]; then
        print_error "YAML file not found: $YAML_FILE"
        exit 1
    fi

    if [ ! -f "$SCHEMA_FILE" ]; then
        print_error "Schema file not found: $SCHEMA_FILE"
        exit 1
    fi

    if [ ! -f "$VALIDATOR_SCRIPT" ]; then
        print_error "Validator script not found: $VALIDATOR_SCRIPT"
        exit 1
    fi

    print_success "All required files found"
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [OPTIONS] [YAML_FILE]"
    echo ""
    echo "Options:"
    echo "  -h, --help     Show this help message"
    echo "  -s, --schema   Specify custom schema file"
    echo ""
    echo "Arguments:"
    echo "  YAML_FILE      Path to the YAML file to validate (default: states.yml)"
    echo ""
    echo "Examples:"
    echo "  $0                           # Validate default states.yml"
    echo "  $0 my-workflow.yml          # Validate custom YAML file"
    echo "  $0 -s custom-schema.json    # Use custom schema"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
    -h | --help)
        show_usage
        exit 0
        ;;
    -s | --schema)
        SCHEMA_FILE="$2"
        shift 2
        ;;
    -*)
        print_error "Unknown option: $1"
        show_usage
        exit 1
        ;;
    *)
        YAML_FILE="$1"
        shift
        ;;
    esac
done

# Main execution
main() {
    echo "🔍 Workflow Validation Script"
    echo "=============================="
    echo ""

    print_status "Checking environment..."
    check_python
    install_dependencies
    validate_files

    echo ""
    print_status "Starting validation..."
    echo ""

    # Run the Python validator
    if python3 "$VALIDATOR_SCRIPT" "$YAML_FILE"; then
        echo ""
        print_success "Validation completed successfully!"
        exit 0
    else
        echo ""
        print_error "Validation failed!"
        exit 1
    fi
}

# Run main function
main "$@"
