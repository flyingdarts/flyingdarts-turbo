import 'package:flutter/material.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;
import 'package:flyingdarts_shared/wrappers/material.wrappers.dart';

import 'appbar_widget.dart';

@widgetbook.UseCase(
  name: 'The button to use in the AppBar',
  type: MyAppBar,
)
Widget defaultMyAppWidget(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    const MyAppBar(),
  );
}
