import 'package:configuration/configuration.dart';
import 'package:equatable/equatable.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:speech_to_text/speech_recognition_result.dart';
import 'package:speech_to_text/speech_to_text.dart';

part 'speech_state.dart';
part 'speech_event.dart';

/// Handles all logic related to speech recognition feature.
class SpeechBloc extends Bloc<SpeechEvent, SpeechState> {
  SpeechBloc() : super(const SpeechState(isListening: false, lastEntry: "", enabled: true)) {
    on<SpeechButtonLongPressed>(_onSpeechButtonLongPressed);
    on<SpeechButtonLongPressEnded>(_onSpeechButtonLongPressEnded);
    on<SpeechButtonLongPressCancelled>(_onSpeechButtonLongPressCancelled);
    on<SpeechStartListeningEvent>(_onSpeechStartListening);
    on<SpeechStopListeningEvent>(_onSpeechStopListening);
    on<SpeechResultFoundEvent>(_onSpeechResultFound);
  }
  Future<void> init() async {
    _speechToText = SpeechToText();
  }

  late SpeechToText _speechToText;

  Future<void> _onSpeechStartListening(
    SpeechStartListeningEvent event,
    Emitter<SpeechState> emit,
  ) async {
    var available = await _speechToText.initialize();
    if (available) {
      var bloc = this;
      _speechToText.listen(
        localeId: GetIt.I<WriteableConfiguration<LanguageConfig>>().config.preferedLocaleId,
        onResult: (SpeechRecognitionResult result) {
          // Handle the speech recognition result
          debugPrint(result.recognizedWords);
          if (result.finalResult || result.hasConfidenceRating) {
            bloc.add(SpeechResultFoundEvent(result.recognizedWords));
          }
        },
      );
    }
  }

  Future<void> _onSpeechStopListening(
    SpeechStopListeningEvent event,
    Emitter<SpeechState> emit,
  ) async {
    if (state.isListening) {
      _speechToText.stop();

      emit(state.copyWith(
        isListening: false,
      ));
    }
  }

  Future<void> _onSpeechResultFound(
    SpeechResultFoundEvent event,
    Emitter<SpeechState> emit,
  ) async {
    final result = event.result;

    if (_isValidEntry(result)) {
      emit(state.copyWith(lastEntry: result, error: 'Great success!'));
    } else {
      emit(state.copyWith(lastEntry: '', error: 'Please try again!'));
    }
  }

  Future<void> _onSpeechButtonLongPressed(
    SpeechButtonLongPressed event,
    Emitter<SpeechState> emit,
  ) async {
    add(const SpeechStartListeningEvent());

    const isListening = true;

    emit(state.copyWith(
      isListening: isListening,
    ));
  }

  Future<void> _onSpeechButtonLongPressEnded(
    SpeechButtonLongPressEnded event,
    Emitter<SpeechState> emit,
  ) async {
    add(const SpeechStopListeningEvent());
  }

  Future<void> _onSpeechButtonLongPressCancelled(
    SpeechButtonLongPressCancelled event,
    Emitter<SpeechState> emit,
  ) async {
    add(const SpeechStopListeningEvent());
  }

  bool _isValidEntry(String entry) {
    try {
      double value = double.parse(entry);
      if (value >= 0 && value <= 180) {
        return true;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }
}
