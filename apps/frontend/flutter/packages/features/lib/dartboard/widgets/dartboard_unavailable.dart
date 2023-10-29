import 'package:flutter/material.dart';
import 'package:flyingdarts_internationalization/internationalization.dart';

class DartboardUnvailable extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Text(AppLocalizations.of(context).underConstruction);
  }
}
