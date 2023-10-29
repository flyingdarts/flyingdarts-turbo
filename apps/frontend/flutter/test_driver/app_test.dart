import 'dart:async';
import 'package:flutter_gherkin/flutter_gherkin.dart';
import 'package:gherkin/gherkin.dart';
import 'package:glob/glob.dart';

import 'hooks/hook_example.dart';
import 'steps/tap_button.step.dart';

Future<void> main() {
  final config = TestConfiguration()
    ..features = [
      Glob(r'test_driver/features/**.feature'),
    ]
    ..reporters = [
      StdoutReporter(MessageLevel.error),
      ProgressReporter(),
      TestRunSummaryReporter(),
      JsonReporter(),
    ]
    ..hooks = [
      HookExample(),
    ]
    ..stepDefinitions = [
      TapButtonStep(),
    ]
    ..stopAfterTestFailed = true
    ..createWorld = (config) => Future.value(FlutterWorld());

  return GherkinRunner().execute(config);
}
