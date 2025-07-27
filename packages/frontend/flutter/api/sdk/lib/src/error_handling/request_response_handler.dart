import 'package:dio/dio.dart';
import 'http_response_type.dart';
import 'package:injectable/injectable.dart';

import '../responses/request_failed_response.dart';
import '../responses/request_response.dart';
import 'strategy/client_error_response_type_strategy.dart';
import 'strategy/informational_response_type_strategy.dart';
import 'strategy/redirection_response_type_strategy.dart';
import 'strategy/response_type_strategy.dart';
import 'strategy/server_error_response_type_strategy.dart';

@injectable
class ErrorResponseHandler {
  ErrorResponse<TSuccess, RequestFailedResponse> handleErrorResponse<TSuccess>(DioError errorResponse) {
    Response<dynamic>? response = errorResponse.response;
    if (response == null || response.statusCode == null) {
      throw response!;
    }
    var responseType = HttpResponseType.fromHttpCode(response.statusCode!);

    ResponseTypeStrategy<TSuccess> strategy;
    switch (responseType) {
      case HttpResponseType.informational:
        strategy = InformationalResponseTypeStrategy<TSuccess>();
        break;
      case HttpResponseType.redirection:
        strategy = RedirectionResponseTypeStrategy<TSuccess>();
        break;
      case HttpResponseType.clientError:
        strategy = ClientErrorResponseTypeStrategy<TSuccess>();
        break;
      case HttpResponseType.serverError:
        strategy = ServerErrorResponseTypeStrategy<TSuccess>();
        break;
      default:
        // Only to make dart trust that I actually handled all the options
        throw Exception();
    }

    return strategy.handleResponse(response);
  }
}
