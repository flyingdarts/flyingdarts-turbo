part of 'speech_bloc.dart';

abstract class SpeechEvent extends Equatable {
  const SpeechEvent();

  @override
  List<Object> get props => [];
}

class SpeechButtonLongPressed extends SpeechEvent {
  const SpeechButtonLongPressed() : super();
}

class SpeechButtonLongPressEnded extends SpeechEvent {
  const SpeechButtonLongPressEnded() : super();
}

class SpeechButtonLongPressCancelled extends SpeechEvent {
  const SpeechButtonLongPressCancelled() : super();
}

class SpeechResultFoundEvent extends SpeechEvent {
  const SpeechResultFoundEvent(this.result);

  final String result;

  @override
  List<Object> get props => [result];
}

class SpeechStartListeningEvent extends SpeechEvent {
  const SpeechStartListeningEvent() : super();
}

class SpeechStopListeningEvent extends SpeechEvent {
  const SpeechStopListeningEvent() : super();
}
