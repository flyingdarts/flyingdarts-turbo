import 'dart:async';
import 'dart:convert';

import 'package:configuration/configuration.dart';
import 'package:shared_preferences/shared_preferences.dart';

class PreferencesConfiguration<T> extends WriteableConfiguration<T> with DefaultConfiguration {
  final SharedPreferences sharedPreferences;
  final Function(Map<String, dynamic>) fromJson;

  PreferencesConfiguration(this.sharedPreferences, this.fromJson);

  @override
  Future<T> read() async {
    final String key = T.toString();
    final String? value = sharedPreferences.getString(key);
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
    await sharedPreferences.setString(key, json);
  }

  factory PreferencesConfiguration.fromString(
      SharedPreferences sharedPreferences, Function(Map<String, dynamic>) fromJson) {
    return PreferencesConfiguration(
      sharedPreferences,
      fromJson,
    );
  }
}
