import 'package:flutter/foundation.dart';
import 'package:injectable/injectable.dart';

import '../../domain/interfaces/speech_recognition_service.dart';
import '../../domain/interfaces/speech_validation_service.dart';
import '../../domain/models/speech_config.dart';
import '../../domain/models/speech_recognition_result.dart';

/// Repository that coordinates speech recognition and validation
@injectable
class SpeechRepository {
  SpeechRepository({
    required SpeechRecognitionService recognitionService,
    required SpeechValidationService validationService,
  }) : _recognitionService = recognitionService,
       _validationService = validationService;

  final SpeechRecognitionService _recognitionService;
  final SpeechValidationService _validationService;

  /// Initialize the speech recognition system
  Future<bool> initialize() async {
    return await _recognitionService.initialize();
  }

  /// Check if speech recognition is available
  Future<bool> isAvailable() async {
    return await _recognitionService.isAvailable();
  }

  /// Start listening for speech input with validation
  Future<bool> startListening({
    required SpeechConfig config,
    required Function(SpeechRecognitionResult) onValidResult,
    required Function(String) onError,
    required VoidCallback onStatusChanged,
  }) async {
    final result = await _recognitionService.startListening(
      config: config,
      onResult: (result) {
        _handleSpeechResult(result, onValidResult, onError);
      },
      onError: onError,
      onStatusChanged: onStatusChanged,
    );
    return result == true; // Always return a non-null bool
  }

  /// Stop listening for speech input
  Future<void> stopListening() async {
    await _recognitionService.stopListening();
  }

  /// Cancel the current listening session
  Future<void> cancel() async {
    await _recognitionService.cancel();
  }

  /// Get available locales
  Future<List<String>> getAvailableLocales() async {
    return await _recognitionService.getAvailableLocales();
  }

  /// Check if currently listening
  bool get isListening => _recognitionService.isListening;

  /// Dispose of resources
  Future<void> dispose() async {
    await _recognitionService.dispose();
  }

  /// Add a custom validation rule
  void addValidationRule(ValidationRule rule) {
    _validationService.addValidationRule(rule);
  }

  /// Remove a validation rule
  void removeValidationRule(String ruleId) {
    _validationService.removeValidationRule(ruleId);
  }

  /// Get current validation rules
  List<ValidationRule> getValidationRules() {
    return _validationService.getValidationRules();
  }

  void _handleSpeechResult(
    SpeechRecognitionResult result,
    Function(SpeechRecognitionResult) onValidResult,
    Function(String) onError,
  ) {
    debugPrint(
      '[PROFILE] Validation started at: ${DateTime.now().toIso8601String()}',
    );
    final validationResult = _validationService.validate(result);
    debugPrint(
      '[PROFILE] Validation ended at: ${DateTime.now().toIso8601String()} (isValid: ${validationResult.isValid}, error: ${validationResult.errorMessage})',
    );

    if (validationResult.isValid) {
      onValidResult(result);
    } else {
      // Don't trigger an error for validation failures, just log them
      // This allows speech recognition to continue listening
      debugPrint('Validation failed: ${validationResult.errorMessage}');
      // Optionally, you can still call onValidResult with the result
      // even if validation fails, to show partial results
      if (result.text.isNotEmpty) {
        onValidResult(result);
      }
    }
  }
}
