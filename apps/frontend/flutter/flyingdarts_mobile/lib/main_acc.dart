import 'package:flutter/material.dart';
import 'package:flyingdarts_app/flavor_config.dart';

import 'main.dart';

void main() async {
  FlavorConfig flavour = FlavorConfig(flavor: "acc");
  Widget app = await initializeApp(flavour);
  runApp(app);
}
