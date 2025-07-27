import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flyingdarts_mobile/flavor_config.dart';

import 'app/app.dart';
import 'di.dart';

class Runner {
  late FlavorConfig flavor;
  late Widget app;

  Runner withFlavor(String flavor) {
    this.flavor = FlavorConfig(flavor: flavor);
    return this;
  }

  void run() {
    _initializeApp().then((app) {
      runApp(app);
    });
  }

  Future<Widget> _initializeApp() async {
    WidgetsFlutterBinding.ensureInitialized();

    await setupDependencies(flavor.flavor);

    return App();
  }
}
