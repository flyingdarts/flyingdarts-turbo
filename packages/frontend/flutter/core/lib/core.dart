library core;

import 'package:injectable/injectable.dart';

export 'src/api.dart';
export 'src/configuration.dart';
export 'src/flavor.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
initMicroPackage() {} // will not be called but needed for code generation
