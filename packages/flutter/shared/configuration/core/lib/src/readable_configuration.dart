import 'dart:async';

import 'package:flutter/foundation.dart';

abstract class ReadableConfiguration<TConfig> {
  late TConfig _config;

  Future<void> load() async {
    this._config = await read();
  }

  TConfig get config => _config;

  @protected
  Future<TConfig> read();
}
