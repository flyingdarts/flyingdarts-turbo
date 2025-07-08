import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:language/language.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import '../state/speech_bloc.dart';
import 'speech_page.dart';

@widgetbook.UseCase(
  name: 'The only page in the app',
  type: SpeechPage,
)
Widget defaultSpeechPage(BuildContext context) {
  return createDefaultMaterialWidget(
    context,
    MultiBlocProvider(
      providers: [
        BlocProvider(create: (ctx) => SpeechBloc()..init()),
        BlocProvider(create: (ctx) => LanguageCubit()..init()),
      ],
      child: const SpeechPage(),
    ),
  );
}
