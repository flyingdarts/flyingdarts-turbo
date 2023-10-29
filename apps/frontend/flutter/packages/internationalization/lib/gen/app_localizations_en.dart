import 'app_localizations.dart';

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get helloWorld => 'Hello World!';

  @override
  String get underConstruction => 'Under construction';

  @override
  String get speechWidgetListening => 'Listening';

  @override
  String get speechWidgetHoldMicrophone => 'Hold the microphone';
}
