import 'package:injectable/injectable.dart';

import '../../domain/interfaces/speech_validation_service.dart';
import '../../domain/models/speech_recognition_result.dart';

/// Concrete implementation of SpeechValidationService
@Injectable(as: SpeechValidationService)
class SpeechValidationServiceImpl implements SpeechValidationService {
  SpeechValidationServiceImpl() {
    _initializeDefaultRules();
  }

  final List<ValidationRule> _validationRules = [];

  @override
  ValidationResult validate(SpeechRecognitionResult result) {
    if (!result.isValid) {
      return ValidationResult.failure('Invalid speech result');
    }

    final errors = <ValidationError>[];

    for (final rule in _validationRules) {
      final ruleResult = rule.execute(result);
      if (!ruleResult.isValid) {
        errors.addAll(ruleResult.errors);
      }
    }

    if (errors.isEmpty) {
      return ValidationResult.success();
    } else {
      return ValidationResult.failure(
        'Validation failed: ${errors.map((e) => e.message).join(', ')}',
        errors,
      );
    }
  }

  @override
  List<ValidationRule> getValidationRules() {
    return List.unmodifiable(_validationRules);
  }

  @override
  void addValidationRule(ValidationRule rule) {
    _validationRules.add(rule);
  }

  @override
  void removeValidationRule(String ruleId) {
    _validationRules.removeWhere((rule) => rule.id == ruleId);
  }

  void _initializeDefaultRules() {
    // Add default validation rules
    addValidationRule(NotEmptyValidationRule());
    addValidationRule(ConfidenceThresholdValidationRule());
  }
}

/// Validation rule to ensure text is not empty
class NotEmptyValidationRule extends ValidationRule {
  NotEmptyValidationRule()
    : super(
        id: 'not_empty',
        name: 'Not Empty',
        description: 'Ensures the speech result is not empty',
      );

  @override
  ValidationResult execute(SpeechRecognitionResult result) {
    if (result.text.isEmpty) {
      return ValidationResult.failure(
        'Speech result cannot be empty',
        [const ValidationError(field: 'text', message: 'Text is empty')],
      );
    }
    return ValidationResult.success();
  }
}

/// Validation rule to ensure confidence meets threshold
class ConfidenceThresholdValidationRule extends ValidationRule {
  static const double _defaultThreshold = 0.9; // Lower threshold for better recognition

  ConfidenceThresholdValidationRule()
    : super(
        id: 'confidence_threshold',
        name: 'Confidence Threshold',
        description: 'Ensures confidence meets minimum threshold',
      );

  @override
  ValidationResult execute(SpeechRecognitionResult result) {
    if (result.confidence < _defaultThreshold) {
      return ValidationResult.failure(
        'Confidence too low',
        [
          ValidationError(
            field: 'confidence',
            message: 'Confidence ${result.confidence} is below threshold $_defaultThreshold',
          ),
        ],
      );
    }
    return ValidationResult.success();
  }
}

/// Validation rule for numeric range (e.g., dart scores 0-180)
class NumericRangeValidationRule extends ValidationRule {
  final double min;
  final double max;

  NumericRangeValidationRule({
    required this.min,
    required this.max,
  }) : super(
         id: 'numeric_range',
         name: 'Numeric Range',
         description: 'Ensures value is within numeric range $min-$max',
       );

  @override
  ValidationResult execute(SpeechRecognitionResult result) {
    try {
      final value = double.parse(result.text);
      if (value < min || value > max) {
        return ValidationResult.failure(
          'Value out of range',
          [
            ValidationError(
              field: 'value',
              message: 'Value $value is outside range $min-$max',
            ),
          ],
        );
      }
      return ValidationResult.success();
    } catch (e) {
      return ValidationResult.failure(
        'Invalid numeric value',
        [
          ValidationError(
            field: 'value',
            message: 'Could not parse "${result.text}" as a number',
          ),
        ],
      );
    }
  }
}

/// Validation rule for dart scores (0-180)
class DartScoreValidationRule extends ValidationRule {
  DartScoreValidationRule()
    : super(
        id: 'dart_score',
        name: 'Dart Score',
        description: 'Validates dart scores between 0 and 180',
      );

  @override
  ValidationResult execute(SpeechRecognitionResult result) {
    final rangeRule = NumericRangeValidationRule(min: 0, max: 180);
    return rangeRule.execute(result);
  }
}
