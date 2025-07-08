import 'http_code_response_mixin.dart';

class RequestSuccessResponse with HttpCodeResponse {
  @override
  final int httpCode;

  const RequestSuccessResponse(this.httpCode);
}
