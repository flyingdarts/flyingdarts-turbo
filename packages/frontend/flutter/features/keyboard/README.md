# Keyboard Feature Package

## Overview

The Keyboard Feature Package is a Flutter library that provides a comprehensive darts scoring keyboard interface for the Flyingdarts Turbo platform. This package implements a specialized numeric keypad with darts-specific functionality, including score shortcuts, validation, and state management.

The package is designed to:
- Provide a darts-specific scoring keyboard interface
- Handle numeric input with validation (0-180 range)
- Support quick score shortcuts for common darts values
- Manage keyboard state and input validation
- Integrate with BLoC pattern for state management
- Support Widgetbook for component documentation and testing

## Features

- **Darts Scoring Keyboard**: Specialized numeric keypad for darts scoring
- **Score Shortcuts**: Quick access to common darts scores (26, 41, 45, 60, 85, 100)
- **Input Validation**: Prevents invalid scores (maximum 180)
- **State Management**: BLoC/Cubit pattern for reactive state management
- **Function Buttons**: Clear, Check, No Score, and OK functionality
- **Responsive Design**: Flexible layout that adapts to different screen sizes
- **Widgetbook Integration**: Component documentation and testing support
- **Dependency Injection**: Injectable support for easy integration

## Prerequisites

- Flutter SDK 3.26.0 or higher
- Dart SDK 3.8.1 or higher
- flutter_bloc for state management
- widgetbook for component documentation

## Installation

Add the package to your `pubspec.yaml`:

```yaml
dependencies:
  keyboard:
    path: ../../packages/frontend/flutter/features/keyboard
```

Or use it as a workspace dependency:

```yaml
dependencies:
  keyboard:
    workspace: true
```

## Usage

### Basic Integration

```dart
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:keyboard/keyboard.dart';

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: BlocProvider(
        create: (context) => KeyboardCubit(),
        child: KeyboardPage(),
      ),
    );
  }
}
```

### Using the Keyboard in Your App

```dart
import 'package:keyboard/keyboard.dart';

class ScoringScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Darts Scoring')),
      body: Column(
        children: [
          // Your game UI here
          Expanded(
            child: KeyboardPage(),
          ),
        ],
      ),
    );
  }
}
```

### Custom Keyboard Integration

```dart
import 'package:keyboard/keyboard.dart';

class CustomScoringWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocBuilder<KeyboardCubit, KeyboardState>(
      builder: (context, state) {
        return Column(
          children: [
            // Display current input
            Text(
              'Current Score: ${state.lastInput}',
              style: TextStyle(fontSize: 24),
            ),
            
            // Custom action buttons
            Row(
              children: [
                ElevatedButton(
                  onPressed: state.shouldDisableFunctions 
                    ? null 
                    : () => context.read<KeyboardCubit>().clear(),
                  child: Text('Clear'),
                ),
                ElevatedButton(
                  onPressed: state.shouldDisableFunctions 
                    ? null 
                    : () => context.read<KeyboardCubit>().check(),
                  child: Text('Check'),
                ),
              ],
            ),
            
            // Include the keyboard
            KeyboardPage(),
          ],
        );
      },
    );
  }
}
```

### Listening to Keyboard Events

```dart
class ScoreListener extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocListener<KeyboardCubit, KeyboardState>(
      listener: (context, state) {
        // React to keyboard state changes
        if (state.lastInput > 0) {
          print('Score entered: ${state.lastInput}');
        }
        
        if (state.shouldDisableFunctions) {
          // Handle function button press
          print('Function button pressed');
        }
      },
      child: KeyboardPage(),
    );
  }
}
```

## API Reference

### Pages

#### KeyboardPage

The main keyboard interface widget.

**Properties:**
- `key` (Key?): Widget key for identification

**Usage:**
```dart
KeyboardPage()
```

### State Management

#### KeyboardCubit

Manages the keyboard state and provides methods for user interactions.

**Methods:**

##### setShortcut(int value)
Sets a quick score shortcut value.

**Parameters:**
- `value` (int): The shortcut score value

**Usage:**
```dart
keyboardCubit.setShortcut(26);
```

##### setDigit(int digit)
Adds a digit to the current input.

**Parameters:**
- `digit` (int): The digit to add (0-9)

**Usage:**
```dart
keyboardCubit.setDigit(5);
```

##### clear()
Clears the current input and resets the keyboard state.

**Usage:**
```dart
keyboardCubit.clear();
```

##### check()
Processes the current input as a "check" action.

**Usage:**
```dart
keyboardCubit.check();
```

##### noScore()
Processes the current input as a "no score" action.

**Usage:**
```dart
keyboardCubit.noScore();
```

##### ok()
Processes the current input as an "OK" action.

**Usage:**
```dart
keyboardCubit.ok();
```

#### KeyboardState

Represents the current state of the keyboard.

**Properties:**
- `lastInput` (int): The current numeric input value
- `shouldDisableNumpad` (bool): Whether the numpad should be disabled
- `shouldDisableShortcuts` (bool): Whether shortcut buttons should be disabled
- `shouldDisableFunctions` (bool): Whether function buttons should be disabled

**Methods:**

##### copyWith({...})
Creates a copy of the state with optional parameter updates.

**Parameters:**
- `lastInput` (int?): New input value
- `shouldDisableNumpad` (bool?): New numpad disable state
- `shouldDisableShortcuts` (bool?): New shortcuts disable state
- `shouldDisableFunctions` (bool?): New functions disable state

**Returns:**
- `KeyboardState`: New state instance

### Widgets

#### KeyboardButton

A customizable button widget for the keyboard.

**Properties:**
- `input` (String): The button text/label
- `onPressed` (VoidCallback?): Callback when button is pressed
- `disabled` (bool): Whether the button is disabled
- `color` (Color?): The button color

**Usage:**
```dart
KeyboardButton(
  input: "5",
  onPressed: () => keyboardCubit.setDigit(5),
  disabled: false,
  color: Colors.blue,
)
```

## Configuration

The package supports the following configuration options:

- **Score Validation**: Maximum score limit of 180
- **Shortcut Values**: Predefined shortcuts for common darts scores
- **State Management**: BLoC/Cubit pattern for reactive updates
- **Widgetbook Integration**: Component documentation and testing

### Score Shortcuts

The keyboard includes the following predefined shortcuts:
- 26 (Double 13)
- 41 (Single 41)
- 45 (Single 45)
- 60 (Single 60)
- 85 (Single 85)
- 100 (Single 100)

## Development

### Building the Package

```bash
flutter build
```

### Running Tests

```bash
flutter test
```

### Code Generation

```bash
flutter packages pub run build_runner build
```

### Widgetbook Development

```bash
flutter packages pub run widgetbook
```

### Code Style

The project follows Flutter coding conventions with:
- BLoC pattern for state management
- Injectable for dependency injection
- Widgetbook for component documentation
- Proper error handling and validation
- Consistent naming conventions

## Dependencies

### Internal Dependencies
- **language**: Language support package
- **ui**: UI components package

### External Dependencies
- **flutter_bloc** (^9.1.1): State management
- **widgetbook** (^3.14.3): Component documentation
- **widgetbook_annotation** (^3.5.0): Widgetbook annotations
- **injectable** (^2.5.0): Dependency injection

### Development Dependencies
- **flutter_test**: Flutter testing framework
- **flutter_lints** (^6.0.0): Code linting
- **build_runner** (^2.5.4): Code generation
- **widgetbook_generator** (^3.13.0): Widgetbook code generation
- **injectable_generator** (^2.7.0): Injectable code generation

## Related Projects

- **language**: Language and localization features
- **ui**: Shared UI components
- **core**: Core infrastructure and configuration
- **profile**: User profile management
- **speech**: Speech recognition features

## Contributing

When contributing to this package:

1. Follow the existing code style and patterns
2. Add documentation for all public APIs
3. Include unit tests for new functionality
4. Update Widgetbook stories for UI components
5. Ensure proper state management with BLoC
6. Test keyboard interactions thoroughly
7. Update this README for any new features or breaking changes

## License

This package is part of the Flyingdarts Turbo monorepo and follows the same licensing terms.
