//@GeneratedMicroModule;ProfilePackageModule;package:profile/profile.module.dart
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// coverage:ignore-file

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'dart:async' as _i2;

import 'package:amplify/amplify.dart' as _i5;
import 'package:injectable/injectable.dart' as _i1;
import 'package:profile/src/state/profile_cubit.dart' as _i3;
import 'package:websocket/websocket.dart' as _i4;

class ProfilePackageModule extends _i1.MicroPackageModule {
// initializes the registration of main-scope dependencies inside of GetIt
  @override
  _i2.FutureOr<void> init(_i1.GetItHelper gh) {
    gh.lazySingleton<_i3.ProfileCubit>(() => _i3.ProfileCubit(
          gh<_i4.WebSocketService>(),
          gh<_i5.AmplifyService>(),
        ));
  }
}
