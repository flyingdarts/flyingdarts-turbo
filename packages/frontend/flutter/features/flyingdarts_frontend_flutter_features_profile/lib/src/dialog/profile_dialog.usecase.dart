import 'package:flutter/material.dart';
import 'package:profile/profile.dart';
import 'package:ui/ui.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

@widgetbook.UseCase(
  name: 'A language dialog',
  type: ProfileDialog,
)
Widget defaultLanguageDialog(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    const ProfileDialog(),
  );
}
