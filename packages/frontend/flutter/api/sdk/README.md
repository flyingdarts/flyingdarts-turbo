# Flyingdarts Flutter API SDK

## Overview

The Flyingdarts Flutter API SDK is a Dart package that provides a comprehensive client-side API interface for the Flyingdarts gaming platform. This SDK enables Flutter applications to interact with the backend services through a clean, type-safe, and well-structured API layer.

The SDK is built using modern Dart patterns with dependency injection, JSON serialization, and comprehensive error handling. It provides a robust foundation for making HTTP requests to the Flyingdarts backend services, handling responses, and managing authentication tokens.

## Features

- **Type-Safe API Client**: Strongly typed API methods with proper response handling
- **Dependency Injection**: Built-in support for dependency injection using Injectable
- **JSON Serialization**: Automatic JSON serialization/deserialization with code generation
- **Error Handling**: Comprehensive error handling with strategy pattern implementation
- **HTTP Client**: Dio-based HTTP client with interceptors and middleware
- **Response Wrapping**: Sealed class response types for better error handling
- **Authentication**: Built-in support for authorization tokens
- **Code Generation**: Automatic code generation for JSON serialization
- **Testing Support**: Mockito integration for unit testing
- **Flutter Integration**: Native Flutter package with proper platform support

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ">=3.26.0"
- **Backend Services**: Access to Flyingdarts backend APIs
- **Authentication**: Valid authorization tokens for API access

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the API SDK**:
   ```bash
   cd packages/frontend/flutter/api/sdk
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
  api_sdk:
    path: ../../packages/frontend/flutter/api/sdk
```

## Usage

### Basic Setup

1. **Configure dependency injection**:
   ```dart
   import 'package:injectable/injectable.dart';
   import 'package:api_sdk/flyingdarts_sdk.dart';

   @InjectableInit()
   void configureDependencies() => $initGetIt();
   ```

2. **Configure API settings**:
   ```dart
   final apiConfig = FlyingdartsApiConfig(
     authorizationToken: 'your-auth-token',
     rootUri: Uri.parse('https://api.flyingdarts.com'),
   );
   ```

### Using the Users API

```dart
import 'package:api_sdk/flyingdarts_sdk.dart';

class UserService {
  final UsersApi _usersApi;

  UserService(this._usersApi);

  Future<UserProfile?> getUserProfile(String userId) async {
    final response = await _usersApi.getUser(userId);
    
    return switch (response) {
      SuccessResponse(:final response) => response.userProfile,
      ErrorResponse(:final failure) => null,
    };
  }
}
```

### Error Handling

```dart
Future<void> handleUserRequest(String userId) async {
  final response = await _usersApi.getUser(userId);
  
  switch (response) {
    case SuccessResponse(:final response):
      print('User: ${response.userProfile.UserName}');
      break;
    case ErrorResponse(:final failure):
      print('Error: ${failure.toString()}');
      break;
  }
}
```

### Using Response Extensions

```dart
Future<void> handleResponse(String userId) async {
  final response = await _usersApi.getUser(userId);
  
  // Unwrap with error handling
  try {
    final successResponse = response.unwrap();
    print('User: ${successResponse.response.userProfile.UserName}');
  } catch (e) {
    print('Error: $e');
  }
  
  // Or use expect with custom message
  final userProfile = response
    .expect('Failed to get user profile')
    .response.userProfile;
}
```

## API Reference

### Core Configuration

#### FlyingdartsApiConfig
Main configuration class for API settings.

```dart
class FlyingdartsApiConfig {
  final String authorizationToken;
  final Uri rootUri;

  const FlyingdartsApiConfig({
    required this.authorizationToken,
    required this.rootUri,
  });
}
```

### API Clients

#### UsersApi
Client for user-related API operations.

```dart
@injectable
class UsersApi {
  final UsersApiConfig config;
  final Dio dio;
  final ErrorResponseHandler responseHandler;

  UsersApi(this.config, this.dio, this.responseHandler);

  Future<RequestResponse<GetUserProfileSuccessResponse, RequestFailedResponse>> 
    getUser(String userId);
}
```

### Data Models

#### UserProfile
Represents a user profile in the system.

```dart
@JsonSerializable(createToJson: false)
class UserProfile extends Equatable {
  final String UserId;
  final String UserName;
  final String Country;
  final String Email;

  const UserProfile({
    required this.UserId,
    required this.UserName,
    required this.Country,
    required this.Email,
  });

  factory UserProfile.fromJson(Map<String, dynamic> json) => 
    _$UserProfileFromJson(json);
}
```

### Response Types

#### RequestResponse
Sealed class for handling API responses.

```dart
sealed class RequestResponse<TSucceeded, TFailed> {}

class SuccessResponse<TSucceeded, TFailed> 
  implements RequestResponse<TSucceeded, TFailed> {
  final TSucceeded response;
  const SuccessResponse(this.response);
}

class ErrorResponse<TSucceeded, TFailed> 
  implements RequestResponse<TSucceeded, TFailed> {
  final TFailed failure;
  const ErrorResponse(this.failure);
}
```

#### Response Extensions
Utility extensions for response handling.

```dart
extension Cast<TSucceeded, TFailed> on RequestResponse<TSucceeded, TFailed> {
  SuccessResponse<TSucceeded, TFailed> asSuccessResponse();
  ErrorResponse<TSucceeded, TFailed> asErrorResponse();
}

extension Expect<TSucceeded, TFailed> on RequestResponse<TSucceeded, TFailed> {
  SuccessResponse<TSucceeded, TFailed> expect(String? message);
  SuccessResponse<TSucceeded, TFailed> unwrap();
  SuccessResponse<TSucceeded, TFailed> unwrapOr(SuccessResponse<TSucceeded, TFailed> or);
  ErrorResponse<TSucceeded, TFailed> expectErr(String message);
}
```

### Error Handling

#### ErrorResponseHandler
Handles HTTP error responses using strategy pattern.

```dart
@injectable
class ErrorResponseHandler {
  ErrorResponse<TSuccess, RequestFailedResponse> 
    handleErrorResponse<TSuccess>(DioError errorResponse);
}
```

#### HttpResponseType
Enumeration for HTTP response types.

```dart
enum HttpResponseType {
  informational,
  redirection,
  clientError,
  serverError,
}
```

## Configuration

### Dependency Injection Setup
Configure the SDK with dependency injection:

```dart
import 'package:get_it/get_it.dart';
import 'package:injectable/injectable.dart';

final getIt = GetIt.instance;

@InjectableInit()
void configureDependencies() => $initGetIt();

void setupApiConfig() {
  getIt.registerSingleton<FlyingdartsApiConfig>(
    FlyingdartsApiConfig(
      authorizationToken: 'your-token',
      rootUri: Uri.parse('https://api.flyingdarts.com'),
    ),
  );
}
```

### Environment Configuration
Configure different environments:

```dart
class EnvironmentConfig {
  static const String dev = 'dev';
  static const String staging = 'staging';
  static const String production = 'production';
  
  static FlyingdartsApiConfig getConfig(String environment) {
    switch (environment) {
      case dev:
        return FlyingdartsApiConfig(
          authorizationToken: 'dev-token',
          rootUri: Uri.parse('https://dev-api.flyingdarts.com'),
        );
      case staging:
        return FlyingdartsApiConfig(
          authorizationToken: 'staging-token',
          rootUri: Uri.parse('https://staging-api.flyingdarts.com'),
        );
      case production:
        return FlyingdartsApiConfig(
          authorizationToken: 'prod-token',
          rootUri: Uri.parse('https://api.flyingdarts.com'),
        );
      default:
        throw ArgumentError('Unknown environment: $environment');
    }
  }
}
```

## Development

### Project Structure
```
api_sdk/
├── lib/
│   ├── flyingdarts_sdk.dart          # Main library export
│   └── src/
│       ├── flyingdarts_api_config.dart # Main API configuration
│       ├── users/                     # Users API implementation
│       │   ├── users_api.dart         # Users API client
│       │   ├── users_api_config.dart  # Users API configuration
│       │   ├── users_endpoints.dart   # Users API endpoints
│       │   ├── models/                # User data models
│       │   │   ├── user_profile.dart  # User profile model
│       │   │   └── user_profile.g.dart # Generated JSON code
│       │   ├── requests/              # API request implementations
│       │   │   └── get_user_profile/  # Get user profile request
│       │   └── options/               # Request options
│       ├── responses/                 # Response handling
│       │   ├── request_response.dart  # Main response types
│       │   ├── request_success_response.dart
│       │   ├── request_failed_response.dart
│       │   └── failures/              # Failure types
│       ├── error_handling/            # Error handling
│       │   ├── request_response_handler.dart
│       │   ├── http_response_type.dart
│       │   └── strategy/              # Error handling strategies
│       └── exceptions/                # Custom exceptions
├── test/                              # Unit tests
├── pubspec.yaml                       # Package dependencies
└── package.json                       # Build scripts
```

### Architecture Patterns
- **Dependency Injection**: Injectable for service management
- **Strategy Pattern**: Error handling strategies
- **Sealed Classes**: Type-safe response handling
- **Factory Pattern**: Response object creation
- **Builder Pattern**: Request options building
- **Extension Pattern**: Response utility methods

### Code Generation
The SDK uses code generation for JSON serialization:

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

### Code Quality
- Follow Dart coding conventions
- Use proper documentation for public APIs
- Implement comprehensive error handling
- Add unit tests for all public methods
- Use code generation for repetitive tasks

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **injectable**: ^2.5.0 - Dependency injection
- **json_annotation**: ^4.9.0 - JSON serialization annotations
- **equatable**: ^2.0.7 - Value equality
- **dio**: ^5.8.0+1 - HTTP client

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **json_serializable**: ^6.9.5 - JSON code generation
- **mockito**: ^5.4.6 - Mocking framework
- **build_runner**: ^2.5.4 - Code generation

## Related Projects

### Backend Services
- **[Friends API](../../../../backend/dotnet/friends/)**: Friend management and social features
- **[Games API](../../../../backend/dotnet/games/)**: Game management and scoring
- **[Auth API](../../../../backend/dotnet/auth/)**: Authentication and authorization

### Frontend Applications
- **[Flutter Mobile App](../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[Core Package](../../../../backend/dotnet/Flyingdarts.Core/)**: Shared business logic
- **[Persistence Package](../../../../backend/dotnet/Flyingdarts.Persistence/)**: Data access layer
- **[Metadata Services](../../../../backend/dotnet/Flyingdarts.Metadata.Services/)**: Metadata generation

## Version History

- **v0.0.6** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.5** (2025-07-10): Add flutter workspace at root
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution
- **v0.0.2** (2025-07-07): Initial change log

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Generate code after model changes
6. Maintain type safety and error handling patterns

## License

This project is part of the Flyingdarts platform and is subject to the project's licensing terms.
