import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../widgets/dartboard.dart';
import '../widgets/dartboard_unavailable.dart';

class DartboardPage extends StatelessWidget {
  DartboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(20.0),
      child: kIsWeb ? DartboardUnvailable() : Dartboard(),
    );
  }
}
