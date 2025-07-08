import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:keyboard/keyboard.dart';
import 'package:language/language.dart';
import 'package:speech/speech.dart';
import 'package:speech_to_text/speech_to_text.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

@widgetbook.UseCase(
  name: 'Homescreen',
  type: WidgetbookHomescreen,
)
Widget defaultDashoard(BuildContext context) {
  return const WidgetbookHomescreen();
}

class WidgetbookHomescreen extends StatelessWidget {
  const WidgetbookHomescreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Row(
          children: [
            createDefaultCardWidget(
              SpeechRecognitionWidget(
                state: const SpeechState(
                  isListening: false,
                  error: '',
                  lastEntry: '',
                ),
                languageState: LanguageState(LocaleName('nl-US', 'LocaleName(Nederlands (Verenigde Staten)'), []),
              ),
            ),
            createDefaultCardWidget(
              SpeechRecognitionWidget(
                state: const SpeechState(
                  isListening: true,
                  error: '',
                  lastEntry: '',
                ),
                languageState: LanguageState(LocaleName('nl-US', 'LocaleName(Nederlands (Verenigde Staten)'), []),
              ),
            ),
            createDefaultCardWidget(
              SpeechRecognitionWidget(
                state: const SpeechState(
                  isListening: false,
                  error: 'Please try again',
                  lastEntry: '',
                ),
                languageState: LanguageState(LocaleName('nl-US', 'LocaleName(Nederlands (Verenigde Staten)'), []),
              ),
            ),
            createDefaultCardWidget(
              SpeechRecognitionWidget(
                state: const SpeechState(
                  isListening: false,
                  error: 'Great success!',
                  lastEntry: '180',
                ),
                languageState: LanguageState(LocaleName('nl-US', 'LocaleName(Nederlands (Verenigde Staten)'), []),
              ),
            )
          ],
        ),
        Row(
          children: [
            Padding(
              padding: const EdgeInsets.all(20.0),
              child: SizedBox(
                width: 400,
                height: 300,
                child: createDefaultMaterialWidget(
                  context,
                  MultiBlocProvider(
                    providers: [
                      BlocProvider(create: (ctx) => KeyboardCubit()),
                    ],
                    child: const KeyboardPage(),
                  ),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(20.0),
              child: SizedBox(
                width: 400,
                height: 300,
                child: createDefaultMaterialWidget(
                  context,
                  MultiBlocProvider(
                    providers: [
                      BlocProvider(create: (ctx) => KeyboardCubit()..setDigit(5)),
                    ],
                    child: const KeyboardPage(),
                  ),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(20.0),
              child: SizedBox(
                width: 400,
                height: 300,
                child: createDefaultMaterialWidget(
                  context,
                  MultiBlocProvider(
                    providers: [
                      BlocProvider(create: (ctx) => KeyboardCubit()..setShortcut(45)),
                    ],
                    child: const KeyboardPage(),
                  ),
                ),
              ),
            )
          ],
        )
      ],
    );
  }
}
