library;

import 'package:injectable/injectable.dart';

export 'package:profile/src/dialog/profile_dialog.dart';
export 'package:profile/src/state/profile_cubit.dart';
export 'package:profile/src/state/profile_state.dart';

export 'profile.module.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
void initMicroPackage() {} // will not be called but needed for code generation
