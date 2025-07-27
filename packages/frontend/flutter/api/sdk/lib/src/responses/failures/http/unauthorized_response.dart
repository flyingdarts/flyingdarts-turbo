import '../../http_code_response_mixin.dart';
import '../../request_failed_response.dart';

class UnauthorizedResponse extends RequestFailedResponse with HttpCodeResponse {
  @override
  final int httpCode;

  const UnauthorizedResponse(this.httpCode);
}
