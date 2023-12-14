library language;

export 'package:language/src/dialog/language_dialog.dart';
export 'package:language/src/state/language_cubit.dart';
export 'package:language/src/state/language_state.dart';
export 'package:language/src/widgetbook.generator.dart';

import 'package:injectable/injectable.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
initMicroPackage() {} // will not be called but needed for code generation
