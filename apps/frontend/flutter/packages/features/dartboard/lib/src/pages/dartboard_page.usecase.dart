import 'package:dartboard/dartboard.dart';
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import 'dartboard_page.dart';

@widgetbook.UseCase(
  name: 'The only page in the app',
  type: DartboardPage,
)
Widget defaultKeyboardPage(BuildContext context) {
  return createDefaultMaterialWidget(
    context,
    DartboardPage(),
  );
}
