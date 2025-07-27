# Bash Script Optimization Rules

## Overview
All bash scripts in this project must be optimized for performance, reliability, and maintainability. These rules ensure consistent bash script development across the project.

## Core Principles

### 1. Shell Compatibility
- **Always use `/usr/bin/env bash` shebang**: `#!/usr/bin/env bash`
- **Set strict error handling**: `set -euo pipefail`
- **Avoid bashisms when possible** for better portability
- **Test scripts in multiple environments** (macOS, Linux)

### 2. Performance Optimization
- **Use built-in commands** over external processes when possible
- **Minimize subshells** and command substitutions
- **Use arrays efficiently** for data structures
- **Avoid unnecessary `cat` usage** - use redirection instead
- **Use `printf` over `echo`** for consistent output formatting

### 3. Error Handling
- **Always check command exit codes**
- **Provide meaningful error messages**
- **Use trap for cleanup**: `trap 'cleanup_function' EXIT`
- **Handle edge cases** (missing files, permissions, etc.)
- **Log errors with timestamps** when appropriate

### 4. State Machine Scripts
- **Use associative arrays** for state data: `declare -A STATE_DATA`
- **Implement proper state transitions** with validation
- **Handle state persistence** when needed
- **Provide clear state logging** and debugging output
- **Ensure state machine completion** with proper exit conditions

### 5. File Operations
- **Use atomic operations** when possible
- **Check file existence** before operations
- **Use temporary files** for intermediate data
- **Clean up temporary files** in cleanup functions
- **Use proper file permissions**

### 6. Variable Management
- **Use local variables** in functions: `local var_name`
- **Quote all variable expansions**: `"$variable"`
- **Use readonly for constants**: `readonly CONSTANT_NAME`
- **Avoid global variables** when possible
- **Use descriptive variable names**

### 7. Function Design
- **Keep functions small and focused**
- **Use meaningful function names**
- **Document function parameters** and return values
- **Return proper exit codes**
- **Use function composition** for complex operations

### 8. Input Validation
- **Validate all user inputs**
- **Check file paths** and permissions
- **Validate command-line arguments**
- **Handle empty or null values**
- **Use parameter expansion** for defaults

### 9. Output and Logging
- **Use consistent output formatting**
- **Implement proper logging levels**
- **Use colors for user feedback** (with fallbacks)
- **Provide progress indicators** for long operations
- **Use structured output** when appropriate

### 10. Testing and Debugging
- **Include debug mode**: `[[ "${DEBUG:-false}" == "true" ]]`
- **Add verbose output options**
- **Use bash debug mode**: `bash -x script.sh`
- **Test with different input scenarios**
- **Validate script syntax**: `bash -n script.sh`

## Code Examples

### Good State Machine Pattern
```bash
#!/usr/bin/env bash
set -euo pipefail

# State constants
declare -r INITIAL="initial"
declare -r COMPLETED="completed"

# State data
declare -A STATE_DATA=()
declare CURRENT_STATE=""
declare IS_COMPLETED=false

# State transition function
transition_to() {
    local new_state="$1"
    echo "🔄 Transitioning to: $new_state"
    CURRENT_STATE="$new_state"
    IS_COMPLETED=false
}

# State execution
execute_state() {
    case "$CURRENT_STATE" in
        "$INITIAL")
            # State logic here
            transition_to "$COMPLETED"
            ;;
        "$COMPLETED")
            IS_COMPLETED=true
            ;;
    esac
}

# Main loop
main() {
    transition_to "$INITIAL"
    while [[ "$IS_COMPLETED" != "true" ]]; do
        execute_state
    done
}
```

### Good Error Handling
```bash
#!/usr/bin/env bash
set -euo pipefail

# Cleanup function
cleanup() {
    local exit_code=$?
    [[ -f "/tmp/temp_file" ]] && rm -f "/tmp/temp_file"
    exit "$exit_code"
}

trap cleanup EXIT

# Error handling function
handle_error() {
    local message="$1"
    echo "❌ Error: $message" >&2
    exit 1
}

# Validate input
[[ -z "${1:-}" ]] && handle_error "Missing required argument"
```

### Good File Operations
```bash
#!/usr/bin/env bash
set -euo pipefail

# Safe file reading
read_file_safe() {
    local file_path="$1"
    [[ -f "$file_path" ]] || return 1
    [[ -r "$file_path" ]] || return 1
    cat "$file_path"
}

# Atomic file writing
write_file_atomic() {
    local file_path="$1"
    local content="$2"
    local temp_file
    temp_file="$(mktemp)"
    printf '%s' "$content" > "$temp_file"
    mv "$temp_file" "$file_path"
}
```

## Enforcement

### Pre-commit Checks
- **Syntax validation**: `bash -n script.sh`
- **ShellCheck analysis**: `shellcheck script.sh`
- **Test execution** in clean environment
- **Performance benchmarking** for critical scripts

### Code Review Checklist
- [ ] Uses proper shebang
- [ ] Has strict error handling
- [ ] Includes input validation
- [ ] Handles edge cases
- [ ] Uses efficient operations
- [ ] Includes proper documentation
- [ ] Follows naming conventions
- [ ] Has appropriate logging

### Performance Benchmarks
- **Execution time** under normal conditions
- **Memory usage** for large operations
- **CPU usage** during intensive tasks
- **I/O efficiency** for file operations

## Tools and Resources

### Recommended Tools
- **ShellCheck**: Static analysis for shell scripts
- **bashcov**: Code coverage for bash scripts
- **bats**: Testing framework for bash scripts
- **shfmt**: Shell script formatter

### Documentation
- **Bash Reference Manual**
- **Shell Scripting Best Practices**
- **POSIX Shell Standards**

## Migration Guidelines

### From PowerShell to Bash
- **Replace PowerShell cmdlets** with bash equivalents
- **Convert PowerShell arrays** to bash arrays
- **Replace PowerShell variables** with bash variables
- **Convert PowerShell functions** to bash functions
- **Update error handling** patterns

### Legacy Script Updates
- **Add strict error handling** to existing scripts
- **Implement proper state management**
- **Add input validation**
- **Improve error messages**
- **Optimize performance bottlenecks** 