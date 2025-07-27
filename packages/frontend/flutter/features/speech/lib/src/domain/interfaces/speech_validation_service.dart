import '../models/speech_recognition_result.dart';

/// Abstract interface for speech validation service
abstract class SpeechValidationService {
  /// Validate a speech recognition result
  ValidationResult validate(SpeechRecognitionResult result);

  /// Get validation rules
  List<ValidationRule> getValidationRules();

  /// Add a custom validation rule
  void addValidationRule(ValidationRule rule);

  /// Remove a validation rule
  void removeValidationRule(String ruleId);
}

/// Result of validation
class ValidationResult {
  final bool isValid;
  final String? errorMessage;
  final List<ValidationError> errors;

  const ValidationResult({
    required this.isValid,
    this.errorMessage,
    this.errors = const [],
  });

  /// Creates a successful validation result
  factory ValidationResult.success() {
    return const ValidationResult(isValid: true);
  }

  /// Creates a failed validation result
  factory ValidationResult.failure(String message, [List<ValidationError> errors = const []]) {
    return ValidationResult(
      isValid: false,
      errorMessage: message,
      errors: errors,
    );
  }
}

/// Validation error details
class ValidationError {
  final String field;
  final String message;
  final String? code;

  const ValidationError({
    required this.field,
    required this.message,
    this.code,
  });
}

/// Validation rule
abstract class ValidationRule {
  final String id;
  final String name;
  final String description;

  const ValidationRule({
    required this.id,
    required this.name,
    required this.description,
  });

  /// Execute the validation rule
  ValidationResult execute(SpeechRecognitionResult result);
}
