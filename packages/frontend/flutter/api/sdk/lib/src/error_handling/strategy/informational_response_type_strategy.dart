import 'package:dio/dio.dart';

import '../../exceptions/unexpect_response_code_exception.dart';
import '../../responses/request_failed_response.dart';
import '../../responses/request_response.dart';
import 'response_type_strategy.dart';

class InformationalResponseTypeStrategy<TSuccess>
    implements ResponseTypeStrategy<TSuccess> {
  @override
  ErrorResponse<TSuccess, RequestFailedResponse> handleResponse(
    Response<dynamic> response,
  ) {
    throw UnexpectedResponseCodeException.fromDioResponse(response);
  }
}

class UnexpectedResponseType {}
