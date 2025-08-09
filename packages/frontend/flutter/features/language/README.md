# Flying Darts Language Feature

## Overview

The Flying Darts Language Feature is a Flutter package that provides comprehensive language and localization management for the Flying Darts gaming platform. This package enables Flutter applications to handle multiple languages, speech-to-text functionality, and user language preferences through a clean, state-managed architecture.

The package is built using modern Flutter patterns with BLoC state management, dependency injection, and speech recognition capabilities. It provides a robust foundation for implementing multi-language support in Flutter apps with automatic language detection and user preference persistence.

## Features

- **Multi-Language Support**: Comprehensive language management with automatic detection
- **Speech-to-Text Integration**: Built-in speech recognition using device capabilities
- **BLoC State Management**: Clean architecture with Cubit for state management
- **Dependency Injection**: Injectable integration for service management
- **Language Preferences**: Persistent user language preferences
- **System Language Detection**: Automatic detection of device language settings
- **Language Selection Dialog**: User-friendly language selection interface
- **Widgetbook Integration**: Component documentation and testing
- **Code Generation**: Automatic code generation for dependency injection
- **Cross-Platform Support**: Works on iOS, Android, and Web

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ^3.26.0 or higher
- **Speech Recognition**: Device with speech recognition capabilities
- **Dependencies**: Access to shared packages (ui, app_config_core, app_config_prefs)

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the language package**:
   ```bash
   cd packages/frontend/flutter/features/language
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
  language:
    path: ../../packages/frontend/flutter/features/language
```

## Usage

### Basic Setup

1. **Initialize the language feature**:

```dart
import 'package:language/language.dart';
import 'package:get_it/get_it.dart';

void main() {
  // Initialize dependency injection
  configureDependencies();
  
  // Initialize language cubit
  final languageCubit = GetIt.I<LanguageCubit>();
  await languageCubit.init();
  
  runApp(MyApp());
}
```

2. **Use the language cubit in your app**:

```dart
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:language/language.dart';

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => GetIt.I<LanguageCubit>(),
      child: MaterialApp(
        title: 'Multi-Language App',
        home: HomePage(),
      ),
    );
  }
}
```

### Language Selection

```dart
class HomePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocBuilder<LanguageCubit, LanguageState>(
      builder: (context, state) {
        return Scaffold(
          appBar: AppBar(
            title: Text('Current Language: ${state.preferedLocale.name}'),
            actions: [
              IconButton(
                icon: Icon(Icons.language),
                onPressed: () => _showLanguageDialog(context),
              ),
            ],
          ),
          body: Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text('Available Languages: ${state.availableLanguages.length}'),
                SizedBox(height: 20),
                ElevatedButton(
                  onPressed: () => _showLanguageDialog(context),
                  child: Text('Change Language'),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  void _showLanguageDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => LanguageDialog(),
    );
  }
}
```

### Programmatic Language Changes

```dart
class LanguageService {
  final LanguageCubit _languageCubit;

  LanguageService(this._languageCubit);

  Future<void> changeLanguage(LocaleName newLocale) async {
    _languageCubit.setPreferedLocale(newLocale);
  }

  Future<void> refreshAvailableLanguages() async {
    await _languageCubit.init();
  }

  LocaleName get currentLanguage => _languageCubit.state.preferedLocale;
  List<LocaleName> get availableLanguages => _languageCubit.state.availableLanguages;
}
```

## API Reference

### Core Components

#### LanguageCubit
The main state management class for language functionality.

```dart
@lazySingleton
class LanguageCubit extends Cubit<LanguageState> {
  LanguageCubit() : super(LanguageState(LocaleName("en-US", "English (United States)"), []));

  Future<void> init();
  void setPreferedLocale(LocaleName preferedLocale);
  void setAvailableLanguages(List<LocaleName> languages);
}
```

#### LanguageState
Represents the current language state.

```dart
class LanguageState {
  final LocaleName preferedLocale;
  final List<LocaleName> availableLanguages;

  LanguageState(this.preferedLocale, this.availableLanguages);
  LanguageState copyWith({
    LocaleName? preferedLocale,
    List<LocaleName>? availableLanguages,
  });
}
```

#### LanguageDialog
A dialog widget for language selection.

```dart
class LanguageDialog extends StatelessWidget {
  const LanguageDialog({Key? key});

  @override
  Widget build(BuildContext context);
}
```

### Data Models

#### LocaleName
Represents a language locale with identifier and display name.

```dart
class LocaleName {
  final String localeId;    // e.g., "en-US"
  final String name;        // e.g., "English (United States)"
}
```

#### LocaleSettings
Configuration for locale settings.

```dart
class LocaleSettings {
  final String localeIdentifier;
}
```

### Methods

#### LanguageCubit Methods

- **`init()`**: Initializes speech recognition and loads available languages
- **`setPreferedLocale(LocaleName locale)`**: Sets the preferred language and persists it
- **`setAvailableLanguages(List<LocaleName> languages)`**: Updates the list of available languages

#### LanguageState Methods

- **`copyWith(...)`**: Creates a copy of the state with updated values

## Configuration

### Dependency Injection Setup
The package uses Injectable for dependency injection:

```dart
import 'package:injectable/injectable.dart';
import 'package:language/language.dart';

@InjectableInit.microPackage()
void initMicroPackage() {}

void configureDependencies() {
  // The LanguageCubit is automatically registered as a lazy singleton
  // through the micro package initialization
}
```

### Language Configuration
Configure language settings through the app configuration:

```dart
class LanguageConfig {
  static const String defaultLanguage = 'en-US';
  static const String defaultLanguageName = 'English (United States)';
  
  static LocaleName getDefaultLocale() {
    return LocaleName(defaultLanguage, defaultLanguageName);
  }
}
```

### Speech Recognition Setup
The package automatically initializes speech recognition:

```dart
// Speech recognition is automatically initialized in LanguageCubit.init()
// No additional setup required for basic functionality
```

## Development

### Project Structure
```
language/
├── lib/
│   ├── language.dart                    # Main library export
│   ├── language.module.dart             # Generated dependency injection
│   └── src/
│       ├── state/                       # State management
│       │   ├── language_cubit.dart      # Main cubit implementation
│       │   └── language_state.dart      # State model
│       ├── dialog/                      # UI components
│       │   ├── language_dialog.dart     # Language selection dialog
│       │   └── language_dialog.usecase.dart # Widgetbook use case
│       └── widgetbook.generator.dart    # Generated widgetbook code
├── test/                                # Unit tests
├── pubspec.yaml                         # Package dependencies
└── package.json                         # Build scripts
```

### Architecture Patterns
- **BLoC Pattern**: Cubit for state management
- **Dependency Injection**: Injectable for service management
- **Repository Pattern**: Configuration persistence
- **Factory Pattern**: State object creation
- **Observer Pattern**: State change notifications

### Code Generation
The package uses code generation for dependency injection and widgetbook:

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
  name: 'A language dialog',
  type: LanguageDialog,
)
Widget defaultLanguageDialog(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    const LanguageDialog(),
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
- **flutter_bloc**: ^9.1.1 - State management
- **get_it**: ^8.0.3 - Dependency injection
- **speech_to_text**: ^7.2.0 - Speech recognition
- **ui**: Shared UI components
- **app_config_core**: Configuration management
- **app_config_prefs**: Preference storage
- **widgetbook**: ^3.14.3 - Component documentation
- **widgetbook_annotation**: ^3.5.0 - Widgetbook annotations
- **injectable**: ^2.5.0 - Dependency injection

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **build_runner**: ^2.5.4 - Code generation
- **widgetbook_generator**: ^3.13.0 - Widgetbook code generation
- **injectable_generator**: ^2.7.0 - Injectable code generation

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[UI Package](../../../ui/)**: Shared UI components
- **[App Config Core](../../../app_config_core/)**: Configuration management
- **[App Config Prefs](../../../app_config_prefs/)**: Preference storage
- **[Authress Login](../../../authress/login/)**: Authentication package
- **[API SDK](../../../api/sdk/)**: Flutter API client

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

1. **Speech recognition not working**
   - Ensure device has speech recognition capabilities
   - Check microphone permissions
   - Verify speech_to_text package is properly configured

2. **Language preferences not persisting**
   - Check app_config_prefs configuration
   - Verify dependency injection is properly set up
   - Ensure configuration writer is working correctly

3. **Available languages not loading**
   - Check speech recognition initialization
   - Verify device locale detection
   - Ensure proper error handling in init() method

4. **Language dialog not showing**
   - Verify LanguageCubit is properly provided
   - Check that dialog is called from correct context
   - Ensure UI package is properly imported

5. **Code generation issues**
   - Run `dart run build_runner clean` then `dart run build_runner build`
   - Check for syntax errors in source files
   - Verify all dependencies are properly installed

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Maintain BLoC pattern for state management
6. Test speech recognition on multiple devices
7. Update widgetbook use cases for UI changes

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
