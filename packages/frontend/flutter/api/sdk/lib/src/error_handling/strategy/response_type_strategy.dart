import 'package:dio/dio.dart';

import '../../responses/request_failed_response.dart';
import '../../responses/request_response.dart';

abstract interface class ResponseTypeStrategy<TSuccess> {
  ErrorResponse<TSuccess, RequestFailedResponse> handleResponse(
    Response<dynamic> response,
  );
}
