import 'package:flutter/foundation.dart';

import '../models/speech_config.dart';
import '../models/speech_recognition_result.dart';

/// Abstract interface for speech recognition service
abstract class SpeechRecognitionService {
  /// Initialize the speech recognition service
  Future<bool> initialize();

  /// Check if speech recognition is available
  Future<bool> isAvailable();

  /// Start listening for speech input
  Future<bool> startListening({
    required SpeechConfig config,
    required Function(SpeechRecognitionResult) onResult,
    required Function(String) onError,
    required VoidCallback onStatusChanged,
  });

  /// Stop listening for speech input
  Future<void> stopListening();

  /// Cancel the current listening session
  Future<void> cancel();

  /// Get available locales
  Future<List<String>> getAvailableLocales();

  /// Check if the service is currently listening
  bool get isListening;

  /// Dispose of resources
  Future<void> dispose();
}
