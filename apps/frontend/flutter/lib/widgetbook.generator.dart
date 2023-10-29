/// A complete example for Widgetbook generator approach
///
/// You can use @App and @UseCase annotation to generate directories
/// [Ref link]: https://docs.widgetbook.io/getting-started/complete-example
import 'package:flutter/material.dart';
import 'package:widgetbook/widgetbook.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;
import 'package:flyingdarts_features/widgetbook.generator.directories.g.dart' as featuresBook;
import 'package:flyingdarts_core/widgetbook.generator.directories.g.dart' as coreBook;
import 'package:flyingdarts_shared/widgetbook.generator.directories.g.dart' as sharedBook;

import 'widgetbook.generator.directories.g.dart';

void main() {
  runApp(const WidgetbookApp());
}

@widgetbook.App()
class WidgetbookApp extends StatelessWidget {
  const WidgetbookApp({super.key});

  @override
  Widget build(BuildContext context) {
    return Widgetbook.material(
      // Use the generated directories variable
      directories: [
        ...directories,
        ...featuresBook.directories,
        ...coreBook.directories,
        ...sharedBook.directories,
      ],
      addons: [
        TextScaleAddon(
          scales: [1.0, 2.0],
        ),
        DeviceFrameAddon(
          devices: [
            Devices.ios.iPhoneSE,
            Devices.ios.iPhone13,
            Devices.ios.iPad12InchesGen4,
            Devices.android.mediumTablet,
            Devices.android.smallPhone,
            Devices.android.onePlus8Pro,
          ],
        ),
        HomeScreenAddon(isConfigured: true)
      ],
      appBuilder: (context, child) {
        return child;
      },
    );
  }
}
