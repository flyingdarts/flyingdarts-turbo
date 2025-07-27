import 'dart:convert';

import 'package:app_config_core/configuration.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class SecureStorageManager<T> extends ConfigWriter<T> with FallbackProvider<T> {
  final FlutterSecureStorage _secureStorage;
  final Function(Map<String, dynamic>) _deserializer;

  SecureStorageManager(this._secureStorage, this._deserializer);

  @override
  Future<T> fetchData() async {
    final String storageKey = T.toString();
    final String? encryptedValue = await _secureStorage.read(key: storageKey);

    if (encryptedValue == null && fallbackValue != null) {
      return fallbackValue!;
    }

    final Map<String, dynamic> jsonData = encryptedValue == null ? {} : jsonDecode(encryptedValue);
    return _deserializer(jsonData);
  }

  @override
  Future<void> storeData(T data) async {
    final String storageKey = T.toString();
    final String serializedData = jsonEncode(data);
    await _secureStorage.write(key: storageKey, value: serializedData);
  }

  static SecureStorageManager<T> create<T>(
    FlutterSecureStorage secureStorage,
    Function(Map<String, dynamic>) deserializer,
  ) {
    return SecureStorageManager<T>(secureStorage, deserializer);
  }
}
