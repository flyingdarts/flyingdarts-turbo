# Flying Darts App Config Secrets

## Overview

The Flying Darts App Config Secrets is a Flutter package that provides a secure storage implementation of the configuration management system for the Flying Darts gaming platform. This package enables Flutter applications to securely store and retrieve sensitive configuration data using Flutter's Secure Storage with encryption, type safety, and fallback support.

The package is built on top of the App Config Core package, extending its abstract configuration classes with a concrete Flutter Secure Storage implementation. It provides a robust foundation for implementing secure configuration storage in Flutter apps with automatic JSON serialization, encryption, caching, and fallback value support. The package is specifically designed for storing sensitive data such as authentication tokens, API keys, and other security-critical configuration.

## Features

- **Secure Storage Integration**: Flutter Secure Storage with encryption for sensitive data
- **Type-Safe Configuration**: Generic configuration classes with compile-time type safety
- **Automatic JSON Serialization**: Built-in JSON encoding/decoding with code generation
- **Encryption**: Automatic encryption/decryption of stored data
- **Fallback Support**: Fallback value mechanism for configuration providers
- **Caching**: Inherited caching mechanism from App Config Core
- **Read/Write Operations**: Support for both reading and writing secure configuration data
- **Error Handling**: Comprehensive error handling for secure storage operations
- **Cross-Platform Support**: Works on iOS, Android, and Web with platform-specific encryption
- **Extensible Architecture**: Built on App Config Core for extensibility
- **Automatic Key Generation**: Type-based storage key generation
- **Code Generation**: Automatic JSON serialization code generation with build_runner
- **Authentication Tokens**: Pre-built AuthTokens model for secure token storage

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ^3.26.0 or higher
- **App Config Core**: Access to the app_config_core package
- **Flutter Secure Storage**: ^10.0.0-beta.4 for encrypted storage
- **JSON Annotation**: ^4.9.0 for code generation
- **Build Runner**: ^2.5.4 for code generation

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the app config secrets package**:
   ```bash
   cd packages/frontend/flutter/shared/config/app_config_secrets
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
  app_config_secrets:
    path: ../../packages/frontend/flutter/shared/config/app_config_secrets
```

## Usage

### Basic Secure Configuration Storage

```dart
import 'package:app_config_secrets/app_config_secrets.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class SecureApiConfig {
  final String apiKey;
  final String secretKey;
  final String endpoint;

  SecureApiConfig({
    required this.apiKey,
    required this.secretKey,
    required this.endpoint,
  });

  Map<String, dynamic> toJson() => {
    'apiKey': apiKey,
    'secretKey': secretKey,
    'endpoint': endpoint,
  };

  factory SecureApiConfig.fromJson(Map<String, dynamic> json) => SecureApiConfig(
    apiKey: json['apiKey'] ?? '',
    secretKey: json['secretKey'] ?? '',
    endpoint: json['endpoint'] ?? '',
  );
}

// Usage
void main() async {
  final secureStorage = FlutterSecureStorage();
  
  final secureConfigManager = SecureStorageManager<SecureApiConfig>.create(
    secureStorage,
    (json) => SecureApiConfig.fromJson(json),
  );

  // Set fallback value
  secureConfigManager.setFallback(SecureApiConfig(
    apiKey: '',
    secretKey: '',
    endpoint: 'https://api.flyingdarts.com',
  ));

  await secureConfigManager.initialize();

  // Store secure configuration
  final config = SecureApiConfig(
    apiKey: 'your-api-key',
    secretKey: 'your-secret-key',
    endpoint: 'https://secure-api.flyingdarts.com',
  );
  await secureConfigManager.persist(config);

  // Retrieve secure configuration
  final currentConfig = secureConfigManager.data;
  print('API Endpoint: ${currentConfig.endpoint}');
}
```

### Authentication Tokens Storage

```dart
import 'package:app_config_secrets/app_config_secrets.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthTokenManager {
  final SecureStorageManager<AuthTokens> _tokenManager;

  AuthTokenManager(FlutterSecureStorage secureStorage)
    : _tokenManager = SecureStorageManager<AuthTokens>.create(
        secureStorage,
        (json) => AuthTokens.fromJson(json),
      );

  Future<void> initialize() async {
    await _tokenManager.initialize();
  }

  Future<void> storeTokens({
    required String accessToken,
    required String idToken,
    required String refreshToken,
  }) async {
    final tokens = AuthTokens(
      accessToken: accessToken,
      idToken: idToken,
      refreshToken: refreshToken,
    );
    await _tokenManager.persist(tokens);
  }

  AuthTokens? get tokens => _tokenManager.data;

  Future<void> clearTokens() async {
    await _tokenManager.persist(AuthTokens(
      accessToken: '',
      idToken: '',
      refreshToken: '',
    ));
  }
}

// Usage
void main() async {
  final secureStorage = FlutterSecureStorage();
  final authManager = AuthTokenManager(secureStorage);

  await authManager.initialize();

  // Store authentication tokens
  await authManager.storeTokens(
    accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...',
    idToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...',
    refreshToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...',
  );

  // Retrieve tokens
  final tokens = authManager.tokens;
  if (tokens != null) {
    print('Access Token: ${tokens.accessToken}');
    print('ID Token: ${tokens.idToken}');
    print('Refresh Token: ${tokens.refreshToken}');
  }

  // Clear tokens on logout
  await authManager.clearTokens();
}
```

### Custom Secure Data Models

```dart
import 'package:app_config_secrets/app_config_secrets.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:json_annotation/json_annotation.dart';

part 'secure_user_data.g.dart';

@JsonSerializable()
class SecureUserData {
  final String userId;
  final String encryptedPassword;
  final String privateKey;
  final Map<String, dynamic> preferences;

  SecureUserData({
    required this.userId,
    required this.encryptedPassword,
    required this.privateKey,
    required this.preferences,
  });

  factory SecureUserData.fromJson(Map<String, dynamic> json) => 
    _$SecureUserDataFromJson(json);

  Map<String, dynamic> toJson() => _$SecureUserDataToJson(this);
}

// Usage
void main() async {
  final secureStorage = FlutterSecureStorage();
  
  final userDataManager = SecureStorageManager<SecureUserData>.create(
    secureStorage,
    (json) => SecureUserData.fromJson(json),
  );

  await userDataManager.initialize();

  // Store secure user data
  final userData = SecureUserData(
    userId: 'user123',
    encryptedPassword: 'encrypted-password-hash',
    privateKey: 'private-key-content',
    preferences: {'theme': 'dark', 'notifications': true},
  );
  await userDataManager.persist(userData);

  // Retrieve secure user data
  final currentUserData = userDataManager.data;
  print('User ID: ${currentUserData?.userId}');
}
```

### API Keys and Secrets Management

```dart
import 'package:app_config_secrets/app_config_secrets.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ApiSecrets {
  final String stripeSecretKey;
  final String awsAccessKey;
  final String awsSecretKey;
  final String firebaseApiKey;

  ApiSecrets({
    required this.stripeSecretKey,
    required this.awsAccessKey,
    required this.awsSecretKey,
    required this.firebaseApiKey,
  });

  Map<String, dynamic> toJson() => {
    'stripeSecretKey': stripeSecretKey,
    'awsAccessKey': awsAccessKey,
    'awsSecretKey': awsSecretKey,
    'firebaseApiKey': firebaseApiKey,
  };

  factory ApiSecrets.fromJson(Map<String, dynamic> json) => ApiSecrets(
    stripeSecretKey: json['stripeSecretKey'] ?? '',
    awsAccessKey: json['awsAccessKey'] ?? '',
    awsSecretKey: json['awsSecretKey'] ?? '',
    firebaseApiKey: json['firebaseApiKey'] ?? '',
  );
}

// Usage
void main() async {
  final secureStorage = FlutterSecureStorage();
  
  final secretsManager = SecureStorageManager<ApiSecrets>.create(
    secureStorage,
    (json) => ApiSecrets.fromJson(json),
  );

  await secretsManager.initialize();

  // Store API secrets
  final secrets = ApiSecrets(
    stripeSecretKey: 'sk_test_...',
    awsAccessKey: 'AKIA...',
    awsSecretKey: 'secret-key...',
    firebaseApiKey: 'AIza...',
  );
  await secretsManager.persist(secrets);

  // Use secrets in API calls
  final currentSecrets = secretsManager.data;
  if (currentSecrets != null) {
    // Use secrets for API authentication
    print('Stripe Key: ${currentSecrets.stripeSecretKey}');
  }
}
```

## API Reference

### Core Components

#### SecureStorageManager<T>
A Flutter Secure Storage-based implementation of ConfigWriter with encryption and fallback support.

```dart
class SecureStorageManager<T> extends ConfigWriter<T> with FallbackProvider<T> {
  final FlutterSecureStorage _secureStorage;
  final Function(Map<String, dynamic>) _deserializer;

  SecureStorageManager(this._secureStorage, this._deserializer);

  static SecureStorageManager<T> create<T>(
    FlutterSecureStorage secureStorage,
    Function(Map<String, dynamic>) deserializer,
  );
}
```

#### AuthTokens
A pre-built model for storing authentication tokens securely.

```dart
@JsonSerializable()
class AuthTokens {
  final String accessToken;
  final String idToken;
  final String refreshToken;

  const AuthTokens({
    required this.accessToken,
    required this.idToken,
    required this.refreshToken,
  });

  factory AuthTokens.fromJson(Map<String, dynamic> json);
  Map<String, dynamic> toJson();
}
```

### Methods

#### SecureStorageManager Methods
- **`create<T>(secureStorage, deserializer)`**: Static factory method for creating instances
- **`fetchData()`**: Fetches encrypted data from secure storage with fallback support
- **`storeData(T data)`**: Stores data to secure storage with encryption and JSON serialization
- **`setFallback(T value)`**: Sets a fallback value (inherited from FallbackProvider)
- **`fallbackValue`**: Getter for accessing the fallback value

### Behavior

#### Storage Key Generation
The package uses the type name as the storage key:

```dart
final String storageKey = T.toString();
```

#### Encryption and JSON Serialization
Data is automatically encrypted and serialized/deserialized:

```dart
// Encryption and serialization
final String serializedData = jsonEncode(data);
await _secureStorage.write(key: storageKey, value: serializedData);

// Decryption and deserialization
final String? encryptedValue = await _secureStorage.read(key: storageKey);
final Map<String, dynamic> jsonData = jsonDecode(encryptedValue);
return _deserializer(jsonData);
```

#### Fallback Support
If no stored data is found and a fallback is set, the fallback value is returned:

```dart
if (encryptedValue == null && fallbackValue != null) {
  return fallbackValue!;
}
```

## Configuration

### Flutter Secure Storage Setup
Initialize Flutter Secure Storage before using the package:

```dart
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final secureStorage = FlutterSecureStorage();
  
  // Use secureStorage with SecureStorageManager
}
```

### Platform-Specific Configuration
Configure secure storage options for different platforms:

```dart
final secureStorage = FlutterSecureStorage(
  aOptions: AndroidOptions(
    encryptedSharedPreferences: true,
  ),
  iOptions: IOSOptions(
    accessibility: IOSAccessibility.first_unlock_this_device,
  ),
  webOptions: WebOptions(
    dbName: "secure_storage",
    publicKey: "your-public-key",
  ),
);
```

### JSON Serialization with Code Generation
For complex data models, use JSON annotation for automatic code generation:

```dart
import 'package:json_annotation/json_annotation.dart';

part 'my_secure_data.g.dart';

@JsonSerializable()
class MySecureData {
  final String secret;
  final int value;

  MySecureData({required this.secret, required this.value});

  factory MySecureData.fromJson(Map<String, dynamic> json) => 
    _$MySecureDataFromJson(json);

  Map<String, dynamic> toJson() => _$MySecureDataToJson(this);
}
```

Generate code with:
```bash
dart run build_runner build --delete-conflicting-outputs
```

### Error Handling
Implement proper error handling for secure storage operations:

```dart
try {
  await secureConfigManager.persist(sensitiveData);
} catch (e) {
  print('Failed to store secure configuration: $e');
  // Handle encryption or storage errors appropriately
}
```

## Development

### Project Structure
```
app_config_secrets/
├── lib/
│   ├── secrets.dart                    # Main library export
│   ├── secure_storage_manager.dart     # Secure storage implementation
│   └── models/                         # Data models
│       ├── auth_tokens.dart            # Auth tokens model
│       └── auth_tokens.g.dart          # Generated JSON code
├── test/                               # Unit tests
├── pubspec.yaml                        # Package dependencies
└── package.json                        # Build scripts
```

### Architecture Patterns
- **Inheritance Pattern**: Extends App Config Core classes
- **Generic Pattern**: Type-safe configuration with generics
- **Factory Pattern**: Static factory method for instance creation
- **Mixin Pattern**: Fallback provider functionality
- **JSON Serialization**: Automatic JSON encoding/decoding with code generation
- **Encryption Pattern**: Secure storage with automatic encryption

### Code Generation
The package uses build_runner for automatic code generation:

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
- Test secure storage integration thoroughly
- Verify encryption/decryption functionality
- Test platform-specific secure storage behavior

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **app_config_core**: Configuration core package
- **flutter_secure_storage**: ^10.0.0-beta.4 - Encrypted storage
- **json_annotation**: ^4.9.0 - JSON serialization annotations

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **json_serializable**: ^6.9.5 - JSON code generation
- **build_runner**: ^2.5.4 - Code generation tool

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[App Config Core](../../../app_config_core/)**: Configuration core package
- **[App Config Prefs](../../../app_config_prefs/)**: SharedPreferences configuration
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

1. **Flutter Secure Storage not initialized**
   - Ensure `WidgetsFlutterBinding.ensureInitialized()` is called
   - Check that FlutterSecureStorage instance is created properly
   - Verify proper async/await usage

2. **Encryption errors**
   - Check platform-specific secure storage configuration
   - Verify encryption keys and certificates are properly set
   - Ensure proper error handling in encryption operations

3. **JSON serialization errors**
   - Check that data models have proper toJson() and fromJson() methods
   - Verify JSON structure matches model definitions
   - Ensure proper error handling in deserializer functions
   - Run code generation if using @JsonSerializable annotations

4. **Storage key conflicts**
   - The package uses type names as storage keys
   - Ensure different configuration types have unique class names
   - Check for naming conflicts in your application

5. **Fallback values not working**
   - Ensure setFallback() is called before accessing data
   - Check that fallback value is not null
   - Verify fallback logic in custom implementations

6. **Platform-specific issues**
   - iOS: Check keychain access permissions
   - Android: Verify encrypted shared preferences configuration
   - Web: Ensure proper public key configuration

7. **Code generation issues**
   - Run `dart run build_runner build --delete-conflicting-outputs`
   - Check for syntax errors in @JsonSerializable classes
   - Verify build_runner dependencies are properly configured

8. **Security concerns**
   - Never log sensitive data to console
   - Use proper encryption for all sensitive data
   - Implement secure key management
   - Test encryption/decryption thoroughly

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Test secure storage integration thoroughly
6. Verify encryption/decryption works correctly
7. Test fallback mechanisms
8. Ensure proper error handling
9. Test platform-specific secure storage behavior
10. Verify code generation works properly
11. Test with sensitive data handling
12. Ensure security best practices are followed

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
