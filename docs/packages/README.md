# Flying Darts Packages

This directory contains all the packages in the Flying Darts Turbo monorepo, organized by technology stack and purpose. These packages provide shared functionality, utilities, and components used across multiple applications.

## 📁 Directory Structure

```
packages/
├── backend/           # Backend packages and libraries
│   ├── dotnet/       # .NET backend packages
│   └── rust/         # Rust backend packages
├── frontend/         # Frontend packages
│   └── flutter/      # Flutter frontend packages
└── tools/            # Development and deployment tools
    ├── config/       # TypeScript configuration tools
    └── dotnet/       # .NET tools and utilities
```

## 🚀 Packages Overview

### Backend Packages

#### .NET Backend Packages (`backend/dotnet/`)

- **[Flyingdarts.Core/](backend/dotnet/Flyingdarts.Core/)** - Core Library
  - Shared models and extensions
  - Common utilities and helpers
  - Business logic abstractions

- **[Flyingdarts.Lambda.Core/](backend/dotnet/Flyingdarts.Lambda.Core/)** - Lambda Core
  - AWS Lambda abstractions
  - Handler patterns and testing utilities
  - Common Lambda infrastructure

- **[Flyingdarts.Persistence/](backend/dotnet/Flyingdarts.Persistence/)** - Persistence Layer
  - Data access and storage abstractions
  - Database operations and patterns
  - Repository implementations

- **[Flyingdarts.Connection.Services/](backend/dotnet/Flyingdarts.Connection.Services/)** - Connection Services
  - Connection management utilities
  - WebSocket connection handling
  - Service connectivity patterns

- **[Flyingdarts.DynamoDb.Service/](backend/dotnet/Flyingdarts.DynamoDb.Service/)** - DynamoDB Service
  - DynamoDB integration and operations
  - Data persistence layer
  - NoSQL database utilities

- **[Flyingdarts.Meetings.Service/](backend/dotnet/Flyingdarts.Meetings.Service/)** - Meetings Service
  - Meeting management functionality
  - Room and session handling
  - Real-time meeting coordination

- **[Flyingdarts.Metadata.Services/](backend/dotnet/Flyingdarts.Metadata.Services/)** - Metadata Services
  - Metadata management
  - Configuration and settings
  - System metadata handling

- **[Flyingdarts.NotifyRooms.Service/](backend/dotnet/Flyingdarts.NotifyRooms.Service/)** - Notification Service
  - Room notification system
  - Real-time updates
  - Event broadcasting

#### Rust Backend Packages (`backend/rust/`)

- **[auth/](backend/rust/auth/)** - Rust Auth Package
  - Authentication utilities
  - Security components
  - JWT token handling

### Frontend Packages

#### Flutter Frontend Packages (`frontend/flutter/`)

- **[api/](frontend/flutter/api/)** - API SDK
  - Backend API integration
  - HTTP client utilities
  - API response handling

- **[sdk/](frontend/flutter/api/sdk/)** - API client SDK
  - Backend API integration
  - Client-side API utilities
  - Request/response models

- **[authress/](frontend/flutter/authress/)** - Authress Integration
  - Authentication and authorization
  - Login components
  - Login examples

- **[login/](frontend/flutter/authress/login/)** - Login components
  - Authentication UI components
  - Login flow management
  - User session handling

- **[login-example/](frontend/flutter/authress/login-example/)** - Login examples
  - Authentication examples
  - Implementation patterns
  - Best practices

- **[core/](frontend/flutter/core/)** - Core Flutter Package
  - Shared Flutter utilities
  - Common functionality
  - Base classes and helpers

- **[features/](frontend/flutter/features/)** - Feature Packages
  - Modular feature implementations
  - Reusable feature components
  - Feature-specific utilities

- **[splash/](frontend/flutter/features/splash/)** - Splash screen
  - Application splash screen
  - Loading animations
  - Initialization handling

- **[profile/](frontend/flutter/features/profile/)** - User profile
  - User profile management
  - Profile UI components
  - Profile data handling

- **[language/](frontend/flutter/features/language/)** - Language selection
  - Multi-language support
  - Language selection UI
  - Internationalization utilities

- **[keyboard/](frontend/flutter/features/keyboard/)** - Keyboard handling
  - Keyboard input management
  - Text input utilities
  - Keyboard event handling

- **[speech/](frontend/flutter/features/speech/)** - Speech recognition
  - Voice input handling
  - Speech-to-text conversion
  - Voice command processing

- **[shared/](frontend/flutter/shared/)** - Shared Components
  - Common UI components
  - Shared utilities
  - Reusable widgets

- **[websocket/](frontend/flutter/shared/websocket/)** - WebSocket utilities
  - Real-time communication
  - WebSocket connection management
  - Message handling

- **[ui/](frontend/flutter/shared/ui/)** - UI components
  - Reusable UI components
  - Design system elements
  - Common widgets

- **[internationalization/](frontend/flutter/shared/internationalization/)** - i18n support
  - Multi-language support
  - Translation management
  - Locale handling

- **[config/](frontend/flutter/shared/config/)** - Configuration management
  - App configuration
  - Environment management
  - Settings handling

### Tools Packages

#### TypeScript Tools (`tools/`)

- **[config/](tools/config/)** - Configuration Package
  - Shared configuration utilities
  - Build and deployment configs
  - Development tooling

#### .NET Tools (`tools/dotnet/`)

- **[Flyingdarts.CDK.Constructs/](tools/dotnet/Flyingdarts.CDK.Constructs/)** - CDK Constructs
  - Reusable AWS CDK constructs
  - Infrastructure components
  - Cloud resource patterns

## 📊 Summary Statistics

- **Total Packages**: 16
- **Backend Packages**: 8 (7 .NET + 1 Rust)
- **Frontend Packages**: 5 (All Flutter)
- **Tools Packages**: 3 (1 TypeScript + 2 .NET)

## 🔗 Quick Navigation

### Backend Libraries
- [Core Library](backend/dotnet/Flyingdarts.Core/) - Shared models and utilities
- [Lambda Core](backend/dotnet/Flyingdarts.Lambda.Core/) - AWS Lambda infrastructure
- [Persistence Layer](backend/dotnet/Flyingdarts.Persistence/) - Data access patterns
- [Connection Services](backend/dotnet/Flyingdarts.Connection.Services/) - WebSocket management
- [DynamoDB Service](backend/dotnet/Flyingdarts.DynamoDb.Service/) - Database operations
- [Meetings Service](backend/dotnet/Flyingdarts.Meetings.Service/) - Meeting management
- [Metadata Services](backend/dotnet/Flyingdarts.Metadata.Services/) - Configuration management
- [Notification Service](backend/dotnet/Flyingdarts.NotifyRooms.Service/) - Real-time notifications
- [Rust Auth](backend/rust/auth/) - Authentication utilities

### Frontend Libraries
- [API SDK](frontend/flutter/api/) - Backend API integration
- [Authress Integration](frontend/flutter/authress/) - Authentication components
- [Core Package](frontend/flutter/core/) - Shared Flutter utilities
- [Feature Packages](frontend/flutter/features/) - Modular features
- [Shared Components](frontend/flutter/shared/) - Common UI components

### Development Tools
- [Configuration Package](tools/config/) - Shared configuration
- [CDK Constructs](tools/dotnet/Flyingdarts.CDK.Constructs/) - Infrastructure components

## 🛠️ Technology Stack

- **.NET/C#**: 9 packages (7 backend + 2 tools)
- **Flutter/Dart**: 5 packages (frontend)
- **Rust**: 1 package (backend)
- **TypeScript**: 1 package (tools)

## 📋 Development Guidelines

### Package Development

#### Creating New Packages

1. **Determine Package Type**: Backend, frontend, or tools
2. **Choose Technology**: .NET, Flutter, Rust, or TypeScript
3. **Define Scope**: Clear boundaries and responsibilities
4. **Set Dependencies**: Internal and external dependencies
5. **Create Documentation**: Comprehensive README and API docs

#### Package Structure

Each package should follow a consistent structure:

```
package-name/
├── src/                    # Source code
├── tests/                  # Test files
├── docs/                   # Documentation
├── examples/               # Usage examples
├── README.md              # Package documentation
├── CHANGELOG.md           # Version history
└── [config files]         # Package-specific config
```

#### Versioning

- **Semantic Versioning**: Follow semver principles
- **Changelog**: Maintain detailed changelog
- **Breaking Changes**: Document and communicate clearly
- **Migration Guides**: Provide upgrade instructions

### Dependency Management

#### Internal Dependencies

- **Workspace References**: Use workspace dependencies where possible
- **Version Alignment**: Keep related packages in sync
- **Circular Dependencies**: Avoid circular package dependencies
- **Dependency Graph**: Maintain clear dependency relationships

#### External Dependencies

- **Version Pinning**: Pin specific versions for stability
- **Security Updates**: Regular security dependency updates
- **License Compliance**: Ensure compatible licenses
- **Size Optimization**: Minimize package size

### Testing Strategy

#### Unit Testing

- **Comprehensive Coverage**: High test coverage for all packages
- **Isolated Testing**: Test packages in isolation
- **Mock Dependencies**: Mock external dependencies
- **Fast Execution**: Optimize test execution time

#### Integration Testing

- **Cross-package Testing**: Test package interactions
- **End-to-end Testing**: Test complete workflows
- **Performance Testing**: Monitor package performance
- **Compatibility Testing**: Test across different environments

### Documentation Standards

#### Package Documentation

- **README.md**: Comprehensive package overview
- **API Documentation**: Detailed API reference
- **Usage Examples**: Practical usage examples
- **Migration Guides**: Upgrade and migration instructions

#### Code Documentation

- **Inline Comments**: Clear code comments
- **XML Documentation**: .NET XML documentation
- **Dart Documentation**: Dart documentation comments
- **Rust Documentation**: Rust documentation comments

## 🔄 Package Lifecycle

### Development Workflow

1. **Planning**: Define package requirements and scope
2. **Implementation**: Develop package functionality
3. **Testing**: Comprehensive testing and validation
4. **Documentation**: Create and update documentation
5. **Review**: Code review and quality assurance
6. **Release**: Version and release package
7. **Maintenance**: Ongoing maintenance and updates

### Release Process

1. **Version Bump**: Update package version
2. **Changelog Update**: Document changes
3. **Testing**: Run full test suite
4. **Documentation**: Update documentation
5. **Release**: Create release tag
6. **Publish**: Publish to package registry
7. **Announce**: Communicate changes to users

## 🚀 Usage Examples

### Using Backend Packages

#### .NET Package Usage

```csharp
// Using Flyingdarts.Core
using Flyingdarts.Core.Models;
using Flyingdarts.Core.Extensions;

var game = new Game { Id = "game-123", Name = "X01 Game" };
var isValid = game.Validate();
```

#### Rust Package Usage

```rust
// Using flyingdarts-auth
use flyingdarts_auth::{validate_token, AuthResult};

let result = validate_token(&token);
match result {
    AuthResult::Valid(user) => println!("User: {}", user.id),
    AuthResult::Invalid => println!("Invalid token"),
}
```

### Using Frontend Packages

#### Flutter Package Usage

```dart
// Using core package
import 'package:core/core.dart';

final apiService = ApiService();
final games = await apiService.getGames();
```

#### Feature Package Usage

```dart
// Using profile feature
import 'package:profile/profile.dart';

final profileBloc = ProfileBloc();
profileBloc.add(LoadProfile(userId));
```

### Using Tools Packages

#### Configuration Package Usage

```typescript
// Using config package
import { getConfig } from '@flyingdarts/config';

const config = getConfig('development');
console.log(config.apiUrl);
```

## 🔧 Contributing

### Package Contributions

1. **Follow Standards**: Adhere to package development standards
2. **Add Tests**: Include comprehensive tests for new functionality
3. **Update Documentation**: Keep documentation current
4. **Version Management**: Follow semantic versioning
5. **Backward Compatibility**: Maintain compatibility where possible

### Quality Assurance

- **Code Review**: All changes require code review
- **Automated Testing**: CI/CD pipeline testing
- **Documentation Review**: Documentation quality checks
- **Performance Testing**: Performance impact assessment

## 📈 Monitoring and Maintenance

### Package Health

- **Dependency Updates**: Regular dependency updates
- **Security Scanning**: Automated security vulnerability scanning
- **Performance Monitoring**: Package performance tracking
- **Usage Analytics**: Package usage statistics

### Maintenance Tasks

- **Bug Fixes**: Prompt bug fix releases
- **Security Patches**: Immediate security updates
- **Feature Updates**: Regular feature enhancements
- **Deprecation Management**: Graceful deprecation handling

---

*This documentation is auto-generated by the Flying Darts Documentation Agent*