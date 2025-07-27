enum HttpResponseType {
  informational,
  success,
  redirection,
  clientError,
  serverError;

  const HttpResponseType();

  static fromHttpCode(int httpCode) {
    if (httpCode < 100) {
      throw Exception("Unexpected httpCode less than a 100: $httpCode");
    }

    if (httpCode < 200) return HttpResponseType.informational;
    if (httpCode < 300) return HttpResponseType.success;
    if (httpCode < 400) return HttpResponseType.redirection;
    if (httpCode < 500) return HttpResponseType.clientError;

    if (httpCode > 599) {
      throw Exception("Unexpected httpCode more than a 599: $httpCode");
    }

    return HttpResponseType.serverError;
  }
}
