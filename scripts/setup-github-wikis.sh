#!/bin/bash

# GitHub Wikis Setup Script for FlyingDarts Turbo Monorepo
# This script helps automate the creation of GitHub wiki pages from README files

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
REPO_NAME="flyingdarts"
WIKI_DIR="${REPO_NAME}.wiki"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_header() {
    echo -e "${BLUE}=== $1 ===${NC}"
}

# Function to convert path to wiki page name
path_to_wiki_name() {
    local path="$1"
    # Convert path to wiki page name, keeping original structure
    echo "$path" | sed 's|^\./||' | sed 's|/|.|g' | sed 's|\.md$||'
}

# Function to convert README content to wiki format
convert_readme_to_wiki() {
    local readme_file="$1"
    local wiki_file="$2"
    local title="$3"
    local source_path="$4"

    if [[ ! -f "$readme_file" ]]; then
        print_warning "README file not found: $readme_file"
        return 1
    fi

    # Create directory if it doesn't exist
    mkdir -p "$(dirname "$wiki_file")"

    # Convert README to wiki format
    {
        echo "# $title"
        echo ""
        echo "> This page was automatically generated from the README file in the main repository."
        echo ""
        echo "**Source:** \`$source_path\`"
        echo ""
        echo "---"
        echo ""

        # Copy README content, skipping the first line if it's a title
        if [[ "$(head -n1 "$readme_file")" =~ ^#\ .* ]]; then
            tail -n +2 "$readme_file"
        else
            cat "$readme_file"
        fi

        echo ""
        echo "---"
        echo ""
        echo "**Last Updated:** $(date '+%Y-%m-%d')"
        echo ""
        echo "[← Back to Wiki Home](Home)"
    } > "$wiki_file"

    print_status "Created wiki page: $wiki_file"
}

# Function to create index page for a section
create_section_index() {
    local section_path="$1"
    local section_name="$2"
    local items=("${@:3}")

    local index_file="$section_path.md"

    {
        echo "# $section_name"
        echo ""
        echo "This section contains documentation for $section_name."
        echo ""
        echo "## Pages"
        echo ""

        for item in "${items[@]}"; do
            local item_name=$(basename "$item" .md)
            local display_name=$(echo "$item_name" | sed 's/-/ /g' | sed 's/\b\w/\U&/g')
            echo "- [$display_name]($item_name)"
        done

        echo ""
        echo "---"
        echo ""
        echo "[← Back to Wiki Home](Home)"
    } > "$index_file"

    print_status "Created section index: $index_file"
}

# Main setup function
setup_wiki() {
    print_header "Setting up GitHub Wiki for FlyingDarts Turbo"

    # Check if we're in the right directory
    if [[ ! -f "$PROJECT_ROOT/README.md" ]]; then
        print_error "Please run this script from the project root directory"
        exit 1
    fi

    # Create wiki directory
    if [[ -d "$WIKI_DIR" ]]; then
        print_warning "Wiki directory already exists: $WIKI_DIR"
        read -p "Do you want to continue and overwrite? (y/N): " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            print_status "Setup cancelled"
            exit 0
        fi
        rm -rf "$WIKI_DIR"
    fi

    mkdir -p "$WIKI_DIR"
    cd "$WIKI_DIR"

    print_status "Created wiki directory: $WIKI_DIR"

    # Convert Apps README files
    print_header "Converting Apps README files"

    convert_readme_to_wiki "$PROJECT_ROOT/README.md" "Home.md" "FlyingDarts Turbo" "README.md"

    # Backend Apps
    convert_readme_to_wiki "$PROJECT_ROOT/apps/backend/dotnet/api/README.md" "apps.backend.dotnet.api.md" "X01 Game API" "apps/backend/dotnet/api/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/apps/backend/dotnet/auth/README.md" "apps.backend.dotnet.auth.md" "Authentication Service" "apps/backend/dotnet/auth/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/apps/backend/dotnet/friends/README.md" "apps.backend.dotnet.friends.md" "Friends API" "apps/backend/dotnet/friends/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/apps/backend/dotnet/signalling/api/README.md" "apps.backend.dotnet.signalling.api.md" "Signalling Service" "apps/backend/dotnet/signalling/api/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/apps/backend/rust/authorizer/README.md" "apps.backend.rust.authorizer.md" "Rust Authorizer" "apps/backend/rust/authorizer/README.md"

    # Frontend Apps
    convert_readme_to_wiki "$PROJECT_ROOT/apps/frontend/angular/fd-app/README.md" "apps.frontend.angular.fd-app.md" "Angular Web App" "apps/frontend/angular/fd-app/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/apps/frontend/flutter/flyingdarts_mobile/README.md" "apps.frontend.flutter.flyingdarts_mobile.md" "Flutter Mobile App" "apps/frontend/flutter/flyingdarts_mobile/README.md"

    # Tools
    convert_readme_to_wiki "$PROJECT_ROOT/apps/tools/dotnet/cdk/README.md" "apps.tools.dotnet.cdk.md" "CDK Infrastructure" "apps/tools/dotnet/cdk/README.md"

    # Convert Packages README files
    print_header "Converting Packages README files"

    # Backend Packages
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.Connection.Services/README.md" "packages.backend.dotnet.Flyingdarts.Connection.Services.md" "Connection Services" "packages/backend/dotnet/Flyingdarts.Connection.Services/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.Core/README.md" "packages.backend.dotnet.Flyingdarts.Core.md" "Core" "packages/backend/dotnet/Flyingdarts.Core/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.DynamoDb.Service/Flyingdarts.DynamoDb.Service/README.md" "packages.backend.dotnet.Flyingdarts.DynamoDb.Service.Flyingdarts.DynamoDb.Service.md" "DynamoDB Service" "packages/backend/dotnet/Flyingdarts.DynamoDb.Service/Flyingdarts.DynamoDb.Service/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.Lambda.Core/README.md" "packages.backend.dotnet.Flyingdarts.Lambda.Core.md" "Lambda Core" "packages/backend/dotnet/Flyingdarts.Lambda.Core/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.Meetings.Service/Flyingdarts.Meetings.Service/README.md" "packages.backend.dotnet.Flyingdarts.Meetings.Service.Flyingdarts.Meetings.Service.md" "Meetings Service" "packages/backend/dotnet/Flyingdarts.Meetings.Service/Flyingdarts.Meetings.Service/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.Metadata.Services/Flyingdarts.Metadata.Services/README.md" "packages.backend.dotnet.Flyingdarts.Metadata.Services.Flyingdarts.Metadata.Services.md" "Metadata Services" "packages/backend/dotnet/Flyingdarts.Metadata.Services/Flyingdarts.Metadata.Services/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.NotifyRooms.Service/Flyingdarts.NotifyRooms.Service/README.md" "packages.backend.dotnet.Flyingdarts.NotifyRooms.Service.Flyingdarts.NotifyRooms.Service.md" "NotifyRooms Service" "packages/backend/dotnet/Flyingdarts.NotifyRooms.Service/Flyingdarts.NotifyRooms.Service/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/dotnet/Flyingdarts.Persistence/README.md" "packages.backend.dotnet.Flyingdarts.Persistence.md" "Persistence" "packages/backend/dotnet/Flyingdarts.Persistence/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/backend/rust/auth/README.md" "packages.backend.rust.auth.md" "Rust Auth" "packages/backend/rust/auth/README.md"

    # Frontend Packages
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/api/sdk/README.md" "packages.frontend.flutter.api.sdk.md" "API SDK" "packages/frontend/flutter/api/sdk/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/authress/login/README.md" "packages.frontend.flutter.authress.login.md" "Authress Login" "packages/frontend/flutter/authress/login/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/core/README.md" "packages.frontend.flutter.core.md" "Core" "packages/frontend/flutter/core/README.md"

    # Frontend Features
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/features/keyboard/README.md" "packages.frontend.flutter.features.keyboard.md" "Keyboard Feature" "packages/frontend/flutter/features/keyboard/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/features/language/README.md" "packages.frontend.flutter.features.language.md" "Language Feature" "packages/frontend/flutter/features/language/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/features/profile/README.md" "packages.frontend.flutter.features.profile.md" "Profile Feature" "packages/frontend/flutter/features/profile/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/features/speech/README.md" "packages.frontend.flutter.features.speech.md" "Speech Feature" "packages/frontend/flutter/features/speech/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/features/splash/README.md" "packages.frontend.flutter.features.splash.md" "Splash Feature" "packages/frontend/flutter/features/splash/README.md"

    # Frontend Shared
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/shared/config/app_config_core/README.md" "packages.frontend.flutter.shared.config.app_config_core.md" "App Config Core" "packages/frontend/flutter/shared/config/app_config_core/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/shared/config/app_config_prefs/README.md" "packages.frontend.flutter.shared.config.app_config_prefs.md" "App Config Prefs" "packages/frontend/flutter/shared/config/app_config_prefs/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/shared/config/app_config_secrets/README.md" "packages.frontend.flutter.shared.config.app_config_secrets.md" "App Config Secrets" "packages/frontend/flutter/shared/config/app_config_secrets/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/shared/internationalization/README.md" "packages.frontend.flutter.shared.internationalization.md" "Internationalization" "packages/frontend/flutter/shared/internationalization/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/shared/ui/README.md" "packages.frontend.flutter.shared.ui.md" "UI" "packages/frontend/flutter/shared/ui/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/frontend/flutter/shared/websocket/README.md" "packages.frontend.flutter.shared.websocket.md" "WebSocket" "packages/frontend/flutter/shared/websocket/README.md"

    # Tool Packages
    convert_readme_to_wiki "$PROJECT_ROOT/packages/tools/config/README.md" "packages.tools.config.md" "Config" "packages/tools/config/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/packages/tools/dotnet/Flyingdarts.CDK.Constructs/README.md" "packages.tools.dotnet.Flyingdarts.CDK.Constructs.md" "CDK Constructs" "packages/tools/dotnet/Flyingdarts.CDK.Constructs/README.md"

    # Convert Documentation README files
    print_header "Converting Documentation README files"

    convert_readme_to_wiki "$PROJECT_ROOT/docs/README.md" "docs.README.md" "Documentation Overview" "docs/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/docs/agent/README.md" "docs.agent.README.md" "Documentation Agent" "docs/agent/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/docs/apps/README.md" "docs.apps.README.md" "Apps Documentation" "docs/apps/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/docs/packages/README.md" "docs.packages.README.md" "Packages Documentation" "docs/packages/README.md"

    # Convert Scripts README files
    print_header "Converting Scripts README files"

    convert_readme_to_wiki "$PROJECT_ROOT/scripts/README.md" "scripts.README.md" "Scripts Overview" "scripts/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/scripts/dotnet/README.md" "scripts.dotnet.README.md" ".NET Scripts" "scripts/dotnet/README.md"
    convert_readme_to_wiki "$PROJECT_ROOT/scripts/flutter/workspace/README.md" "scripts.flutter.workspace.README.md" "Flutter Workspace Scripts" "scripts/flutter/workspace/README.md"

    # Create section index pages
    print_header "Creating section index pages"

    # Apps Backend index
    create_section_index "apps.backend" "Backend Applications" \
        "apps.backend.dotnet.api.md" \
        "apps.backend.dotnet.auth.md" \
        "apps.backend.dotnet.friends.md" \
        "apps.backend.dotnet.signalling.api.md" \
        "apps.backend.rust.authorizer.md"

    # Apps Frontend index
    create_section_index "apps.frontend" "Frontend Applications" \
        "apps.frontend.angular.fd-app.md" \
        "apps.frontend.flutter.flyingdarts_mobile.md"

    # Apps Tools index
    create_section_index "apps.tools" "Tools" \
        "apps.tools.dotnet.cdk.md"

    # Apps index
    create_section_index "apps" "Applications" \
        "apps.backend.md" \
        "apps.frontend.md" \
        "apps.tools.md"

    # Packages Backend index
    create_section_index "packages.backend" "Backend Packages" \
        "packages.backend.dotnet.Flyingdarts.Connection.Services.md" \
        "packages.backend.dotnet.Flyingdarts.Core.md" \
        "packages.backend.dotnet.Flyingdarts.DynamoDb.Service.Flyingdarts.DynamoDb.Service.md" \
        "packages.backend.dotnet.Flyingdarts.Lambda.Core.md" \
        "packages.backend.dotnet.Flyingdarts.Meetings.Service.Flyingdarts.Meetings.Service.md" \
        "packages.backend.dotnet.Flyingdarts.Metadata.Services.Flyingdarts.Metadata.Services.md" \
        "packages.backend.dotnet.Flyingdarts.NotifyRooms.Service.Flyingdarts.NotifyRooms.Service.md" \
        "packages.backend.dotnet.Flyingdarts.Persistence.md" \
        "packages.backend.rust.auth.md"

    # Packages Frontend index
    create_section_index "packages.frontend" "Frontend Packages" \
        "packages.frontend.flutter.api.sdk.md" \
        "packages.frontend.flutter.authress.login.md" \
        "packages.frontend.flutter.core.md" \
        "packages.frontend.flutter.features.keyboard.md" \
        "packages.frontend.flutter.features.language.md" \
        "packages.frontend.flutter.features.profile.md" \
        "packages.frontend.flutter.features.speech.md" \
        "packages.frontend.flutter.features.splash.md" \
        "packages.frontend.flutter.shared.config.app_config_core.md" \
        "packages.frontend.flutter.shared.config.app_config_prefs.md" \
        "packages.frontend.flutter.shared.config.app_config_secrets.md" \
        "packages.frontend.flutter.shared.internationalization.md" \
        "packages.frontend.flutter.shared.ui.md" \
        "packages.frontend.flutter.shared.websocket.md"

    # Packages Tools index
    create_section_index "packages.tools" "Tool Packages" \
        "packages.tools.config.md" \
        "packages.tools.dotnet.Flyingdarts.CDK.Constructs.md"

    # Packages index
    create_section_index "packages" "Packages" \
        "packages.backend.md" \
        "packages.frontend.md" \
        "packages.tools.md"

    # Documentation index
    create_section_index "docs" "Documentation" \
        "docs.README.md" \
        "docs.agent.README.md" \
        "docs.apps.README.md" \
        "docs.packages.README.md"

    # Scripts index
    create_section_index "scripts" "Scripts" \
        "scripts.README.md" \
        "scripts.dotnet.README.md" \
        "scripts.flutter.workspace.README.md"

    # Create navigation files
    print_header "Creating navigation files"

    # Create _Sidebar.md
    cat > "_Sidebar.md" << 'EOF'
# FlyingDarts Turbo Wiki

## Getting Started
- [Home](Home)
- [Documentation Overview](docs.README)

## Applications

### Backend Applications
- [Backend Applications Overview](apps.backend)
- [X01 Game API](apps.backend.dotnet.api)
- [Authentication Service](apps.backend.dotnet.auth)
- [Friends API](apps.backend.dotnet.friends)
- [Signalling Service](apps.backend.dotnet.signalling.api)
- [Rust Authorizer](apps.backend.rust.authorizer)

### Frontend Applications
- [Frontend Applications Overview](apps.frontend)
- [Angular Web App](apps.frontend.angular.fd-app)
- [Flutter Mobile App](apps.frontend.flutter.flyingdarts_mobile)

### Tools
- [Tools Overview](apps.tools)
- [CDK Infrastructure](apps.tools.dotnet.cdk)

## Packages

### Backend Packages
- [Backend Packages Overview](packages.backend)
- [Connection Services](packages.backend.dotnet.Flyingdarts.Connection.Services)
- [Core](packages.backend.dotnet.Flyingdarts.Core)
- [DynamoDB Service](packages.backend.dotnet.Flyingdarts.DynamoDb.Service.Flyingdarts.DynamoDb.Service)
- [Lambda Core](packages.backend.dotnet.Flyingdarts.Lambda.Core)
- [Meetings Service](packages.backend.dotnet.Flyingdarts.Meetings.Service.Flyingdarts.Meetings.Service)
- [Metadata Services](packages.backend.dotnet.Flyingdarts.Metadata.Services.Flyingdarts.Metadata.Services)
- [NotifyRooms Service](packages.backend.dotnet.Flyingdarts.NotifyRooms.Service.Flyingdarts.NotifyRooms.Service)
- [Persistence](packages.backend.dotnet.Flyingdarts.Persistence)
- [Rust Auth](packages.backend.rust.auth)

### Frontend Packages
- [Frontend Packages Overview](packages.frontend)
- [API SDK](packages.frontend.flutter.api.sdk)
- [Authress Login](packages.frontend.flutter.authress.login)
- [Core](packages.frontend.flutter.core)

#### Features
- [Keyboard Feature](packages.frontend.flutter.features.keyboard)
- [Language Feature](packages.frontend.flutter.features.language)
- [Profile Feature](packages.frontend.flutter.features.profile)
- [Speech Feature](packages.frontend.flutter.features.speech)
- [Splash Feature](packages.frontend.flutter.features.splash)

#### Shared
- [App Config Core](packages.frontend.flutter.shared.config.app_config_core)
- [App Config Prefs](packages.frontend.flutter.shared.config.app_config_prefs)
- [App Config Secrets](packages.frontend.flutter.shared.config.app_config_secrets)
- [Internationalization](packages.frontend.flutter.shared.internationalization)
- [UI](packages.frontend.flutter.shared.ui)
- [WebSocket](packages.frontend.flutter.shared.websocket)

### Tool Packages
- [Tool Packages Overview](packages.tools)
- [Config](packages.tools.config)
- [CDK Constructs](packages.tools.dotnet.Flyingdarts.CDK.Constructs)

## Documentation
- [Documentation Overview](docs.README)
- [Documentation Agent](docs.agent.README)
- [Apps Documentation](docs.apps.README)
- [Packages Documentation](docs.packages.README)

## Scripts
- [Scripts Overview](scripts.README)
- [.NET Scripts](scripts.dotnet.README)
- [Flutter Workspace Scripts](scripts.flutter.workspace.README)
EOF

    print_status "Created navigation files"

    # Create README for the wiki repository
    cat > "README.md" << 'EOF'
# FlyingDarts Turbo Wiki

This is the GitHub wiki for the FlyingDarts Turbo monorepo. It contains documentation for all applications, packages, and tools in the project.

## Getting Started

1. Start with the [Home](Home) page for an overview
2. Explore the [Documentation Overview](docs.README) for complete documentation
3. Navigate through the sidebar to find specific information

## Structure

- **Applications** - Application documentation (backend, frontend, tools)
- **Packages** - Shared library documentation
- **Documentation** - General documentation and guides
- **Scripts** - Development and maintenance scripts

## Contributing

To contribute to this wiki:

1. Clone this repository: `git clone https://github.com/your-username/flyingdarts.wiki.git`
2. Make your changes
3. Commit and push: `git add . && git commit -m "Update wiki" && git push`

## Sync with Main Repository

This wiki is automatically generated from README files in the main repository. To update:

1. Update the README files in the main repository
2. Run the wiki setup script: `./scripts/setup-github-wikis.sh`
3. Review and commit changes to the wiki repository
EOF

    print_status "Created wiki README"

    # Initialize git repository
    git init
    git add .
    git commit -m "Initial wiki setup from README files"

    print_header "Setup Complete!"
    print_status "Wiki files have been created in: $WIKI_DIR"
    print_status ""
    print_status "Next steps:"
    print_status "1. Review the generated files"
    print_status "2. Customize content as needed"
    print_status "3. Push to GitHub:"
    print_status "   cd $WIKI_DIR"
    print_status "   git remote add origin https://github.com/your-username/$REPO_NAME.wiki.git"
    print_status "   git push -u origin main"
    print_status ""
    print_status "4. Enable the wiki in your GitHub repository settings"
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -h, --help     Show this help message"
    echo "  --dry-run      Show what would be created without actually creating files"
    echo ""
    echo "This script sets up a GitHub wiki from README files in the FlyingDarts Turbo monorepo."
}

# Parse command line arguments
DRY_RUN=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_usage
            exit 0
            ;;
        --dry-run)
            DRY_RUN=true
            shift
            ;;
        *)
            print_error "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Run the setup
if [[ "$DRY_RUN" == "true" ]]; then
    print_header "DRY RUN - No files will be created"
    print_status "This would create a wiki structure with the following files:"
    echo ""
    echo "flyingdarts.wiki/"
    echo "├── Home.md"
    echo "├── apps.backend.md"
    echo "├── apps.frontend.md"
    echo "├── apps.tools.md"
    echo "├── apps.backend.dotnet.api.md"
    echo "├── apps.backend.dotnet.auth.md"
    echo "├── apps.backend.dotnet.friends.md"
    echo "├── apps.backend.dotnet.signalling.api.md"
    echo "├── apps.backend.rust.authorizer.md"
    echo "├── apps.frontend.angular.fd-app.md"
    echo "├── apps.frontend.flutter.flyingdarts_mobile.md"
    echo "├── apps.tools.dotnet.cdk.md"
    echo "├── packages.backend.md"
    echo "├── packages.frontend.md"
    echo "├── packages.tools.md"
    echo "├── packages.backend.dotnet.Flyingdarts.Connection.Services.md"
    echo "├── packages.backend.dotnet.Flyingdarts.Core.md"
    echo "├── packages.backend.dotnet.Flyingdarts.DynamoDb.Service.Flyingdarts.DynamoDb.Service.md"
    echo "├── packages.backend.dotnet.Flyingdarts.Lambda.Core.md"
    echo "├── packages.backend.dotnet.Flyingdarts.Meetings.Service.Flyingdarts.Meetings.Service.md"
    echo "├── packages.backend.dotnet.Flyingdarts.Metadata.Services.Flyingdarts.Metadata.Services.md"
    echo "├── packages.backend.dotnet.Flyingdarts.NotifyRooms.Service.Flyingdarts.NotifyRooms.Service.md"
    echo "├── packages.backend.dotnet.Flyingdarts.Persistence.md"
    echo "├── packages.backend.rust.auth.md"
    echo "├── packages.frontend.flutter.api.sdk.md"
    echo "├── packages.frontend.flutter.authress.login.md"
    echo "├── packages.frontend.flutter.core.md"
    echo "├── packages.frontend.flutter.features.keyboard.md"
    echo "├── packages.frontend.flutter.features.language.md"
    echo "├── packages.frontend.flutter.features.profile.md"
    echo "├── packages.frontend.flutter.features.speech.md"
    echo "├── packages.frontend.flutter.features.splash.md"
    echo "├── packages.frontend.flutter.shared.config.app_config_core.md"
    echo "├── packages.frontend.flutter.shared.config.app_config_prefs.md"
    echo "├── packages.frontend.flutter.shared.config.app_config_secrets.md"
    echo "├── packages.frontend.flutter.shared.internationalization.md"
    echo "├── packages.frontend.flutter.shared.ui.md"
    echo "├── packages.frontend.flutter.shared.websocket.md"
    echo "├── packages.tools.config.md"
    echo "├── packages.tools.dotnet.Flyingdarts.CDK.Constructs.md"
    echo "├── docs.README.md"
    echo "├── docs.agent.README.md"
    echo "├── docs.apps.README.md"
    echo "├── docs.packages.README.md"
    echo "├── scripts.README.md"
    echo "├── scripts.dotnet.README.md"
    echo "├── scripts.flutter.workspace.README.md"
    echo "├── _Sidebar.md"
    echo "└── README.md"
    echo ""
    print_status "Run without --dry-run to actually create the files"
else
    setup_wiki
fi
