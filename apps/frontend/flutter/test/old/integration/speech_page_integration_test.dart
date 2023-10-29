import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:iyltdsu_voice/main.dart';
import 'package:integration_test/integration_test.dart';
import 'package:flyingdarts_features/speech/speech_command_gesture_detector.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();
  TestWidgetsFlutterBinding.ensureInitialized();
  group('speech_page_integration_tests', skip: true, () {
    testWidgets('scanner dialog opens and closes as expected', (tester) async {
      await tester.pumpWidget(const MyApp(
        isAmplifySuccessfullyConfigured: true,
      ));

      final buttonFinder = find.byKey(const Key("QrCodeScannerButton"));
      final dialogFinder = find.byKey(const Key("QrCodeScannerDialog"));
      final closeButtonFinder = find.byKey(const Key("QrCodeScannerCloseDialog"));

      // open the dialog
      await tester.tap(buttonFinder);
      await tester.pumpAndSettle();

      // verify the dialog is open
      expect(dialogFinder, findsOneWidget);

      // close the dialog
      await tester.tap(closeButtonFinder);
      await tester.pumpAndSettle();

      // verify the dialog is closed
      expect(dialogFinder, findsNothing);
    });

    testWidgets('language dialog opens and closes as expected', (tester) async {
      await tester.pumpWidget(const MyApp(
        isAmplifySuccessfullyConfigured: true,
      ));

      final buttonFinder = find.byKey(const Key("SettingsButton"));
      final dialogFinder = find.byKey(const Key("LanguageDialog"));
      final closeButtonFinder = find.byKey(const Key("LanguageCloseDialog"));

      // open the dialog
      await tester.tap(buttonFinder);
      await tester.pumpAndSettle();

      // verify dialog is openend
      expect(dialogFinder, findsOneWidget);

      // close the dialog
      await tester.tap(closeButtonFinder);
      await tester.pumpAndSettle();

      // verify dialog is closed
      expect(dialogFinder, findsNothing);
    });

    testWidgets('speech button changes state onLongPress', (tester) async {
      await tester.pumpWidget(const MyApp(
        isAmplifySuccessfullyConfigured: true,
      ));

      // verify that the speech commmand status has correct initial text
      expect(find.text("Hold the microphone"), findsOneWidget);

      // do a long press on the gesture detector
      await tester.longPress(find.byType(SpeechCommandGestureDetector));

      // verify that the speech command status has updated text correctly
      expect(find.text("Listening"), findsOneWidget);

      // allow the onLongPress to end
      await tester.pumpAndSettle();

      // verify that the speech command status is back to its initial state
      expect(find.text("Hold the microphone"), findsOneWidget);
    });
  });
}
