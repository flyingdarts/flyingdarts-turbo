# Flyingdarts Profile Feature

## Overview

The Flyingdarts Profile Feature is a Flutter package that provides comprehensive user profile management for the Flyingdarts gaming platform. This package enables Flutter applications to display, manage, and interact with user profile information through a clean, state-managed architecture.

The package is built using modern Flutter patterns with BLoC state management, dependency injection, and API integration. It provides a robust foundation for implementing user profile functionality in Flutter apps with secure authentication token management and real-time profile data synchronization.

## Features

- **User Profile Management**: Comprehensive user profile display and management
- **API Integration**: Seamless integration with Flyingdarts backend APIs
- **BLoC State Management**: Clean architecture with Cubit for state management
- **Dependency Injection**: Injectable integration for service management
- **Authentication Integration**: Secure token-based API communication
- **Profile Dialog**: User-friendly profile information display
- **Internationalization Support**: Multi-language support for profile content
- **Widgetbook Integration**: Component documentation and testing
- **Code Generation**: Automatic code generation for JSON serialization
- **Cross-Platform Support**: Works on iOS, Android, and Web

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ">=3.26.0"
- **Authentication**: Valid authentication tokens from Authress
- **Dependencies**: Access to shared packages (api_sdk, authress_login, ui, internationalization, websocket, app_config_core, app_config_secrets)

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the profile package**:
   ```bash
   cd packages/frontend/flutter/features/profile
   ```

3. **Get dependencies**:
   ```bash
   flutter pub get
   ```

4. **Generate code** (if needed):
   ```bash
   dart run build_runner build --delete-conflicting-outputs
   ```

### In Your Flutter Project
Add the package to your `pubspec.yaml`:

```yaml
dependencies:
  profile:
    path: ../../packages/frontend/flutter/features/profile
```

## Usage

### Basic Setup

1. **Initialize the profile feature**:

```dart
import 'package:profile/profile.dart';
import 'package:get_it/get_it.dart';

void main() {
  // Initialize dependency injection
  configureDependencies();
  
  // Initialize profile cubit
  final profileCubit = GetIt.I<ProfileCubit>();
  await profileCubit.init();
  
  runApp(MyApp());
}
```

2. **Use the profile cubit in your app**:

```dart
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:profile/profile.dart';

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => GetIt.I<ProfileCubit>(),
      child: MaterialApp(
        title: 'Profile App',
        home: HomePage(),
      ),
    );
  }
}
```

### Profile Display

```dart
class HomePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocBuilder<ProfileCubit, ProfileState>(
      builder: (context, state) {
        return Scaffold(
          appBar: AppBar(
            title: Text('Profile: ${state.userName}'),
            actions: [
              IconButton(
                icon: Icon(Icons.person),
                onPressed: () => _showProfileDialog(context),
              ),
            ],
          ),
          body: Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text('Welcome, ${state.userName}!'),
                SizedBox(height: 10),
                Text('Email: ${state.email}'),
                SizedBox(height: 10),
                Text('Country: ${state.country}'),
                SizedBox(height: 20),
                ElevatedButton(
                  onPressed: () => _showProfileDialog(context),
                  child: Text('View Profile'),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  void _showProfileDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => ProfileDialog(),
    );
  }
}
```

### Programmatic Profile Management

```dart
class ProfileService {
  final ProfileCubit _profileCubit;

  ProfileService(this._profileCubit);

  Future<void> loadUserProfile() async {
    await _profileCubit.init();
  }

  String get userName => _profileCubit.state.userName;
  String get email => _profileCubit.state.email;
  String get country => _profileCubit.state.country;
  String get userId => _profileCubit.state.userId;
}
```

## API Reference

### Core Components

#### ProfileCubit
The main state management class for profile functionality.

```dart
@lazySingleton
class ProfileCubit extends Cubit<ProfileState> {
  ProfileCubit(ConfigWriter<AuthTokens> writeableCredentials) 
    : super(ProfileState.initialState);

  Future<void> init();
}
```

#### ProfileState
Represents the current profile state.

```dart
class ProfileState {
  final String userId;
  final String userName;
  final String country;
  final String email;

  ProfileState(this.userId, this.userName, this.country, this.email);
  
  static ProfileState get initialState;
  ProfileState copyWith({
    String? userId,
    String? userName,
    String? country,
    String? email,
  });
}
```

#### ProfileDialog
A dialog widget for displaying profile information.

```dart
class ProfileDialog extends StatelessWidget {
  const ProfileDialog({Key? key});

  @override
  Widget build(BuildContext context);
}
```

### Data Models

#### GetUserProfileResponse
Represents the API response for user profile data.

```dart
@JsonSerializable()
class GetUserProfileResponse {
  final String userId;
  final String userName;
  final String email;
  final String country;

  GetUserProfileResponse({
    required this.userId,
    required this.userName,
    required this.email,
    required this.country,
  });

  factory GetUserProfileResponse.fromJson(Map<String, dynamic> json);
  Map<String, dynamic> toJson();
}
```

#### GetUserProfileQuery
Represents the API query for fetching user profile.

```dart
@JsonSerializable()
class GetUserProfileQuery {
  final String userId;

  GetUserProfileQuery({required this.userId});

  factory GetUserProfileQuery.fromJson(Map<String, dynamic> json);
  Map<String, dynamic> toJson();
}
```

### Methods

#### ProfileCubit Methods

- **`init()`**: Initializes the profile by fetching user data from the API

#### ProfileState Methods

- **`copyWith(...)`**: Creates a copy of the state with updated values
- **`initialState`**: Static getter for the initial profile state

## Configuration

### Dependency Injection Setup
The package uses Injectable for dependency injection:

```dart
import 'package:injectable/injectable.dart';
import 'package:profile/profile.dart';

@InjectableInit.microPackage()
void initMicroPackage() {}

void configureDependencies() {
  // The ProfileCubit is automatically registered as a lazy singleton
  // through the micro package initialization
}
```

### API Configuration
The package integrates with the API SDK for backend communication:

```dart
// API configuration is handled through the API SDK
// Authentication tokens are managed through app_config_secrets
// The profile cubit automatically configures the API client
```

### Authentication Setup
The package requires authentication tokens for API access:

```dart
// Authentication tokens are provided through ConfigWriter<AuthTokens>
// The profile cubit automatically uses these tokens for API requests
```

## Development

### Project Structure
```
profile/
├── lib/
│   ├── profile.dart                     # Main library export
│   ├── profile.module.dart              # Generated dependency injection
│   └── src/
│       ├── state/                       # State management
│       │   ├── profile_cubit.dart       # Main cubit implementation
│       │   └── profile_state.dart       # State model
│       ├── dialog/                      # UI components
│       │   ├── profile_dialog.dart      # Profile display dialog
│       │   └── profile_dialog.usecase.dart # Widgetbook use case
│       ├── models/                      # Data models
│       │   ├── get_user_profile_query.dart # API query model
│       │   ├── get_user_profile_response.dart # API response model
│       │   └── *.g.dart                 # Generated JSON code
│       └── widgetbook.generator.dart    # Generated widgetbook code
├── test/                                # Unit tests
├── pubspec.yaml                         # Package dependencies
└── package.json                         # Build scripts
```

### Architecture Patterns
- **BLoC Pattern**: Cubit for state management
- **Dependency Injection**: Injectable for service management
- **Repository Pattern**: API data access
- **Factory Pattern**: State object creation
- **Observer Pattern**: State change notifications

### Code Generation
The package uses code generation for JSON serialization and dependency injection:

```bash
# Generate code
dart run build_runner build --delete-conflicting-outputs

# Clean generated files
dart run build_runner clean

# Watch for changes
dart run build_runner watch
```

### Testing
Run unit tests to ensure code quality:
```bash
flutter test
```

### Widgetbook Integration
The package includes Widgetbook integration for component documentation:

```dart
@widgetbook.UseCase(
  name: 'A profile dialog',
  type: ProfileDialog,
)
Widget defaultProfileDialog(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    const ProfileDialog(),
  );
}
```

### Code Quality
- Follow Dart coding conventions
- Use proper documentation for public APIs
- Implement comprehensive error handling
- Add unit tests for all public methods
- Use BLoC pattern for state management

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **api_sdk**: API client for backend communication
- **authress_login**: Authentication management
- **ui**: Shared UI components
- **internationalization**: Multi-language support
- **websocket**: Real-time communication
- **app_config_core**: Configuration management
- **app_config_secrets**: Secure secret storage
- **flutter_bloc**: ^9.1.1 - State management
- **dio**: ^5.8.0+1 - HTTP client
- **get_it**: ^8.0.3 - Dependency injection
- **widgetbook**: ^3.14.3 - Component documentation
- **widgetbook_annotation**: ^3.5.0 - Widgetbook annotations
- **json_annotation**: ^4.9.0 - JSON serialization
- **injectable**: ^2.5.0 - Dependency injection

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **build_runner**: ^2.5.4 - Code generation
- **widgetbook_generator**: ^3.13.0 - Widgetbook code generation
- **json_serializable**: ^6.9.5 - JSON code generation
- **injectable_generator**: ^2.7.0 - Injectable code generation

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[API SDK](../../../api/sdk/)**: Flutter API client
- **[Authress Login](../../../authress/login/)**: Authentication package
- **[UI Package](../../../ui/)**: Shared UI components
- **[Internationalization](../../../internationalization/)**: Multi-language support
- **[WebSocket](../../../websocket/)**: Real-time communication
- **[App Config Core](../../../app_config_core/)**: Configuration management
- **[App Config Secrets](../../../app_config_secrets/)**: Secure secret storage

### Backend Services
- **[Friends API](../../../../backend/dotnet/friends/)**: Friend management
- **[Games API](../../../../backend/dotnet/games/)**: Game management
- **[Auth API](../../../../backend/dotnet/auth/)**: Authentication

## Version History

- **v0.0.8** (2025-07-27): Various updates
- **v0.0.7** (2025-07-26): Implemented friends feature
- **v0.0.6** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.5** (2025-07-10): Add flutter workspace at root
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution
- **v0.0.2** (2025-07-07): Initial change log

## Troubleshooting

### Common Issues

1. **Profile data not loading**
   - Check authentication tokens are valid
   - Verify API endpoint configuration
   - Ensure proper error handling in init() method

2. **Authentication errors**
   - Verify Authress login is properly configured
   - Check token refresh mechanism
   - Ensure app_config_secrets is properly set up

3. **Profile dialog not showing**
   - Verify ProfileCubit is properly provided
   - Check that dialog is called from correct context
   - Ensure UI package is properly imported

4. **API communication failures**
   - Check network connectivity
   - Verify API endpoint URLs
   - Ensure proper error handling for API responses

5. **Code generation issues**
   - Run `dart run build_runner clean` then `dart run build_runner build`
   - Check for syntax errors in source files
   - Verify all dependencies are properly installed

6. **Internationalization not working**
   - Check internationalization package configuration
   - Verify locale settings
   - Ensure proper text localization

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Maintain BLoC pattern for state management
6. Test API integration thoroughly
7. Update widgetbook use cases for UI changes
8. Verify authentication integration works correctly

## License

This project is part of the Flyingdarts platform and is subject to the project's licensing terms.
