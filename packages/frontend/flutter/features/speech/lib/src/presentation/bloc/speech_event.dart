part of 'speech_bloc.dart';

abstract class SpeechEvent extends Equatable {
  const SpeechEvent();

  @override
  List<Object?> get props => [];
}

class SpeechButtonLongPressed extends SpeechEvent {
  const SpeechButtonLongPressed();
}

class SpeechButtonLongPressEnded extends SpeechEvent {
  const SpeechButtonLongPressEnded();
}

class SpeechButtonLongPressCancelled extends SpeechEvent {
  const SpeechButtonLongPressCancelled();
}
