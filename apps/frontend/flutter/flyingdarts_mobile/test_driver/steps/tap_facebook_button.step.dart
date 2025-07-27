import 'package:flutter_driver/flutter_driver.dart';
import 'package:flutter_gherkin/flutter_gherkin.dart';
import 'package:gherkin/gherkin.dart';

class TapFacebookButtonStep extends WhenWithWorld<FlutterWorld> {
  @override
  Future<void> executeStep() async {
    final locator = find.byType('LoginWithFacebookButton');
    await FlutterDriverUtils.tap(world.driver, locator, timeout: timeout);
  }

  @override
  Pattern get pattern => 'I tap the Login with Facebook button';
}
