import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:lottie/lottie.dart';

class LottieWidget extends StatelessWidget {
  final String? jsonString;
  final String? assetPath;
  final double? width;
  final double? height;
  final bool repeat;
  final bool animate;
  final BoxFit? fit;
  final AlignmentGeometry? alignment;
  final String? package;

  const LottieWidget({
    super.key,
    this.jsonString,
    this.assetPath,
    this.width,
    this.height,
    this.repeat = true,
    this.animate = true,
    this.fit,
    this.alignment,
    this.package,
  }) : assert(
         (jsonString != null) ^ (assetPath != null),
         'Either jsonString or assetPath must be provided, but not both',
       );

  /// Constructor for loading from JSON string
  const LottieWidget.fromJson({
    super.key,
    required String jsonString,
    this.width,
    this.height,
    this.repeat = true,
    this.animate = true,
    this.fit,
    this.alignment,
    this.package,
  }) : jsonString = jsonString,
       assetPath = null;

  /// Constructor for loading from asset
  const LottieWidget.fromAsset({
    super.key,
    required String assetPath,
    this.width,
    this.height,
    this.repeat = true,
    this.animate = true,
    this.fit,
    this.alignment,
    this.package,
  }) : assetPath = assetPath,
       jsonString = null;

  @override
  Widget build(BuildContext context) {
    try {
      if (jsonString != null) {
        // Parse the JSON string to validate it
        final Map<String, dynamic> animationData = json.decode(jsonString!);
        return Lottie.memory(
          utf8.encode(jsonString!),
          width: width,
          height: height,
          repeat: repeat,
          animate: animate,
          fit: fit,
          alignment: alignment,
        );
      } else if (assetPath != null) {
        return Lottie.asset(
          assetPath!,
          width: width,
          height: height,
          repeat: repeat,
          animate: animate,
          fit: fit,
          alignment: alignment,
          package: package,
        );
      } else {
        throw Exception('No animation source provided');
      }
    } catch (e) {
      // Return a fallback widget if loading fails
      return Container(
        width: width ?? 100,
        height: height ?? 100,
        decoration: BoxDecoration(
          color: Colors.grey.withOpacity(0.1),
          borderRadius: BorderRadius.circular(8),
        ),
        child: const Center(
          child: Icon(
            Icons.error_outline,
            color: Colors.grey,
            size: 32,
          ),
        ),
      );
    }
  }
}
