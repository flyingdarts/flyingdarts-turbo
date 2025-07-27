#!/bin/sh

# Script to find and delete all .gitignore files except the one at the repo root

ROOT_GITIGNORE=".gitignore"
REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null || pwd)

echo "🔍 Searching for all .gitignore files except the root one..."

# Find all .gitignore files, excluding the root
GITIGNORE_FILES=$(find "$REPO_ROOT" -type f -name ".gitignore" ! -path "$REPO_ROOT/.gitignore")

if [ -z "$GITIGNORE_FILES" ]; then
    echo "✅ No extra .gitignore files found. Nothing to delete!"
    exit 0
fi

echo "🗂️ The following .gitignore files will be deleted:"
echo "$GITIGNORE_FILES" | while IFS= read -r file; do
    echo "  - $file"
done

echo ""
printf "⚠️  Are you sure you want to delete ALL these files? (y/N) "
read confirm

case "$confirm" in
y | Y)
    echo "$GITIGNORE_FILES" | while IFS= read -r file; do
        rm -f "$file" && echo "🗑️  Deleted: $file"
    done
    echo "🎉 All extra .gitignore files have been deleted!"
    ;;
*)
    echo "❌ Aborted. No files were deleted."
    ;;
esac
