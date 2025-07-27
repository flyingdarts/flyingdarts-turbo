import 'package:flutter/foundation.dart';

import 'config_reader.dart';

abstract class ConfigWriter<TData> extends ConfigReader<TData> {
  Future<TData> persist(TData newData) async {
    await storeData(newData);
    cachedData = newData;
    isInitialized = true;
    return cachedData!;
  }

  @protected
  Future<void> storeData(TData data);
}
