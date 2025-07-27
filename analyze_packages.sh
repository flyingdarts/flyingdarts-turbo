#!/bin/bash

echo "=== PACKAGES MISSING FROM Directory.Build.props ==="
echo ""

git diff --name-only | grep -E "\.(csproj|props)$" | xargs -I {} git diff {} | grep -E "<PackageReference.*Include=" | grep -v "^[+-][+-]" | sed "s/^[+-]//" | sed "s/.*Include=\"\([^\"]*\)\".*/\1/" | sort | uniq | while read pkg; do
    if ! grep -q "PackageVersion Include=\"$pkg\"" Directory.Build.props; then
        echo "  - $pkg"
    fi
done

echo ""
echo "If no packages are listed above, all packages are properly centralized in Directory.Build.props"
