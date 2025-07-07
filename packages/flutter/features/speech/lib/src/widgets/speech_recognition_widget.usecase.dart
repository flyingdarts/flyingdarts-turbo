import 'package:flutter/material.dart';
import 'package:language/language.dart';
import 'package:speech_to_text/speech_to_text.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

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
      languageState: LanguageState(LocaleName("", ""), []),
    ),
  );
}
