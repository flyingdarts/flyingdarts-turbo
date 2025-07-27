import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:get_it/get_it.dart';
import 'package:language/language.dart';
import 'package:speech/speech.dart';

void main() {
  group('Speech Integration Tests', () {
    late GetIt getIt;

    setUp(() {
      getIt = GetIt.instance;
    });

    tearDown(() {
      getIt.reset();
    });

    testWidgets('Speech page renders correctly', (WidgetTester tester) async {
      // Build the widget
      await tester.pumpWidget(
        MaterialApp(
          home: MultiBlocProvider(
            providers: [
              BlocProvider<SpeechBloc>(
                create: (context) => getIt<SpeechBloc>(),
              ),
              BlocProvider<LanguageCubit>(
                create: (context) => LanguageCubit()..init(),
              ),
            ],
            child: const SpeechPage(),
          ),
        ),
      );

      // Wait for the widget to build
      await tester.pumpAndSettle();

      // Verify that the speech recognition widget is present
      expect(find.byType(SpeechPage), findsOneWidget);

      // Verify that the status text is displayed
      expect(find.byKey(const Key("SpeechCommandStatusText")), findsOneWidget);
    });

    testWidgets('Speech bloc initializes correctly', (WidgetTester tester) async {
      final bloc = getIt<SpeechBloc>();

      // Initial state should be initial
      expect(bloc.state.status, SpeechStatus.initial);

      // Initialize the bloc
      bloc.add(const SpeechInitialize());
      await tester.pump();

      // State should change to initializing
      expect(bloc.state.status, SpeechStatus.initializing);

      // Wait for initialization to complete
      await tester.pumpAndSettle();

      // State should be ready or error (depending on device capabilities)
      expect(
        bloc.state.status == SpeechStatus.ready || bloc.state.status == SpeechStatus.error,
        isTrue,
      );
    });

    testWidgets('Speech bloc handles result correctly', (WidgetTester tester) async {
      final bloc = getIt<SpeechBloc>();

      // Add a test result
      bloc.add(
        SpeechResultReceived(
          SpeechRecognitionResult(
            text: '180',
            confidence: 0.9,
            isFinal: true,
            timestamp: DateTime.now(),
            locale: 'en_US',
          ),
        ),
      );

      await tester.pump();

      // Verify the result is stored
      expect(bloc.state.lastResult?.text, '180');
      expect(bloc.state.status, SpeechStatus.ready);
    });

    testWidgets('Speech bloc handles errors correctly', (WidgetTester tester) async {
      final bloc = getIt<SpeechBloc>();

      // Add a test error
      bloc.add(const SpeechErrorOccurred('Test error message'));

      await tester.pump();

      // Verify the error is stored
      expect(bloc.state.errorMessage, 'Test error message');
      expect(bloc.state.status, SpeechStatus.error);
    });
  });
}
