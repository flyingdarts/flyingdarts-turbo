import 'package:alchemist/alchemist.dart';
import 'package:flutter/material.dart';
import 'package:language/language.dart';
import 'package:speech/speech.dart';
import 'package:speech_to_text/speech_to_text.dart';
import 'package:ui/ui.dart';

void main() {
  goldenTest(
    skip: true,
    'Speech recognition widget', // (1)
    fileName: 'speech_recognition_widget_golden_test', // (2)
    builder: () => GoldenTestGroup(
      // (3)
      children: [
        GoldenTestScenario(
          name: 'Initial state.',
          constraints: const BoxConstraints(maxWidth: 430), // (4)
          child: createDefaultCardWidget(
            SpeechRecognitionWidget(
              state: const SpeechState(
                isListening: false,
                error: '',
                lastEntry: '',
              ),
              languageState: LanguageState(
                LocaleName('nl-US', 'Nederlands (Verenigde Staten)'),
                [],
              ),
            ),
          ),
        ),
        GoldenTestScenario(
          name: 'While listening.',
          constraints: const BoxConstraints(maxWidth: 430), // (4)
          child: createDefaultCardWidget(
            SpeechRecognitionWidget(
              state: const SpeechState(
                isListening: false,
                error: '',
                lastEntry: '',
              ),
              languageState: LanguageState(
                LocaleName('nl-US', 'Nederlands (Verenigde Staten)'),
                [],
              ),
            ),
          ),
        ),
        GoldenTestScenario(
          name: 'When a good result has been found.',
          constraints: const BoxConstraints(maxWidth: 430), // (4)
          child: createDefaultCardWidget(
            SpeechRecognitionWidget(
              state: const SpeechState(
                isListening: false,
                error: '',
                lastEntry: '',
              ),
              languageState: LanguageState(
                LocaleName('nl-US', 'Nederlands (Verenigde Staten)'),
                [],
              ),
            ),
          ),
        ),
        GoldenTestScenario(
          name: 'When a bad result has been found.',
          constraints: const BoxConstraints(maxWidth: 430), // (4)
          child: createDefaultCardWidget(
            SpeechRecognitionWidget(
              state: const SpeechState(
                isListening: false,
                error: '',
                lastEntry: '',
              ),
              languageState: LanguageState(
                LocaleName('nl-US', 'Nederlands (Verenigde Staten)'),
                [],
              ),
            ),
          ),
        ),
      ],
    ),
  );
}
