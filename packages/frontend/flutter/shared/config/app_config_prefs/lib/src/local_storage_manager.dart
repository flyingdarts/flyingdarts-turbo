import 'dart:async';
import 'dart:convert';

import 'package:app_config_core/configuration.dart';
import 'package:shared_preferences/shared_preferences.dart';

class LocalStorageManager<T> extends ConfigWriter<T> with FallbackProvider<T> {
  final SharedPreferences _storage;
  final Function(Map<String, dynamic>) _deserializer;

  LocalStorageManager(this._storage, this._deserializer);

  @override
  Future<T> fetchData() async {
    final String storageKey = T.toString();
    final String? storedValue = _storage.getString(storageKey);

    if (storedValue == null && fallbackValue != null) {
      return fallbackValue!;
    }

    final Map<String, dynamic> jsonData = storedValue == null
        ? {}
        : jsonDecode(storedValue);
    return _deserializer(jsonData);
  }

  @override
  Future<void> storeData(T data) async {
    final String storageKey = T.toString();
    final String serializedData = jsonEncode(data);
    await _storage.setString(storageKey, serializedData);
  }

  static LocalStorageManager<T> create<T>(
    SharedPreferences storage,
    Function(Map<String, dynamic>) deserializer,
  ) {
    return LocalStorageManager<T>(storage, deserializer);
  }
}
