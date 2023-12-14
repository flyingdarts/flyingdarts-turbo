library amplify;

export 'src/amplify_service.dart';

import 'package:injectable/injectable.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
initMicroPackage() {} // will not be called but needed for code generation
