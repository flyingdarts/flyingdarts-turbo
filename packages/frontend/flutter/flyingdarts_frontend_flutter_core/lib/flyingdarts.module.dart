//@GeneratedMicroModule;FlyingdartsPackageModule;package:flyingdarts/flyingdarts.module.dart
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// coverage:ignore-file

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'dart:async' as _i2;

import 'package:configuration/configuration.dart' as _i5;
import 'package:configuration_secrets/secrets.dart' as _i7;
import 'package:dio/dio.dart' as _i3;
import 'package:flutter_secure_storage/flutter_secure_storage.dart' as _i8;
import 'package:flyingdarts/src/api.dart' as _i10;
import 'package:flyingdarts/src/configuration.dart' as _i11;
import 'package:flyingdarts/src/flavor.dart' as _i9;
import 'package:injectable/injectable.dart' as _i1;
import 'package:shared_preferences/shared_preferences.dart' as _i4;
import 'package:websocket/websocket.dart' as _i6;

const String _dev = 'dev';
const String _prod = 'prod';
const String _acc = 'acc';

class FlyingdartsPackageModule extends _i1.MicroPackageModule {
// initializes the registration of main-scope dependencies inside of GetIt
  @override
  _i2.FutureOr<void> init(_i1.GetItHelper gh) async {
    final flavorModule = _$FlavorModule();
    final apiModule = _$ApiModule();
    final configurationModule = _$ConfigurationModule();
    gh.singleton<String>(
      flavorModule.flavorDev,
      instanceName: 'flavor',
      registerFor: {_dev},
    );
    gh.singleton<String>(
      flavorModule.flavorProd,
      instanceName: 'flavor',
      registerFor: {_prod},
    );
    gh.singleton<String>(
      flavorModule.flavorAcc,
      instanceName: 'flavor',
      registerFor: {_acc},
    );
    gh.singleton<_i3.Dio>(apiModule.dio);
    await gh.factoryAsync<_i4.SharedPreferences>(
      () => configurationModule.sharedPreferences,
      preResolve: true,
    );
    gh.factory<String>(
      () => apiModule.rootWebSocketUriDev,
      instanceName: 'root_websocket_uri',
      registerFor: {
        _dev,
        _acc,
      },
    );
    gh.factory<String>(
      () => apiModule.rootWebSocketUriProd,
      instanceName: 'root_websocket_uri',
      registerFor: {_prod},
    );
    gh.factory<_i5.WebSocketConfig>(
      () => apiModule.config(gh<String>(instanceName: 'root_websocket_uri')),
      registerFor: {
        _dev,
        _acc,
        _prod,
      },
    );
    gh.factory<_i6.WebSocketService>(
      () => apiModule.webSocketService(gh<_i5.WebSocketConfig>()),
      registerFor: {
        _dev,
        _acc,
        _prod,
      },
    );
    await gh.factoryAsync<_i5.WriteableConfiguration<_i5.LanguageConfig>>(
      () => configurationModule.languageConfig(gh<_i4.SharedPreferences>()),
      preResolve: true,
    );
    await gh.factoryAsync<_i5.WriteableConfiguration<_i7.Credentials>>(
      () => configurationModule
          .secretsConfiguration(gh<_i8.FlutterSecureStorage>()),
      preResolve: true,
    );
    gh.factory<_i5.ReadableConfiguration<_i5.LanguageConfig>>(() =>
        configurationModule.readableLanguageConfig(
            gh<_i5.WriteableConfiguration<_i5.LanguageConfig>>()));
  }
}

class _$FlavorModule extends _i9.FlavorModule {}

class _$ApiModule extends _i10.ApiModule {}

class _$ConfigurationModule extends _i11.ConfigurationModule {}
