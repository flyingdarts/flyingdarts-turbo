import 'dart:io';

import 'package:dio/dio.dart';
import '../../flyingdarts_api_config.dart';

class OptionsBuilder {
  final Options _options;
  final FlyingdartsApiConfig _config;
  OptionsBuilder(this._config) : _options = Options();

  OptionsBuilder useMethod(String method) {
    _options.method = method;
    return this;
  }

  OptionsBuilder useAuthorizationToken() {
    _options.headers ??= {};
    _options.headers![HttpHeaders.authorizationHeader] =
        _config.authorizationToken;
    return this;
  }

  OptionsBuilder useContentTypeJson() {
    _options.headers ??= {};
    _options.headers![HttpHeaders.contentTypeHeader] = 'application/json';
    return this;
  }

  Options build() {
    return _options.copyWith();
  }
}
