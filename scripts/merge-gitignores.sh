#!/bin/sh

# Script to merge all .gitignore files in the repo into root/.gitignore-merged
# Each file's rules are grouped with a header, and duplicate lines are removed
# For non-root .gitignore files, rules are made relative to their directory

REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null || pwd)
MERGED_FILE="$REPO_ROOT/.gitignore-merged"

# Find all .gitignore files (including root)
GITIGNORE_FILES=$(find "$REPO_ROOT" -type f -name ".gitignore")

if [ -z "$GITIGNORE_FILES" ]; then
    echo "❌ No .gitignore files found in the repository!"
    exit 1
fi

echo "📝 The following .gitignore files will be merged into $MERGED_FILE:"
echo "$GITIGNORE_FILES" | while IFS= read -r file; do
    echo "  - $file"
done

echo ""
printf "⚠️  Proceed with merging and overwriting %s? (y/N) " "$MERGED_FILE"
read confirm

case "$confirm" in
y | Y)
    # Merge with headers, make rules relative, then deduplicate
    {
        echo "$GITIGNORE_FILES" | while IFS= read -r file; do
            echo
            echo "############################################################"
            echo "# Rules from: ${file#$REPO_ROOT/}"
            echo "############################################################"
            dir=$(dirname "$file")
            rel_dir="${dir#$REPO_ROOT}"
            [ "$rel_dir" = "." ] && rel_dir=""
            while IFS= read -r line || [ -n "$line" ]; do
                # If line is blank or a comment, print as-is
                case "$line" in
                '' | '#'*) echo "$line" ;;
                *)
                    # If not root, prefix with relative dir
                    if [ -n "$rel_dir" ]; then
                        # Remove leading ./ if present
                        rel_dir_clean="${rel_dir#/}"
                        # Avoid double slashes
                        echo "$rel_dir_clean/${line#./}" | sed 's#//*#/#g'
                    else
                        echo "$line"
                    fi
                    ;;
                esac
            done <"$file"
        done
    } | awk '!seen[$0]++' >"$MERGED_FILE"
    echo "✅ All .gitignore files have been merged (with relative rules) and deduplicated into $MERGED_FILE! 🎉"
    ;;
*)
    echo "❌ Aborted. No files were merged."
    exit 0
    ;;
esac
