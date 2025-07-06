#!/bin/bash

# Beachball Setup Script for Flyingdarts Monorepo
echo "🚀 Setting up Beachball for Flyingdarts..."

# Check if npm is available
if ! command -v npm &>/dev/null; then
    echo "❌ npm is not installed. Please install Node.js and npm first."
    exit 1
fi

# Function to get beachball version from package.json
get_beachball_version() {
    if [ -f "package.json" ]; then
        local version=$(grep '"beachball":' package.json | sed 's/.*"beachball": *"\([^"]*\)".*/\1/')
        if [ -n "$version" ]; then
            echo "$version"
            return 0
        fi
    fi
    # No version found in package.json - package not installed yet
    return 1
}

# Function to check if beachball is installed globally
check_global_beachball() {
    if command -v beachball &>/dev/null; then
        local current_version=$(beachball --version 2>/dev/null | head -n1)
        echo "🔍 Found global beachball installation: $current_version"
        return 0
    else
        echo "🔍 No global beachball installation found"
        return 1
    fi
}

# Function to check if beachball is installed locally
check_local_beachball() {
    if [ -f "package.json" ] && npm list beachball --depth=0 &>/dev/null; then
        local current_version=$(npm list beachball --depth=0 | grep beachball | awk '{print $2}' | sed 's/@//')
        echo "🔍 Found local beachball installation: $current_version"
        return 0
    else
        echo "🔍 No local beachball installation found"
        return 1
    fi
}

# Function to prompt user for upgrade
prompt_for_upgrade() {
    local installation_type=$1
    local current_version=$2
    local target_version=$3

    echo ""
    echo "🤔 $installation_type beachball is already installed (version: $current_version)"
    echo "   Target version: $target_version"
    echo ""
    read -p "   Do you want to upgrade to version $target_version? (y/N): " -n 1 -r
    echo ""

    if [[ $REPLY =~ ^[Yy]$ ]]; then
        return 0
    else
        return 1
    fi
}

# Get beachball version from package.json
if get_beachball_version; then
    beachball_version=$(get_beachball_version)
    echo "📋 Using beachball version from package.json: $beachball_version"
else
    echo "📋 No beachball version found in package.json - will install latest version"
    beachball_version="latest"
fi

# Check and handle global beachball installation
echo "🔍 Checking global beachball installation..."
if check_global_beachball; then
    current_global_version=$(beachball --version 2>/dev/null | head -n1)
    # Extract just the version number for comparison
    current_version_clean=$(echo "$current_global_version" | sed 's/beachball\///')

    # If using "latest" and versions are equal, skip silently
    if [ "$beachball_version" = "latest" ] && [ "$current_version_clean" = "$(npm view beachball version 2>/dev/null)" ]; then
        # Do nothing - versions are equal
        :
    else
        if prompt_for_upgrade "Global" "$current_global_version" "$beachball_version"; then
            echo "📦 Upgrading global beachball..."
            npm install -g beachball@$beachball_version
        else
            echo "⏭️  Skipping global beachball upgrade"
        fi
    fi
else
    echo "📦 Installing beachball globally..."
    npm install -g beachball@$beachball_version
fi

# Check and handle local beachball installation
echo ""
echo "🔍 Checking local beachball installation..."
if check_local_beachball; then
    current_local_version=$(npm list beachball --depth=0 | grep beachball | awk '{print $2}' | sed 's/@//')

    # If using "latest" and versions are equal, skip silently
    if [ "$beachball_version" = "latest" ] && [ "$current_local_version" = "$(npm view beachball version 2>/dev/null)" ]; then
        # Do nothing - versions are equal
        :
    else
        if prompt_for_upgrade "Local" "$current_local_version" "$beachball_version"; then
            echo "📦 Upgrading local beachball..."
            npm install beachball@$beachball_version --save-dev --no-workspaces
        else
            echo "⏭️  Skipping local beachball upgrade"
        fi
    fi
else
    echo "📦 Installing beachball as dev dependency..."
    npm install beachball@$beachball_version --save-dev --no-workspaces
fi

# Find all .csproj files and create changelog files in their directories (only if they don't exist)
echo ""
echo "🔍 Finding all .csproj files and creating changelog files..."
csproj_files=$(find . -name "*.csproj" -type f)

if [ -z "$csproj_files" ]; then
    echo "⚠️  No .csproj files found in the project"
else
    echo "📝 Creating CHANGELOG.md and CHANGELOG.json files for each .csproj project (if they don't exist)..."

    created_count=0
    skipped_count=0

    while IFS= read -r csproj_file; do
        # Get the directory containing the .csproj file
        project_dir=$(dirname "$csproj_file")
        project_name=$(basename "$csproj_file" .csproj)

        echo "   📁 Processing: $project_name in $project_dir"

        # Check if CHANGELOG.md already exists
        if [ -f "$project_dir/CHANGELOG.md" ]; then
            echo "      ⏭️  CHANGELOG.md already exists, skipping..."
            skipped_count=$((skipped_count + 1))
        else
            echo "      ✅ Creating CHANGELOG.md..."
            # Create CHANGELOG.md
            cat >"$project_dir/CHANGELOG.md" <<EOF
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial changelog setup

### Changed

### Deprecated

### Removed

### Fixed

### Security

EOF
            created_count=$((created_count + 1))
        fi

        # Check if CHANGELOG.json already exists
        if [ -f "$project_dir/CHANGELOG.json" ]; then
            echo "      ⏭️  CHANGELOG.json already exists, skipping..."
            skipped_count=$((skipped_count + 1))
        else
            echo "      ✅ Creating CHANGELOG.json..."
            # Create CHANGELOG.json
            cat >"$project_dir/CHANGELOG.json" <<EOF
{
  "name": "$project_name",
  "version": "0.1.0",
  "changes": [
    {
      "type": "added",
      "description": "Initial changelog setup",
      "version": "0.1.0"
    }
  ]
}
EOF
            created_count=$((created_count + 1))
        fi

    done <<<"$csproj_files"

    echo ""
    echo "📊 Summary:"
    echo "   ✅ Created: $created_count files"
    echo "   ⏭️  Skipped: $skipped_count files (already existed)"
    echo "   📁 Total projects processed: $(echo "$csproj_files" | wc -l | tr -d ' ')"
fi

echo ""
echo "✅ Beachball setup complete!"
echo ""
echo "📋 Next steps:"
echo "1. Run 'beachball check' to see current status"
echo "2. Run 'beachball change' to create change files"
echo "3. Run 'beachball bump' to bump versions"
echo "4. Check BEACHBALL.md for detailed usage instructions"
echo ""
echo "🔧 Available npm scripts:"
echo "  npm run beachball:check    - Check for pending changes"
echo "  npm run beachball:change   - Create change file"
echo "  npm run beachball:bump     - Bump versions"
echo "  npm run release:patch      - Patch release"
echo "  npm run release:minor      - Minor release"
echo "  npm run release:major      - Major release"
