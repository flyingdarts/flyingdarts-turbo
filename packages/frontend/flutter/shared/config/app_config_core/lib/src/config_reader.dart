import 'dart:async';

import 'package:flutter/foundation.dart';

abstract class ConfigReader<TData> {
  TData? cachedData;
  bool isInitialized = false;

  Future<void> initialize() async {
    if (!isInitialized) {
      cachedData = await fetchData();
      isInitialized = true;
    }
  }

  TData get data {
    if (!isInitialized) {
      throw StateError('ConfigReader must be initialized before accessing data');
    }
    return cachedData!;
  }

  @protected
  Future<TData> fetchData();
}
