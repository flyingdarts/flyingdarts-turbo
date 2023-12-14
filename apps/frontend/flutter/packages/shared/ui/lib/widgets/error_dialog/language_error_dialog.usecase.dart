import 'package:flutter/material.dart';

import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import '../../wrappers/material.wrappers.dart';
import 'language_error_dialog.dart';

@widgetbook.UseCase(
  name: 'A language error dialog',
  type: LanguageErrorDialog,
)
Widget defaultLanguageErrorDialog(BuildContext context) {
  return createDefaultMaterialWidget(
    context,
    const LanguageErrorDialog(error: "Could not find that language"),
  );
}
