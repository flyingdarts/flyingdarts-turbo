// ignore: avoid_relative_lib_imports
import 'package:flyingdarts_app/flavor_config.dart';

import 'package:flutter_driver/driver_extension.dart';
import 'package:flyingdarts_app/main.dart';

void main() async {
  // This line enables the extension
  enableFlutterDriverExtension();

  // Call the `main()` function of your app or call `runApp` with any widget you
  // are interested in testing.
  await initializeApp(FlavorConfig(flavor: "dev"));
}
