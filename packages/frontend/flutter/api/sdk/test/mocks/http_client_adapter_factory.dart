import 'package:dio/dio.dart';
import 'package:mockito/annotations.dart';

import './http_client_adapter_factory.mocks.dart';

@GenerateMocks([HttpClientAdapter])
class HttpClientAdapterFactory {
  MockHttpClientAdapter create() {
    return MockHttpClientAdapter();
  }
}
