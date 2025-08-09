# GitHub Wikis Quick Start Guide

## Overview

This guide provides a quick way to set up GitHub wikis from your FlyingDarts Turbo monorepo README files.

## Prerequisites

- Git installed and configured
- Access to your GitHub repository
- Bash shell (for running the automation script)

## Quick Setup (5 minutes)

### Step 1: Enable Wiki on GitHub

1. Go to your GitHub repository: `https://github.com/your-username/flyingdarts`
2. Click on the **"Wiki"** tab
3. Click **"Create the first page"** or **"Add page"**
4. This creates a wiki repository at: `https://github.com/your-username/flyingdarts.wiki.git`

### Step 2: Run the Automation Script

```bash
# From your project root directory
./scripts/setup-github-wikis.sh
```

This will:
- Create a `flyingdarts.wiki` directory
- Convert all README files to wiki pages with proper naming
- Set up navigation and structure
- Initialize a Git repository

### Step 3: Push to GitHub

```bash
cd flyingdarts.wiki
git remote add origin https://github.com/your-username/flyingdarts.wiki.git
git push -u origin main
```

### Step 4: Verify Setup

1. Go back to your GitHub repository
2. Click on the **"Wiki"** tab
3. You should see your organized wiki with all the documentation

## What Gets Created

The script creates a structured wiki with:

- **Home page** - Main landing page
- **Section index pages** - Overview pages for each major section
- **Individual documentation pages** - All README files converted to wiki pages
- **Navigation** - Sidebar with proper links

## File Structure

The wiki uses a flat file structure with dot-separated names that match your repository structure:

```
flyingdarts.wiki/
├── Home.md                                    # Main landing page
├── apps.md                                    # Applications overview
├── apps.backend.md                           # Backend applications overview
├── apps.frontend.md                          # Frontend applications overview
├── apps.tools.md                             # Tools overview
├── apps.backend.dotnet.api.md               # X01 Game API
├── apps.backend.dotnet.auth.md              # Authentication Service
├── apps.backend.dotnet.friends.md           # Friends API
├── apps.backend.dotnet.signalling.api.md    # Signalling Service
├── apps.backend.rust.authorizer.md          # Rust Authorizer
├── apps.frontend.angular.fd-app.md          # Angular Web App
├── apps.frontend.flutter.flyingdarts_mobile.md # Flutter Mobile App
├── apps.tools.dotnet.cdk.md                 # CDK Infrastructure
├── packages.md                               # Packages overview
├── packages.backend.md                       # Backend packages overview
├── packages.frontend.md                      # Frontend packages overview
├── packages.tools.md                         # Tool packages overview
├── packages.backend.dotnet.Flyingdarts.Connection.Services.md
├── packages.backend.dotnet.Flyingdarts.Core.md
├── packages.backend.dotnet.Flyingdarts.DynamoDb.Service.Flyingdarts.DynamoDb.Service.md
├── packages.backend.dotnet.Flyingdarts.Lambda.Core.md
├── packages.backend.dotnet.Flyingdarts.Meetings.Service.Flyingdarts.Meetings.Service.md
├── packages.backend.dotnet.Flyingdarts.Metadata.Services.Flyingdarts.Metadata.Services.md
├── packages.backend.dotnet.Flyingdarts.NotifyRooms.Service.Flyingdarts.NotifyRooms.Service.md
├── packages.backend.dotnet.Flyingdarts.Persistence.md
├── packages.backend.rust.auth.md
├── packages.frontend.flutter.api.sdk.md
├── packages.frontend.flutter.authress.login.md
├── packages.frontend.flutter.core.md
├── packages.frontend.flutter.features.keyboard.md
├── packages.frontend.flutter.features.language.md
├── packages.frontend.flutter.features.profile.md
├── packages.frontend.flutter.features.speech.md
├── packages.frontend.flutter.features.splash.md
├── packages.frontend.flutter.shared.config.app_config_core.md
├── packages.frontend.flutter.shared.config.app_config_prefs.md
├── packages.frontend.flutter.shared.config.app_config_secrets.md
├── packages.frontend.flutter.shared.internationalization.md
├── packages.frontend.flutter.shared.ui.md
├── packages.frontend.flutter.shared.websocket.md
├── packages.tools.config.md
├── packages.tools.dotnet.Flyingdarts.CDK.Constructs.md
├── docs.README.md                            # Documentation overview
├── docs.agent.README.md                      # Documentation agent
├── docs.apps.README.md                       # Apps documentation
├── docs.packages.README.md                   # Packages documentation
├── scripts.README.md                         # Scripts overview
├── scripts.dotnet.README.md                  # .NET scripts
├── scripts.flutter.workspace.README.md       # Flutter workspace scripts
├── _Sidebar.md                               # Navigation sidebar
└── README.md                                 # Wiki repository README
```

## Navigation Structure

The wiki uses proper GitHub wiki navigation with a comprehensive sidebar:

- **Complete repository overview** - All pages listed in the main sidebar
- **Organized sections** - Grouped by applications, packages, documentation, and scripts
- **Subsections** - Further organized by backend/frontend/tools and features/shared
- **Section index pages** - Overview pages with links to individual items
- **Breadcrumb navigation** - Back links to parent pages
- **Flat file structure** - No directory-based navigation issues

### Sidebar Structure

The main sidebar provides a complete overview of your entire repository:

```
# FlyingDarts Turbo Wiki

## Getting Started
- Home
- Documentation Overview

## Applications
### Backend Applications
- Backend Applications Overview
- X01 Game API
- Authentication Service
- Friends API
- Signalling Service
- Rust Authorizer

### Frontend Applications
- Frontend Applications Overview
- Angular Web App
- Flutter Mobile App

### Tools
- Tools Overview
- CDK Infrastructure

## Packages
### Backend Packages
- Backend Packages Overview
- Connection Services
- Core
- DynamoDB Service
- Lambda Core
- Meetings Service
- Metadata Services
- NotifyRooms Service
- Persistence
- Rust Auth

### Frontend Packages
- Frontend Packages Overview
- API SDK
- Authress Login
- Core

#### Features
- Keyboard Feature
- Language Feature
- Profile Feature
- Speech Feature
- Splash Feature

#### Shared
- App Config Core
- App Config Prefs
- App Config Secrets
- Internationalization
- UI
- WebSocket

### Tool Packages
- Tool Packages Overview
- Config
- CDK Constructs

## Documentation
- Documentation Overview
- Documentation Agent
- Apps Documentation
- Packages Documentation

## Scripts
- Scripts Overview
- .NET Scripts
- Flutter Workspace Scripts
```

## Customization

After running the script, you can:

1. **Review and edit** the generated files
2. **Add custom content** to any page
3. **Modify navigation** in `_Sidebar.md`
4. **Add images** or other media files
5. **Create additional pages** as needed

## Maintenance

### Updating Wiki from README Files

When you update README files in your main repository:

```bash
# Re-run the script to sync changes
./scripts/setup-github-wikis.sh

# Review changes and push
cd flyingdarts.wiki
git add .
git commit -m "Update wiki from README files"
git push
```

### Manual Updates

You can also edit wiki pages directly on GitHub or clone the wiki repository:

```bash
git clone https://github.com/your-username/flyingdarts.wiki.git
# Make changes
git add .
git commit -m "Update wiki content"
git push
```

## Troubleshooting

### Script Fails to Run

- Ensure you're in the project root directory
- Check that the script is executable: `chmod +x scripts/setup-github-wikis.sh`
- Verify all README files exist

### Wiki Not Showing on GitHub

- Ensure the wiki is enabled in repository settings
- Check that you've pushed to the correct branch (usually `main`)
- Verify the wiki repository URL is correct

### Navigation Issues

- Check that `_Sidebar.md` is properly formatted
- Ensure page links use correct wiki page names
- Verify that all referenced pages exist

## Advanced Options

### Dry Run

To see what would be created without actually creating files:

```bash
./scripts/setup-github-wikis.sh --dry-run
```

### Help

To see all available options:

```bash
./scripts/setup-github-wikis.sh --help
```

## Benefits

Setting up GitHub wikis provides:

- **Better organization** - Structured documentation
- **Easy navigation** - Sidebar and proper wiki links
- **Version control** - Track changes to documentation
- **Collaboration** - Multiple contributors can edit
- **Search functionality** - Built-in search across all pages
- **Mobile-friendly** - Responsive design
- **Proper structure** - Maintains repository folder structure in naming

## Key Features

- **Exact repository structure** - File names match your folder structure
- **Proper navigation** - No broken directory links
- **Section overviews** - Index pages for each major section
- **Source tracking** - Each page shows which README it came from
- **Maintenance workflow** - Easy to sync updates from README files

## Next Steps

1. **Customize content** - Add project-specific information
2. **Add screenshots** - Include visual documentation
3. **Create tutorials** - Step-by-step guides
4. **Set up templates** - Standardize page formats
5. **Establish workflow** - Process for keeping wiki updated

## Support

For issues or questions:
- Check the main setup guide: `scripts/setup-github-wikis.md`
- Review GitHub's wiki documentation
- Contact your development team 
