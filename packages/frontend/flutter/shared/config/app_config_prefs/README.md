# Flying Darts App Config Prefs

## Overview

The Flying Darts App Config Prefs is a Flutter package that provides a SharedPreferences-based implementation of the configuration management system for the Flying Darts gaming platform. This package enables Flutter applications to persist and retrieve configuration data using Flutter's SharedPreferences storage with type safety and fallback support.

The package is built on top of the App Config Core package, extending its abstract configuration classes with a concrete SharedPreferences implementation. It provides a robust foundation for implementing persistent configuration storage in Flutter apps with automatic JSON serialization, caching, and fallback value support.

## Features

- **SharedPreferences Integration**: Native Flutter SharedPreferences storage implementation
- **Type-Safe Configuration**: Generic configuration classes with compile-time type safety
- **Automatic JSON Serialization**: Built-in JSON encoding/decoding for data persistence
- **Fallback Support**: Fallback value mechanism for configuration providers
- **Caching**: Inherited caching mechanism from App Config Core
- **Read/Write Operations**: Support for both reading and writing configuration data
- **Error Handling**: Comprehensive error handling for storage operations
- **Cross-Platform Support**: Works on iOS, Android, and Web
- **Extensible Architecture**: Built on App Config Core for extensibility
- **Automatic Key Generation**: Type-based storage key generation

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ^3.26.0 or higher
- **App Config Core**: Access to the app_config_core package
- **SharedPreferences**: ^2.5.3 for local storage

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd flyingdarts-turbo
   ```

2. **Navigate to the app config prefs package**:
   ```bash
   cd packages/frontend/flutter/shared/config/app_config_prefs
   ```

3. **Get dependencies**:
   ```bash
   flutter pub get
   ```

### In Your Flutter Project
Add the package to your `pubspec.yaml`:

```yaml
dependencies:
  app_config_prefs:
    path: ../../packages/frontend/flutter/shared/config/app_config_prefs
```

## Usage

### Basic Configuration Storage

```dart
import 'package:app_config_prefs/app_config_prefs.dart';
import 'package:shared_preferences/shared_preferences.dart';

class UserSettings {
  final String theme;
  final bool notifications;
  final int fontSize;

  UserSettings({
    required this.theme,
    required this.notifications,
    required this.fontSize,
  });

  Map<String, dynamic> toJson() => {
    'theme': theme,
    'notifications': notifications,
    'fontSize': fontSize,
  };

  factory UserSettings.fromJson(Map<String, dynamic> json) => UserSettings(
    theme: json['theme'] ?? 'light',
    notifications: json['notifications'] ?? true,
    fontSize: json['fontSize'] ?? 14,
  );
}

// Usage
void main() async {
  final prefs = await SharedPreferences.getInstance();
  
  final configManager = LocalStorageManager<UserSettings>.create(
    prefs,
    (json) => UserSettings.fromJson(json),
  );

  // Set fallback value
  configManager.setFallback(UserSettings(
    theme: 'light',
    notifications: true,
    fontSize: 14,
  ));

  await configManager.initialize();

  // Store configuration
  final settings = UserSettings(
    theme: 'dark',
    notifications: false,
    fontSize: 16,
  );
  await configManager.persist(settings);

  // Retrieve configuration
  final currentSettings = configManager.data;
  print('Theme: ${currentSettings.theme}');
}
```

### Locale Settings Storage

```dart
import 'package:app_config_prefs/app_config_prefs.dart';
import 'package:app_config_core/app_config_core.dart';
import 'package:shared_preferences/shared_preferences.dart';

class LocaleStorageManager extends LocalStorageManager<LocaleSettings> {
  LocaleStorageManager(SharedPreferences storage) 
    : super(storage, (json) => LocaleSettings.fromJson(json));
}

// Usage
void main() async {
  final prefs = await SharedPreferences.getInstance();
  final localeManager = LocaleStorageManager(prefs);

  // Set fallback locale
  localeManager.setFallback(LocaleSettings(localeIdentifier: 'en-US'));

  await localeManager.initialize();

  // Change locale
  final newLocale = LocaleSettings(localeIdentifier: 'es-ES');
  await localeManager.persist(newLocale);

  // Get current locale
  final currentLocale = localeManager.data;
  print('Current locale: ${currentLocale.localeIdentifier}');
}
```

### Game Settings Storage

```dart
import 'package:app_config_prefs/app_config_prefs.dart';
import 'package:shared_preferences/shared_preferences.dart';

class GameSettings {
  final int soundVolume;
  final bool vibrationEnabled;
  final String difficulty;
  final List<String> recentGames;

  GameSettings({
    required this.soundVolume,
    required this.vibrationEnabled,
    required this.difficulty,
    required this.recentGames,
  });

  Map<String, dynamic> toJson() => {
    'soundVolume': soundVolume,
    'vibrationEnabled': vibrationEnabled,
    'difficulty': difficulty,
    'recentGames': recentGames,
  };

  factory GameSettings.fromJson(Map<String, dynamic> json) => GameSettings(
    soundVolume: json['soundVolume'] ?? 50,
    vibrationEnabled: json['vibrationEnabled'] ?? true,
    difficulty: json['difficulty'] ?? 'medium',
    recentGames: List<String>.from(json['recentGames'] ?? []),
  );
}

// Usage
void main() async {
  final prefs = await SharedPreferences.getInstance();
  
  final gameConfig = LocalStorageManager<GameSettings>.create(
    prefs,
    (json) => GameSettings.fromJson(json),
  );

  gameConfig.setFallback(GameSettings(
    soundVolume: 50,
    vibrationEnabled: true,
    difficulty: 'medium',
    recentGames: [],
  ));

  await gameConfig.initialize();

  // Update game settings
  final updatedSettings = GameSettings(
    soundVolume: 75,
    vibrationEnabled: false,
    difficulty: 'hard',
    recentGames: ['game1', 'game2', 'game3'],
  );
  await gameConfig.persist(updatedSettings);
}
```

### Network Configuration Storage

```dart
import 'package:app_config_prefs/app_config_prefs.dart';
import 'package:app_config_core/app_config_core.dart';
import 'package:shared_preferences/shared_preferences.dart';

class NetworkConfig {
  final String apiUrl;
  final int timeout;
  final bool useHttps;

  NetworkConfig({
    required this.apiUrl,
    required this.timeout,
    required this.useHttps,
  });

  Map<String, dynamic> toJson() => {
    'apiUrl': apiUrl,
    'timeout': timeout,
    'useHttps': useHttps,
  };

  factory NetworkConfig.fromJson(Map<String, dynamic> json) => NetworkConfig(
    apiUrl: json['apiUrl'] ?? 'https://api.flyingdarts.com',
    timeout: json['timeout'] ?? 30,
    useHttps: json['useHttps'] ?? true,
  );
}

// Usage
void main() async {
  final prefs = await SharedPreferences.getInstance();
  
  final networkConfig = LocalStorageManager<NetworkConfig>.create(
    prefs,
    (json) => NetworkConfig.fromJson(json),
  );

  networkConfig.setFallback(NetworkConfig(
    apiUrl: 'https://api.flyingdarts.com',
    timeout: 30,
    useHttps: true,
  ));

  await networkConfig.initialize();

  // Configure network settings
  final config = NetworkConfig(
    apiUrl: 'https://staging-api.flyingdarts.com',
    timeout: 60,
    useHttps: true,
  );
  await networkConfig.persist(config);
}
```

## API Reference

### Core Components

#### LocalStorageManager<T>
A SharedPreferences-based implementation of ConfigWriter with fallback support.

```dart
class LocalStorageManager<T> extends ConfigWriter<T> with FallbackProvider<T> {
  final SharedPreferences _storage;
  final Function(Map<String, dynamic>) _deserializer;

  LocalStorageManager(this._storage, this._deserializer);

  static LocalStorageManager<T> create<T>(
    SharedPreferences storage,
    Function(Map<String, dynamic>) deserializer,
  );
}
```

### Methods

#### LocalStorageManager Methods
- **`create<T>(storage, deserializer)`**: Static factory method for creating instances
- **`fetchData()`**: Fetches data from SharedPreferences with fallback support
- **`storeData(T data)`**: Stores data to SharedPreferences with JSON serialization
- **`setFallback(T value)`**: Sets a fallback value (inherited from FallbackProvider)
- **`fallbackValue`**: Getter for accessing the fallback value

### Behavior

#### Storage Key Generation
The package uses the type name as the storage key:

```dart
final String storageKey = T.toString();
```

#### JSON Serialization
Data is automatically serialized/deserialized using JSON:

```dart
// Serialization
final String serializedData = jsonEncode(data);
await _storage.setString(storageKey, serializedData);

// Deserialization
final Map<String, dynamic> jsonData = jsonDecode(storedValue);
return _deserializer(jsonData);
```

#### Fallback Support
If no stored data is found and a fallback is set, the fallback value is returned:

```dart
if (storedValue == null && fallbackValue != null) {
  return fallbackValue!;
}
```

## Configuration

### SharedPreferences Setup
Initialize SharedPreferences before using the package:

```dart
import 'package:shared_preferences/shared_preferences.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final prefs = await SharedPreferences.getInstance();
  
  // Use prefs with LocalStorageManager
}
```

### JSON Serialization
Ensure your data models support JSON serialization:

```dart
class MyConfig {
  final String name;
  final int value;

  MyConfig({required this.name, required this.value});

  Map<String, dynamic> toJson() => {
    'name': name,
    'value': value,
  };

  factory MyConfig.fromJson(Map<String, dynamic> json) => MyConfig(
    name: json['name'] ?? '',
    value: json['value'] ?? 0,
  );
}
```

### Error Handling
Implement proper error handling for storage operations:

```dart
try {
  await configManager.persist(data);
} catch (e) {
  print('Failed to persist configuration: $e');
  // Handle error appropriately
}
```

## Development

### Project Structure
```
app_config_prefs/
├── lib/
│   ├── preferences.dart                 # Main library export
│   └── src/                            # Source files
│       └── local_storage_manager.dart   # SharedPreferences implementation
├── test/                               # Unit tests
├── pubspec.yaml                        # Package dependencies
└── package.json                        # Build scripts
```

### Architecture Patterns
- **Inheritance Pattern**: Extends App Config Core classes
- **Generic Pattern**: Type-safe configuration with generics
- **Factory Pattern**: Static factory method for instance creation
- **Mixin Pattern**: Fallback provider functionality
- **JSON Serialization**: Automatic JSON encoding/decoding

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
- Test SharedPreferences integration thoroughly

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **app_config_core**: Configuration core package
- **shared_preferences**: ^2.5.3 - Local storage

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[App Config Core](../../../app_config_core/)**: Configuration core package
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

1. **SharedPreferences not initialized**
   - Ensure `WidgetsFlutterBinding.ensureInitialized()` is called
   - Check that SharedPreferences.getInstance() completes successfully
   - Verify proper async/await usage

2. **JSON serialization errors**
   - Check that data models have proper toJson() and fromJson() methods
   - Verify JSON structure matches model definitions
   - Ensure proper error handling in deserializer functions

3. **Storage key conflicts**
   - The package uses type names as storage keys
   - Ensure different configuration types have unique class names
   - Check for naming conflicts in your application

4. **Fallback values not working**
   - Ensure setFallback() is called before accessing data
   - Check that fallback value is not null
   - Verify fallback logic in custom implementations

5. **Data persistence failures**
   - Check SharedPreferences permissions and storage space
   - Verify JSON serialization is working correctly
   - Ensure proper error handling in storage operations

6. **Type safety issues**
   - Ensure proper generic type parameters are used
   - Check that deserializer functions return the correct type
   - Verify type compatibility in data operations

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Test SharedPreferences integration thoroughly
6. Verify JSON serialization works correctly
7. Test fallback mechanisms
8. Ensure proper error handling

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.