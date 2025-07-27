import 'package:flutter/foundation.dart';
import 'package:injectable/injectable.dart';
import 'package:speech_to_text/speech_recognition_result.dart' as stt;
import 'package:speech_to_text/speech_to_text.dart';

import '../../domain/interfaces/speech_recognition_service.dart';
import '../../domain/models/speech_config.dart';
import '../../domain/models/speech_recognition_result.dart';

/// Concrete implementation of SpeechRecognitionService using speech_to_text package
@Injectable(as: SpeechRecognitionService)
class SpeechToTextService implements SpeechRecognitionService {
  SpeechToTextService();

  late final SpeechToText _speechToText;

  bool _isInitialized = false;
  bool _isListening = false;

  @override
  bool get isListening => _isListening;

  @override
  Future<bool> initialize() async {
    final start = DateTime.now();
    debugPrint('[PROFILE] SpeechToTextService.initialize() called at: ${start.toIso8601String()}');
    try {
      // Don't re-initialize if already initialized
      if (_isInitialized) {
        debugPrint('[PROFILE] Already initialized, returning isAvailable: ${_speechToText.isAvailable}');
        return _speechToText.isAvailable;
      }

      _speechToText = SpeechToText();
      final available = await _speechToText.initialize();

      _isInitialized = available;
      debugPrint(
        '[PROFILE] Speech recognition initialized: $available at: ${DateTime.now().toIso8601String()} (duration: ${DateTime.now().difference(start).inMilliseconds}ms)',
      );
      return available;
    } catch (e) {
      debugPrint('Failed to initialize speech recognition: $e');
      _isInitialized = false;
      return false;
    }
  }

  @override
  Future<bool> isAvailable() async {
    if (!_isInitialized) {
      return await initialize();
    }
    return _speechToText.isAvailable;
  }

  @override
  Future<bool> startListening({
    required SpeechConfig config,
    required Function(SpeechRecognitionResult) onResult,
    required Function(String) onError,
    required VoidCallback onStatusChanged,
  }) async {
    final start = DateTime.now();
    debugPrint('[PROFILE] SpeechToTextService.startListening() called at: ${start.toIso8601String()}');
    try {
      if (!_isInitialized) {
        final initialized = await initialize();
        if (!initialized) {
          onError('Speech recognition not available');
          return false;
        }
      }

      if (_isListening) {
        await stopListening();
      }

      final success = await _speechToText.listen(
        localeId: config.localeId,
        listenFor: config.listenFor, // Set maximum listening duration
        pauseFor: config.pauseFor, // Set pause duration
        onResult: (stt.SpeechRecognitionResult result) {
          debugPrint('[PROFILE] onResult callback at: ${DateTime.now().toIso8601String()}');
          _handleResult(result, config, onResult);
        },
        listenOptions: SpeechListenOptions(
          cancelOnError: true,
          listenMode: ListenMode.dictation,
          partialResults: false,
        ),
      );
      debugPrint('[PROFILE] _speechToText.listen() returned: ${success.runtimeType} value: $success');
      // Defensive: treat null as false
      final started = success == true;
      if (started) {
        _isListening = true;
        onStatusChanged();
        debugPrint(
          '[PROFILE] Started listening for speech input at: ${DateTime.now().toIso8601String()} (duration: ${DateTime.now().difference(start).inMilliseconds}ms)',
        );
      } else {
        debugPrint(
          '[PROFILE] Failed to start listening at: ${DateTime.now().toIso8601String()} (duration: ${DateTime.now().difference(start).inMilliseconds}ms)',
        );
      }

      return started;
    } catch (e) {
      debugPrint('Failed to start listening: $e');
      onError('Failed to start speech recognition: $e');
      return false;
    }
  }

  @override
  Future<void> stopListening() async {
    final start = DateTime.now();
    debugPrint('[PROFILE] SpeechToTextService.stopListening() called at: ${start.toIso8601String()}');
    try {
      if (_isListening) {
        await _speechToText.stop();
        _isListening = false;
        debugPrint(
          '[PROFILE] Stopped listening for speech input at: ${DateTime.now().toIso8601String()} (duration: ${DateTime.now().difference(start).inMilliseconds}ms)',
        );
      }
    } catch (e) {
      debugPrint('Failed to stop listening: $e');
    }
  }

  @override
  Future<void> cancel() async {
    final start = DateTime.now();
    debugPrint('[PROFILE] SpeechToTextService.cancel() called at: ${start.toIso8601String()}');
    try {
      if (_isListening) {
        await _speechToText.cancel();
        _isListening = false;
        debugPrint(
          '[PROFILE] Cancelled speech recognition at: ${DateTime.now().toIso8601String()} (duration: ${DateTime.now().difference(start).inMilliseconds}ms)',
        );
      }
    } catch (e) {
      debugPrint('Failed to cancel speech recognition: $e');
    }
  }

  @override
  Future<List<String>> getAvailableLocales() async {
    try {
      if (!_isInitialized) {
        await initialize();
      }
      final locales = await _speechToText.locales();
      return locales.map((locale) => locale.localeId).toList();
    } catch (e) {
      debugPrint('Failed to get available locales: $e');
      return [];
    }
  }

  @override
  Future<void> dispose() async {
    try {
      if (_isListening) {
        await stopListening();
      }
      _isInitialized = false;
      debugPrint('Speech recognition service disposed');
    } catch (e) {
      debugPrint('Failed to dispose speech recognition service: $e');
    }
  }

  void _handleResult(
    stt.SpeechRecognitionResult result,
    SpeechConfig config,
    Function(SpeechRecognitionResult) onResult,
  ) {
    try {
      final confidence = result.confidence;
      final isFinal = result.finalResult;
      final text = result.recognizedWords;

      debugPrint('Speech result: $text (confidence: $confidence, final: $isFinal)');

      // Handle confidence properly - speech_to_text may not provide confidence on all platforms
      double adjustedConfidence = confidence;
      if (confidence == 0.0 && text.isNotEmpty) {
        // If confidence is 0 but we have text, assume a reasonable confidence
        // This is common on iOS and some Android devices
        adjustedConfidence = isFinal ? 0.8 : 0.6;
        debugPrint('Adjusted confidence from $confidence to $adjustedConfidence');
      }

      // For speech_to_text, we want to process:
      // 1. Final results (most reliable)
      // 2. Partial results with good confidence
      // 3. Any result with text (for debugging)
      if (isFinal || adjustedConfidence >= config.confidenceThreshold || text.isNotEmpty) {
        final speechResult = SpeechRecognitionResult.fromSpeechToText(
          text,
          adjustedConfidence,
          isFinal,
          config.localeId,
        );

        onResult(speechResult);
      }

      // speech_to_text automatically stops after final results or timeout
      // We don't need to manually restart listening
      if (isFinal) {
        debugPrint('Final result received, speech_to_text will handle stopping');
      }
    } catch (e) {
      debugPrint('Failed to handle speech result: $e');
    }
  }
}
