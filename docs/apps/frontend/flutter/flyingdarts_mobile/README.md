# Flying Darts Mobile App

## Overview

The Flying Darts Mobile App is a cross-platform mobile application built with Flutter that provides a comprehensive darts gaming experience on iOS, Android, and Web platforms. This application offers real-time multiplayer gameplay, voice communication, and social features optimized for mobile devices.

This application is responsible for:
- Providing a native mobile experience for darts games
- Supporting cross-platform deployment (iOS, Android, Web)
- Managing user authentication and authorization
- Handling real-time multiplayer gameplay
- Supporting voice and speech recognition features
- Providing offline capabilities and local game modes
- Integrating with backend services via WebSocket and REST APIs
- Managing device-specific features (camera, microphone, sensors)

## Features

- **Cross-platform Support**: Native performance on iOS, Android, and Web
- **Real-time Multiplayer Gaming**: Live multiplayer darts games with WebSocket communication
- **Voice Communication**: Integrated voice chat and speech recognition
- **Offline Mode**: Local game modes for offline play
- **Social Features**: Friends management and social interactions
- **Responsive Design**: Adaptive UI for different screen sizes and orientations
- **State Management**: BLoC pattern for complex application state
- **Internationalization**: Multi-language support with i18n
- **Accessibility**: Full accessibility support for all users
- **Push Notifications**: Real-time notifications for game events
- **Device Integration**: Camera, microphone, and sensor integration

## Prerequisites

- Flutter SDK (3.26.0 or higher)
- Dart SDK (3.8.1 or higher)
- Android Studio or VS Code with Flutter extensions
- Xcode (for iOS development)
- Android SDK (for Android development)
- Node.js (for web development)

## Installation

1. Clone the monorepo and navigate to the Flutter app:
```bash
cd apps/frontend/flutter/flyingdarts_mobile
```

2. Install Flutter dependencies:
```bash
flutter pub get
```

3. Configure environment variables:
```bash
cp lib/app/config/env.example.dart lib/app/config/env.dart
```

4. Update the environment configuration with your settings.

## Usage

### Development

Run the app in development mode:

```bash
# For iOS
flutter run -d ios

# For Android
flutter run -d android

# For Web
flutter run -d chrome
```

### Building for Production

Build the app for different platforms:

```bash
# iOS
flutter build ios --release

# Android
flutter build apk --release
flutter build appbundle --release

# Web
flutter build web --release
```

### Testing

Run the test suite:

```bash
# Unit tests
flutter test

# Integration tests
flutter test integration_test/

# Widget tests
flutter test test/widget_test.dart
```

## Application Structure

### Core Architecture

The application follows Flutter best practices with a modular architecture:

```
lib/
├── app/                          # Application core
│   ├── config/                  # Configuration and environment
│   ├── di/                      # Dependency injection
│   ├── router/                  # Navigation and routing
│   └── theme/                   # App theming and styling
├── pages/                       # Screen pages
│   ├── auth/                   # Authentication pages
│   ├── game/                   # Game-related pages
│   ├── profile/                # Profile management pages
│   └── settings/               # Settings pages
├── widgets/                     # Reusable widgets
│   ├── common/                 # Common UI components
│   ├── game/                   # Game-specific widgets
│   └── forms/                  # Form components
└── main_*.dart                 # Platform-specific entry points
```

### Key Components

#### Core Services

##### `ApiService`

Handles API communication with backend services.

**Methods:**
- `getGames()`: Fetch available games
- `createGame()`: Create a new game
- `joinGame()`: Join an existing game
- `updateScore()`: Update player score

##### `WebSocketService`

Manages real-time communication with backend services.

**Methods:**
- `connect()`: Establish WebSocket connection
- `disconnect()`: Close WebSocket connection
- `sendMessage()`: Send message to server
- `subscribe()`: Subscribe to real-time updates

##### `AuthService`

Handles user authentication and authorization.

**Methods:**
- `login()`: Authenticate user
- `logout()`: Logout current user
- `getCurrentUser()`: Get current authenticated user
- `isAuthenticated()`: Check if user is authenticated

#### Feature Modules

##### Authentication Module (`auth/`)

- Login/Logout screens
- User registration
- Password reset
- Profile management

##### Game Module (`game/`)

- Game creation and joining
- Score tracking
- Game state management
- Real-time game updates

##### Profile Module (`profile/`)

- User profile management
- Game statistics
- Achievement tracking
- Settings management

##### Settings Module (`settings/`)

- App configuration
- Notification settings
- Privacy settings
- Language preferences

### State Management

The application uses BLoC (Business Logic Component) pattern for state management:

#### BLoC Structure

```dart
abstract class GameEvent extends Equatable {
  const GameEvent();
}

abstract class GameState extends Equatable {
  const GameState();
}

class GameBloc extends Bloc<GameEvent, GameState> {
  // BLoC implementation
}
```

#### Key BLoCs

- `AuthBloc`: Authentication state management
- `GameBloc`: Game state management
- `ProfileBloc`: Profile state management
- `SettingsBloc`: Settings state management

## Configuration

### Environment Configuration

The application uses environment-specific configuration:

#### Development Environment (`env.dart`)

```dart
class Environment {
  static const String apiUrl = 'http://localhost:3000';
  static const String wsUrl = 'ws://localhost:3000';
  static const String authressUrl = 'https://your-domain.authress.io';
  static const bool isProduction = false;
}
```

#### Production Environment

```dart
class Environment {
  static const String apiUrl = 'https://api.flyingdarts.com';
  static const String wsUrl = 'wss://api.flyingdarts.com';
  static const String authressUrl = 'https://your-domain.authress.io';
  static const bool isProduction = true;
}
```

### Platform Configuration

#### iOS Configuration (`ios/`)

- Info.plist settings
- App permissions
- Signing and provisioning
- Bundle identifier

#### Android Configuration (`android/`)

- AndroidManifest.xml
- Gradle configuration
- Signing configuration
- Package name

#### Web Configuration (`web/`)

- index.html
- Web-specific assets
- Service worker configuration

## Dependencies

### Core Dependencies

- **Flutter**: Core framework
- **BLoC**: State management
- **Go Router**: Navigation
- **Get It**: Dependency injection
- **Injectable**: Code generation for DI
- **Authress Login**: Authentication
- **Speech to Text**: Voice recognition

### Development Dependencies

- **Flutter Test**: Testing framework
- **BLoC Test**: BLoC testing utilities
- **Mockito**: Mocking framework
- **Build Runner**: Code generation
- **Flutter Lints**: Code linting

### Workspace Packages

- **api_sdk**: API client SDK
- **core**: Core utilities and services
- **ui**: Shared UI components
- **websocket**: WebSocket utilities
- **internationalization**: i18n support
- **keyboard**: Keyboard handling
- **speech**: Speech recognition
- **profile**: Profile management

## Development

### Project Structure

```
flyingdarts_mobile/
├── lib/                         # Application source code
│   ├── app/                    # Application core
│   ├── pages/                  # Screen pages
│   ├── widgets/                # Reusable widgets
│   └── main_*.dart            # Entry points
├── test/                       # Unit and widget tests
├── integration_test/           # Integration tests
├── assets/                     # Static assets
├── ios/                       # iOS-specific configuration
├── android/                   # Android-specific configuration
├── web/                       # Web-specific configuration
├── pubspec.yaml               # Dependencies and configuration
└── README.md                  # This documentation
```

### Code Organization

#### Feature-based Architecture

Each feature is organized with its own:
- Pages/Screens
- Widgets
- BLoCs
- Services
- Models

#### Shared Resources

Common utilities and components are shared:
- UI components
- Services
- Models
- Utilities
- Constants

### Testing Strategy

#### Unit Tests

- BLoC testing with bloc_test
- Service testing with mocks
- Utility function testing
- Model testing

#### Widget Tests

- Component testing
- Screen testing
- Navigation testing
- User interaction testing

#### Integration Tests

- End-to-end user flows
- Cross-platform compatibility
- Performance testing
- Accessibility testing

### Code Quality

The project uses:
- **Flutter Lints**: Code linting and style enforcement
- **Dart Analyzer**: Static analysis
- **BLoC Test**: State management testing
- **Mockito**: Mocking for testing

Run quality checks:

```bash
flutter analyze
flutter test
flutter build
```

## User Interface

### Design System

The application uses Material Design 3 with custom theming:

#### Components

- **Navigation**: Bottom navigation with tab-based routing
- **Game Board**: Interactive darts board with touch controls
- **Chat Interface**: Real-time messaging during games
- **Friends List**: Social features and friend management
- **Profile Dashboard**: User statistics and achievements

#### Responsive Design

- Adaptive layouts for different screen sizes
- Orientation support (portrait/landscape)
- Tablet optimizations
- Web responsive design

### Accessibility

- Screen reader support
- High contrast mode
- Large text support
- Voice control integration
- Keyboard navigation (web)

## Platform-specific Features

### iOS Features

- **Face ID/Touch ID**: Biometric authentication
- **Push Notifications**: APNs integration
- **Siri Shortcuts**: Voice commands
- **Share Extension**: Content sharing
- **Widgets**: Home screen widgets

### Android Features

- **Fingerprint Authentication**: Biometric authentication
- **Push Notifications**: FCM integration
- **Widgets**: Home screen widgets
- **Share Intent**: Content sharing
- **Background Services**: Background processing

### Web Features

- **PWA Support**: Progressive Web App capabilities
- **Service Worker**: Offline functionality
- **Web Push**: Browser notifications
- **Keyboard Navigation**: Full keyboard support
- **Responsive Design**: Adaptive layouts

## Integration

### Backend Services

#### REST API Integration

- Game management endpoints
- User profile endpoints
- Friends management endpoints
- Statistics and achievements

#### WebSocket Integration

- Real-time game updates
- Live chat functionality
- Player presence and status
- Game state synchronization

### Third-party Services

#### Authress

- User authentication
- Social login providers
- Role-based access control
- User management

#### Speech Recognition

- Voice commands
- Speech-to-text conversion
- Multi-language support
- Offline speech recognition

## Performance Optimization

### Build Optimization

- Tree shaking for unused code
- Asset optimization and compression
- Code splitting for web
- Native compilation for mobile

### Runtime Optimization

- Efficient state management
- Memory leak prevention
- Image caching and optimization
- Background processing optimization

### Platform-specific Optimization

#### Mobile

- Native performance optimization
- Battery usage optimization
- Memory management
- Network optimization

#### Web

- Bundle size optimization
- Loading performance
- Caching strategies
- Service worker optimization

## Security

### Authentication

- Secure token storage
- Biometric authentication
- Session management
- Token refresh handling

### Data Protection

- Secure communication (HTTPS/WSS)
- Data encryption
- Input validation
- Privacy protection

### Platform Security

#### iOS

- Keychain integration
- App Transport Security
- Code signing
- Sandboxing

#### Android

- Keystore integration
- Network Security Config
- App signing
- Permission management

## Deployment

### Build Process

1. **Development Build**:
   ```bash
   flutter build apk --debug
   ```

2. **Production Build**:
   ```bash
   flutter build appbundle --release
   ```

3. **Testing**:
   ```bash
   flutter test
   flutter test integration_test/
   ```

### Deployment Options

#### Mobile App Stores

- **iOS App Store**: Apple App Store deployment
- **Google Play Store**: Android Play Store deployment
- **TestFlight**: iOS beta testing
- **Internal Testing**: Android beta testing

#### Web Deployment

- **Static Hosting**: Netlify, Vercel, GitHub Pages
- **CDN**: CloudFront, Cloudflare
- **Container**: Docker, Kubernetes

### CI/CD Pipeline

- Automated testing
- Code quality checks
- Build automation
- Deployment automation

## Troubleshooting

### Common Issues

1. **Build Failures**: Check Flutter and Dart versions
2. **Runtime Errors**: Check device logs and Flutter inspector
3. **Authentication Issues**: Verify Authress configuration
4. **WebSocket Connection**: Check network connectivity

### Debugging

Enable debug logging:

```dart
// In main.dart
void main() {
  debugPrint = (String? message, {int? wrapWidth}) {
    print('DEBUG: $message');
  };
  runApp(MyApp());
}
```

### Platform-specific Issues

#### iOS

- Xcode build issues
- Simulator problems
- Device deployment issues
- Code signing problems

#### Android

- Gradle build issues
- Emulator problems
- Device deployment issues
- Permission problems

#### Web

- Browser compatibility
- Build optimization
- Service worker issues
- PWA configuration

## Related Projects

- **flyingdarts-backend**: Backend services and APIs
- **flyingdarts-web**: Angular web application
- **flyingdarts-packages**: Shared Flutter packages
- **flyingdarts-config**: Shared configuration

## Contributing

1. Follow Flutter style guide and best practices
2. Write comprehensive tests for new features
3. Update documentation for API changes
4. Ensure accessibility compliance
5. Follow the monorepo coding standards

## License

Part of the Flying Darts Turbo monorepo. See root LICENSE file for details.