import 'package:flutter/material.dart';
import 'package:flutter_inappwebview/flutter_inappwebview.dart';

class Dartboard extends StatefulWidget {
  @override
  State<Dartboard> createState() => _DartboardState();
}

class _DartboardState extends State<Dartboard> {
  InAppWebViewController? webViewController;

  InAppWebViewOptions settings = InAppWebViewOptions();

  @override
  Widget build(BuildContext context) {
    return InAppWebView(
      initialFile: "assets/dartboard.html",
      onWebViewCreated: (controller) async {
        webViewController = controller;
      },
      onConsoleMessage: (controller, consoleMessage) {
        print(consoleMessage);
      },
    );
  }
}
