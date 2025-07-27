import 'package:app_config_core/configuration.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:injectable/injectable.dart';
import 'package:speech_to_text/speech_to_text.dart';

import 'language_state.dart';

@lazySingleton
class LanguageCubit extends Cubit<LanguageState> {
  final ConfigWriter<LocaleSettings> _languageConfig = GetIt.I<ConfigWriter<LocaleSettings>>();

  LanguageCubit() : super(LanguageState(LocaleName("en-US", "English (United States)"), []));

  void setPreferedLocale(LocaleName preferedLocale) async {
    await _languageConfig.persist(LocaleSettings(localeIdentifier: preferedLocale.localeId));
    emit(state.copyWith(preferedLocale: preferedLocale));
  }

  void setAvailableLanguages(List<LocaleName> languages) {
    emit(state.copyWith(availableLanguages: languages));
  }

  late SpeechToText _speechToText;

  Future<void> init() async {
    _speechToText = SpeechToText();
    var available = await _speechToText.initialize();
    if (available) {
      var locales = await _speechToText.locales();
      if (locales.isNotEmpty) {
        setAvailableLanguages(locales);
        if (_languageConfig.data.localeIdentifier != "") {
          setPreferedLocale(
            locales.firstWhere((element) => element.localeId == _languageConfig.data.localeIdentifier),
          );
        } else {
          var locale = await _speechToText.systemLocale();
          if (locale != null) {
            setPreferedLocale(locale);
          }
        }
      }
    }
  }
}
