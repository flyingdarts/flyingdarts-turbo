# Flying Darts Splash Feature

## Overview

The Flying Darts Splash Feature is a Flutter package that provides a splash screen implementation for the Flying Darts gaming platform. This package enables Flutter applications to display a branded splash screen during app initialization with automatic navigation to the main application.

The package is built using modern Flutter patterns with Go Router integration and SVG asset support. It provides a clean, professional splash screen experience that enhances the user's first impression of the application while maintaining consistent branding across the platform.

## Features

- **Branded Splash Screen**: Professional splash screen with Flying Darts branding
- **Automatic Navigation**: Seamless transition to main application after splash
- **SVG Asset Support**: High-quality vector graphics for crisp display
- **Go Router Integration**: Clean routing with automatic navigation
- **Responsive Design**: Adapts to different screen sizes and orientations
- **Theme Integration**: Consistent theming with shared UI package
- **Cross-Platform Support**: Works on iOS, Android, and Web
- **Lightweight**: Minimal dependencies for fast loading
- **Customizable Duration**: Configurable splash screen duration

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ">=3.26.0"
- **Go Router**: ^16.0.0 for navigation
- **Dependencies**: Access to shared packages (ui)

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the splash package**:
   ```bash
   cd packages/frontend/flutter/features/splash
   ```

3. **Get dependencies**:
   ```bash
   flutter pub get
   ```

### In Your Flutter Project
Add the package to your `pubspec.yaml`:

```yaml
dependencies:
  splash:
    path: ../../packages/frontend/flutter/features/splash
```

## Usage

### Basic Setup

1. **Add the splash screen to your app**:

```dart
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:splash/splash.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  final _router = GoRouter(
    routes: [
      GoRoute(
        path: '/',
        builder: (context, state) => SplashScreen(),
      ),
      GoRoute(
        path: '/home',
        builder: (context, state) => HomePage(),
      ),
    ],
  );

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Flying Darts',
      routerConfig: _router,
    );
  }
}
```

2. **Custom splash screen with different duration**:

```dart
class CustomSplashScreen extends StatefulWidget {
  final Duration duration;
  final String nextRoute;

  const CustomSplashScreen({
    super.key,
    this.duration = const Duration(seconds: 1),
    this.nextRoute = '/home',
  });

  @override
  State<CustomSplashScreen> createState() => _CustomSplashScreenState();
}

class _CustomSplashScreenState extends State<CustomSplashScreen> {
  @override
  void initState() {
    super.initState();

    Timer(widget.duration, () {
      context.go(widget.nextRoute);
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: MediaQuery.of(context).size.width,
      height: MediaQuery.of(context).size.height,
      color: MyTheme.primaryColor,
      child: Center(
        child: SvgPicture.asset(
          'assets/icons/fd_icon.svg',
          width: MediaQuery.of(context).size.width / 2,
        ),
      ),
    );
  }
}
```

### Integration with Authentication

```dart
class AuthAwareSplashScreen extends StatefulWidget {
  const AuthAwareSplashScreen({super.key});

  @override
  State<AuthAwareSplashScreen> createState() => _AuthAwareSplashScreenState();
}

class _AuthAwareSplashScreenState extends State<AuthAwareSplashScreen> {
  @override
  void initState() {
    super.initState();

    Timer(const Duration(seconds: 1), () {
      // Check authentication status and navigate accordingly
      final isAuthenticated = checkAuthenticationStatus();
      if (isAuthenticated) {
        context.go('/home');
      } else {
        context.go('/login');
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return SplashScreen();
  }

  bool checkAuthenticationStatus() {
    // Implement your authentication check logic
    return false;
  }
}
```

### Custom Branding

```dart
class CustomBrandedSplashScreen extends StatefulWidget {
  final String logoPath;
  final Color backgroundColor;
  final Duration duration;

  const CustomBrandedSplashScreen({
    super.key,
    this.logoPath = 'assets/icons/fd_icon.svg',
    this.backgroundColor = MyTheme.primaryColor,
    this.duration = const Duration(seconds: 1),
  });

  @override
  State<CustomBrandedSplashScreen> createState() => _CustomBrandedSplashScreenState();
}

class _CustomBrandedSplashScreenState extends State<CustomBrandedSplashScreen> {
  @override
  void initState() {
    super.initState();

    Timer(widget.duration, () {
      context.go('/home');
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: MediaQuery.of(context).size.width,
      height: MediaQuery.of(context).size.height,
      color: widget.backgroundColor,
      child: Center(
        child: SvgPicture.asset(
          widget.logoPath,
          width: MediaQuery.of(context).size.width / 2,
        ),
      ),
    );
  }
}
```

## API Reference

### Core Components

#### SplashScreen
The main splash screen widget.

```dart
class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}
```

### Properties

#### SplashScreen Properties
- **`key`**: Optional widget key for identification

### Methods

#### SplashScreen Methods
- **`createState()`**: Creates the state for the splash screen widget

#### _SplashScreenState Methods
- **`initState()`**: Initializes the splash screen and sets up navigation timer
- **`build(BuildContext context)`**: Builds the splash screen UI

### Behavior

#### Automatic Navigation
The splash screen automatically navigates to `/home` after 1 second:

```dart
Timer(const Duration(seconds: 1), () {
  context.go('/home');
});
```

#### UI Layout
The splash screen displays:
- Full-screen container with primary theme color
- Centered SVG logo (Flying Darts icon)
- Responsive design that adapts to screen size

## Configuration

### Asset Configuration
Ensure the Flying Darts logo is available in your assets:

```yaml
# pubspec.yaml
flutter:
  assets:
    - assets/icons/fd_icon.svg
```

### Theme Integration
The splash screen uses the shared UI theme:

```dart
import 'package:ui/ui.dart';

// Uses MyTheme.primaryColor for background
color: MyTheme.primaryColor,
```

### Navigation Configuration
Configure Go Router for proper navigation:

```dart
final _router = GoRouter(
  routes: [
    GoRoute(
      path: '/',
      builder: (context, state) => SplashScreen(),
    ),
    GoRoute(
      path: '/home',
      builder: (context, state) => HomePage(),
    ),
  ],
);
```

## Development

### Project Structure
```
splash/
├── lib/
│   ├── splash.dart                    # Main library export
│   └── pages/                         # UI components
│       └── splash_screen.dart         # Splash screen implementation
├── test/                              # Unit tests
├── pubspec.yaml                       # Package dependencies
└── package.json                       # Build scripts
```

### Architecture Patterns
- **Stateful Widget Pattern**: State management for timer and navigation
- **Timer Pattern**: Automatic navigation after delay
- **Responsive Design**: Adapts to different screen sizes
- **Theme Integration**: Consistent theming with shared UI

### Testing
Run unit tests to ensure code quality:
```bash
flutter test
```

### Code Quality
- Follow Dart coding conventions
- Use proper documentation for public APIs
- Implement responsive design patterns
- Test on multiple screen sizes and orientations

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **flutter_svg**: ^2.2.0 - SVG asset support
- **go_router**: ^16.0.0 - Navigation
- **ui**: Shared UI components

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[UI Package](../../../ui/)**: Shared UI components
- **[Authress Login](../../../authress/login/)**: Authentication package
- **[Profile Feature](../../../features/profile/)**: User profile management
- **[Language Feature](../../../features/language/)**: Multi-language support

### Backend Services
- **[Friends API](../../../../backend/dotnet/friends/)**: Friend management
- **[Games API](../../../../backend/dotnet/games/)**: Game management
- **[Auth API](../../../../backend/dotnet/auth/)**: Authentication

## Version History

- **v0.0.7** (2025-07-26): Implemented friends feature
- **v0.0.6** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.5** (2025-07-10): Add flutter workspace at root
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution
- **v0.0.2** (2025-07-07): Initial change log

## Troubleshooting

### Common Issues

1. **Splash screen not showing**
   - Verify the route is properly configured in Go Router
   - Check that the splash screen is set as the initial route
   - Ensure the package is properly imported

2. **Navigation not working**
   - Verify Go Router is properly configured
   - Check that the target route exists
   - Ensure the navigation path is correct

3. **Logo not displaying**
   - Check that the SVG asset is properly included in pubspec.yaml
   - Verify the asset path is correct
   - Ensure flutter_svg package is properly installed

4. **Theme not applying**
   - Verify the UI package is properly imported
   - Check that MyTheme.primaryColor is available
   - Ensure theme configuration is correct

5. **Splash screen duration issues**
   - Check the Timer duration configuration
   - Verify the navigation callback is working
   - Ensure no blocking operations in initState

6. **Responsive design issues**
   - Test on different screen sizes
   - Verify MediaQuery is working correctly
   - Check that the logo scales appropriately

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Test on multiple screen sizes and orientations
6. Verify navigation works correctly
7. Test with different theme configurations
8. Ensure SVG assets are properly optimized

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
