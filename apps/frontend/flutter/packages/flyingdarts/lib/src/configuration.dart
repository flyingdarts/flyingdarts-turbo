import 'package:configuration/configuration.dart';
import 'package:configuration_preferences/preferences.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:injectable/injectable.dart';
import 'package:shared_preferences/shared_preferences.dart';

@module
abstract class ConfigurationModule {
  @preResolve
  Future<SharedPreferences> get sharedPreferences => SharedPreferences.getInstance();

  @preResolve
  Future<WriteableConfiguration<LanguageConfig>> languageConfig(SharedPreferences sharedPreferences) async {
    final config = PreferencesConfiguration<LanguageConfig>(sharedPreferences, LanguageConfig.fromJson);
    config.setDefault(LanguageConfig(preferedLocaleId: 'nl-US'));
    await config.load();
    return config;
  }

  @injectable
  ReadableConfiguration<LanguageConfig> readableLanguageConfig(WriteableConfiguration<LanguageConfig> config) => config;

    @preResolve
  Future<FlutterSecureStorage> get secureStorage => Future.value(const FlutterSecureStorage());

  @preResolve 
  Future<WriteableConfiguration<

}

