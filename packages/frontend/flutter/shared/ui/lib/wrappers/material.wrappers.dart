import 'package:flutter/material.dart';

import '../themes/theme.dart';

Widget createDefaultMaterialWidget(BuildContext context, Widget child) {
  return MaterialApp(
    theme: myTheme,
    home: Material(
      child: child,
    ),
  );
}

Widget createDefaultWidgetInACard(BuildContext? context, Widget child) {
  return createDefaultCardWidget(child);
}

Widget createDefaultCardWidget(Widget child) {
  return Center(
    child: Container(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Card(
            color: MyTheme.primaryColor,
            shadowColor: MyTheme.secondaryColor,
            child: child,
          ),
        ],
      ),
    ),
  );
}
