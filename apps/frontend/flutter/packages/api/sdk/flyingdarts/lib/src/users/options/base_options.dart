import 'package:dio/dio.dart';
import 'package:flyingdarts_sdk/src/flyingdarts_api_config.dart';

class BaseOptionsFactory {
  static const dioConnectTimeout = 20 * 1000;
  static const dioReceiveTimeout = 20 * 1000;

  BaseOptions fromConfig(FlyingdartsApiConfig config) => BaseOptions(
        baseUrl: config.rootUri.toString(),
        connectTimeout: dioConnectTimeout,
        receiveTimeout: dioReceiveTimeout,
      );
}
