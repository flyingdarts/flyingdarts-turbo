import 'package:equatable/equatable.dart';

/// Domain model representing a speech recognition result
class SpeechRecognitionResult extends Equatable {
  final String text;
  final double confidence;
  final bool isFinal;
  final DateTime timestamp;
  final String locale;

  const SpeechRecognitionResult({
    required this.text,
    required this.confidence,
    required this.isFinal,
    required this.timestamp,
    required this.locale,
  });

  /// Creates a SpeechRecognitionResult from speech_to_text package result
  factory SpeechRecognitionResult.fromSpeechToText(
    String text,
    double confidence,
    bool isFinal,
    String locale,
  ) {
    return SpeechRecognitionResult(
      text: text.trim(),
      confidence: confidence,
      isFinal: isFinal,
      timestamp: DateTime.now(),
      locale: locale,
    );
  }

  /// Creates an empty result
  factory SpeechRecognitionResult.empty() {
    return SpeechRecognitionResult(
      text: '',
      confidence: 0.0,
      isFinal: false,
      timestamp: DateTime.now(),
      locale: '',
    );
  }

  /// Returns true if the result is valid (has text and is final)
  bool get isValid => text.isNotEmpty && isFinal;

  /// Returns true if the confidence is above the threshold
  bool isConfident(double threshold) => confidence >= threshold;

  @override
  List<Object?> get props => [text, confidence, isFinal, timestamp, locale];

  @override
  String toString() {
    return 'SpeechRecognitionResult(text: $text, confidence: $confidence, isFinal: $isFinal, locale: $locale)';
  }
}
