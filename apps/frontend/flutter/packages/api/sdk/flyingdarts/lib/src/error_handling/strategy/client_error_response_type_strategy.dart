import 'package:dio/dio.dart';
import '../../exceptions/unexpect_response_code_exception.dart';
import '../../responses/failures/http/forbidden_response.dart';
import '../../responses/failures/http/not_found_response.dart';
import '../../responses/failures/http/unauthorized_response.dart';
import '../../responses/request_failed_response.dart';
import '../../responses/request_response.dart';
import 'response_type_strategy.dart';

class ClientErrorResponseTypeStrategy<TSuccess> implements ResponseTypeStrategy<TSuccess> {
  @override
  ErrorResponse<TSuccess, RequestFailedResponse> handleResponse(Response<dynamic> response) {
    switch (response.statusCode) {
      case 401:
        return ErrorResponse(UnauthorizedResponse(response.statusCode!));
      case 403:
        return ErrorResponse(ForbiddenResponse(response.statusCode!));
      case 404:
        return ErrorResponse(NotFoundResponse(response.statusCode!));
      default:
        throw UnexpectedResponseCodeException.fromDioResponse(response);
    }
  }
}
