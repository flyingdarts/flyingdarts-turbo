library flyingdarts_frontend_flutter_shared_amplify;

import 'package:injectable/injectable.dart';

export 'src/amplify_service.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
initMicroPackage() {} // will not be called but needed for code generation
