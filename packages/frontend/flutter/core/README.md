# Flyingdarts Core Package

## Overview

The Flyingdarts Core Package is a Flutter library that provides essential infrastructure and configuration management for the Flyingdarts Turbo platform. It serves as the foundation for dependency injection, network configuration, and application setup across all Flutter applications in the monorepo.

This package is responsible for:
- Configuring dependency injection with GetIt and Injectable
- Managing network endpoints and API configurations
- Providing secure storage and preferences management
- Supporting multiple build flavors (dev, acc, prod)
- Setting up HTTP client with retry logic
- Managing application configuration and secrets
- Providing core infrastructure for all Flutter applications

## Features

- **Dependency Injection**: Centralized dependency injection configuration
- **Network Configuration**: Environment-specific API and WebSocket endpoints
- **Secure Storage**: Secure token and sensitive data storage
- **Preferences Management**: User preferences and settings storage
- **Build Flavors**: Support for development, acceptance, and production environments
- **HTTP Client**: Configured Dio client with smart retry logic
- **Configuration Management**: Centralized configuration for all Flutter apps
- **Code Generation**: Injectable code generation for dependency injection

## Prerequisites

- Flutter SDK 3.26.0 or higher
- Dart SDK 3.8.1 or higher
- build_runner for code generation

## Installation

Add the package to your `pubspec.yaml`:

```yaml
dependencies:
  core:
    path: ../../packages/frontend/flutter/core
```

Or use it as a workspace dependency:

```yaml
dependencies:
  core:
    workspace: true
```

## Usage

### Basic Setup

Import the core package in your main application:

```dart
import 'package:core/core.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  // Initialize dependency injection
  await configureDependencies();
  
  runApp(MyApp());
}
```

### Dependency Injection

The core package provides centralized dependency injection:

```dart
import 'package:core/core.dart';
import 'package:get_it/get_it.dart';

final getIt = GetIt.instance;

// Access injected dependencies
final dio = getIt<Dio>();
final sharedPreferences = getIt<SharedPreferences>();
final secureStorage = getIt<FlutterSecureStorage>();
```

### Network Configuration

Access environment-specific network endpoints:

```dart
import 'package:core/core.dart';

// Get WebSocket endpoint for current environment
final webSocketUri = getIt<String>(instanceName: 'root_websocket_uri');

// Get configured network endpoint
final networkEndpoint = getIt<NetworkEndpoint>();
```

### Configuration Management

Access application configuration:

```dart
import 'package:core/core.dart';

// Access language configuration
final languageConfig = getIt<ConfigReader<LocaleSettings>>();

// Access secure secrets configuration
final secretsConfig = getIt<ConfigWriter<AuthTokens>>();
```

## API Reference

### Modules

#### `ApiModule`

Manages network-related dependencies and configurations.

**Properties:**

##### `rootWebSocketUriDev: String`

Development WebSocket endpoint.

**Returns:**
- `String`: Development WebSocket URL

##### `rootWebSocketUriProd: String`

Production WebSocket endpoint.

**Returns:**
- `String`: Production WebSocket URL

##### `config(rootWebSocketUri: String): NetworkEndpoint`

Creates a network endpoint configuration.

**Parameters:**
- `rootWebSocketUri` (String): WebSocket connection URL

**Returns:**
- `NetworkEndpoint`: Configured network endpoint

##### `dio: Dio`

Configured HTTP client with retry logic.

**Returns:**
- `Dio`: HTTP client instance with interceptors

#### `ConfigurationModule`

Manages application configuration and storage.

**Properties:**

##### `sharedPreferences: Future<SharedPreferences>`

Shared preferences instance for user settings.

**Returns:**
- `Future<SharedPreferences>`: Shared preferences instance

##### `flutterSecureStorage: Future<FlutterSecureStorage>`

Secure storage instance for sensitive data.

**Returns:**
- `Future<FlutterSecureStorage>`: Secure storage instance

##### `languageConfig(sharedPreferences: SharedPreferences): Future<ConfigWriter<LocaleSettings>>`

Creates language configuration manager.

**Parameters:**
- `sharedPreferences` (SharedPreferences): Shared preferences instance

**Returns:**
- `Future<ConfigWriter<LocaleSettings>>`: Language configuration writer

##### `readableLanguageConfig(config: ConfigWriter<LocaleSettings>): ConfigReader<LocaleSettings>`

Creates readable language configuration.

**Parameters:**
- `config` (ConfigWriter<LocaleSettings>): Language configuration writer

**Returns:**
- `ConfigReader<LocaleSettings>`: Language configuration reader

##### `secretsConfiguration(flutterSecureStorage: FlutterSecureStorage): Future<ConfigWriter<AuthTokens>>`

Creates secure secrets configuration manager.

**Parameters:**
- `flutterSecureStorage` (FlutterSecureStorage): Secure storage instance

**Returns:**
- `Future<ConfigWriter<AuthTokens>>`: Secrets configuration writer

#### `FlavorModule`

Manages build flavor configuration.

**Properties:**

##### `flavorDev: String`

Development build flavor identifier.

**Returns:**
- `String`: "dev"

##### `flavorProd: String`

Production build flavor identifier.

**Returns:**
- `String`: "prod"

##### `flavorAcc: String`

Acceptance testing build flavor identifier.

**Returns:**
- `String`: "acc"

### Constants

#### `rootWebSocketUri`

Named constant for WebSocket URI injection.

#### `rootUsersApiUri`

Named constant for Users API URI injection.

#### `flavor`

Named constant for build flavor injection.

#### `acc`

Environment constant for acceptance testing.

### Global Variables

#### `getIt`

Global GetIt instance for dependency injection.

## Configuration

### Environment-Specific Endpoints

The package provides different endpoints based on build flavor:

#### Development Environment
- **WebSocket**: `wss://r051b8g6e7.execute-api.eu-west-1.amazonaws.com/Development/`

#### Production Environment
- **WebSocket**: `wss://pd3h2kmulf.execute-api.eu-west-1.amazonaws.com/Production/`

### HTTP Client Configuration

The configured Dio client includes:

- **Retry Logic**: 5 retry attempts with exponential backoff
- **Retry Delays**: 250ms, 500ms, 1s, 2s, 4s
- **Interceptors**: Smart retry interceptor

### Default Configurations

#### Language Settings
- **Default Locale**: `nl-US`
- **Storage**: Shared preferences

#### Authentication Tokens
- **Default Tokens**: Empty strings for access, ID, and refresh tokens
- **Storage**: Secure storage

## Dependencies

### Internal Dependencies (Workspace Packages)

- **websocket**: WebSocket communication
- **app_config_core**: Core configuration management
- **app_config_prefs**: Preferences storage
- **app_config_secrets**: Secure configuration storage

### External Dependencies

- **dio**: HTTP client
- **dio_smart_retry**: Retry logic for HTTP requests
- **flutter_secure_storage**: Secure storage for sensitive data
- **shared_preferences**: User preferences storage
- **get_it**: Dependency injection
- **injectable**: Code generation for dependency injection

### Development Dependencies

- **flutter_test**: Testing framework
- **flutter_lints**: Code linting
- **injectable_generator**: Code generation
- **build_runner**: Build system

## Development

### Project Structure

```
core/
├── lib/
│   ├── core.dart              # Main library entry point
│   └── src/
│       ├── api.dart           # API and network configuration
│       ├── configuration.dart # Configuration management
│       └── flavor.dart        # Build flavor configuration
├── pubspec.yaml               # Dependencies and metadata
├── package.json               # Node.js package metadata
└── README.md                  # This documentation
```

### Code Generation

Run code generation for dependency injection:

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

### Building

Build for development:

```bash
flutter build
```

## Integration Examples

### Application Setup

```dart
import 'package:core/core.dart';
import 'package:flutter/material.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  // Initialize core dependencies
  await configureDependencies();
  
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flyingdarts',
      home: MyHomePage(),
    );
  }
}
```

### Network Service Integration

```dart
import 'package:core/core.dart';
import 'package:dio/dio.dart';

class NetworkService {
  final Dio _dio;
  final NetworkEndpoint _endpoint;

  NetworkService(this._dio, this._endpoint);

  Future<void> connect() async {
    // Use configured Dio client with retry logic
    final response = await _dio.get('${_endpoint.connectionUrl}/health');
    print('Connection status: ${response.statusCode}');
  }
}
```

### Configuration Access

```dart
import 'package:core/core.dart';

class SettingsService {
  final ConfigReader<LocaleSettings> _languageConfig;
  final ConfigWriter<AuthTokens> _secretsConfig;

  SettingsService(this._languageConfig, this._secretsConfig);

  Future<void> updateLanguage(String locale) async {
    final settings = LocaleSettings(localeIdentifier: locale);
    await _languageConfig.set(settings);
  }

  Future<void> storeTokens(AuthTokens tokens) async {
    await _secretsConfig.set(tokens);
  }
}
```

## Build Flavors

The package supports three build flavors:

### Development (`dev`)
- Debug features enabled
- Development endpoints
- Verbose logging

### Acceptance Testing (`acc`)
- Testing endpoints
- Debug features enabled
- Test data configuration

### Production (`prod`)
- Production endpoints
- Optimized configuration
- Minimal logging

## Security Considerations

- **Secure Storage**: Sensitive data is stored using Flutter Secure Storage
- **Token Management**: Authentication tokens are stored securely
- **Network Security**: Use HTTPS/WSS for all network communications
- **Configuration**: Environment-specific configurations prevent data leakage

## Related Projects

- **flyingdarts-mobile**: Flutter mobile application using this core package
- **flyingdarts-app-config-core**: Core configuration management
- **flyingdarts-app-config-prefs**: Preferences storage
- **flyingdarts-app-config-secrets**: Secure configuration storage
- **flyingdarts-websocket**: WebSocket communication

## Troubleshooting

### Common Issues

1. **Code Generation**: Run `build_runner build` after dependency changes
2. **Dependency Resolution**: Ensure all workspace dependencies are available
3. **Configuration**: Verify environment-specific configurations
4. **Secure Storage**: Check platform-specific secure storage requirements

### Debugging

Enable debug logging:

```dart
// In your code
print('Debug: Configuration loaded');
```

Check dependency injection:

```dart
// Verify dependencies are registered
final dio = getIt<Dio>();
print('Dio instance: $dio');
```

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow Flutter best practices and conventions

## License

Part of the Flyingdarts Turbo monorepo. See root LICENSE file for details.
