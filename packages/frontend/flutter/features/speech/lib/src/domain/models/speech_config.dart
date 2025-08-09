import 'package:equatable/equatable.dart';

/// Configuration for speech recognition
class SpeechConfig extends Equatable {
  final String localeId;
  final double confidenceThreshold;
  final Duration timeout;
  final bool enablePartialResults;
  final bool enableDebugMode;
  final int maxAlternatives;
  final Duration pauseFor;
  final Duration listenFor;

  const SpeechConfig({
    required this.localeId,
    this.confidenceThreshold = 0.3, // Lower threshold for better recognition
    this.timeout = const Duration(seconds: 30),
    this.enablePartialResults = true,
    this.enableDebugMode = false,
    this.maxAlternatives = 1,
    this.pauseFor = const Duration(
      seconds: 2,
    ), // Shorter pause for better responsiveness
    this.listenFor = const Duration(
      seconds: 10,
    ), // Shorter listen duration for commands
  });

  /// Creates a default configuration
  factory SpeechConfig.defaultConfig() {
    return const SpeechConfig(
      localeId: 'en_US',
      confidenceThreshold: 0.3, // Lower threshold for better recognition
      timeout: Duration(seconds: 30),
      enablePartialResults: true,
      enableDebugMode: false,
      maxAlternatives: 1,
      pauseFor: Duration(seconds: 2), // Shorter pause for better responsiveness
      listenFor: Duration(seconds: 10), // Shorter listen duration for commands
    );
  }

  /// Creates a configuration with custom locale
  factory SpeechConfig.withLocale(String localeId) {
    return SpeechConfig(
      localeId: localeId,
      confidenceThreshold: 0.3, // Lower threshold for better recognition
      timeout: const Duration(seconds: 30),
      enablePartialResults: true,
      enableDebugMode: false,
      maxAlternatives: 1,
      pauseFor: const Duration(
        seconds: 2,
      ), // Shorter pause for better responsiveness
      listenFor: const Duration(
        seconds: 10,
      ), // Shorter listen duration for commands
    );
  }

  /// Creates a copy with updated values
  SpeechConfig copyWith({
    String? localeId,
    double? confidenceThreshold,
    Duration? timeout,
    bool? enablePartialResults,
    bool? enableDebugMode,
    int? maxAlternatives,
    Duration? pauseFor,
    Duration? listenFor,
  }) {
    return SpeechConfig(
      localeId: localeId ?? this.localeId,
      confidenceThreshold: confidenceThreshold ?? this.confidenceThreshold,
      timeout: timeout ?? this.timeout,
      enablePartialResults: enablePartialResults ?? this.enablePartialResults,
      enableDebugMode: enableDebugMode ?? this.enableDebugMode,
      maxAlternatives: maxAlternatives ?? this.maxAlternatives,
      pauseFor: pauseFor ?? this.pauseFor,
      listenFor: listenFor ?? this.listenFor,
    );
  }

  @override
  List<Object?> get props => [
    localeId,
    confidenceThreshold,
    timeout,
    enablePartialResults,
    enableDebugMode,
    maxAlternatives,
    pauseFor,
    listenFor,
  ];

  @override
  String toString() {
    return 'SpeechConfig(localeId: $localeId, confidenceThreshold: $confidenceThreshold, timeout: $timeout)';
  }
}
