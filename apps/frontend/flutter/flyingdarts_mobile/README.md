# Flying Darts Mobile App

## Overview

The Flying Darts Mobile App is a Flutter-based cross-platform mobile application that enables users to play darts games, track scores, and participate in multiplayer sessions. Built with modern Flutter architecture, the app provides a seamless darts gaming experience with real-time connectivity, authentication, and comprehensive game management features.

This application is responsible for:
- Providing a native mobile interface for darts gameplay
- Managing user authentication through Authress integration
- Enabling real-time multiplayer darts games via WebSocket connections
- Supporting multiple game modes including X01 variants
- Offering voice commands and speech-to-text functionality
- Providing internationalization support for multiple languages
- Managing user profiles and preferences

## Features

- **Cross-Platform Support**: iOS and Android compatibility with single codebase
- **Real-time Multiplayer**: WebSocket-based live gaming with other players
- **Authentication**: Secure login via Authress enterprise authentication
- **Voice Commands**: Speech-to-text functionality for hands-free score input
- **Internationalization**: Multi-language support with localization
- **Game Modes**: Support for X01, 301, 501, and 701 game variants
- **Profile Management**: User profiles with statistics and preferences
- **Offline Support**: Basic functionality when offline
- **Responsive Design**: Adaptive UI for different screen sizes
- **Dark/Light Theme**: Theme support with Material Design 3

## Prerequisites

- Flutter SDK 3.26.0 or higher
- Dart SDK 3.8.1 or higher
- Android Studio or VS Code with Flutter extensions
- iOS development tools (for iOS builds)
- Android development tools (for Android builds)

## Installation

1. Clone the monorepo and navigate to the mobile app:
```bash
cd apps/frontend/flutter/flyingdarts_mobile
```

2. Install dependencies:
```bash
flutter pub get
```

3. Run the app in development mode:
```bash
flutter run
```

## Usage

### Development

Run the app in development mode:

```bash
flutter run --flavor dev
```

### Production

Build and run the production version:

```bash
flutter run --flavor prod
```

### Platform-Specific Builds

#### Android
```bash
# Debug build
flutter build apk --flavor dev

# Release build
flutter build apk --flavor prod --release

# App bundle for Play Store
flutter build appbundle --flavor prod --release
```

#### iOS
```bash
# Debug build
flutter build ios --flavor dev

# Release build
flutter build ios --flavor prod --release
```

#### Web
```bash
# Debug build
flutter build web --flavor dev

# Release build
flutter build web --flavor prod --release
```

## Architecture

### Project Structure

```
lib/
├── app/
│   ├── app.dart              # Main app widget
│   ├── routes/               # App routing configuration
│   └── shell/                # App shell components
├── pages/                    # Screen implementations
├── widgets/                  # Reusable UI components
├── di.dart                   # Dependency injection setup
├── flavor_config.dart        # Build flavor configuration
├── main_acc.dart            # Acceptance testing entry point
├── main_dev.dart            # Development entry point
├── main_prod.dart           # Production entry point
└── runner.dart              # App initialization runner
```

### Key Components

#### `App`

The main application widget that sets up authentication and routing.

**Properties:**
- `AuthressProvider`: Wraps the app with authentication context
- `AuthressPageGuard`: Protects routes based on authentication status
- `MaterialApp.router`: Main app with routing configuration

**Methods:**

##### `build(BuildContext context): Widget`

Builds the main application widget with authentication and routing setup.

**Parameters:**
- `context` (BuildContext): The build context

**Returns:**
- `Widget`: The configured app widget

#### `Runner`

Handles app initialization and dependency setup.

**Properties:**
- `FlavorConfig flavor`: Build flavor configuration
- `Widget app`: The main app widget

**Methods:**

##### `withFlavor(String flavor): Runner`

Configures the build flavor for the app.

**Parameters:**
- `flavor` (String): Build flavor (dev, acc, prod)

**Returns:**
- `Runner`: Self for method chaining

##### `run(): void`

Initializes and runs the application.

##### `_initializeApp(): Future<Widget>`

Asynchronously initializes app dependencies and returns the app widget.

**Returns:**
- `Future<Widget>`: The initialized app widget

### State Management

The app uses BLoC (Business Logic Component) pattern for state management:

- **flutter_bloc**: BLoC implementation for Flutter
- **bloc**: Core BLoC library for state management
- **provider**: Additional state management utilities

### Routing

Navigation is handled by GoRouter for declarative routing:

- **go_router**: Modern routing solution for Flutter
- **Deep linking**: Support for custom URL schemes
- **Route guards**: Authentication-based route protection

## Dependencies

### Internal Dependencies (Workspace Packages)

- **api_sdk**: API client for backend communication
- **app_config_core**: Core configuration management
- **app_config_prefs**: User preferences storage
- **app_config_secrets**: Secure configuration storage
- **core**: Core business logic and utilities
- **internationalization**: Multi-language support
- **keyboard**: Custom keyboard components
- **language**: Language selection and management
- **profile**: User profile management
- **speech**: Speech-to-text functionality
- **splash**: Splash screen components
- **ui**: Shared UI components and themes
- **websocket**: WebSocket connection management
- **authress_login**: Authress authentication integration

### External Dependencies

- **flutter_bloc**: State management
- **go_router**: Navigation and routing
- **get_it**: Dependency injection
- **injectable**: Code generation for dependency injection
- **flutter_svg**: SVG image support
- **lottie**: Animation support
- **speech_to_text**: Voice input functionality
- **curved_navigation_bar**: Custom navigation bar
- **loading_animation_widget**: Loading animations
- **widgetbook**: Component documentation and testing

### Development Dependencies

- **flutter_test**: Unit testing framework
- **integration_test**: Integration testing
- **bloc_test**: BLoC testing utilities
- **mockito**: Mocking framework
- **build_runner**: Code generation
- **injectable_generator**: Dependency injection code generation
- **widgetbook_generator**: Component documentation generation

## Configuration

### Build Flavors

The app supports multiple build flavors:

- **dev**: Development environment with debug features
- **acc**: Acceptance testing environment
- **prod**: Production environment with optimizations

### Environment Variables

- `AUTHRESS_APPLICATION_ID`: Authress application identifier
- `AUTHRESS_API_URL`: Authress service URL
- `API_BASE_URL`: Backend API base URL
- `WEBSOCKET_URL`: WebSocket connection URL

### Authentication Configuration

```dart
AuthressConfiguration(
  applicationId: 'app_2YKyhM6M31XVtuCeuDsSJ2',
  authressApiUrl: 'https://authress.flyingdarts.net',
)
```

## Development

### Code Generation

Run code generation for dependencies and annotations:

```bash
flutter packages pub run build_runner build
```

Watch for changes and regenerate automatically:

```bash
flutter packages pub run build_runner watch
```

### Testing

Run unit tests:

```bash
flutter test
```

Run integration tests:

```bash
flutter test integration_test/
```

Run widget tests:

```bash
flutter test test/widget_test.dart
```

### Code Quality

The project uses:
- **flutter_lints**: Flutter-specific linting rules
- **equatable**: Value equality for objects
- **injectable**: Dependency injection with code generation

Run linting:

```bash
flutter analyze
```

### Asset Management

Assets are organized in the `assets/` directory:

- **animations/**: Lottie animation files
- **icons/**: SVG and PNG icons
- **fonts/**: Custom fonts (Poppins)

## Deployment

### Android Deployment

1. Build the release APK:
```bash
flutter build apk --flavor prod --release
```

2. Build the app bundle for Play Store:
```bash
flutter build appbundle --flavor prod --release
```

### iOS Deployment

1. Build for iOS:
```bash
flutter build ios --flavor prod --release
```

2. Archive and upload to App Store Connect using Xcode.

### Web Deployment

1. Build for web:
```bash
flutter build web --flavor prod --release
```

2. Deploy the `build/web/` directory to your web server.

## Related Projects

- **flyingdarts-api-sdk**: API client package
- **flyingdarts-ui**: Shared UI components
- **flyingdarts-core**: Core business logic
- **flyingdarts-websocket**: WebSocket communication
- **flyingdarts-authress-login**: Authentication integration

## Troubleshooting

### Common Issues

1. **Dependency Resolution**: Run `flutter pub get` after workspace changes
2. **Code Generation**: Ensure `build_runner` is up to date
3. **Platform-Specific Issues**: Check platform-specific setup guides
4. **Authentication**: Verify Authress configuration
5. **WebSocket Connection**: Check network connectivity and server status

### Debugging

Enable debug logging:

```dart
// In your code
debugPrint('Debug message');
```

Use Flutter Inspector for UI debugging:

```bash
flutter run --debug
```

### Performance

- **Memory Management**: Monitor memory usage in debug mode
- **Network Optimization**: Use appropriate image formats and sizes
- **State Management**: Avoid unnecessary widget rebuilds
- **Asset Optimization**: Compress images and animations

## Security Considerations

- **Authentication**: All API calls require valid authentication tokens
- **Data Storage**: Sensitive data is stored securely using app_config_secrets
- **Network Security**: Use HTTPS for all API communications
- **Input Validation**: Validate all user inputs
- **Code Obfuscation**: Enable code obfuscation for release builds

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for UI changes
4. Ensure all tests pass before submitting PR
5. Follow Flutter best practices and conventions

## License

Part of the Flying Darts Turbo monorepo. See root LICENSE file for details.