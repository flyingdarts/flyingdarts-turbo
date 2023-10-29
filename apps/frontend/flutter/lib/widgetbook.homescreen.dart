import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flyingdarts_features/keyboard/keyboard.dart';
import 'package:flyingdarts_features/keyboard/pages/keyboard_page.usecase.dart';
import 'package:flyingdarts_features/keyboard/state/keyboard_cubit.dart';
import 'package:flyingdarts_features/language/state/language_state.dart';
import 'package:flyingdarts_features/speech/state/speech_bloc.dart';
import 'package:flyingdarts_features/speech/widgets/speech_recognition_widget.dart';
import 'package:flyingdarts_shared/wrappers/material.wrappers.dart';
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
                languageState: LanguageState('Nederlands (Verenigde Staten)', []),
              ),
            ),
            createDefaultCardWidget(
              SpeechRecognitionWidget(
                state: const SpeechState(
                  isListening: true,
                  error: '',
                  lastEntry: '',
                ),
                languageState: LanguageState('Nederlands (Verenigde Staten)', []),
              ),
            ),
            createDefaultCardWidget(
              SpeechRecognitionWidget(
                state: const SpeechState(
                  isListening: false,
                  error: 'Please try again',
                  lastEntry: '',
                ),
                languageState: LanguageState('Nederlands (Verenigde Staten)', []),
              ),
            ),
            createDefaultCardWidget(
              SpeechRecognitionWidget(
                state: const SpeechState(
                  isListening: false,
                  error: 'Great success!',
                  lastEntry: '180',
                ),
                languageState: LanguageState('Nederlands (Verenigde Staten)', []),
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
                    child: KeyboardPage(),
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
                    child: KeyboardPage(),
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
                    child: KeyboardPage(),
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
