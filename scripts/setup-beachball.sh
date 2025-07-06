#!/bin/bash

# Beachball Setup Script for Flyingdarts Monorepo
echo "🚀 Setting up Beachball for Flyingdarts..."

# Check if npm is available
if ! command -v npm &> /dev/null; then
    echo "❌ npm is not installed. Please install Node.js and npm first."
    exit 1
fi

# Install Beachball globally (recommended for easier access)
echo "📦 Installing Beachball globally..."
npm install -g beachball

# Install Beachball as dev dependency in the project
echo "📦 Installing Beachball as dev dependency..."
npm install beachball@^2.35.0 --save-dev --no-workspaces

# Create .beachball directory for change files
echo "📁 Creating .beachball directory..."
mkdir -p .beachball

# Create initial changelog directories
echo "📝 Creating changelog directories..."
mkdir -p packages
mkdir -p apps/backend
mkdir -p apps/frontend
mkdir -p apps/tools

# Create initial changelog files
echo "📝 Creating initial changelog files..."
cat > packages/changelog.md << EOF
# Changelog

## [Unreleased]

### Added
- Initial changelog setup

EOF

cat > apps/backend/changelog.md << EOF
# Backend Changelog

## [Unreleased]

### Added
- Initial changelog setup

EOF

cat > apps/frontend/changelog.md << EOF
# Frontend Changelog

## [Unreleased]

### Added
- Initial changelog setup

EOF

cat > apps/tools/changelog.md << EOF
# Tools Changelog

## [Unreleased]

### Added
- Initial changelog setup

EOF

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