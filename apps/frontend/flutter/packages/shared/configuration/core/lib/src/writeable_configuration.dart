import 'package:flutter/foundation.dart';

import 'readable_configuration.dart';

abstract class WriteableConfiguration<TConfig> extends ReadableConfiguration<TConfig> {
  Future<TConfig> update(TConfig model) async {
    await write(model);
    await load();
    return config;
  }

  @protected
  Future<void> write(TConfig model);
}
