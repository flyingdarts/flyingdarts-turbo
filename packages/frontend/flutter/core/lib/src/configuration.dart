import 'package:app_config_core/configuration.dart';
import 'package:app_config_prefs/preferences.dart';
import 'package:app_config_secrets/secrets.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:injectable/injectable.dart';
import 'package:shared_preferences/shared_preferences.dart';

@module
abstract class ConfigurationModule {
  @preResolve
  Future<SharedPreferences> get sharedPreferences => SharedPreferences.getInstance();

  @preResolve
  Future<FlutterSecureStorage> get flutterSecureStorage => Future.value(FlutterSecureStorage());

  @preResolve
  Future<ConfigWriter<LocaleSettings>> languageConfig(SharedPreferences sharedPreferences) async {
    final config = LocalStorageManager<LocaleSettings>(sharedPreferences, LocaleSettings.fromJson);
    config.setFallback(LocaleSettings(localeIdentifier: 'nl-US'));
    await config.initialize();
    return config;
  }

  @injectable
  ConfigReader<LocaleSettings> readableLanguageConfig(ConfigWriter<LocaleSettings> config) => config;

  @preResolve
  Future<ConfigWriter<AuthTokens>> secretsConfiguration(FlutterSecureStorage flutterSecureStorage) async {
    final config = SecureStorageManager<AuthTokens>(flutterSecureStorage, AuthTokens.fromJson);
    config.setFallback(AuthTokens(accessToken: '', idToken: '', refreshToken: ''));
    await config.initialize();
    return config;
  }
}
