import 'package:flutter/material.dart';
import 'package:flyingdarts_shared/wrappers/material.wrappers.dart';

import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import 'appbar_button_widget.dart';

@widgetbook.UseCase(
  name: 'The button to use in the AppBar',
  type: AppBarButton,
)
Widget defaultAppBarButtonWidget(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    AppBarButton(
      iconData: Icons.abc,
      onPressed: () => {},
    ),
  );
}
