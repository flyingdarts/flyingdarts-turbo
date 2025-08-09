#!/bin/sh

# flyingdarts Solution Rebuilder Script (POSIX sh compatible)
# This script will:
# 1. Find all .csproj files in the repo, but only in directories specified by the workspaces in package.json
# 2. Delete the existing .sln file at the root (after confirmation)
# 3. Create a new solution file at the root
# 4. Add all projects to the new solution

set -e

# Operate relative to the current working directory
ROOT_DIR="."
SOLUTION_NAME="flyingdarts.sln"
SOLUTION_PATH="$ROOT_DIR/$SOLUTION_NAME"
PACKAGE_JSON="$ROOT_DIR/package.json"

# No cd to script location; operate from where the script is called

# Extract workspaces from package.json (assumes simple array of globs)
echo "🔍 Extracting workspaces from package.json..."
WORKSPACES=$(grep -A 10 '"workspaces"' "$PACKAGE_JSON" | grep -o '"[^"]*"' | sed 's/\"//g' | grep -v workspaces)

if [ -z "$WORKSPACES" ]; then
    echo "❌ No workspaces found in package.json. Exiting."
    exit 1
fi

echo "✅ Found workspaces:"
echo "$WORKSPACES" | while IFS= read ws; do
    echo "   - $ws"
done

# Find all .csproj files in the workspace directories
CSPROJ_FILES=""
for ws in $WORKSPACES; do
    # Expand the glob and find .csproj files
    for file in $(find $ws -name "*.csproj" 2>/dev/null); do
        CSPROJ_FILES="$CSPROJ_FILES\n$file"
    done
    # Also check if the workspace itself is a direct directory with .csproj
    if [ -d "$ws" ]; then
        for file in $(find "$ws" -maxdepth 1 -name "*.csproj" 2>/dev/null); do
            CSPROJ_FILES="$CSPROJ_FILES\n$file"
        done
    fi
    # Remove duplicates (if any)
    CSPROJ_FILES=$(echo "$CSPROJ_FILES" | sort | uniq)
done

CSPROJ_FILES=$(echo "$CSPROJ_FILES" | grep -v '^$')

if [ -z "$CSPROJ_FILES" ]; then
    echo "❌ No .csproj files found in workspaces. Exiting."
    exit 1
fi

CSPROJ_COUNT=$(echo "$CSPROJ_FILES" | grep -c ".csproj")
echo "✅ Found $CSPROJ_COUNT .csproj files in workspaces."

if [ -f "$SOLUTION_PATH" ]; then
    echo "⚠️  Solution file '$SOLUTION_NAME' already exists at the root."
    echo "Are you sure you want to DELETE it? This cannot be undone! (yes/NO): \c"
    read CONFIRM
    if [ "$CONFIRM" != "yes" ]; then
        echo "❌ Aborted by user."
        exit 1
    fi
    rm "$SOLUTION_PATH"
    echo "🗑️  Deleted existing solution file."
fi

echo "🆕 Creating new solution file: $SOLUTION_NAME"
dotnet new sln -n "flyingdarts" --output "$ROOT_DIR"

# Add each project
echo "$CSPROJ_FILES" | while IFS= read PROJECT; do
    if [ -n "$PROJECT" ]; then
        echo "➕ Adding project: $PROJECT"
        dotnet sln "$SOLUTION_PATH" add "$PROJECT"
    fi
done

echo "🎉 Solution rebuilt successfully! All projects added."
