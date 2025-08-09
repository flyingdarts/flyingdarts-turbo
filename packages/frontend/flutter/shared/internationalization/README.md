# Flyingdarts Internationalization

## Overview

The Flyingdarts Internationalization is a Flutter package that provides comprehensive internationalization (i18n) and localization (l10n) support for the Flyingdarts gaming platform. This package enables Flutter applications to display text, dates, numbers, and other locale-specific content in multiple languages with automatic code generation and type-safe access to localized strings.

The package is built using Flutter's official internationalization framework with ARB (Application Resource Bundle) files for translation management. It provides a robust foundation for implementing multi-language support in Flutter apps with automatic code generation, type safety, and comprehensive locale support. The package currently supports English (en) and Dutch (nl) languages with extensible architecture for adding more languages.

## Features

- **Multi-Language Support**: Support for English (en) and Dutch (nl) with extensible architecture
- **ARB File Management**: Application Resource Bundle files for translation management
- **Automatic Code Generation**: Type-safe localization classes generated from ARB files
- **Type-Safe Access**: Compile-time type safety for localized string access
- **Flutter Integration**: Native Flutter localization delegates and widgets
- **Cross-Platform Support**: Works on iOS, Android, and Web
- **Extensible Architecture**: Easy to add new languages and translations
- **Speech Recognition Support**: Localized strings for speech recognition features
- **Template-Based Generation**: Template ARB file for consistent translation structure
- **Description Metadata**: Rich metadata for translation context and documentation
- **Locale Detection**: Automatic locale detection and fallback support
- **Performance Optimized**: Efficient loading and caching of localized content

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ">=3.26.0"
- **Flutter Localizations**: Access to flutter_localizations package
- **Intl Package**: ^0.20.2 for internationalization utilities
- **Build Tools**: Flutter's built-in localization generation tools

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the internationalization package**:
   ```bash
   cd packages/frontend/flutter/shared/internationalization
   ```

3. **Get dependencies**:
   ```bash
   flutter pub get
   ```

4. **Generate localization files** (if needed):
   ```bash
   flutter gen-l10n
   ```

### In Your Flutter Project
Add the package to your `pubspec.yaml`:

```yaml
dependencies:
  internationalization:
    path: ../../packages/frontend/flutter/shared/internationalization
  flutter_localizations:
    sdk: flutter
  intl: ^0.20.2
```

## Usage

### Basic Setup in Flutter App

```dart
import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:internationalization/internationalization.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flyingdarts',
      
      // Localization configuration
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      
      // Default locale
      locale: const Locale('en'),
      
      home: MyHomePage(),
    );
  }
}

class MyHomePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context).helloWorld),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(AppLocalizations.of(context).helloWorld),
            Text(AppLocalizations.of(context).underConstruction),
          ],
        ),
      ),
    );
  }
}
```

### Using Localized Strings

```dart
import 'package:flutter/material.dart';
import 'package:internationalization/internationalization.dart';

class LocalizedWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    
    return Column(
      children: [
        // Basic localized strings
        Text(l10n.helloWorld),
        Text(l10n.underConstruction),
        
        // Speech recognition strings
        Text(l10n.speechWidgetListening),
        Text(l10n.speechWidgetHoldMicrophone),
      ],
    );
  }
}
```

### Dynamic Locale Switching

```dart
import 'package:flutter/material.dart';
import 'package:internationalization/internationalization.dart';

class LocaleSwitcher extends StatefulWidget {
  @override
  _LocaleSwitcherState createState() => _LocaleSwitcherState();
}

class _LocaleSwitcherState extends State<LocaleSwitcher> {
  Locale _currentLocale = const Locale('en');

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      locale: _currentLocale,
      
      home: Scaffold(
        appBar: AppBar(
          title: Text(AppLocalizations.of(context).helloWorld),
          actions: [
            PopupMenuButton<Locale>(
              onSelected: (Locale locale) {
                setState(() {
                  _currentLocale = locale;
                });
              },
              itemBuilder: (BuildContext context) => [
                PopupMenuItem(
                  value: const Locale('en'),
                  child: Text('English'),
                ),
                PopupMenuItem(
                  value: const Locale('nl'),
                  child: Text('Nederlands'),
                ),
              ],
            ),
          ],
        ),
        body: Center(
          child: Text(AppLocalizations.of(context).helloWorld),
        ),
      ),
    );
  }
}
```

### Speech Recognition Integration

```dart
import 'package:flutter/material.dart';
import 'package:internationalization/internationalization.dart';

class SpeechRecognitionWidget extends StatefulWidget {
  @override
  _SpeechRecognitionWidgetState createState() => _SpeechRecognitionWidgetState();
}

class _SpeechRecognitionWidgetState extends State<SpeechRecognitionWidget> {
  bool _isListening = false;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    
    return GestureDetector(
      onTapDown: (_) => setState(() => _isListening = true),
      onTapUp: (_) => setState(() => _isListening = false),
      onTapCancel: () => setState(() => _isListening = false),
      
      child: Container(
        padding: EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: _isListening ? Colors.red : Colors.blue,
          borderRadius: BorderRadius.circular(8),
        ),
        child: Text(
          _isListening 
            ? l10n.speechWidgetListening 
            : l10n.speechWidgetHoldMicrophone,
          style: TextStyle(color: Colors.white),
        ),
      ),
    );
  }
}
```

### Adding New Translations

1. **Create a new ARB file** for the language (e.g., `app_fr.arb` for French):

```json
{
  "helloWorld": "Bonjour le monde!",
  "underConstruction": "En construction",
  "speechWidgetListening": "Écoute",
  "speechWidgetHoldMicrophone": "Maintenez le microphone"
}
```

2. **Update the template file** (`app_en.arb`) to include new keys:

```json
{
  "helloWorld": "Hello World!",
  "@helloWorld": {
    "description": "The conventional newborn programmer greeting"
  },
  "underConstruction": "Under construction",
  "@underConstruction": {
    "description": "A typical under construction message"
  },
  "speechWidgetListening": "Listening",
  "@speechWidgetListening": {
    "description": "A message to show the speech recognition is active"
  },
  "speechWidgetHoldMicrophone": "Hold the microphone",
  "@speechWidgetHoldMicrophone": {
    "description": "A message to call for action to enable speech recognition"
  },
  "newKey": "New translation",
  "@newKey": {
    "description": "Description for the new translation key"
  }
}
```

3. **Regenerate localization files**:

```bash
flutter gen-l10n
```

## API Reference

### Core Components

#### AppLocalizations
The main localization class that provides access to all localized strings.

```dart
abstract class AppLocalizations {
  AppLocalizations(String locale);
  
  final String localeName;
  
  static AppLocalizations of(BuildContext context);
  static const LocalizationsDelegate<AppLocalizations> delegate;
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates;
  static const List<Locale> supportedLocales;
  
  // Localized string getters
  String get helloWorld;
  String get underConstruction;
  String get speechWidgetListening;
  String get speechWidgetHoldMicrophone;
}
```

#### AppLocalizationsDelegate
The localization delegate for loading and managing localized content.

```dart
class _AppLocalizationsDelegate extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();
  
  @override
  Future<AppLocalizations> load(Locale locale);
  @override
  bool isSupported(Locale locale);
  @override
  bool shouldReload(_AppLocalizationsDelegate old);
}
```

### Methods

#### AppLocalizations Static Methods
- **`of(BuildContext context)`**: Get the current AppLocalizations instance
- **`delegate`**: Get the localization delegate
- **`localizationsDelegates`**: Get the list of all localization delegates
- **`supportedLocales`**: Get the list of supported locales

#### AppLocalizations Instance Methods
- **`helloWorld`**: Get the "Hello World!" localized string
- **`underConstruction`**: Get the "Under construction" localized string
- **`speechWidgetListening`**: Get the "Listening" localized string
- **`speechWidgetHoldMicrophone`**: Get the "Hold the microphone" localized string

### Behavior

#### Locale Resolution
The package supports the following locales:
- **English (en)**: Default language
- **Dutch (nl)**: Dutch/Flemish language

#### Fallback Mechanism
If a translation is not available for a specific locale, the system falls back to the default language (English).

#### Code Generation
Localization classes are automatically generated from ARB files using Flutter's `gen-l10n` tool.

## Configuration

### Flutter App Configuration
Configure your Flutter app to use the internationalization package:

```dart
import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:internationalization/internationalization.dart';

MaterialApp(
  localizationsDelegates: AppLocalizations.localizationsDelegates,
  supportedLocales: AppLocalizations.supportedLocales,
  locale: const Locale('en'), // Default locale
  // ... other app configuration
)
```

### ARB File Configuration
The package uses the following ARB file structure:

```json
{
  "key": "translation",
  "@key": {
    "description": "Description for translators",
    "placeholders": {
      "placeholderName": {
        "type": "String",
        "example": "example value"
      }
    }
  }
}
```

### i10n.yaml Configuration
The package uses the following configuration in `i10n.yaml`:

```yaml
arb-dir: lib/i10n
template-arb-file: app_en.arb
output-localization-file: app_localizations.dart
```

### Platform-Specific Configuration

#### iOS Configuration
Add supported locales to `ios/Runner/Info.plist`:

```xml
<key>CFBundleLocalizations</key>
<array>
  <string>en</string>
  <string>nl</string>
</array>
```

#### Android Configuration
Add supported locales to `android/app/src/main/res/values/strings.xml`:

```xml
<resources>
  <string name="app_name">Flyingdarts</string>
</resources>
```

### Error Handling
Implement proper error handling for localization:

```dart
try {
  final l10n = AppLocalizations.of(context);
  return Text(l10n.helloWorld);
} catch (e) {
  // Fallback to default text
  return Text('Hello World!');
}
```

## Development

### Project Structure
```
internationalization/
├── lib/
│   ├── internationalization.dart     # Main library export
│   ├── i10n/                         # ARB translation files
│   │   ├── app_en.arb               # English template
│   │   └── app_nl.arb               # Dutch translations
│   └── gen/                         # Generated localization files
│       ├── app_localizations.dart   # Main localization class
│       ├── app_localizations_en.dart # English implementation
│       └── app_localizations_nl.dart # Dutch implementation
├── i10n.yaml                        # Localization configuration
├── test/                            # Unit tests
├── pubspec.yaml                     # Package dependencies
└── package.json                     # Build scripts
```

### Architecture Patterns
- **Template Pattern**: Template ARB file for consistent structure
- **Delegate Pattern**: Localization delegates for loading content
- **Factory Pattern**: Factory methods for creating localization instances
- **Code Generation**: Automatic code generation from ARB files
- **Fallback Pattern**: Graceful fallback to default language

### Code Generation
The package uses Flutter's built-in localization generation:

```bash
# Generate localization files
flutter gen-l10n

# Watch for changes (if supported)
flutter gen-l10n --watch

# Clean generated files
flutter clean
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
- Test localization loading thoroughly
- Verify fallback mechanisms
- Test locale switching functionality

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **intl**: ^0.20.2 - Internationalization utilities
- **flutter_localizations**: Flutter localization framework

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[Language Feature](../../../features/language/)**: Language management feature
- **[App Config Core](../../../config/app_config_core/)**: Configuration core package
- **[App Config Prefs](../../../config/app_config_prefs/)**: SharedPreferences configuration
- **[App Config Secrets](../../../config/app_config_secrets/)**: Secure configuration storage
- **[UI Package](../../../ui/)**: Shared UI components
- **[Authress Login](../../../authress/login/)**: Authentication package
- **[Profile Feature](../../../features/profile/)**: User profile management

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

1. **Localization not working**
   - Ensure `localizationsDelegates` and `supportedLocales` are properly configured
   - Check that the locale is set correctly in MaterialApp
   - Verify ARB files are properly formatted

2. **Code generation issues**
   - Run `flutter gen-l10n` to regenerate localization files
   - Check ARB file syntax and structure
   - Verify i10n.yaml configuration

3. **Missing translations**
   - Ensure all keys in template ARB file have corresponding translations
   - Check for typos in translation keys
   - Verify fallback mechanism is working

4. **Locale switching not working**
   - Ensure setState() is called when changing locale
   - Check that the new locale is in supportedLocales list
   - Verify MaterialApp is properly configured

5. **Platform-specific issues**
   - iOS: Check Info.plist configuration for supported locales
   - Android: Verify strings.xml configuration
   - Web: Ensure proper locale detection

6. **Performance issues**
   - Check for unnecessary rebuilds when locale changes
   - Verify localization delegates are properly cached
   - Monitor memory usage with multiple locales

7. **ARB file syntax errors**
   - Validate JSON syntax in ARB files
   - Check for missing or malformed metadata
   - Verify placeholder definitions are correct

8. **Generated code issues**
   - Clean and regenerate localization files
   - Check for conflicts in generated code
   - Verify import statements are correct

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Test localization loading thoroughly
6. Verify fallback mechanisms work correctly
7. Test locale switching functionality
8. Ensure proper error handling
9. Test with multiple languages
10. Verify ARB file syntax and structure
11. Test platform-specific behavior
12. Ensure accessibility compliance

## License

This project is part of the Flyingdarts platform and is subject to the project's licensing terms.
