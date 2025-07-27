import 'package:flutter/material.dart';

/// Custom gesture detector for speech commands with long press support
class SpeechCommandGestureDetector extends GestureDetector {
  @override
  final VoidCallback onLongPress;

  @override
  final Function(LongPressEndDetails) onLongPressEnd;

  @override
  final VoidCallback onLongPressCancel;

  SpeechCommandGestureDetector({
    super.key,
    required this.onLongPress,
    required this.onLongPressEnd,
    required this.onLongPressCancel,
    required Widget child,
  }) : super(
         child: child,
         onLongPress: onLongPress,
         onLongPressEnd: onLongPressEnd,
         onLongPressCancel: onLongPressCancel,
       );
}
