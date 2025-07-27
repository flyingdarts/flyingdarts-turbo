import '../../http_code_response_mixin.dart';
import '../../request_failed_response.dart';

class ForbiddenResponse extends RequestFailedResponse with HttpCodeResponse {
  @override
  final int httpCode;

  const ForbiddenResponse(this.httpCode);
}
