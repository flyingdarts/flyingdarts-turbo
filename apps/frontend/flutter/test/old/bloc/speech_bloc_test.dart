import 'package:bloc_test/bloc_test.dart';
import 'package:flutter/material.dart';
import 'package:speech/speech.dart';
import 'package:test/test.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  group('Happy flow scenarios: ', () {
    late SpeechBloc speechBloc;

    blocTest<SpeechBloc, SpeechState>(
      'When button is pressed and received an expected result',
      build: (() => speechBloc = SpeechBloc()..init()),
      act: (bloc) => {
        bloc.add(const SpeechButtonLongPressed()),
        bloc.add(const SpeechResultFoundEvent("12")),
        bloc.add(const SpeechButtonLongPressEnded())
      },
      expect: () => [
        const SpeechState(isListening: true),
        const SpeechState(isListening: true, lastEntry: "12", error: 'Great success!'),
        const SpeechState(
          isListening: false,
          lastEntry: "12",
          error: 'Great success!',
        )
      ],
    );

    blocTest<SpeechBloc, SpeechState>(
      'When button is pressed and received an unexpected result',
      build: () => speechBloc = SpeechBloc()..init(),
      act: (bloc) => {
        bloc.add(const SpeechButtonLongPressed()),
        bloc.add(const SpeechResultFoundEvent("Twaalf")),
        bloc.add(const SpeechButtonLongPressEnded())
      },
      expect: () => [
        const SpeechState(isListening: true),
        const SpeechState(isListening: true, error: "Please try again!"),
        const SpeechState(isListening: false, error: "Please try again!"),
      ],
    );
  });
}
