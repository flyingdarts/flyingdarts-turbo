import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:speech_to_text/speech_to_text.dart';

import 'language_state.dart';

class LanguageCubit extends Cubit<LanguageState> {
  LanguageCubit() : super(LanguageState('', []));

  void setLanguage(String language) {
    emit(state.copyWith(currentLanguage: language));
  }

  void setAvailableLanguages(List<String> languages) {
    emit(state.copyWith(availableLanguages: languages));
  }

  late SpeechToText _speechToText;

  Future<void> init() async {
    _speechToText = SpeechToText();
    var available = await _speechToText.initialize();
    if (available) {
      var locale = await _speechToText.systemLocale();
      if (locale != null) setLanguage(locale.name);
      var locales = await _speechToText.locales();
      if (locales.isNotEmpty) setAvailableLanguages(locales.map((e) => e.name).toList());
    }
  }
}
