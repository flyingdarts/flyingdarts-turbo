import 'package:speech_to_text/speech_to_text.dart';

class LanguageState {
  final LocaleName preferedLocale;
  final List<LocaleName> availableLanguages;

  LanguageState(this.preferedLocale, this.availableLanguages);

  LanguageState copyWith({
    LocaleName? preferedLocale,
    List<LocaleName>? availableLanguages,
  }) {
    return LanguageState(
      preferedLocale ?? this.preferedLocale,
      availableLanguages ?? this.availableLanguages,
    );
  }
}
