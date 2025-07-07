import 'package:flutter/material.dart';

import '../../models/keys.dart';

class LoginWithFacebookButton extends ElevatedButton {
  @override
  final VoidCallback onPressed;
  const LoginWithFacebookButton({super.key, required this.onPressed})
      : super(
            onPressed: onPressed,
            child: const Flex(
              direction: Axis.horizontal,
              children: [
                Padding(
                  padding: EdgeInsets.only(right: 20),
                  child: Icon(
                    Icons.facebook,
                    size: 28,
                    color: Colors.white,
                  ),
                ),
                Text("Login with facebook")
              ],
            ));

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      key: Keys.loginWithFacebookBtn.key,
      onPressed: onPressed,
      style: Theme.of(context).elevatedButtonTheme.style,
      child: child,
    );
  }
}
