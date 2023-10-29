import 'package:flutter/material.dart';
import 'package:flyingdarts_shared/wrappers/material.wrappers.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import 'login_with_facebook_button.dart';

@widgetbook.UseCase(
  name: 'Login with facebook button',
  type: LoginWithFacebookButton,
)
Widget defaultLoginWithFacebookBtn(BuildContext context) {
  return createDefaultWidgetInACard(
    context,
    LoginWithFacebookButton(
      onPressed: () {
        debugPrint('logging in');
      },
    ),
  );
}
