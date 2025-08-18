#!/bin/bash

# 🚀 Performance-Optimized E2E Test Runner
# This script runs the optimized e2e tests with performance configurations

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BUILD_DIR="$PROJECT_DIR/bin"
CONFIG_DIR="$PROJECT_DIR/Configuration"

echo -e "${BLUE}🚀 Flying Darts E2E Performance Test Runner${NC}"
echo -e "${BLUE}=============================================${NC}"
echo ""

# Check if we're in the right directory
if [[ ! -f "$PROJECT_DIR/Flyingdarts.E2E.csproj" ]]; then
    echo -e "${RED}❌ Error: Not in the E2E project directory${NC}"
    echo "Please run this script from the e2e directory"
    exit 1
fi

# Function to print section headers
print_section() {
    echo -e "\n${YELLOW}$1${NC}"
    echo -e "${YELLOW}$(printf '%.0s=' {1..${#1}})${NC}"
}

# Function to check prerequisites
check_prerequisites() {
    print_section "Checking Prerequisites"

    # Check .NET
    if ! command -v dotnet &> /dev/null; then
        echo -e "${RED}❌ .NET SDK not found${NC}"
        echo "Please install .NET 9.0 SDK"
        exit 1
    fi

    # Check .NET version
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✅ .NET SDK version: $DOTNET_VERSION${NC}"

    # Check Playwright
    if [[ ! -d "$PROJECT_DIR/bin/Debug/net9.0/playwright" ]]; then
        echo -e "${YELLOW}⚠️  Playwright not installed. Installing...${NC}"
        dotnet build
        npx playwright install chromium
    else
        echo -e "${GREEN}✅ Playwright already installed${NC}"
    fi
}

# Function to set performance environment variables
set_performance_env() {
    print_section "Setting Performance Environment Variables"

    # Performance optimizations
    export E2E_MAX_CONCURRENT_TESTS=1
    export E2E_BROWSER_POOL_SIZE=2
    export E2E_HEADLESS_MODE=false
    export E2E_ELEMENT_TIMEOUT=15000
    export E2E_NAVIGATION_TIMEOUT=15000
    export E2E_ENABLE_RESOURCE_BLOCKING=true
    export E2E_ENABLE_TOKEN_CACHING=true
    export E2E_TOKEN_CACHE_EXPIRATION=55

    # Browser optimizations
    export E2E_VIEWPORT_WIDTH=1600
    export E2E_VIEWPORT_HEIGHT=900
    export E2E_ENABLE_PERFORMANCE_MONITORING=true

    # Execution optimizations
    export E2E_ENABLE_PARALLEL=true
    export E2E_ENABLE_RETRY=true
    export E2E_MAX_RETRIES=2
    export E2E_ENABLE_PERFORMANCE_REPORTING=true

    # Resource blocking
    export E2E_BLOCK_IMAGES=false
    export E2E_BLOCK_CSS=false

    echo -e "${GREEN}✅ Performance environment variables set${NC}"

    # Print configuration
    echo -e "\n${BLUE}📊 Current Configuration:${NC}"
    echo "Max Concurrent Tests: $E2E_MAX_CONCURRENT_TESTS"
    echo "Browser Pool Size: $E2E_BROWSER_POOL_SIZE"
    echo "Element Timeout: ${E2E_ELEMENT_TIMEOUT}ms"
    echo "Navigation Timeout: ${E2E_NAVIGATION_TIMEOUT}ms"
    echo "Resource Blocking: $E2E_ENABLE_RESOURCE_BLOCKING"
    echo "Token Caching: $E2E_ENABLE_TOKEN_CACHING"
    echo "Parallel Execution: $E2E_ENABLE_PARALLEL"
}

# Function to clean build artifacts
clean_build() {
    print_section "Cleaning Build Artifacts"

    echo "Cleaning previous build..."
    dotnet clean --verbosity quiet

    echo "Removing bin and obj directories..."
    echo "This will delete $PROJECT_DIR/bin and $PROJECT_DIR/obj. Are you sure? (yes/no)"
    read -r CONFIRM
    if [[ "$CONFIRM" == "yes" ]]; then
        rm -rf "$PROJECT_DIR/bin" "$PROJECT_DIR/obj"
    else
        echo "Skipping deletion of bin/obj."
    fi

    echo -e "${GREEN}✅ Build artifacts cleaned${NC}"
}

# Function to build the project
build_project() {
    print_section "Building Project"

    echo "Building E2E project..."
    dotnet build --configuration Release --verbosity minimal

    if [[ $? -eq 0 ]]; then
        echo -e "${GREEN}✅ Project built successfully${NC}"
    else
        echo -e "${RED}❌ Build failed${NC}"
        exit 1
    fi
}

# Function to run tests
run_tests() {
    print_section "Running Performance Tests"

    echo "Starting performance-optimized test execution..."
    echo ""

    # Run tests with performance monitoring
    dotnet test --configuration Release \
        --logger "console;verbosity=detailed" \
        --logger "trx;LogFileName=performance-test-results.trx" \
        --collect:"XPlat Code Coverage" \
        --results-directory "$BUILD_DIR/TestResults" \
        --verbosity normal

    TEST_EXIT_CODE=$?

    if [[ $TEST_EXIT_CODE -eq 0 ]]; then
        echo -e "\n${GREEN}✅ All tests passed!${NC}"
    else
        echo -e "\n${RED}❌ Some tests failed${NC}"
    fi

    return $TEST_EXIT_CODE
}

# Function to generate performance report
generate_report() {
    print_section "Generating Performance Report"

    REPORT_DIR="$BUILD_DIR/TestResults/Performance"
    mkdir -p "$REPORT_DIR"

    echo "Generating performance report..."

    # Create a simple performance summary
    cat > "$REPORT_DIR/performance-summary.md" << EOF
# E2E Test Performance Report

Generated: $(date)

## Configuration
- Max Concurrent Tests: $E2E_MAX_CONCURRENT_TESTS
- Browser Pool Size: $E2E_BROWSER_POOL_SIZE
- Element Timeout: ${E2E_ELEMENT_TIMEOUT}ms
- Navigation Timeout: ${E2E_NAVIGATION_TIMEOUT}ms
- Resource Blocking: $E2E_ENABLE_RESOURCE_BLOCKING
- Token Caching: $E2E_ENABLE_TOKEN_CACHING

## Test Results
- Test Results: $BUILD_DIR/TestResults/
- Coverage: $BUILD_DIR/TestResults/

## Performance Optimizations Applied
✅ Browser pooling for instance reuse
✅ Token caching to reduce API calls
✅ Resource blocking for faster loading
✅ Parallel test execution
✅ Smart element waiting with retry logic
✅ Optimized timeouts and configurations
EOF

    echo -e "${GREEN}✅ Performance report generated: $REPORT_DIR/performance-summary.md${NC}"
}

# Function to show help
show_help() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -h, --help          Show this help message"
    echo "  -c, --clean         Clean build artifacts before building"
    echo "  -f, --fast          Skip cleaning and run tests directly"
    echo "  -r, --report        Generate performance report after tests"
    echo "  -v, --verbose       Enable verbose output"
    echo ""
    echo "Examples:"
    echo "  $0                  # Run full performance test suite"
    echo "  $0 --clean          # Clean and run tests"
    echo "  $0 --fast           # Run tests without cleaning"
    echo "  $0 --report         # Run tests and generate report"
}

# Main execution
main() {
    local CLEAN_BUILD=false
    local FAST_MODE=false
    local GENERATE_REPORT=false
    local VERBOSE=false

    # Parse command line arguments
    while [[ $# -gt 0 ]]; do
        case $1 in
            -h|--help)
                show_help
                exit 0
                ;;
            -c|--clean)
                CLEAN_BUILD=true
                shift
                ;;
            -f|--fast)
                FAST_MODE=true
                shift
                ;;
            -r|--report)
                GENERATE_REPORT=true
                shift
                ;;
            -v|--verbose)
                VERBOSE=true
                shift
                ;;
            *)
                echo -e "${RED}❌ Unknown option: $1${NC}"
                show_help
                exit 1
                ;;
        esac
    done

    # Set verbose mode
    if [[ "$VERBOSE" == true ]]; then
        set -x
    fi

    echo -e "${BLUE}🚀 Starting Performance Test Execution${NC}"
    echo "Project Directory: $PROJECT_DIR"
    echo "Build Directory: $BUILD_DIR"
    echo ""

    # Check prerequisites
    check_prerequisites

    # Set performance environment
    set_performance_env

    # Clean build if requested
    if [[ "$CLEAN_BUILD" == true ]]; then
        clean_build
    fi

    # Build project
    build_project

    # Run tests
    run_tests
    TESTS_EXIT_CODE=$?

    # Generate report if requested
    if [[ "$GENERATE_REPORT" == true ]]; then
        generate_report
    fi

    # Final status
    echo ""
    if [[ $TESTS_EXIT_CODE -eq 0 ]]; then
        echo -e "${GREEN}🎉 Performance test execution completed successfully!${NC}"
        echo -e "${GREEN}📊 Check the test results in: $BUILD_DIR/TestResults/${NC}"
    else
        echo -e "${RED}💥 Performance test execution completed with failures${NC}"
        echo -e "${YELLOW}📊 Check the test results in: $BUILD_DIR/TestResults/${NC}"
    fi

    echo ""
    echo -e "${BLUE}🚀 Performance optimizations applied:${NC}"
    echo "✅ Browser pooling and reuse"
    echo "✅ Token caching and optimization"
    echo "✅ Resource blocking for faster loading"
    echo "✅ Parallel test execution"
    echo "✅ Smart element waiting strategies"
    echo "✅ Optimized timeouts and configurations"

    exit $TESTS_EXIT_CODE
}

# Run main function with all arguments
main "$@"
