import 'package:flutter/material.dart';
import 'package:flyingdarts_shared/wrappers/material.wrappers.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import '../../language/state/language_state.dart';
import '../state/speech_bloc.dart';

import 'speech_recognition_widget.dart';

@widgetbook.UseCase(
  name: 'The widget that contains button, result and status widgets',
  type: SpeechRecognitionWidget,
)
Widget defaultSpeechRecognitionWidget(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    SpeechRecognitionWidget(
      state: SpeechState(isListening: true),
      languageState: LanguageState('', []),
    ),
  );
}
