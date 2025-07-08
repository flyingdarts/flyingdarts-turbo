library profile;

export 'package:profile/src/dialog/profile_dialog.dart';
export 'package:profile/src/state/profile_cubit.dart';
export 'package:profile/src/state/profile_state.dart';

import 'package:injectable/injectable.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
initMicroPackage() {} // will not be called but needed for code generation
