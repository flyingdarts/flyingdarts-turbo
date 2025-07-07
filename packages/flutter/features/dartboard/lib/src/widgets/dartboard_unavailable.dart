import 'package:flutter/material.dart';
import 'package:internationalization/internationalization.dart';

class DartboardUnvailable extends StatelessWidget {
  const DartboardUnvailable({super.key});

  @override
  Widget build(BuildContext context) {
    return Text(AppLocalizations.of(context).underConstruction);
  }
}
