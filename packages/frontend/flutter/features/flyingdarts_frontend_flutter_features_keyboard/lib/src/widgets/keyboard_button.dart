import 'package:flutter/material.dart';

class KeyboardButton extends StatelessWidget {
  const KeyboardButton({
    super.key,
    required this.input,
    required this.onPressed,
    required this.disabled,
  });

  final String input;
  final VoidCallback onPressed;
  final bool disabled;

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Padding(
        padding: const EdgeInsets.all(2.0),
        child: disabled
            ? ElevatedButton(
                onPressed: null,
                child: Text(
                  input,
                  textAlign: TextAlign.center,
                  style: Theme.of(context)
                      .primaryTextTheme
                      .labelSmall
                      ?.copyWith(fontSize: 12, fontWeight: FontWeight.bold),
                ),
              )
            : ElevatedButton(
                onPressed: onPressed,
                child: Text(
                  input,
                  textAlign: TextAlign.center,
                  style: Theme.of(context)
                      .primaryTextTheme
                      .labelSmall
                      ?.copyWith(fontSize: 12, fontWeight: FontWeight.bold),
                ),
              ),
      ),
    );
  }
}
