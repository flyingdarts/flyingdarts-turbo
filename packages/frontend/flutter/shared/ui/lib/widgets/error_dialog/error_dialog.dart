import 'package:flutter/material.dart';

abstract class ErrorDialog<T> extends StatelessWidget {
  const ErrorDialog({super.key, required this.error});
  final T error;
}
