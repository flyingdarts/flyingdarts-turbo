import 'package:flutter/material.dart' hide BackButton;
import 'package:ui/ui.dart';

class SettingsPage extends StatelessWidget {
  const SettingsPage({super.key});

  @override
  Widget build(BuildContext context) {
    return FlyingdartsScaffold(
      leading: BackButton(),
      child: Text('Settings'),
    );
  }
}
