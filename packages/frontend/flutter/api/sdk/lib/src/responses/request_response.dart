sealed class RequestResponse<TSucceeded, TFailed> {}

class SuccessResponse<TSucceeded, TFailed> implements RequestResponse<TSucceeded, TFailed> {
  final TSucceeded response;

  const SuccessResponse(this.response);
}

class ErrorResponse<TSucceeded, TFailed> implements RequestResponse<TSucceeded, TFailed> {
  final TFailed failure;
  const ErrorResponse(this.failure);
}

extension Cast<TSucceeded, TFailed> on RequestResponse<TSucceeded, TFailed> {
  /// Make sure to check if it is actually an ok response before calling this
  SuccessResponse<TSucceeded, TFailed> asSuccessResponse() {
    return this as SuccessResponse<TSucceeded, TFailed>;
  }

  /// Make sure to check if it is actually an error response before calling this
  ErrorResponse<TSucceeded, TFailed> asErrorResponse() {
    return this as ErrorResponse<TSucceeded, TFailed>;
  }
}

extension Expect<TSucceeded, TFailed> on RequestResponse<TSucceeded, TFailed> {
  SuccessResponse<TSucceeded, TFailed> expect(String? message) {
    if (this is SuccessResponse) return asSuccessResponse();
    if (message == null) throw Exception(asErrorResponse().failure);
    throw Exception(message);
  }

  SuccessResponse<TSucceeded, TFailed> unwrap() {
    if (this is SuccessResponse) return asSuccessResponse();
    throw Exception(asErrorResponse().failure);
  }

  SuccessResponse<TSucceeded, TFailed> unwrapOr(SuccessResponse<TSucceeded, TFailed> or) {
    if (this is SuccessResponse) return asSuccessResponse();
    return or;
  }

  ErrorResponse<TSucceeded, TFailed> expectErr(String message) {
    if (this is ErrorResponse) return asErrorResponse();
    throw Exception(message);
  }
}
