import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:keyboard/keyboard.dart';
import 'package:navigation/navigation.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import 'home_screen.dart';

@widgetbook.UseCase(
  name: 'The home page in the app',
  type: HomeScreen,
)
Widget defaultHomePage(BuildContext context) {
  return createDefaultMaterialWidget(
    context,
    MultiBlocProvider(
      providers: [
        BlocProvider(
          create: (ctx) => NavigationCubit()..initTest(),
        ),
        BlocProvider(create: (ctx) => KeyboardCubit()),
      ],
      child: const HomeScreen(),
    ),
  );
}
