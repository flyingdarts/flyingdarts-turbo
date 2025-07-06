#!/bin/bash

# Beachball Wrapper Script for Flyingdarts Monorepo
# This script ensures Beachball runs from the correct directory

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Change to the project root directory
cd "$PROJECT_ROOT"

echo "📍 Running Beachball from: $(pwd)"

# Check if we're in a git repository
if [ ! -d ".git" ]; then
    echo "❌ Error: Not in a git repository"
    echo "   Current directory: $(pwd)"
    echo "   Expected to be in: $PROJECT_ROOT"
    exit 1
fi

echo "✅ Git repository found"

# Set environment variables to help Beachball find the git repository
export GIT_DIR="$PROJECT_ROOT/.git"
export GIT_WORK_TREE="$PROJECT_ROOT"
export GIT_ROOT="$PROJECT_ROOT"

echo "🔧 Set GIT_DIR to: $GIT_DIR"
echo "🔧 Set GIT_WORK_TREE to: $GIT_WORK_TREE"
echo "🔧 Set GIT_ROOT to: $GIT_ROOT"

# Set the working directory explicitly for Beachball
export BEACHBALL_ROOT="$PROJECT_ROOT"
echo "🔧 Set BEACHBALL_ROOT to: $BEACHBALL_ROOT"

# Debug: Show current working directory and list some files
echo "🔍 Current working directory: $(pwd)"
echo "🔍 Checking if package.json exists: $(ls -la package.json 2>/dev/null || echo 'package.json not found')"
echo "🔍 Checking if beachball.config.js exists: $(ls -la beachball.config.js 2>/dev/null || echo 'beachball.config.js not found')"

# Run Beachball with explicit working directory and root
echo "🚀 Running: npx beachball --root \"$PROJECT_ROOT\" \"$@\""
cd "$PROJECT_ROOT" && npx beachball --root "$PROJECT_ROOT" "$@"
