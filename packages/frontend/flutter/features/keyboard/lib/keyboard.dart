library;

import 'package:injectable/injectable.dart';

export 'package:keyboard/src/pages/keyboard_page.dart';
export 'package:keyboard/src/state/keyboard_cubit.dart';
export 'package:keyboard/src/state/keyboard_state.dart';
export 'package:keyboard/src/widgetbook.generator.dart';
export 'package:keyboard/src/widgets/keyboard_button.dart';

export 'keyboard.module.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
void initMicroPackage() {} // will not be called but needed for code generation
