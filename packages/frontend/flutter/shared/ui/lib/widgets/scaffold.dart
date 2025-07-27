import 'package:flutter/material.dart';

import '../themes/theme.dart' show MyTheme;

class FlyingdartsScaffold extends StatelessWidget {
  final Widget child;
  final bool showAppBar;
  final bool showActionBar;
  final List<Widget> actionButtons;
  final Widget? leading;

  const FlyingdartsScaffold({
    super.key,
    required this.child,
    this.showAppBar = true,
    this.showActionBar = false,
    this.actionButtons = const [],
    this.leading,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: MyTheme.primaryColor,
      appBar: showAppBar
          ? AppBar(
              title: const Text(
                'Flyingdarts',
                style: TextStyle(fontWeight: FontWeight.w600, color: Colors.white),
              ),
              backgroundColor: MyTheme.primaryColor,
              actions: showActionBar ? actionButtons : null,
              leading: leading,
            )
          : null,
      body: child,
    );
  }
}
