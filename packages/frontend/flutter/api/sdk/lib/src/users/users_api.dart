import 'package:dio/dio.dart';
import 'users_api_config.dart';
import 'package:injectable/injectable.dart';

import '../error_handling/request_response_handler.dart';
import 'options/base_options.dart';

@injectable
class UsersApi {
  final UsersApiConfig config;
  final Dio dio;
  final ErrorResponseHandler responseHandler;

  UsersApi(this.config, this.dio, this.responseHandler) {
    dio.options = BaseOptionsFactory().fromConfig(config);
  }
}
