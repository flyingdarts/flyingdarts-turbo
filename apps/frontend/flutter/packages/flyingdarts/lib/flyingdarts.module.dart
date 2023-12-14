//@GeneratedMicroModule;FlyingdartsPackageModule;package:flyingdarts/flyingdarts.module.dart
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// coverage:ignore-file

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'dart:async' as _i2;

import 'package:configuration/configuration.dart' as _i4;
import 'package:flyingdarts/src/configuration.dart' as _i7;
import 'package:flyingdarts/src/flavor.dart' as _i6;
import 'package:flyingdarts/src/web_socket.dart' as _i8;
import 'package:injectable/injectable.dart' as _i1;
import 'package:shared_preferences/shared_preferences.dart' as _i3;
import 'package:websocket/websocket.dart' as _i5;

const String _dev = 'dev';
const String _prod = 'prod';
const String _acc = 'acc';

class FlyingdartsPackageModule extends _i1.MicroPackageModule {
// initializes the registration of main-scope dependencies inside of GetIt
  @override
  _i2.FutureOr<void> init(_i1.GetItHelper gh) async {
    final flavorModule = _$FlavorModule();
    final configurationModule = _$ConfigurationModule();
    final webSocketModule = _$WebSocketModule();
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
    await gh.factoryAsync<_i3.SharedPreferences>(
      () => configurationModule.sharedPreferences,
      preResolve: true,
    );
    gh.factory<String>(
      () => webSocketModule.rootWebSocketUriDev,
      instanceName: 'root_websocket_uri',
      registerFor: {
        _dev,
        _acc,
      },
    );
    gh.factory<String>(
      () => webSocketModule.rootWebSocketUriProd,
      instanceName: 'root_websocket_uri',
      registerFor: {_prod},
    );
    gh.factory<_i4.WebSocketConfig>(
      () => webSocketModule
          .config(gh<String>(instanceName: 'root_websocket_uri')),
      registerFor: {
        _dev,
        _acc,
        _prod,
      },
    );
    gh.factory<_i5.WebSocketService>(
      () => webSocketModule.webSocketService(gh<_i4.WebSocketConfig>()),
      registerFor: {
        _dev,
        _acc,
        _prod,
      },
    );
    await gh.factoryAsync<_i4.WriteableConfiguration<_i4.LanguageConfig>>(
      () => configurationModule.languageConfig(gh<_i3.SharedPreferences>()),
      preResolve: true,
    );
    gh.factory<_i4.ReadableConfiguration<_i4.LanguageConfig>>(() =>
        configurationModule.readableCredentials(
            gh<_i4.WriteableConfiguration<_i4.LanguageConfig>>()));
  }
}

class _$FlavorModule extends _i6.FlavorModule {}

class _$ConfigurationModule extends _i7.ConfigurationModule {}

class _$WebSocketModule extends _i8.WebSocketModule {}
