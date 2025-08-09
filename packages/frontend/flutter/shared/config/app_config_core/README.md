# Flying Darts App Config Core

## Overview

The Flying Darts App Config Core is a Flutter package that provides a comprehensive configuration management system for the Flying Darts gaming platform. This package enables Flutter applications to handle readable and writeable configurations with type safety, caching, and persistence capabilities.

The package is built using modern Flutter patterns with abstract classes, generics, and JSON serialization. It provides a robust foundation for implementing configuration management in Flutter apps with support for various data types, automatic caching, and flexible storage backends.

## Features

- **Type-Safe Configuration**: Generic configuration classes with compile-time type safety
- **Read/Write Operations**: Support for both reading and writing configuration data
- **Automatic Caching**: Built-in caching mechanism for improved performance
- **JSON Serialization**: Automatic JSON serialization/deserialization with code generation
- **Abstract Architecture**: Extensible abstract classes for custom implementations
- **Fallback Support**: Fallback value mechanism for configuration providers
- **Initialization Management**: Proper initialization state management
- **Error Handling**: Comprehensive error handling for configuration operations
- **Cross-Platform Support**: Works on iOS, Android, and Web
- **Code Generation**: Automatic code generation for JSON serialization

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ^3.26.0 or higher
- **Build Tools**: build_runner for code generation

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the app config core package**:
   ```bash
   cd packages/frontend/flutter/shared/config/app_config_core
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
  app_config_core:
    path: ../../packages/frontend/flutter/shared/config/app_config_core
```

## Usage

### Basic Configuration Reader

```dart
import 'package:app_config_core/app_config_core.dart';

class MyConfigReader extends ConfigReader<MyConfig> {
  @override
  Future<MyConfig> fetchData() async {
    // Implement your data fetching logic
    // This could be from SharedPreferences, API, file system, etc.
    return MyConfig(apiUrl: 'https://api.example.com');
  }
}

class MyConfig {
  final String apiUrl;
  
  MyConfig({required this.apiUrl});
}

// Usage
void main() async {
  final configReader = MyConfigReader();
  await configReader.initialize();
  
  final config = configReader.data;
  print('API URL: ${config.apiUrl}');
}
```

### Configuration Writer

```dart
import 'package:app_config_core/app_config_core.dart';

class MyConfigWriter extends ConfigWriter<MyConfig> {
  @override
  Future<MyConfig> fetchData() async {
    // Implement your data fetching logic
    return MyConfig(apiUrl: 'https://api.example.com');
  }

  @override
  Future<void> storeData(MyConfig data) async {
    // Implement your data storage logic
    // This could be to SharedPreferences, API, file system, etc.
    print('Storing config: ${data.apiUrl}');
  }
}

// Usage
void main() async {
  final configWriter = MyConfigWriter();
  await configWriter.initialize();
  
  // Update configuration
  final newConfig = MyConfig(apiUrl: 'https://new-api.example.com');
  await configWriter.persist(newConfig);
  
  // Access updated data
  final currentConfig = configWriter.data;
  print('Current API URL: ${currentConfig.apiUrl}');
}
```

### Locale Settings

```dart
import 'package:app_config_core/app_config_core.dart';

class LocaleConfigWriter extends ConfigWriter<LocaleSettings> {
  @override
  Future<LocaleSettings> fetchData() async {
    // Load locale settings from storage
    return LocaleSettings(localeIdentifier: 'en-US');
  }

  @override
  Future<void> storeData(LocaleSettings data) async {
    // Save locale settings to storage
    print('Saving locale: ${data.localeIdentifier}');
  }
}

// Usage
void main() async {
  final localeWriter = LocaleConfigWriter();
  await localeWriter.initialize();
  
  // Change locale
  final newLocale = LocaleSettings(localeIdentifier: 'es-ES');
  await localeWriter.persist(newLocale);
  
  // Access current locale
  final currentLocale = localeWriter.data;
  print('Current locale: ${currentLocale.localeIdentifier}');
}
```

### Network Endpoint Configuration

```dart
import 'package:app_config_core/app_config_core.dart';

class NetworkConfigReader extends ConfigReader<NetworkEndpoint> {
  @override
  Future<NetworkEndpoint> fetchData() async {
    // Load network endpoint from configuration
    return NetworkEndpoint(connectionUrl: 'wss://api.flyingdarts.com');
  }
}

// Usage
void main() async {
  final networkReader = NetworkConfigReader();
  await networkReader.initialize();
  
  final endpoint = networkReader.data;
  print('Connection URL: ${endpoint.connectionUrl}');
}
```

### Fallback Provider

```dart
import 'package:app_config_core/app_config_core.dart';

class MyConfigProvider with FallbackProvider<String> {
  String? getValue() {
    // Try to get the actual value
    final actualValue = getActualValue();
    
    // If no actual value, return fallback
    return actualValue ?? fallbackValue;
  }
  
  String? getActualValue() {
    // Implement your actual value retrieval logic
    return null;
  }
}

// Usage
void main() {
  final provider = MyConfigProvider();
  provider.setFallback('default-value');
  
  final value = provider.getValue();
  print('Value: $value'); // Prints: default-value
}
```

## API Reference

### Core Components

#### ConfigReader<TData>
Abstract base class for read-only configuration access.

```dart
abstract class ConfigReader<TData> {
  TData? cachedData;
  bool isInitialized = false;

  Future<void> initialize();
  TData get data;
  
  @protected
  Future<TData> fetchData();
}
```

#### ConfigWriter<TData>
Abstract base class for read/write configuration access.

```dart
abstract class ConfigWriter<TData> extends ConfigReader<TData> {
  Future<TData> persist(TData newData);
  
  @protected
  Future<void> storeData(TData data);
}
```

#### FallbackProvider<TValue>
Mixin for providing fallback values.

```dart
mixin FallbackProvider<TValue> {
  void setFallback(TValue value);
  TValue? get fallbackValue;
}
```

### Data Models

#### LocaleSettings
Configuration model for locale settings.

```dart
@JsonSerializable()
class LocaleSettings {
  final String localeIdentifier;

  const LocaleSettings({required this.localeIdentifier});

  factory LocaleSettings.fromJson(Map<String, dynamic> json);
  Map<String, dynamic> toJson();
}
```

#### NetworkEndpoint
Configuration model for network endpoints.

```dart
class NetworkEndpoint {
  final String connectionUrl;

  const NetworkEndpoint({required this.connectionUrl});
}
```

### Methods

#### ConfigReader Methods
- **`initialize()`**: Initializes the configuration reader and loads data
- **`data`**: Getter for accessing the cached configuration data
- **`fetchData()`**: Abstract method for implementing data fetching logic

#### ConfigWriter Methods
- **`persist(TData newData)`**: Persists new configuration data and updates cache
- **`storeData(TData data)`**: Abstract method for implementing data storage logic

#### FallbackProvider Methods
- **`setFallback(TValue value)`**: Sets a fallback value
- **`fallbackValue`**: Getter for accessing the fallback value

## Configuration

### JSON Serialization Setup
The package uses JSON serialization for data models:

```dart
import 'package:json_annotation/json_annotation.dart';

@JsonSerializable()
class MyConfig {
  final String apiUrl;
  final int timeout;
  
  MyConfig({required this.apiUrl, required this.timeout});
  
  factory MyConfig.fromJson(Map<String, dynamic> json) => _$MyConfigFromJson(json);
  Map<String, dynamic> toJson() => _$MyConfigToJson(this);
}
```

### Code Generation
Generate JSON serialization code:

```bash
# Generate code
dart run build_runner build --delete-conflicting-outputs

# Clean generated files
dart run build_runner clean

# Watch for changes
dart run build_runner watch
```

### Custom Implementation
Create custom configuration readers and writers:

```dart
class SharedPreferencesConfigWriter<T> extends ConfigWriter<T> {
  final SharedPreferences _prefs;
  final String _key;
  final T Function(Map<String, dynamic>) _fromJson;
  final Map<String, dynamic> Function(T) _toJson;

  SharedPreferencesConfigWriter(
    this._prefs,
    this._key,
    this._fromJson,
    this._toJson,
  );

  @override
  Future<T> fetchData() async {
    final jsonString = _prefs.getString(_key);
    if (jsonString == null) {
      throw Exception('No data found for key: $_key');
    }
    final json = jsonDecode(jsonString);
    return _fromJson(json);
  }

  @override
  Future<void> storeData(T data) async {
    final json = _toJson(data);
    final jsonString = jsonEncode(json);
    await _prefs.setString(_key, jsonString);
  }
}
```

## Development

### Project Structure
```
app_config_core/
├── lib/
│   ├── configuration.dart              # Main library export
│   └── src/                           # Source files
│       ├── config_reader.dart          # Abstract reader class
│       ├── config_writer.dart          # Abstract writer class
│       ├── fallback_provider.dart      # Fallback provider mixin
│       ├── locale_settings.dart        # Locale settings model
│       ├── locale_settings.g.dart      # Generated JSON code
│       └── network_endpoint.dart       # Network endpoint model
├── test/                              # Unit tests
├── pubspec.yaml                       # Package dependencies
└── package.json                       # Build scripts
```

### Architecture Patterns
- **Abstract Class Pattern**: Base classes for configuration operations
- **Generic Pattern**: Type-safe configuration with generics
- **Mixin Pattern**: Fallback provider functionality
- **Factory Pattern**: JSON serialization factories
- **Caching Pattern**: Built-in caching for performance

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
- Use abstract classes for extensibility

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **json_annotation**: ^4.9.0 - JSON serialization annotations

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **json_serializable**: ^6.9.5 - JSON code generation
- **build_runner**: ^2.5.4 - Code generation

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[App Config Prefs](../../../app_config_prefs/)**: Preference storage implementation
- **[UI Package](../../../ui/)**: Shared UI components
- **[Authress Login](../../../authress/login/)**: Authentication package
- **[Profile Feature](../../../features/profile/)**: User profile management
- **[Language Feature](../../../features/language/)**: Multi-language support

### Backend Services
- **[Friends API](../../../../../backend/dotnet/friends/)**: Friend management
- **[Games API](../../../../../backend/dotnet/games/)**: Game management
- **[Auth API](../../../../../backend/dotnet/auth/)**: Authentication

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

1. **Configuration not initializing**
   - Ensure `initialize()` is called before accessing data
   - Check that `fetchData()` implementation is working correctly
   - Verify error handling in data fetching logic

2. **JSON serialization errors**
   - Run `dart run build_runner build` to generate JSON code
   - Check that models are properly annotated with `@JsonSerializable()`
   - Verify JSON structure matches model definitions

3. **Type safety issues**
   - Ensure proper generic type parameters are used
   - Check that custom implementations extend the correct base classes
   - Verify type compatibility in data operations

4. **Caching problems**
   - Check that `isInitialized` flag is properly managed
   - Verify `cachedData` is being set correctly
   - Ensure proper state management in custom implementations

5. **Persistence failures**
   - Check that `storeData()` implementation is working correctly
   - Verify error handling in storage logic
   - Ensure proper async/await usage in storage operations

6. **Fallback provider issues**
   - Check that fallback value is set before accessing
   - Verify fallback logic in custom implementations
   - Ensure proper null handling

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Maintain abstract class patterns for extensibility
6. Test JSON serialization thoroughly
7. Verify type safety in all implementations
8. Test caching and persistence mechanisms

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
