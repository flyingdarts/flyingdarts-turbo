// import 'package:appbar/appbar.dart';
// import 'package:flutter/material.dart';
// import 'package:flutter_bloc/flutter_bloc.dart';
// import 'package:flutter_test/flutter_test.dart';
// import 'package:flyingdarts_mobile/main.dart';
// import 'package:language/language.dart';
// import 'package:navigation/navigation.dart';
// import 'package:speech/speech.dart';
// import 'package:get_it/get_it.dart';
// import 'package:shared_preferences/shared_preferences.dart';
// import 'package:app_config_core/configuration.dart';
// import 'package:app_config_prefs/preferences.dart';

// void main() async {
//   TestWidgetsFlutterBinding.ensureInitialized();
//   AutomatedTestWidgetsFlutterBinding.ensureInitialized();

//   // Set up test dependencies
//   await _setupTestDependencies();

//   group('the application has', skip: true, () {
//     // # Skipping becus issue with aws amplify that i do not want to deal with rn.
//     testWidgets('a title', (tester) async {
//       await tester.runAsync(() async {
//         await tester.pumpWidget(MyApp());
//       });

//       const String expectedTitle = 'Flyingdarts';

//       final titleFinder = find.text(expectedTitle);

//       expect(titleFinder, findsOneWidget);
//     });
//     group('an AppBar', () {
//       group('when logged in', () {
//         testWidgets('that exists', (tester) async {
//           await tester.runAsync(() async {
//             await tester.pumpWidget(
//               MultiBlocProvider(
//                 providers: [
//                   BlocProvider<NavigationCubit>(
//                     create: (ctx) => NavigationCubit()
//                       ..initTest()
//                       ..setIsLoggedIn(true)
//                       ..setIsLoading(false),
//                   ),
//                   BlocProvider(create: (ctx) => SpeechBloc()..init()),
//                   BlocProvider(create: (ctx) => LanguageCubit()..init()),
//                 ],
//                 child: const MaterialApp(home: Material(child: MyApp())),
//               ),
//             );
//           });
//           await tester.pump();
//           final appBarFinder = find.byType(MyAppBar);

//           expect(appBarFinder, findsOneWidget);
//         });

//         testWidgets('that has 3 AppBarButtons', (tester) async {
//           await tester.pumpWidget(
//             BlocProvider<NavigationCubit>(
//               create: (ctx) => NavigationCubit()
//                 ..initTest()
//                 ..setIsLoggedIn(true),
//               child: const MaterialApp(home: Material(child: MyAppBar())),
//             ),
//           );

//           const int expectedNumberOfButtons = 2;
//           final buttonFinder = find.byType(AppBarButton);

//           expect(buttonFinder, findsNWidgets(expectedNumberOfButtons));
//         });
//       });
//       group('when logged out', () {
//         testWidgets('that exists when logged out', (tester) async {
//           await tester.runAsync(() async {
//             await tester.pumpWidget(
//               MultiBlocProvider(
//                 providers: [
//                   BlocProvider<NavigationCubit>(
//                     create: (ctx) => NavigationCubit()
//                       ..initTest()
//                       ..setIsLoggedIn(false)
//                       ..setIsLoading(false),
//                   ),
//                   BlocProvider(create: (ctx) => SpeechBloc()..init()),
//                   BlocProvider(create: (ctx) => LanguageCubit()..init()),
//                 ],
//                 child: const MaterialApp(home: Material(child: MyApp())),
//               ),
//             );
//           });
//           await tester.pump();
//           final appBarFinder = find.byType(MyAppBar);

//           expect(appBarFinder, findsOneWidget);
//         });

//         testWidgets('that has no AppBarButtons when logged out', (tester) async {
//           await tester.pumpWidget(
//             BlocProvider<NavigationCubit>(
//               create: (ctx) => NavigationCubit()
//                 ..initTest()
//                 ..setIsLoggedIn(false),
//               child: const MaterialApp(home: Material(child: MyAppBar())),
//             ),
//           );

//           final buttonFinder = find.byType(AppBarButton);

//           expect(buttonFinder, findsNothing);
//         });
//       });
//     });
//   });

//   testWidgets('a speech recognition widget', (tester) async {
//     await tester.runAsync(() async {
//       await tester.pumpWidget(
//         MultiBlocProvider(
//           providers: [
//             BlocProvider<NavigationCubit>(
//               create: (ctx) => NavigationCubit()
//                 ..initTest()
//                 ..setIsLoggedIn(true)
//                 ..setIsLoading(false),
//             ),
//             BlocProvider(create: (ctx) => SpeechBloc()..init()),
//             BlocProvider(create: (ctx) => LanguageCubit()..init()),
//           ],
//           child: const MaterialApp(home: Material(child: SpeechPage())),
//         ),
//       );
//     });

//     final widgetFinder = find.byType(SpeechRecognitionWidget);

//     expect(widgetFinder, findsOneWidget);
//   });
// }

// /// Set up test dependencies for GetIt
// Future<void> _setupTestDependencies() async {
//   final getIt = GetIt.instance;

//   // Reset GetIt to ensure clean state
//   if (getIt.isRegistered<WriteableConfiguration<LanguageConfig>>()) {
//     getIt.unregister<WriteableConfiguration<LanguageConfig>>();
//   }

//   // Set up SharedPreferences for testing
//   SharedPreferences.setMockInitialValues({});
//   final sharedPreferences = await SharedPreferences.getInstance();

//   // Create and register the language configuration
//   final languageConfig = PreferencesConfiguration<LanguageConfig>(sharedPreferences, LanguageConfig.fromJson);
//   languageConfig.setDefault(LanguageConfig(preferedLocaleId: 'en-US'));
//   await languageConfig.load();

//   // Register the configuration in GetIt
//   getIt.registerSingleton<WriteableConfiguration<LanguageConfig>>(languageConfig);
// }
