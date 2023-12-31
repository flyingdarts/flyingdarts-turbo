import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:widgetbook/src/knobs/knobs.dart';
import 'package:widgetbook/src/settings/settings.dart';

import 'knob_helper.dart';

void main() {
  testWidgets(
    'Bool knob added',
    (tester) async {
      await tester.pumpWithKnob(
        (context) => Text(
          context.knobs.boolean(
            label: 'label',
            initialValue: true,
          )
              ? 'Hi'
              : 'Bye',
        ),
      );

      expect(
        find.byType(KnobProperty<bool>),
        findsOneWidget,
      );
    },
  );
}
