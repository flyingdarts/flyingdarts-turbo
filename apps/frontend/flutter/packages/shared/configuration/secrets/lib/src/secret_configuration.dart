import 'dart:convert';

import 'package:configuration/configuration.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class SecretsConfiguration<T> extends WriteableConfiguration<T> with DefaultConfiguration<T> {
  final FlutterSecureStorage secureStorage;
  final Function(Map<String, dynamic>) fromJson;

  SecretsConfiguration(this.secureStorage, this.fromJson);

  @override
  Future<T> read() async {
    final String key = T.toString();
    final String? value = await secureStorage.read(key: key);
    if (value == null && defaultModel != null) {
      return defaultModel!;
    }
    final Map<String, dynamic> json = value == null ? {} : jsonDecode(value);
    return fromJson(json);
  }

  @override
  Future<void> write(T model) async {
    final String key = T.toString();
    final String json = jsonEncode(model);
    await secureStorage.write(key: key, value: json);
  }

  factory SecretsConfiguration.fromString(FlutterSecureStorage secureStorage, Function(Map<String, dynamic>) fromJson) {
    return SecretsConfiguration(
      secureStorage,
      fromJson,
    );
  }
}
