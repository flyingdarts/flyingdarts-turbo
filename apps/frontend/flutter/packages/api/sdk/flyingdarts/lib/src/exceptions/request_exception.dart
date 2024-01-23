class RequestException {
  final String requestUrl;
  final String requestMethod;
  const RequestException(this.requestMethod, this.requestUrl);
}
