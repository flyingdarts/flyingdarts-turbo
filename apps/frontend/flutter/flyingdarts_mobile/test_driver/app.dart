import 'package:flutter_driver/driver_extension.dart';
import 'package:flyingdarts_mobile/runner.dart';

void main() async {
  // This line enables the extension
  enableFlutterDriverExtension();

  // Call the `main()` function of your app or call `runApp` with any widget you
  // are interested in testing.
  Runner()
      .withFlavor('dev') //
      .run();
}
