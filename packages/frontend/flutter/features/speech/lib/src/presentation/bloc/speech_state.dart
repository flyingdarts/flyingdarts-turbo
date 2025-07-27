part of 'speech_bloc.dart';

class SpeechState extends Equatable {
  final bool isListening;
  final String lastEntry;
  final bool enabled;
  final String error;

  const SpeechState({
    this.isListening = false,
    this.lastEntry = "",
    this.enabled = true,
    this.error = "",
  });

  SpeechState copyWith({
    bool? isListening,
    String? lastEntry,
    bool? enabled,
    String? error,
  }) {
    return SpeechState(
      isListening: isListening ?? this.isListening,
      lastEntry: lastEntry ?? this.lastEntry,
      enabled: enabled ?? this.enabled,
      error: error ?? this.error,
    );
  }

  @override
  List<Object> get props => [isListening, lastEntry, enabled, error];
}
