import 'package:dio/dio.dart';

import 'request_exception.dart';

class UnexpectedResponseCodeException extends RequestException {
  final int? responseCode;
  UnexpectedResponseCodeException(
    this.responseCode,
    super.requestMethod,
    super.requestUrl,
  );
  factory UnexpectedResponseCodeException.fromDioResponse(
    Response<dynamic> response,
  ) {
    return UnexpectedResponseCodeException(
      response.statusCode,
      response.requestOptions.method,
      response.requestOptions.uri.toString(),
    );
  }
}
