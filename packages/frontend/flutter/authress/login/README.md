# Flying Darts Authress Login

## Overview

The Flying Darts Authress Login is a Flutter package that provides comprehensive authentication integration with the [Authress](https://authress.io) authentication service. This package offers a provider-based approach with Go Router integration for seamless authentication flows in Flutter applications.

The package is built using modern Flutter patterns with dependency injection, secure token management, and comprehensive error handling. It provides a robust foundation for implementing authentication in Flutter apps with support for multiple platforms including iOS, Android, and Web.

## Features

- **Provider-based Authentication**: Wrap your app with authentication state management
- **Go Router Integration**: Built-in route guards for protected routes
- **Smart Browser Management**: Platform-optimized browser handling for authentication flows
- **Automatic Token Management**: Handles token storage, refresh, and expiration
- **Secure Token Storage**: Uses Flutter's secure storage mechanisms
- **Real-time Auth State**: Listen to authentication state changes
- **Cross-Platform Support**: Works on iOS, Android, and Web
- **Role & Group Based Access**: Support for role and group-based authorization
- **Type-Safe State Management**: Sealed classes for authentication states
- **Comprehensive Error Handling**: Detailed error states and handling

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: >=3.26.0 or higher
- **Authress Account**: Active Authress account with configured application
- **Go Router**: ^16.0.0 for routing integration

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the authress login package**:
   ```bash
   cd packages/frontend/flutter/authress/login
   ```

3. **Get dependencies**:
   ```bash
   flutter pub get
   ```

### In Your Flutter Project
Add the package to your `pubspec.yaml`:

```yaml
dependencies:
  authress_login:
    path: ../../packages/frontend/flutter/authress/login
```

Then run:
```bash
flutter pub get
```

## Usage

### Basic Setup

1. **Wrap your app with AuthressProvider**:

```dart
import 'package:flutter/material.dart';
import 'package:flyingdarts_authress_login/authress_login.dart';
import 'package:go_router/go_router.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  final _router = GoRouter(
    routes: [
      GoRoute(
        path: '/',
        builder: (context, state) => HomePage(),
        redirect: AuthressRouteGuard.redirectLogic,
      ),
      GoRoute(
        path: '/login',
        builder: (context, state) => LoginPage(),
      ),
    ],
  );

  @override
  Widget build(BuildContext context) {
    return AuthressProvider(
      config: AuthressConfiguration(
        authressApiUrl: 'https://login.yourdomain.com',
        applicationId: 'your-application-id',
        redirectUrl: 'https://yourdomain.com/callback',
        enableDebugLogging: true,
      ),
      onAuthenticated: (user) {
        print('User logged in: ${user.email}');
      },
      onLoggedOut: () {
        print('User logged out');
      },
      onError: (error) {
        print('Auth error: $error');
      },
      child: MaterialApp.router(
        title: 'My Secure App',
        routerConfig: _router,
      ),
    );
  }
}
```

2. **Access Authentication Data**:

```dart
class HomePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final user = context.currentUser;
    
    return Scaffold(
      appBar: AppBar(
        title: Text('Welcome ${user?.name ?? 'User'}'),
        actions: [
          IconButton(
            icon: Icon(Icons.logout),
            onPressed: () => context.logout(),
          ),
        ],
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text('Hello, ${user?.email ?? 'Guest'}!'),
            SizedBox(height: 20),
            Text('Authentication State: ${context.authState}'),
            SizedBox(height: 10),
            if (context.accessToken != null)
              Container(
                padding: EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: Colors.grey[200],
                  borderRadius: BorderRadius.circular(4),
                ),
                child: Text(
                  'Token: ${context.accessToken!.substring(0, 50)}...',
                  style: TextStyle(fontFamily: 'monospace'),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
```

### Using Context Extensions

```dart
class MyWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    // Check if user is authenticated
    if (!context.isAuthenticated) {
      return Text('Please log in');
    }

    // Access user profile
    final user = context.currentUser;
    
    // Access auth token for API calls
    final token = context.accessToken;
    
    // Access the auth state
    final authState = context.authState;
    
    // Access the full authress context
    final authContext = context.authress;
    
    return Column(
      children: [
        Text('Welcome ${user?.name}'),
        Text('Auth State: $authState'),
        ElevatedButton(
          onPressed: () => context.logout(),
          child: Text('Logout'),
        ),
      ],
    );
  }
}
```

## Route Protection with Go Router

### Basic Route Guard

```dart
final router = GoRouter(
  routes: [
    // Protected route
    GoRoute(
      path: '/dashboard',
      builder: (context, state) => DashboardPage(),
      redirect: AuthressRouteGuard.redirectLogic,
    ),
    // Public route
    GoRoute(
      path: '/login',
      builder: (context, state) => LoginPage(),
    ),
  ],
);
```

### Role-based Route Protection

```dart
GoRoute(
  path: '/admin',
  builder: (context, state) => AdminPage(),
  redirect: (context, state) => AuthressRouteGuard.roleGuard(
    context,
    state,
    requiredRoles: ['admin', 'moderator'],
    redirectTo: '/unauthorized',
  ),
)
```

### Group-based Route Protection

```dart
GoRoute(
  path: '/premium',
  builder: (context, state) => PremiumPage(),
  redirect: (context, state) => AuthressRouteGuard.groupGuard(
    context,
    state,
    requiredGroups: ['premium-users'],
    redirectTo: '/upgrade',
  ),
)
```

## Advanced Usage

### Listening to Auth State Changes

```dart
AuthressProvider(
  config: authConfig,
  onStateChanged: (state) {
    switch (state) {
      case AuthStateAuthenticated():
        print('User authenticated: ${state.user.email}');
        // Update app state, send analytics, etc.
        break;
      case AuthStateUnauthenticated():
        print('User logged out');
        // Clear app state, redirect to public content, etc.
        break;
      case AuthStateLoading():
        print('Authentication in progress');
        break;
      case AuthStateError():
        print('Auth error: ${state.message}');
        // Show error dialog, retry logic, etc.
        break;
    }
  },
  child: MyApp(),
)
```

### Direct Authentication Methods

```dart
class MyService {
  Future<void> loginUser(BuildContext context) async {
    await context.authenticate(
      connectionId: 'specific-provider', // Optional
      tenantLookupIdentifier: 'tenant-id', // Optional
      additionalParams: {'theme': 'dark'}, // Optional
    );
  }
  
  Future<void> makeAuthenticatedRequest(BuildContext context) async {
    final token = context.accessToken;
    if (token == null) {
      throw Exception('Not authenticated');
    }
    
    // Use token for API calls
    final response = await http.get(
      Uri.parse('https://api.yourdomain.com/protected'),
      headers: {'Authorization': 'Bearer $token'},
    );
  }
}
```

## API Reference

### Core Components

#### AuthressProvider Widget
The main provider widget that manages authentication state.

```dart
class AuthressProvider extends StatefulWidget {
  final AuthressConfiguration config;
  final Widget child;
  final DeepLinkConfig? deepLinkConfig;
  final void Function(AuthState state)? onStateChanged;
  final void Function(UserProfile user)? onAuthenticated;
  final VoidCallback? onLoggedOut;
  final void Function(String error)? onError;
}
```

#### AuthressConfiguration Class
Configuration for the Authress service.

```dart
class AuthressConfiguration {
  final String authressApiUrl;        // Required
  final String applicationId;         // Required
  final String? redirectUrl;          // Optional
  final String? customDomain;         // Optional
  final bool enableDebugLogging;      // Default: false
  final Duration requestTimeout;      // Default: 30s
  final Duration authTimeout;         // Default: 5m
  
  void validate(); // Validates configuration
  AuthressConfiguration copyWith({...}); // Creates validated copy
}
```

#### AuthressRouteGuard Class
Static methods for protecting routes with Go Router.

```dart
class AuthressRouteGuard {
  static String? redirectLogic(BuildContext context, GoRouterState state);
  static String? roleGuard(BuildContext context, GoRouterState state, {
    required List<String> requiredRoles,
    String? redirectTo,
  });
  static String? groupGuard(BuildContext context, GoRouterState state, {
    required List<String> requiredGroups,
    String? redirectTo,
  });
}
```

### Context Extensions
Convenient extensions for accessing auth data:

```dart
extension AuthressProviderBuilderContextExtension on BuildContext {
  AuthressContext get authress;           // Get AuthressContext instance
  AuthState get authState;                // Get current auth state
  bool get isAuthenticated;               // Check if user is logged in
  UserProfile? get currentUser;           // Get current user profile
  String? get accessToken;                // Get current access token
  Future<void> logout();                  // Log out current user
  Future<void> authenticate({...});       // Start authentication flow
}
```

### Authentication States
The package uses sealed classes for type-safe state management:

```dart
sealed class AuthState {}

class AuthStateUnauthenticated extends AuthState {}
class AuthStateLoading extends AuthState {}
class AuthStateAuthenticated extends AuthState {
  final UserProfile user;
  final String accessToken;
  final String? refreshToken;
  final DateTime expiresAt;
  
  bool get isTokenExpired;
  bool get willExpireSoon;
}
class AuthStateError extends AuthState {
  final String message;
  final Object? error;
}
```

### Data Models

#### UserProfile
Represents a user profile in the system.

```dart
class UserProfile {
  final String userId;
  final String email;
  final String? name;
  final String? picture;
  final List<String> roles;
  final List<String> groups;
  final Map<String, dynamic> metadata;
}
```

#### DeepLinkConfig
Configuration for deep link handling.

```dart
class DeepLinkConfig {
  final String? customScheme;
  final String? host;
}
```

## Configuration

### Environment Setup
Configure different environments:

```dart
class EnvironmentConfig {
  static AuthressConfiguration getConfig(String environment) {
    switch (environment) {
      case 'dev':
        return AuthressConfiguration(
          authressApiUrl: 'https://dev-login.yourdomain.com',
          applicationId: 'dev-app-id',
          enableDebugLogging: true,
        );
      case 'production':
        return AuthressConfiguration(
          authressApiUrl: 'https://login.yourdomain.com',
          applicationId: 'prod-app-id',
          enableDebugLogging: false,
        );
      default:
        throw ArgumentError('Unknown environment: $environment');
    }
  }
}
```

### Deep Link Configuration
For production apps, configure custom URL schemes:

```dart
AuthressProvider(
  config: authConfig,
  deepLinkConfig: DeepLinkConfig(
    customScheme: 'myapp',
    host: 'auth',
  ),
  child: MyApp(),
)
```

## Development

### Project Structure
```
authress_login/
├── lib/
│   ├── authress_login.dart           # Main library export
│   └── src/
│       ├── core/                     # Core functionality
│       │   ├── auth_provider.dart    # Main provider widget
│       │   ├── auth_context.dart     # Authentication context
│       │   ├── auth_client.dart      # Auth client implementation
│       │   ├── auth_extension.dart   # Context extensions
│       │   ├── route_guard.dart      # Go Router guards
│       │   └── page_guard.dart       # Page-level guards
│       ├── models/                   # Data models
│       │   ├── auth_config.dart      # Configuration model
│       │   ├── auth_state.dart       # Authentication states
│       │   ├── user_profile.dart     # User profile model
│       │   └── deep_link_config.dart # Deep link configuration
│       └── services/                 # Service implementations
├── test/                             # Unit tests
├── pubspec.yaml                      # Package dependencies
└── package.json                      # Build scripts
```

### Architecture Patterns
- **Provider Pattern**: State management with InheritedWidget
- **Sealed Classes**: Type-safe authentication states
- **Extension Pattern**: Context extensions for easy access
- **Strategy Pattern**: Route guard strategies
- **Service Pattern**: Authentication service abstraction

### Testing
Run unit tests to ensure code quality:
```bash
flutter test
```

### Code Quality
- Follow Dart coding conventions
- Use proper documentation for public APIs
- Implement comprehensive error handling
- Add unit tests for all public methods
- Use sealed classes for type safety

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **go_router**: ^16.0.0 - Routing integration
- **webview_flutter**: ^4.4.2 - In-app browser authentication
- **url_launcher**: ^6.2.1 - External URL launching
- **shared_preferences**: ^2.2.2 - Token storage
- **jwt_decode**: ^0.3.1 - JWT token parsing
- **app_links**: ^6.3.2 - Deep link handling
- **http**: ^1.1.0 - HTTP requests
- **crypto**: ^3.0.3 - Cryptographic operations

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **mockito**: ^5.4.4 - Mocking framework
- **build_runner**: ^2.4.7 - Code generation
- **fake_async**: ^1.3.1 - Async testing utilities
- **mocktail**: ^1.0.3 - Mocking utilities

## Related Projects

### Backend Services
- **[Friends API](../../../../backend/dotnet/friends/)**: Friend management and social features
- **[Games API](../../../../backend/dotnet/games/)**: Game management and scoring
- **[Auth API](../../../../backend/dotnet/auth/)**: Authentication and authorization

### Frontend Applications
- **[Flutter Mobile App](../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[Core Package](../../../../backend/dotnet/Flyingdarts.Core/)**: Shared business logic
- **[Persistence Package](../../../../backend/dotnet/Flyingdarts.Persistence/)**: Data access layer
- **[API SDK](../../../api/sdk/)**: Flutter API client

## Version History

- **v0.0.2** (2025-07-26): Implemented friends feature
- **v1.0.0**: Initial implementation

## Troubleshooting

### Common Issues

1. **Authentication not working on mobile**
   - Verify your Authress configuration is correct
   - Check that redirect URLs are properly configured
   - Ensure deep link configuration is set up correctly

2. **Route guard not working**
   - Ensure `AuthressProvider` wraps your entire app
   - Verify that route redirects are properly configured
   - Check that you're using the correct guard methods

3. **Context extensions not available**
   - Make sure you've imported `package:flyingdarts_authress_login/authress_login.dart`
   - Verify that your widget is within the provider tree
   - Check that context is properly passed down

4. **Token not persisting**
   - The package uses `shared_preferences` for token storage
   - Make sure you're not clearing app data during testing
   - Verify that token refresh is working correctly

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Maintain type safety with sealed classes
6. Test on multiple platforms (iOS, Android, Web)

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms. 
