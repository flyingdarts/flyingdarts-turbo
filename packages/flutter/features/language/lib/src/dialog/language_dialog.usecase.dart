import 'package:flutter/material.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import 'language_dialog.dart';

@widgetbook.UseCase(
  name: 'A language dialog',
  type: LanguageDialog,
)
Widget defaultLanguageDialog(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    const LanguageDialog(),
  );
}
