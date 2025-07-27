# Flying Darts WebSocket

## Overview

The Flying Darts WebSocket is a Flutter package that provides comprehensive WebSocket communication capabilities for the Flying Darts gaming platform. This package enables Flutter applications to establish real-time bidirectional communication with backend services using WebSocket connections with authentication, automatic reconnection, and structured message handling.

The package is built using Flutter's WebSocket capabilities with RxDart for reactive programming, dependency injection with GetIt, and authentication integration with Authress. It provides a robust foundation for implementing real-time features in Flutter apps such as live game updates, chat functionality, notifications, and multiplayer game synchronization. The package includes comprehensive error handling, automatic reconnection logic, and structured message types for different game actions.

## Features

- **Real-Time Communication**: Bidirectional WebSocket communication with backend services
- **Authentication Integration**: Automatic token-based authentication with Authress
- **Automatic Reconnection**: Built-in reconnection logic with exponential backoff
- **Reactive Programming**: RxDart integration for reactive state management
- **Structured Messages**: Type-safe message handling with JSON serialization
- **Game Actions**: Pre-defined actions for X01 games, user profiles, and general communication
- **Connection Management**: Comprehensive connection state management and monitoring
- **Error Handling**: Robust error handling with automatic recovery
- **Dependency Injection**: GetIt integration for service management
- **Cross-Platform Support**: Works on iOS, Android, and Web
- **Type Safety**: Strongly typed message handling with generics
- **Logging**: Comprehensive logging for debugging and monitoring
- **Testing Support**: Mockito integration for unit testing

## Prerequisites

- **Dart SDK**: ^3.8.1 or higher
- **Flutter SDK**: ^3.26.0 or higher
- **Authress Login**: Access to the authress_login package
- **RxDart**: ^0.28.0 for reactive programming
- **Web Socket Channel**: ^3.0.3 for WebSocket communication
- **Injectable**: ^2.5.0 for dependency injection
- **GetIt**: ^8.0.3 for service locator

## Installation

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd flyingdarts-turbo
   ```

2. **Navigate to the WebSocket package**:
   ```bash
   cd packages/frontend/flutter/shared/websocket
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
  websocket:
    path: ../../packages/frontend/flutter/shared/websocket
  authress_login:
    path: ../../packages/frontend/flutter/shared/authress/login
  rxdart: ^0.28.0
  web_socket_channel: ^3.0.3
  injectable: ^2.5.0
  get_it: ^8.0.3
```

## Usage

### Basic WebSocket Setup

```dart
import 'package:flutter/material.dart';
import 'package:websocket/websocket.dart';
import 'package:get_it/get_it.dart';

void main() {
  // Initialize dependency injection
  GetIt.I.registerSingleton<WebSocketService>(
    WebSocketService('wss://api.flyingdarts.com/websocket'),
  );
  
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flying Darts',
      home: WebSocketExample(),
    );
  }
}

class WebSocketExample extends StatefulWidget {
  @override
  _WebSocketExampleState createState() => _WebSocketExampleState();
}

class _WebSocketExampleState extends State<WebSocketExample> {
  late WebSocketService _webSocketService;
  bool _isConnected = false;

  @override
  void initState() {
    super.initState();
    _webSocketService = GetIt.I<WebSocketService>();
    _initializeWebSocket();
  }

  void _initializeWebSocket() {
    _webSocketService.initialize();
    
    // Listen to connection status
    _webSocketService.connected$.listen((connected) {
      setState(() {
        _isConnected = connected;
      });
    });

    // Listen to incoming messages
    _webSocketService.messages.listen((message) {
      _handleMessage(message);
    });
  }

  void _handleMessage(WebSocketMessage message) {
    switch (message.action) {
      case WebSocketActions.Connect:
        print('Connected to WebSocket');
        break;
      case WebSocketActions.Disconnect:
        print('Disconnected from WebSocket');
        break;
      case WebSocketActions.UserProfileGet:
        print('Received user profile: ${message.message}');
        break;
      default:
        print('Received message: ${message.action} - ${message.message}');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('WebSocket Example'),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text('Connection Status: ${_isConnected ? "Connected" : "Disconnected"}'),
            ElevatedButton(
              onPressed: () {
                _webSocketService.postMessage(
                  WebSocketMessage(
                    action: WebSocketActions.UserProfileGet,
                    message: {'userId': '123'},
                  ).toJson().toString(),
                );
              },
              child: Text('Request User Profile'),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _webSocketService.dispose();
    super.dispose();
  }
}
```

### Game-Specific WebSocket Usage

```dart
import 'package:websocket/websocket.dart';
import 'package:get_it/get_it.dart';

class X01GameService {
  final WebSocketService _webSocketService = GetIt.I<WebSocketService>();

  void joinX01Queue() {
    _webSocketService.postMessage(
      WebSocketMessage(
        action: WebSocketActions.X01JoinQueue,
        message: {
          'gameType': 'X01',
          'target': 501,
          'playerId': 'player123',
        },
      ).toJson().toString(),
    );
  }

  void joinX01Game(String gameId) {
    _webSocketService.postMessage(
      WebSocketMessage(
        action: WebSocketActions.X01Join,
        message: {
          'gameId': gameId,
          'playerId': 'player123',
        },
      ).toJson().toString(),
    );
  }

  void submitX01Score(String gameId, int score, List<int> darts) {
    _webSocketService.postMessage(
      WebSocketMessage(
        action: WebSocketActions.X01Score,
        message: {
          'gameId': gameId,
          'playerId': 'player123',
          'score': score,
          'darts': darts,
          'timestamp': DateTime.now().toIso8601String(),
        },
      ).toJson().toString(),
    );
  }
}
```

### User Profile Management

```dart
import 'package:websocket/websocket.dart';
import 'package:get_it/get_it.dart';

class UserProfileService {
  final WebSocketService _webSocketService = GetIt.I<WebSocketService>();

  void createUserProfile(Map<String, dynamic> profile) {
    _webSocketService.postMessage(
      WebSocketMessage(
        action: WebSocketActions.UserProfileCreate,
        message: profile,
      ).toJson().toString(),
    );
  }

  void updateUserProfile(String userId, Map<String, dynamic> updates) {
    _webSocketService.postMessage(
      WebSocketMessage(
        action: WebSocketActions.UserProfileUpdate,
        message: {
          'userId': userId,
          'updates': updates,
        },
      ).toJson().toString(),
    );
  }

  void getUserProfile(String userId) {
    _webSocketService.postMessage(
      WebSocketMessage(
        action: WebSocketActions.UserProfileGet,
        message: {'userId': userId},
      ).toJson().toString(),
    );
  }
}
```

### Custom Message Handling

```dart
import 'package:websocket/websocket.dart';
import 'package:get_it/get_it.dart';

class CustomWebSocketHandler {
  final WebSocketService _webSocketService = GetIt.I<WebSocketService>();

  void initialize() {
    _webSocketService.messages.listen((message) {
      _handleCustomMessage(message);
    });
  }

  void _handleCustomMessage(WebSocketMessage message) {
    switch (message.action) {
      case WebSocketActions.Connect:
        _handleConnect(message);
        break;
      case WebSocketActions.Disconnect:
        _handleDisconnect(message);
        break;
      case WebSocketActions.Default:
        _handleDefault(message);
        break;
      default:
        _handleUnknownAction(message);
    }
  }

  void _handleConnect(WebSocketMessage message) {
    print('WebSocket connected successfully');
    // Handle connection established
  }

  void _handleDisconnect(WebSocketMessage message) {
    print('WebSocket disconnected');
    // Handle disconnection
  }

  void _handleDefault(WebSocketMessage message) {
    print('Default message received: ${message.message}');
    // Handle default messages
  }

  void _handleUnknownAction(WebSocketMessage message) {
    print('Unknown action: ${message.action}');
    // Handle unknown actions
  }

  void sendCustomMessage(String action, dynamic data) {
    _webSocketService.postMessage(
      WebSocketMessage(
        action: action,
        message: data,
      ).toJson().toString(),
    );
  }
}
```

### Reactive State Management

```dart
import 'package:websocket/websocket.dart';
import 'package:get_it/get_it.dart';
import 'package:rxdart/rxdart.dart';

class WebSocketStateManager {
  final WebSocketService _webSocketService = GetIt.I<WebSocketService>();
  
  // Reactive streams for different message types
  late Stream<WebSocketMessage> _userProfileMessages;
  late Stream<WebSocketMessage> _gameMessages;
  late Stream<WebSocketMessage> _systemMessages;

  void initialize() {
    // Filter messages by action type
    _userProfileMessages = _webSocketService.messages
        .where((message) => message.action.startsWith('user/'));

    _gameMessages = _webSocketService.messages
        .where((message) => message.action.startsWith('games/'));

    _systemMessages = _webSocketService.messages
        .where((message) => message.action.endsWith('\$'));

    // Listen to connection status
    _webSocketService.connected$.listen((connected) {
      if (connected) {
        print('WebSocket connected - ready for communication');
      } else {
        print('WebSocket disconnected - attempting reconnection');
      }
    });

    // Listen to user profile messages
    _userProfileMessages.listen((message) {
      print('User profile message: ${message.action}');
    });

    // Listen to game messages
    _gameMessages.listen((message) {
      print('Game message: ${message.action}');
    });

    // Listen to system messages
    _systemMessages.listen((message) {
      print('System message: ${message.action}');
    });
  }

  Stream<WebSocketMessage> get userProfileMessages => _userProfileMessages;
  Stream<WebSocketMessage> get gameMessages => _gameMessages;
  Stream<WebSocketMessage> get systemMessages => _systemMessages;
  Stream<bool> get connectionStatus => _webSocketService.connected$;
}
```

## API Reference

### Core Components

#### WebSocketService
The main WebSocket service class that manages connections and message handling.

```dart
class WebSocketService {
  IOWebSocketChannel? _socket;
  final BehaviorSubject<bool> _connectedSubject;
  final StreamController<WebSocketMessage<dynamic>> _messages;

  WebSocketService(String websocketUri);
  
  Stream<bool> get connected$;
  Stream<WebSocketMessage> get messages;
  
  void initialize();
  void postMessage(String payload);
  void dispose();
}
```

#### WebSocketMessage<T>
A generic message class for structured WebSocket communication.

```dart
class WebSocketMessage<T> {
  String action;
  T? message;

  WebSocketMessage({required this.action, required this.message});

  factory WebSocketMessage.fromJson(Map<String, dynamic> json);
  Map<String, dynamic> toJson();
}
```

#### WebSocketActions
A static class containing predefined action constants.

```dart
class WebSocketActions {
  static const Connect = "connect\$";
  static const Disconnect = "disconnect\$";
  static const Default = "default\$";
  static const UserProfileCreate = "user/profile/create";
  static const UserProfileUpdate = "user/profile/update";
  static const UserProfileGet = "user/profile/get";
  static const X01JoinQueue = "games/x01/joinqueue";
  static const X01Score = "games/x01/score";
  static const X01Join = "games/x01/join";
}
```

#### WebSocketRequest
Abstract base class for WebSocket request types.

```dart
abstract class WebSocketRequest {}

class MessageRequest extends WebSocketRequest {
  late DateTime Date;
  late String Message;
  late String Owner;
}
```

#### WebSocketStatus
Enumeration for WebSocket connection status.

```dart
enum WebSocketStatus {
  Unknown,
  Disconnected,
  Connected,
}
```

### Methods

#### WebSocketService Methods
- **`initialize()`**: Initialize the WebSocket connection with authentication
- **`postMessage(String payload)`**: Send a message to the WebSocket server
- **`dispose()`**: Clean up resources and close connections

#### WebSocketMessage Methods
- **`fromJson(Map<String, dynamic> json)`**: Create message from JSON
- **`toJson()`**: Convert message to JSON format

### Properties

#### WebSocketService Properties
- **`connected$`**: Stream of connection status (bool)
- **`messages`**: Stream of incoming WebSocket messages

#### WebSocketMessage Properties
- **`action`**: The action type for the message
- **`message`**: The message payload (generic type T)

### Behavior

#### Authentication
The service automatically handles authentication using Authress tokens:

```dart
if (await _authressLoginClient.ensureToken() == null) {
  return;
}
_socket = IOWebSocketChannel.connect('${_websocketUri}?token=${token}');
```

#### Automatic Reconnection
The service automatically reconnects when the connection is lost:

```dart
onDone: () {
  _connectedSubject.add(false);
  _messages.add(WebSocketMessage(action: WebSocketActions.Disconnect, message: null));
  Future.delayed(Duration(seconds: 1), () {
    initialize();
  });
}
```

#### Error Handling
Comprehensive error handling with automatic recovery:

```dart
onError: (error) {
  _messages.add(WebSocketMessage(action: WebSocketActions.Default, message: error));
}
```

## Configuration

### WebSocket URI Configuration
Configure the WebSocket service with the appropriate URI:

```dart
final webSocketService = WebSocketService('wss://api.flyingdarts.com/websocket');
```

### Authentication Configuration
The service automatically integrates with Authress for authentication:

```dart
final AuthressLoginClient _authressLoginClient = GetIt.I<AuthressLoginClient>();
```

### Dependency Injection Setup
Configure GetIt for dependency injection:

```dart
import 'package:get_it/get_it.dart';
import 'package:websocket/websocket.dart';

void setupDependencies() {
  GetIt.I.registerSingleton<WebSocketService>(
    WebSocketService('wss://api.flyingdarts.com/websocket'),
  );
}
```

### Error Handling Configuration
Implement custom error handling:

```dart
_webSocketService.messages.listen(
  (message) {
    if (message.action == WebSocketActions.Default) {
      // Handle error messages
      print('Error: ${message.message}');
    }
  },
  onError: (error) {
    // Handle stream errors
    print('Stream error: $error');
  },
);
```

## Development

### Project Structure
```
websocket/
├── lib/
│   ├── websocket.dart                    # Main library export
│   └── src/                             # Source files
│       ├── websocket_service.dart       # Main WebSocket service
│       ├── websocket_actions.dart       # Action constants
│       └── models/                      # Data models
│           ├── websocket_message.dart   # Message model
│           ├── websocket_request.dart   # Request model
│           └── websocket_status.dart    # Status enum
├── test/                                # Unit tests
│   └── models/                          # Model tests
│       └── websocket_message_test.dart  # Message tests
├── pubspec.yaml                         # Package dependencies
└── package.json                         # Build scripts
```

### Architecture Patterns
- **Service Pattern**: WebSocket service for connection management
- **Reactive Pattern**: RxDart integration for reactive programming
- **Dependency Injection**: GetIt for service management
- **Factory Pattern**: JSON factory methods for message creation
- **Observer Pattern**: Stream-based message handling
- **Singleton Pattern**: Service registration with GetIt

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
- Test WebSocket connections thoroughly
- Verify authentication integration
- Test reconnection logic
- Mock external dependencies

## Dependencies

### Runtime Dependencies
- **flutter**: Flutter SDK
- **authress_login**: Authentication package
- **rxdart**: ^0.28.0 - Reactive programming
- **web_socket_channel**: ^3.0.3 - WebSocket communication
- **injectable**: ^2.5.0 - Dependency injection
- **json_annotation**: ^4.9.0 - JSON serialization
- **get_it**: ^8.0.3 - Service locator

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints**: ^6.0.0 - Linting rules
- **json_serializable**: ^6.9.5 - JSON code generation
- **mockito**: ^5.4.6 - Mocking framework
- **injectable_generator**: ^2.7.0 - Injectable code generation
- **build_runner**: ^2.5.4 - Code generation tool

## Related Projects

### Frontend Applications
- **[Flutter Mobile App](../../../../flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../../angular/fd-app/)**: Web application

### Shared Packages
- **[Authress Login](../../../authress/login/)**: Authentication package
- **[UI Package](../../../ui/)**: Shared UI components
- **[Internationalization](../../../internationalization/)**: Multi-language support
- **[Language Feature](../../../features/language/)**: Language management
- **[Profile Feature](../../../features/profile/)**: User profile management
- **[App Config Core](../../../config/app_config_core/)**: Configuration core
- **[App Config Prefs](../../../config/app_config_prefs/)**: SharedPreferences configuration
- **[App Config Secrets](../../../config/app_config_secrets/)**: Secure configuration storage

### Backend Services
- **[Friends API](../../../../../backend/dotnet/friends/)**: Friend management
- **[Games API](../../../../../backend/dotnet/games/)**: Game management
- **[Auth API](../../../../../backend/dotnet/auth/)**: Authentication
- **[Notify Rooms Service](../../../../../backend/dotnet/notifyrooms/)**: Real-time notifications

## Version History

- **v0.0.7** (2025-07-26): Implemented friends feature
- **v0.0.6** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.5** (2025-07-10): Add flutter workspace at root
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution
- **v0.0.2** (2025-07-07): Initial change log

## Troubleshooting

### Common Issues

1. **WebSocket connection not establishing**
   - Check WebSocket URI is correct and accessible
   - Verify authentication token is valid
   - Ensure network connectivity
   - Check server is running and accepting connections

2. **Authentication failures**
   - Verify Authress configuration is correct
   - Check token expiration and refresh logic
   - Ensure AuthressLoginClient is properly initialized
   - Verify token format in WebSocket URL

3. **Message serialization errors**
   - Check JSON format is valid
   - Verify message structure matches expected format
   - Ensure all required fields are provided
   - Test message serialization/deserialization

4. **Connection drops and reconnection issues**
   - Check network stability
   - Verify reconnection logic is working
   - Monitor connection status stream
   - Test automatic reconnection behavior

5. **Dependency injection issues**
   - Ensure GetIt is properly configured
   - Verify WebSocketService is registered
   - Check AuthressLoginClient registration
   - Test service instantiation

6. **Performance issues**
   - Monitor message frequency and size
   - Check for memory leaks in stream subscriptions
   - Verify proper disposal of resources
   - Test with high message volumes

7. **Platform-specific issues**
   - iOS: Check WebSocket permissions and network security
   - Android: Verify network security configuration
   - Web: Ensure WebSocket is supported in browser

8. **Testing issues**
   - Mock WebSocket connections for unit tests
   - Use Mockito for external dependencies
   - Test error scenarios and edge cases
   - Verify stream behavior in tests

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Test WebSocket connections thoroughly
6. Verify authentication integration works correctly
7. Test reconnection logic and error handling
8. Ensure proper error handling
9. Test with different message types and payloads
10. Verify cross-platform compatibility
11. Test performance with high message volumes
12. Ensure proper resource cleanup

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.