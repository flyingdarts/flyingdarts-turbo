# Flying Darts UI

## Overview

The Flying Darts UI is a Flutter package that provides a comprehensive set of shared UI components, themes, and utilities for the Flying Darts gaming platform. This package enables Flutter applications to maintain consistent design patterns, branding, and user experience across the entire platform with reusable components, standardized themes, and development tools.

The package is built using Flutter's Material Design framework with custom theming, Lottie animations, and Widgetbook for component documentation and testing. It provides a robust foundation for implementing consistent UI patterns in Flutter apps with pre-built components, standardized error handling, navigation utilities, and comprehensive theming support. The package includes components for authentication, error dialogs, navigation, animations, and development tooling.

## Features

- **Shared UI Components**: Reusable widgets for common UI patterns
- **Custom Theme System**: Comprehensive Material Design theme with Flying Darts branding
- **Lottie Animation Support**: Integrated Lottie animations with error handling
- **Error Dialog System**: Standardized error handling and display components
- **Navigation Components**: Custom back button and navigation utilities
- **Authentication UI**: Facebook login button and authentication components
- **Widgetbook Integration**: Component documentation and testing framework
- **Key Management**: Centralized key management for testing and automation
- **Material Wrappers**: Utility wrappers for Material Design components
- **Cross-Platform Support**: Works on iOS, Android, and Web
- **Type Safety**: Strongly typed components with proper error handling
- **Accessibility Support**: Built-in accessibility features and testing keys
- **Development Tools**: Code generation and testing utilities

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ^3.26.0 or higher
- **Lottie**: ^3.1.2 for animation support
- **Widgetbook**: ^3.14.3 for component documentation
- **Build Tools**: build_runner for code generation

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the UI package**:
   ```bash
   cd packages/frontend/flutter/shared/ui
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
  ui:
    path: ../../packages/frontend/flutter/shared/ui
  lottie: ^3.1.2
  widgetbook: ^3.14.3
```

## Usage

### Basic Theme Setup

```dart
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flying Darts',
      theme: myTheme, // Use the shared theme
      home: MyHomePage(),
    );
  }
}

class MyHomePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return FlyingdartsScaffold(
      child: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text('Welcome to Flying Darts'),
            LoginWithFacebookButton(
              onPressed: () {
                // Handle Facebook login
              },
            ),
          ],
        ),
      ),
    );
  }
}
```

### Using Lottie Animations

```dart
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

class AnimationExample extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Load animation from asset
        LottieWidget.fromAsset(
          assetPath: 'assets/animations/loading.json',
          width: 200,
          height: 200,
          repeat: true,
          animate: true,
        ),
        
        // Load animation from JSON string
        LottieWidget.fromJson(
          jsonString: '{"v": "5.5.7", "fr": 29.9700012207031, ...}',
          width: 150,
          height: 150,
          repeat: false,
          animate: true,
        ),
        
        // Custom animation with error handling
        LottieWidget(
          assetPath: 'assets/animations/custom.json',
          width: 100,
          height: 100,
          fit: BoxFit.contain,
          alignment: Alignment.center,
        ),
      ],
    );
  }
}
```

### Error Dialog Implementation

```dart
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

class ErrorHandlingExample extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: () {
        showDialog(
          context: context,
          builder: (context) => LanguageErrorDialog(
            error: 'Failed to load language settings. Please try again.',
          ),
        );
      },
      child: Text('Show Error Dialog'),
    );
  }
}

// Custom error dialog implementation
class CustomErrorDialog extends ErrorDialog<String> {
  const CustomErrorDialog({
    super.key,
    required super.error,
  });

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text('Error'),
      content: Text(error),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: Text('OK'),
        ),
      ],
    );
  }
}
```

### Navigation Components

```dart
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

class NavigationExample extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return FlyingdartsScaffold(
      leading: BackButton(), // Custom back button with navigation
      child: Center(
        child: Column(
          children: [
            Text('Navigation Example'),
            ElevatedButton(
              onPressed: () {
                // Navigate to next screen
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => NextScreen()),
                );
              },
              child: Text('Next Screen'),
            ),
          ],
        ),
      ),
    );
  }
}

class NextScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return FlyingdartsScaffold(
      leading: BackButton(), // Will automatically pop when pressed
      child: Center(
        child: Text('Next Screen'),
      ),
    );
  }
}
```

### Authentication Components

```dart
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

class AuthenticationExample extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return FlyingdartsScaffold(
      child: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text('Login to Flying Darts'),
            SizedBox(height: 20),
            LoginWithFacebookButton(
              onPressed: () async {
                try {
                  // Handle Facebook authentication
                  await _handleFacebookLogin();
                } catch (e) {
                  // Show error dialog
                  showDialog(
                    context: context,
                    builder: (context) => LanguageErrorDialog(
                      error: 'Facebook login failed: ${e.toString()}',
                    ),
                  );
                }
              },
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _handleFacebookLogin() async {
    // Implement Facebook login logic
  }
}
```

### Material Wrappers

```dart
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

class WrapperExample extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Default material widget wrapper
        createDefaultMaterialWidget(
          context,
          Text('Wrapped in Material App'),
        ),
        
        // Default card widget wrapper
        createDefaultCardWidget(
          Column(
            children: [
              Text('Content in Card'),
              ElevatedButton(
                onPressed: () {},
                child: Text('Button'),
              ),
            ],
          ),
        ),
        
        // Default widget in card with context
        createDefaultWidgetInACard(
          context,
          Text('Content with Context'),
        ),
      ],
    );
  }
}
```

### Custom Error Handling

```dart
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

class CustomError extends FlyingdartsError {
  CustomError(String message) : super(message);
  
  @override
  String toString() => 'CustomError: $errorMessage';
}

class ErrorHandlingExample extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: () {
        try {
          // Simulate an error
          throw CustomError('Something went wrong');
        } catch (e) {
          if (e is FlyingdartsError) {
            showDialog(
              context: context,
              builder: (context) => LanguageErrorDialog(
                error: e.errorMessage,
              ),
            );
          }
        }
      },
      child: Text('Trigger Error'),
    );
  }
}
```

## API Reference

### Core Components

#### FlyingdartsScaffold
A custom scaffold widget with Flying Darts theming and navigation support.

```dart
class FlyingdartsScaffold extends StatelessWidget {
  final Widget child;
  final bool showAppBar;
  final bool showActionBar;
  final List<Widget> actionButtons;
  final Widget? leading;

  const FlyingdartsScaffold({
    super.key,
    required this.child,
    this.showAppBar = true,
    this.showActionBar = false,
    this.actionButtons = const [],
    this.leading,
  });
}
```

#### LottieWidget
A wrapper widget for Lottie animations with error handling and multiple loading options.

```dart
class LottieWidget extends StatelessWidget {
  final String? jsonString;
  final String? assetPath;
  final double? width;
  final double? height;
  final bool repeat;
  final bool animate;
  final BoxFit? fit;
  final AlignmentGeometry? alignment;
  final String? package;

  const LottieWidget({
    super.key,
    this.jsonString,
    this.assetPath,
    this.width,
    this.height,
    this.repeat = true,
    this.animate = true,
    this.fit,
    this.alignment,
    this.package,
  });

  const LottieWidget.fromJson({...});
  const LottieWidget.fromAsset({...});
}
```

#### ErrorDialog<T>
An abstract base class for error dialog implementations.

```dart
abstract class ErrorDialog<T> extends StatelessWidget {
  const ErrorDialog({super.key, required this.error});
  final T error;
}
```

#### LoginWithFacebookButton
A custom elevated button for Facebook authentication.

```dart
class LoginWithFacebookButton extends ElevatedButton {
  final VoidCallback onPressed;
  
  const LoginWithFacebookButton({
    super.key,
    required this.onPressed,
  });
}
```

#### BackButton
A custom back button with navigation support using Go Router.

```dart
class BackButton extends StatelessWidget {
  const BackButton({super.key});
}
```

### Theme System

#### MyTheme
A mixin providing Flying Darts color palette and theme constants.

```dart
mixin MyTheme {
  static const MaterialColor primaryColor;
  static const MaterialColor secondaryColor;
}
```

#### myTheme
A complete Material Design theme with Flying Darts branding.

```dart
var myTheme = ThemeData(
  useMaterial3: false,
  primaryColor: MyTheme.secondaryColor,
  scaffoldBackgroundColor: MyTheme.primaryColor,
  // ... complete theme configuration
);
```

### Key Management

#### Keys
An enum providing centralized key management for testing and automation.

```dart
enum Keys {
  appBar,
  appBarBtnSettings,
  appBarBtnLanguage,
  speechRecognitionBtn,
  errorDialogLanguage,
  errorDialogLanguageTitle,
  errorDialogLanguageError,
  loginWithFacebookBtn,
}
```

### Utility Functions

#### Material Wrappers
Utility functions for creating Material Design components with consistent theming.

```dart
Widget createDefaultMaterialWidget(BuildContext context, Widget child);
Widget createDefaultWidgetInACard(BuildContext? context, Widget child);
Widget createDefaultCardWidget(Widget child);
```

## Configuration

### Theme Configuration
The package provides a complete theme system with Flying Darts branding:

```dart
import 'package:ui/ui.dart';

MaterialApp(
  theme: myTheme, // Use the shared theme
  // ... other app configuration
)
```

### Color Palette
The package defines a comprehensive color palette:

- **Primary Color**: Blue (#3C46A7) with 10 shades
- **Secondary Color**: Pink (#F9617D) with 10 shades
- **Typography**: Poppins font family
- **Brightness**: Dark theme

### Widgetbook Configuration
Configure Widgetbook for component documentation:

```dart
import 'package:widgetbook/widgetbook.dart';
import 'package:ui/ui.dart';

@WidgetbookApp.material(
  name: 'Flying Darts UI',
  builder: MaterialApp.new,
)
class FlyingDartsWidgetbook extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Widgetbook(
      stories: [
        // Define your stories here
      ],
    );
  }
}
```

### Error Handling Configuration
Implement custom error handling:

```dart
class CustomError extends FlyingdartsError {
  CustomError(String message) : super(message);
  
  @override
  String toString() => 'CustomError: $errorMessage';
}
```

## Development

### Project Structure
```
ui/
├── lib/
│   ├── ui.dart                           # Main library export
│   ├── flyingdarts_error.dart           # Error base class
│   ├── widgets/                          # UI components
│   │   ├── lottie_widget.dart           # Lottie animation wrapper
│   │   ├── scaffold.dart                # Custom scaffold
│   │   ├── back_button.dart             # Navigation button
│   │   ├── error_dialog/                # Error dialog components
│   │   └── login_with_facebook_button/  # Authentication components
│   ├── themes/                          # Theme definitions
│   │   └── theme.dart                   # Material theme
│   ├── models/                          # Data models
│   │   └── keys.dart                    # Key management
│   └── wrappers/                        # Utility wrappers
│       └── material.wrappers.dart       # Material wrappers
├── test/                                # Unit tests
├── pubspec.yaml                         # Package dependencies
└── package.json                         # Build scripts
```

### Architecture Patterns
- **Component Pattern**: Reusable UI components
- **Theme Pattern**: Centralized theming system
- **Error Handling Pattern**: Standardized error management
- **Key Management Pattern**: Centralized testing keys
- **Wrapper Pattern**: Utility wrappers for common patterns
- **Mixin Pattern**: Theme mixin for color constants

### Code Generation
The package uses build_runner for code generation:

```bash
# Generate code
dart run build_runner build --delete-conflicting-outputs

# Watch for changes
dart run build_runner watch

# Clean generated files
dart run build_runner clean
```

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
- Test component rendering thoroughly
- Verify theme consistency
- Test accessibility features

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **lottie**: ^3.1.2 - Animation support
- **widgetbook**: ^3.14.3 - Component documentation
- **widgetbook_annotation**: ^3.5.0 - Widgetbook annotations

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **build_runner**: ^2.5.4 - Code generation
- **widgetbook_generator**: ^3.13.0 - Widgetbook code generation

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[Internationalization](../../../internationalization/)**: Multi-language support
- **[Language Feature](../../../features/language/)**: Language management
- **[Profile Feature](../../../features/profile/)**: User profile management
- **[Splash Feature](../../../features/splash/)**: Splash screen
- **[Authress Login](../../../authress/login/)**: Authentication package
- **[App Config Core](../../../config/app_config_core/)**: Configuration core
- **[App Config Prefs](../../../config/app_config_prefs/)**: SharedPreferences configuration
- **[App Config Secrets](../../../config/app_config_secrets/)**: Secure configuration storage

### Backend Services
- **[Friends API](../../../../../backend/dotnet/friends/)**: Friend management
- **[Games API](../../../../../backend/dotnet/games/)**: Game management
- **[Auth API](../../../../../backend/dotnet/auth/)**: Authentication

## Version History

- **v0.0.7** (2025-07-26): Implemented friends feature
- **v0.0.6** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.5** (2025-07-10): Add flutter workspace at root
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution
- **v0.0.2** (2025-07-07): Initial change log

## Troubleshooting

### Common Issues

1. **Theme not applying correctly**
   - Ensure `myTheme` is properly imported and applied to MaterialApp
   - Check that all theme properties are correctly configured
   - Verify color constants are accessible

2. **Lottie animations not loading**
   - Check asset paths are correct and assets are included in pubspec.yaml
   - Verify JSON string format is valid
   - Ensure Lottie package is properly installed

3. **Error dialogs not displaying**
   - Check that error dialog is properly implemented
   - Verify context is valid when showing dialog
   - Ensure error message is not null

4. **Navigation not working**
   - Verify Go Router is properly configured
   - Check that BackButton is used in correct context
   - Ensure navigation stack is properly managed

5. **Widgetbook not generating**
   - Run `dart run build_runner build --delete-conflicting-outputs`
   - Check Widgetbook annotations are correct
   - Verify Widgetbook dependencies are installed

6. **Key management issues**
   - Ensure Keys enum is properly imported
   - Check that key values are unique
   - Verify key usage in widget tree

7. **Component rendering issues**
   - Check widget tree structure
   - Verify all required parameters are provided
   - Test component in isolation

8. **Accessibility problems**
   - Ensure all interactive elements have proper keys
   - Check semantic labels are provided
   - Test with screen readers

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Test component rendering thoroughly
6. Verify theme consistency across components
7. Test accessibility features
8. Ensure proper error handling
9. Test with multiple screen sizes
10. Verify Widgetbook documentation
11. Test animation performance
12. Ensure cross-platform compatibility

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
