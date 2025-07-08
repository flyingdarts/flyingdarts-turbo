import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import '../state/keyboard_cubit.dart';
import 'keyboard_page.dart';

@widgetbook.UseCase(
  name: 'The only page in the app',
  type: KeyboardPage,
)
Widget defaultKeyboardPage(BuildContext context) {
  return createDefaultMaterialWidget(
    context,
    MultiBlocProvider(
      providers: [
        BlocProvider(create: (ctx) => KeyboardCubit()),
      ],
      child: KeyboardPage(),
    ),
  );
}
