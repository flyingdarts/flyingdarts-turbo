class LanguageState {
  final String currentLanguage;
  final List<String> availableLanguages;

  LanguageState(this.currentLanguage, this.availableLanguages);

  LanguageState copyWith({
    String? currentLanguage,
    List<String>? availableLanguages,
  }) {
    return LanguageState(
      currentLanguage ?? this.currentLanguage,
      availableLanguages ?? this.availableLanguages,
    );
  }
}
