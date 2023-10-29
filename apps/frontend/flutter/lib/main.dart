import 'dart:async';

import 'package:amplify_auth_cognito/amplify_auth_cognito.dart';
import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flyingdarts_core/navigation/navigation_cubit.dart';
import 'package:flyingdarts_features/keyboard/state/keyboard_cubit.dart';
import 'package:flyingdarts_features/language/state/language_cubit.dart';
import 'package:flyingdarts_features/speech/state/speech_bloc.dart';
import 'amplifyconfiguration.dart';
import 'app/app.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  bool isAmplifySuccessfullyConfigured = false;
  try {
    await _configureAmplify();
    isAmplifySuccessfullyConfigured = true;
  } on AmplifyAlreadyConfiguredException {
    debugPrint('Amplify configuration failed.');
  }

  runApp(
    MyApp(
      isAmplifySuccessfullyConfigured: isAmplifySuccessfullyConfigured,
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key, required this.isAmplifySuccessfullyConfigured});
  final bool isAmplifySuccessfullyConfigured;

  @override
  Widget build(BuildContext context) {
    try {
      return MultiBlocProvider(
        providers: [
          BlocProvider(create: (ctx) => NavigationCubit()..init()),
          BlocProvider(create: (ctx) => SpeechBloc()..init()),
          BlocProvider(create: (ctx) => LanguageCubit()..init()),
          BlocProvider(create: (ctx) => KeyboardCubit()),
        ],
        child: const App(),
      );
    } catch (e) {
      return Container();
    }
  }
}

Future<void> _configureAmplify() async {
  await Amplify.addPlugins([
    AmplifyAuthCognito(),
  ]);
  await Amplify.configure(amplifyconfig);
}
