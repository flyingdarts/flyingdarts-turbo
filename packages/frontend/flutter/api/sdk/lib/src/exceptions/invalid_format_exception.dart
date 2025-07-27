import './request_exception.dart';

class InvalidFormatException extends RequestException {
  final Object? inner;

  InvalidFormatException(this.inner, super.requestMethod, super.requestUrl);
}
