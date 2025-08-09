# GitHub Wikis Setup Guide

This guide explains how to set up GitHub wikis from the README files in your FlyingDarts Turbo monorepo.

## Overview

GitHub wikis are separate Git repositories that can be cloned, edited locally, and pushed back to GitHub. Each repository can have its own wiki, and you can create multiple wiki pages.

## Current README Files in Your Monorepo

Based on the analysis of your monorepo, here are the README files that can be converted to GitHub wiki pages:

### Apps (Applications)
- `./README.md` - Main project README
- `./apps/backend/dotnet/api/README.md` - X01 Game API
- `./apps/backend/dotnet/auth/README.md` - Authentication Service
- `./apps/backend/dotnet/friends/README.md` - Friends API
- `./apps/backend/dotnet/signalling/api/README.md` - Signalling Service
- `./apps/backend/rust/authorizer/README.md` - Rust Authorizer
- `./apps/frontend/angular/fd-app/README.md` - Angular Web App
- `./apps/frontend/flutter/flyingdarts_mobile/README.md` - Flutter Mobile App
- `./apps/tools/dotnet/cdk/README.md` - CDK Infrastructure

### Packages (Shared Libraries)
- `./packages/backend/dotnet/Flyingdarts.Connection.Services/README.md`
- `./packages/backend/dotnet/Flyingdarts.Core/README.md`
- `./packages/backend/dotnet/Flyingdarts.DynamoDb.Service/Flyingdarts.DynamoDb.Service/README.md`
- `./packages/backend/dotnet/Flyingdarts.Lambda.Core/README.md`
- `./packages/backend/dotnet/Flyingdarts.Meetings.Service/Flyingdarts.Meetings.Service/README.md`
- `./packages/backend/dotnet/Flyingdarts.Metadata.Services/Flyingdarts.Metadata.Services/README.md`
- `./packages/backend/dotnet/Flyingdarts.NotifyRooms.Service/Flyingdarts.NotifyRooms.Service/README.md`
- `./packages/backend/dotnet/Flyingdarts.Persistence/README.md`
- `./packages/backend/rust/auth/README.md`
- `./packages/frontend/flutter/api/sdk/README.md`
- `./packages/frontend/flutter/authress/login/README.md`
- `./packages/frontend/flutter/core/README.md`
- `./packages/frontend/flutter/features/keyboard/README.md`
- `./packages/frontend/flutter/features/language/README.md`
- `./packages/frontend/flutter/features/profile/README.md`
- `./packages/frontend/flutter/features/speech/README.md`
- `./packages/frontend/flutter/features/splash/README.md`
- `./packages/frontend/flutter/shared/config/app_config_core/README.md`
- `./packages/frontend/flutter/shared/config/app_config_prefs/README.md`
- `./packages/frontend/flutter/shared/config/app_config_secrets/README.md`
- `./packages/frontend/flutter/shared/internationalization/README.md`
- `./packages/frontend/flutter/shared/ui/README.md`
- `./packages/frontend/flutter/shared/websocket/README.md`
- `./packages/tools/config/README.md`
- `./packages/tools/dotnet/Flyingdarts.CDK.Constructs/README.md`

### Documentation
- `./docs/README.md` - Documentation overview
- `./docs/agent/README.md` - Documentation agent
- `./docs/apps/README.md` - Apps documentation
- `./docs/packages/README.md` - Packages documentation

### Scripts
- `./scripts/README.md` - Scripts overview
- `./scripts/dotnet/README.md` - .NET scripts
- `./scripts/flutter/workspace/README.md` - Flutter workspace scripts

## Setup Options

### Option 1: Single Wiki Repository (Recommended)
Create one wiki repository for the entire monorepo with organized sections.

### Option 2: Multiple Wiki Repositories
Create separate wiki repositories for different parts of the monorepo (apps, packages, docs).

### Option 3: Hybrid Approach
Create a main wiki with links to separate documentation repositories.

## Implementation Steps

### Step 1: Enable Wiki on Your GitHub Repository

1. Go to your GitHub repository
2. Click on the "Wiki" tab
3. Click "Create the first page" or "Add page"
4. This will create a wiki repository at `https://github.com/your-username/flyingdarts.wiki.git`

### Step 2: Clone the Wiki Repository

```bash
git clone https://github.com/your-username/flyingdarts.wiki.git
cd flyingdarts.wiki
```

### Step 3: Create Wiki Structure

Create the following directory structure in your wiki:

```
flyingdarts.wiki/
├── Home.md                    # Main landing page
├── Apps/                      # Applications documentation
│   ├── Backend/
│   │   ├── X01-Game-API.md
│   │   ├── Authentication-Service.md
│   │   ├── Friends-API.md
│   │   ├── Signalling-Service.md
│   │   └── Rust-Authorizer.md
│   ├── Frontend/
│   │   ├── Angular-Web-App.md
│   │   └── Flutter-Mobile-App.md
│   └── Tools/
│       └── CDK-Infrastructure.md
├── Packages/                  # Shared libraries documentation
│   ├── Backend/
│   │   ├── Connection-Services.md
│   │   ├── Core.md
│   │   ├── DynamoDB-Service.md
│   │   ├── Lambda-Core.md
│   │   ├── Meetings-Service.md
│   │   ├── Metadata-Services.md
│   │   ├── NotifyRooms-Service.md
│   │   ├── Persistence.md
│   │   └── Rust-Auth.md
│   ├── Frontend/
│   │   ├── API-SDK.md
│   │   ├── Authress-Login.md
│   │   ├── Core.md
│   │   ├── Features/
│   │   │   ├── Keyboard.md
│   │   │   ├── Language.md
│   │   │   ├── Profile.md
│   │   │   ├── Speech.md
│   │   │   └── Splash.md
│   │   └── Shared/
│   │       ├── App-Config-Core.md
│   │       ├── App-Config-Prefs.md
│   │       ├── App-Config-Secrets.md
│   │       ├── Internationalization.md
│   │       ├── UI.md
│   │       └── WebSocket.md
│   └── Tools/
│       ├── Config.md
│       └── CDK-Constructs.md
├── Documentation/             # Documentation
│   ├── Overview.md
│   ├── Agent.md
│   ├── Apps.md
│   └── Packages.md
├── Scripts/                   # Scripts documentation
│   ├── Overview.md
│   ├── DotNet.md
│   └── Flutter-Workspace.md
└── _Sidebar.md               # Navigation sidebar
```

### Step 4: Convert README Files to Wiki Pages

For each README file, you'll need to:

1. **Copy content** from the README.md file
2. **Adjust formatting** for GitHub wiki (some markdown features may differ)
3. **Update internal links** to point to other wiki pages
4. **Add navigation links** at the top/bottom

### Step 5: Create Navigation

Create a `_Sidebar.md` file for navigation:

```markdown
# FlyingDarts Turbo Wiki

## Getting Started
- [Home](Home)
- [Overview](Documentation/Overview)

## Applications
- [Backend Services](Apps/Backend/)
- [Frontend Applications](Apps/Frontend/)
- [Tools](Apps/Tools/)

## Packages
- [Backend Packages](Packages/Backend/)
- [Frontend Packages](Packages/Frontend/)
- [Tool Packages](Packages/Tools/)

## Documentation
- [Documentation Overview](Documentation/Overview)
- [Apps Documentation](Documentation/Apps)
- [Packages Documentation](Documentation/Packages)

## Scripts
- [Scripts Overview](Scripts/Overview)
- [.NET Scripts](Scripts/DotNet)
- [Flutter Scripts](Scripts/Flutter-Workspace)
```

### Step 6: Create Home Page

Create a `Home.md` file as the main landing page:

```markdown
# Welcome to FlyingDarts Turbo

FlyingDarts Turbo is a comprehensive monorepo containing all the applications, packages, and tools for the FlyingDarts platform.

## Quick Navigation

### Applications
- **[Backend Services](Apps/Backend/)** - API services, authentication, and real-time communication
- **[Frontend Applications](Apps/Frontend/)** - Web and mobile applications
- **[Tools](Apps/Tools/)** - Infrastructure and development tools

### Packages
- **[Backend Packages](Packages/Backend/)** - Shared .NET and Rust libraries
- **[Frontend Packages](Packages/Frontend/)** - Shared Flutter components and utilities
- **[Tool Packages](Packages/Tools/)** - Configuration and infrastructure packages

### Documentation
- **[Documentation Overview](Documentation/Overview)** - Complete documentation guide
- **[Scripts](Scripts/Overview)** - Development and maintenance scripts

## Getting Started

1. Check out the [Documentation Overview](Documentation/Overview) for a complete guide
2. Explore the [Applications](Apps/) to understand the system architecture
3. Review the [Packages](Packages/) to see available shared components

## Contributing

Please refer to the main repository for contribution guidelines and development setup.
```

## Automation Script

I've created a script to help automate this process. See `scripts/setup-github-wikis.sh` for the automated setup.

## Best Practices

1. **Consistent Formatting**: Use consistent markdown formatting across all wiki pages
2. **Clear Navigation**: Maintain clear navigation with breadcrumbs and related links
3. **Regular Updates**: Keep wiki pages in sync with README files
4. **Version Control**: Use Git to track changes to your wiki
5. **Cross-References**: Link between related pages for better navigation

## Maintenance

- Regularly sync wiki content with README files
- Update navigation when adding new pages
- Review and clean up broken links
- Keep the wiki structure organized and intuitive

## Next Steps

1. Run the automation script to set up the initial wiki structure
2. Review and customize the generated pages
3. Add any missing documentation
4. Set up a process for keeping wiki and README files in sync 
