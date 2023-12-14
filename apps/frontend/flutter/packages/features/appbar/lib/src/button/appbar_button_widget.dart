import 'package:flutter/material.dart';

class AppBarButton extends StatelessWidget {
  const AppBarButton({
    super.key,
    required this.iconData,
    required this.onPressed,
  });

  final IconData iconData;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    return IconButton(
      alignment: Alignment.center,
      icon: Padding(
        padding: const EdgeInsets.only(right: 20),
        child: Icon(
          iconData,
          size: 28,
          color: Colors.white,
        ),
      ),
      onPressed: onPressed,
    );
  }
}
