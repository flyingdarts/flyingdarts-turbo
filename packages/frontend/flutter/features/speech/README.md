# Speech Package

A production-ready speech recognition package with scalable architecture, comprehensive error handling, and extensive testing. This package is designed as a micro-package that integrates seamlessly with the main app's dependency injection system.

## Features

- **Clean Architecture**: Domain-driven design with clear separation of concerns
- **Micro-Package Architecture**: Seamless integration with main app's DI system
- **Dependency Injection**: Proper DI setup using injectable for testability and maintainability
- **Error Handling**: Comprehensive error handling and user feedback
- **Accessibility**: Full accessibility support with semantic labels
- **Internationalization**: Multi-language support
- **Testing**: Extensive unit and integration tests
- **Configuration**: Flexible configuration management
- **Validation**: Pluggable validation rules
- **Lifecycle Management**: Proper app lifecycle handling

## Architecture

The package follows clean architecture principles with the following layers:

### Domain Layer
- **Models**: `SpeechConfig`, `SpeechRecognitionResult`
- **Interfaces**: `SpeechRecognitionService`, `SpeechValidationService`

### Data Layer
- **Services**: `SpeechToTextService`, `SpeechValidationServiceImpl`
- **Repository**: `SpeechRepository`

### Presentation Layer
- **BLoC**: `SpeechBloc` with proper state management
- **Widgets**: `SpeechRecognitionWidget`, `SpeechPage`
- **Micro-Package Module**: `SpeechPackageModule`

## Micro-Package Setup

### 1. Add to Main App's DI Configuration

In your main app's `di.dart` file:

```dart
import 'package:speech/speech.module.dart';

@InjectableInit(
  includeMicroPackages: true,
  externalPackageModulesBefore: [ExternalModule(CorePackageModule)],
  externalPackageModulesAfter: [
    ExternalModule(LanguagePackageModule), 
    ExternalModule(ProfilePackageModule),
    ExternalModule(SpeechPackageModule), // Add this line
  ],
)
Future<void> setupDependencies(String flavor) async {
  getIt.init(environment: flavor);
}
```

### 2. Usage in Your App

```dart
import 'package:speech/speech.dart';
import 'package:get_it/get_it.dart';

// Get the SpeechBloc from the DI container
final speechBloc = GetIt.instance<SpeechBloc>();

// Use in your widget
BlocProvider(
  create: (context) => GetIt.instance<SpeechBloc>(),
  child: SpeechPage(
    onResult: (text) => print('Recognized: $text'),
    onError: (error) => print('Error: $error'),
  ),
)
```

## Basic Usage

### Simple Integration

```dart
import 'package:speech/speech.dart';

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => GetIt.instance<SpeechBloc>(),
      child: SpeechPage(
        onResult: (text) {
          print('Recognized: $text');
        },
        onError: (error) {
          print('Error: $error');
        },
      ),
    );
  }
}
```

### Custom Configuration

```dart
// Create custom configuration
final config = SpeechConfig(
  localeId: 'nl_NL',
  confidenceThreshold: 0.8,
  timeout: Duration(seconds: 45),
);

// Update the configuration in the bloc
final bloc = GetIt.instance<SpeechBloc>();
bloc.add(SpeechUpdateConfig(config));
```

### Custom Validation Rules

```dart
// Create custom validation rule
class CustomValidationRule extends ValidationRule {
  CustomValidationRule() : super(
    id: 'custom',
    name: 'Custom Rule',
    description: 'Custom validation logic',
  );

  @override
  ValidationResult execute(SpeechRecognitionResult result) {
    // Your validation logic here
    if (result.text.contains('custom')) {
      return ValidationResult.success();
    }
    return ValidationResult.failure('Custom validation failed');
  }
}

// Add to repository
final repository = GetIt.instance<SpeechRepository>();
repository.addValidationRule(CustomValidationRule());
```

## Configuration

### SpeechConfig Options

- `localeId`: Speech recognition locale (e.g., 'en_US', 'nl_NL')
- `confidenceThreshold`: Minimum confidence score (0.0 - 1.0)
- `timeout`: Maximum listening duration
- `enablePartialResults`: Whether to show partial results
- `enableDebugMode`: Enable debug logging
- `maxAlternatives`: Number of alternative results
- `pauseFor`: Pause duration before stopping
- `listenFor`: Maximum listening duration

### Default Configuration

```dart
SpeechConfig.defaultConfig() // Uses en_US locale with 0.7 confidence
SpeechConfig.withLocale('nl_NL') // Custom locale with default settings
```

## Validation

The package includes several built-in validation rules:

- **NotEmptyValidationRule**: Ensures text is not empty
- **ConfidenceThresholdValidationRule**: Ensures confidence meets threshold
- **NumericRangeValidationRule**: Validates numeric ranges
- **DartScoreValidationRule**: Validates dart scores (0-180)

### Adding Custom Validation

```dart
class MyValidationRule extends ValidationRule {
  MyValidationRule() : super(
    id: 'my_rule',
    name: 'My Rule',
    description: 'Custom validation',
  );

  @override
  ValidationResult execute(SpeechRecognitionResult result) {
    // Your validation logic
    return ValidationResult.success();
  }
}
```

## Testing

### Unit Tests

```bash
flutter test test/unit/
```

### Integration Tests

```bash
flutter test test/integration/
```

### Widget Tests

```bash
flutter test test/widget/
```

## Error Handling

The package provides comprehensive error handling:

- **Initialization Errors**: Speech recognition not available
- **Permission Errors**: Microphone access denied
- **Network Errors**: Connection issues
- **Validation Errors**: Invalid speech results
- **Timeout Errors**: Listening timeout

## Accessibility

The package includes full accessibility support:

- **Semantic Labels**: Proper labels for screen readers
- **Hints**: Helpful hints for users
- **Status Updates**: Real-time status information
- **Keyboard Navigation**: Full keyboard support

## Performance

- **Lazy Loading**: Services are initialized on demand
- **Resource Management**: Proper disposal of resources
- **Memory Management**: Efficient memory usage
- **Background Handling**: Proper app lifecycle management

## Recent Fixes

### Speech Recognition Behavior (Based on speech_to_text Package)
- **Issue**: Speech recognition stopped working after detecting the first number
- **Root Cause**: The [speech_to_text package](https://github.com/csdcorp/speech_to_text) is designed for **short phrases and commands**, not continuous listening
- **Fix**: Adjusted to work with the package's intended usage pattern
- **Fix**: Updated validation to be more permissive (lowered confidence threshold from 0.7 to 0.3)
- **Fix**: Added proper timeout configuration (`listenFor: 10s`, `pauseFor: 2s`)
- **Fix**: Improved confidence handling for platforms that don't provide confidence scores

### Long Press Control
- **Issue**: Speech service wasn't properly controlled by long press gestures
- **Fix**: Enhanced gesture detection to properly start/stop speech recognition
- **Fix**: Added debug logging to track button press events
- **Fix**: Improved state management to work with speech_to_text's automatic stopping

### Debug Tools
- Added `SpeechDebugWidget` for troubleshooting speech recognition issues
- Enhanced logging throughout the speech recognition pipeline
- Created test example (`speech_test_example.dart`) to demonstrate fixes

### Key Insights from speech_to_text Package
- **Target Use Case**: Commands and short phrases, not continuous spoken conversion
- **Confidence**: May not be provided on all platforms (iOS, some Android devices)
- **Auto-Stop**: Automatically stops after final results or timeout
- **Duration**: Designed for short listening sessions (10-30 seconds max)

## Migration from Legacy

The package maintains backward compatibility while providing new features:

```dart
// Old way (still supported)
final bloc = SpeechBloc()..init();

// New way (recommended) - Micro-package approach
final bloc = GetIt.instance<SpeechBloc>();
```

## Dependencies

- `flutter_bloc`: State management
- `get_it`: Dependency injection
- `injectable`: Micro-package support
- `equatable`: Value equality
- `speech_to_text`: Speech recognition
- `language`: Language support
- `ui`: UI components

## Contributing

1. Follow the clean architecture principles
2. Add tests for new features
3. Update documentation
4. Follow the existing code style
5. Ensure accessibility compliance
6. Maintain micro-package compatibility

## License

This package is part of the Flyingdarts project and follows the project's licensing terms. 