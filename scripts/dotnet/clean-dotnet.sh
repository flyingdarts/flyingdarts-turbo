#!/bin/sh

# flyingdarts-turbo .NET Cleaner Script (POSIX sh compatible)
# This script will:
# 1. Find all bin and obj folders in .NET projects across the workspace
# 2. Remove them to free up disk space and ensure clean builds
# 3. Provide clear feedback about the operation

set -e

# Operate relative to the current working directory
ROOT_DIR="."
PACKAGE_JSON="$ROOT_DIR/package.json"

echo "🧹 .NET Cleaner for flyingdarts-turbo"
echo "🔍 Scanning for .NET projects..."

# Extract workspaces from package.json (more precise extraction)
WORKSPACES=$(grep -A 10 '"workspaces"' "$PACKAGE_JSON" | sed -n '/^  \],$/q;p' | grep -o '"[^"]*"' | sed 's/\"//g' | grep -v workspaces)

if [ -z "$WORKSPACES" ]; then
    echo "❌ No workspaces found in package.json. Exiting."
    exit 1
fi

# Find all bin and obj folders in the workspace directories
BIN_FOLDERS=""
OBJ_FOLDERS=""

for ws in $WORKSPACES; do
    # Find bin folders
    for folder in $(find $ws -type d -name "bin" 2>/dev/null); do
        BIN_FOLDERS="$BIN_FOLDERS\n$folder"
    done

    # Find obj folders
    for folder in $(find $ws -type d -name "obj" 2>/dev/null); do
        OBJ_FOLDERS="$OBJ_FOLDERS\n$folder"
    done
done

# Clean up the lists
BIN_FOLDERS=$(echo "$BIN_FOLDERS" | grep -v '^$' | sort | uniq)
OBJ_FOLDERS=$(echo "$OBJ_FOLDERS" | grep -v '^$' | sort | uniq)

BIN_COUNT=$(echo "$BIN_FOLDERS" | grep -c "bin" || echo "0")
OBJ_COUNT=$(echo "$OBJ_FOLDERS" | grep -c "obj" || echo "0")

echo "📊 Found $BIN_COUNT bin folders and $OBJ_COUNT obj folders"

if [ "$BIN_COUNT" -eq 0 ] && [ "$OBJ_COUNT" -eq 0 ]; then
    echo "✨ No .NET build folders found. Nothing to clean!"
    exit 0
fi

# Show what will be deleted
if [ "$BIN_COUNT" -gt 0 ]; then
    echo "📁 Bin folders to be removed:"
    echo "$BIN_FOLDERS" | while IFS= read folder; do
        if [ -n "$folder" ]; then
            echo "   - $folder"
        fi
    done
fi

if [ "$OBJ_COUNT" -gt 0 ]; then
    echo "📁 Obj folders to be removed:"
    echo "$OBJ_FOLDERS" | while IFS= read folder; do
        if [ -n "$folder" ]; then
            echo "   - $folder"
        fi
    done
fi

# Check if --force flag is provided
FORCE_MODE=false
if [ "$1" = "--force" ] || [ "$1" = "-f" ]; then
    FORCE_MODE=true
fi

if [ "$FORCE_MODE" = true ]; then
    echo "⚡ Force mode enabled - skipping confirmation"
else
    echo ""
    echo "⚠️  This will permanently delete all bin and obj folders."
    echo "   This will free up disk space but require rebuilding all projects."
    echo "Are you sure you want to DELETE all .NET build folders? This cannot be undone! (yes/NO): \c"
    read CONFIRM
    if [ "$CONFIRM" != "yes" ]; then
        echo "❌ Aborted by user."
        exit 1
    fi
fi

# Remove bin folders
if [ "$BIN_COUNT" -gt 0 ]; then
    echo "🗑️  Removing bin folders..."
    echo "$BIN_FOLDERS" | while IFS= read folder; do
        if [ -n "$folder" ] && [ -d "$folder" ]; then
            rm -rf "$folder"
            echo "   ✅ Removed: $folder"
        fi
    done
fi

# Remove obj folders
if [ "$OBJ_COUNT" -gt 0 ]; then
    echo "🗑️  Removing obj folders..."
    echo "$OBJ_FOLDERS" | while IFS= read folder; do
        if [ -n "$folder" ] && [ -d "$folder" ]; then
            rm -rf "$folder"
            echo "   ✅ Removed: $folder"
        fi
    done
fi

echo ""
echo "🎉 .NET cleanup completed successfully!"
echo "💡 Tip: Run 'dotnet restore' to restore packages and rebuild projects"
echo "💡 Tip: Run './scripts/dotnet/restore-sln.sh' to regenerate the solution file if needed"
