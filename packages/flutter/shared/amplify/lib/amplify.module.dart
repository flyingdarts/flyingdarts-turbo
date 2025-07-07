//@GeneratedMicroModule;AmplifyPackageModule;package:amplify/amplify.module.dart
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// coverage:ignore-file

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'dart:async' as _i2;

import 'package:amplify/src/amplify_service.dart' as _i3;
import 'package:injectable/injectable.dart' as _i1;

class AmplifyPackageModule extends _i1.MicroPackageModule {
// initializes the registration of main-scope dependencies inside of GetIt
  @override
  _i2.FutureOr<void> init(_i1.GetItHelper gh) {
    gh.factory<_i3.AmplifyService>(() => _i3.AmplifyService());
  }
}
