import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:language/language.dart';

import '../state/speech_bloc.dart';
import '../widgets/speech_recognition_widget.dart';

class SpeechPage extends StatefulWidget with WidgetsBindingObserver {
  const SpeechPage({Key? key}) : super(key: key);

  @override
  State<SpeechPage> createState() => _SpeechPageState();
}

class _SpeechPageState extends State<SpeechPage> {
  @override
  Widget build(BuildContext context) {
    final state = context.watch<SpeechBloc>().state;
    final languageState = context.watch<LanguageCubit>().state;
    return Flex(
      direction: Axis.vertical,
      children: [
        Container(
          alignment: Alignment.center,
          child: buildSpeechRecognitionWidget(context, state, languageState),
        ),
      ],
    );
  }
}
