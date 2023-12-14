import 'dart:async';

import 'package:amplify_auth_cognito/amplify_auth_cognito.dart';
import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flyingdarts_app/flavor_config.dart';
import 'package:keyboard/keyboard.dart';
import 'package:navigation/navigation.dart';
import 'package:speech/speech.dart';
import 'amplifyconfiguration.dart';
import 'app/app.dart';
import 'di.dart';

Future<Widget> initializeApp(FlavorConfig flavorConfig) async {
  WidgetsFlutterBinding.ensureInitialized();
  bool isAmplifySuccessfullyConfigured = false;
  try {
    await _configureAmplify();
    isAmplifySuccessfullyConfigured = true;
  } on AmplifyAlreadyConfiguredException {
    debugPrint('Amplify configuration failed.');
  }

  await setupDependencies(flavorConfig.flavor);

  return MyApp(
    isAmplifySuccessfullyConfigured: isAmplifySuccessfullyConfigured,
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
    AmplifyAuthCognito(
        secureStorageFactory: AmplifySecureStorage.factoryFrom(
      webOptions: WebSecureStorageOptions(
        persistenceOption: WebPersistenceOption.inMemory,
      ),
    )),
  ]);
  await Amplify.configure(amplifyconfig);
}
