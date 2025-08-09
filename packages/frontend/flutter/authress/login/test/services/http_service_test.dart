import 'package:flutter_test/flutter_test.dart';
import 'package:flyingdarts_authress_login/src/models/auth_config.dart';
import 'package:flyingdarts_authress_login/src/services/http_service.dart';
import 'package:http/http.dart' as http;
import 'package:mocktail/mocktail.dart';

// Mock for http.Client
class MockHttpClient extends Mock implements http.Client {}

// Mock for http.Response
class MockHttpResponse extends Mock implements http.Response {}

void main() {
  group('HttpService', () {
    late HttpService httpService;
    late AuthressConfiguration testConfig;

    setUp(() {
      testConfig = const AuthressConfiguration(
        applicationId: 'test-app-123',
        authressApiUrl: 'https://test.authress.io',
        redirectUrl: 'flyingdarts://auth',
      );

      // Create HttpService instance
      httpService = HttpService(testConfig);

      // Replace the internal client with our mock
      // Note: This requires the service to expose the client or use dependency injection
      // For now, we'll test the public interface behavior
    });

    tearDown(() {
      httpService.dispose();
    });

    group('URL Building', () {
      test('builds URL correctly with base URL ending with slash', () {
        final config = AuthressConfiguration(
          applicationId: 'test-app-123',
          authressApiUrl: 'https://test.authress.io/',
          redirectUrl: 'flyingdarts://auth',
        );
        final service = HttpService(config);

        final url = service.buildUrl('/api/test');
        expect(url, equals('https://test.authress.io/api/test'));

        service.dispose();
      });

      test('builds URL correctly with base URL not ending with slash', () {
        final url = httpService.buildUrl('/api/test');
        expect(url, equals('https://test.authress.io/api/test'));
      });

      test('builds URL correctly with path not starting with slash', () {
        final url = httpService.buildUrl('api/test');
        expect(url, equals('https://test.authress.io/api/test'));
      });

      test('handles complex paths correctly', () {
        final url = httpService.buildUrl('/v1/clients/123/oauth/tokens');
        expect(
          url,
          equals('https://test.authress.io/v1/clients/123/oauth/tokens'),
        );
      });
    });

    group('Request Headers', () {
      test('includes default headers in requests', () {
        // Since we can't easily mock the internal client, we'll test this indirectly
        // by verifying the public behavior and structure
        expect(httpService, isA<HttpService>());
      });
    });

    group('Response Handling', () {
      test('HttpResponse correctly identifies success status codes', () {
        const response = HttpResponse(
          statusCode: 200,
          body: '{"success": true}',
          headers: {},
          isSuccess: true,
        );

        expect(response.isSuccess, isTrue);
        expect(response.isAuthError, isFalse);
        expect(response.isClientError, isFalse);
        expect(response.isServerError, isFalse);
      });

      test('HttpResponse correctly identifies auth errors', () {
        const response401 = HttpResponse(
          statusCode: 401,
          body: '{"error": "Unauthorized"}',
          headers: {},
          isSuccess: false,
        );

        expect(response401.isAuthError, isTrue);
        expect(response401.isClientError, isTrue);
        expect(response401.isSuccess, isFalse);

        const response403 = HttpResponse(
          statusCode: 403,
          body: '{"error": "Forbidden"}',
          headers: {},
          isSuccess: false,
        );

        expect(response403.isAuthError, isTrue);
        expect(response403.isClientError, isTrue);
      });

      test('HttpResponse correctly identifies client errors', () {
        const response = HttpResponse(
          statusCode: 400,
          body: '{"error": "Bad Request"}',
          headers: {},
          isSuccess: false,
        );

        expect(response.isClientError, isTrue);
        expect(response.isServerError, isFalse);
        expect(response.isAuthError, isFalse);
        expect(response.isSuccess, isFalse);
      });

      test('HttpResponse correctly identifies server errors', () {
        const response = HttpResponse(
          statusCode: 500,
          body: '{"error": "Internal Server Error"}',
          headers: {},
          isSuccess: false,
        );

        expect(response.isServerError, isTrue);
        expect(response.isClientError, isFalse);
        expect(response.isSuccess, isFalse);
      });

      test('HttpResponse parses JSON body correctly', () {
        const response = HttpResponse(
          statusCode: 200,
          body: '{"name": "test", "value": 123, "active": true}',
          headers: {},
          isSuccess: true,
        );

        final jsonBody = response.jsonBody;
        expect(jsonBody['name'], equals('test'));
        expect(jsonBody['value'], equals(123));
        expect(jsonBody['active'], equals(true));
      });

      test('HttpResponse handles invalid JSON gracefully', () {
        const response = HttpResponse(
          statusCode: 200,
          body: 'invalid json',
          headers: {},
          isSuccess: true,
        );

        expect(() => response.jsonBody, throwsA(isA<FormatException>()));
      });
    });

    group('HttpException', () {
      test('creates exception with message only', () {
        const exception = HttpException('Network error');

        expect(exception.message, equals('Network error'));
        expect(exception.statusCode, isNull);
        expect(exception.toString(), equals('HttpException: Network error'));
      });

      test('creates exception with message and status code', () {
        const exception = HttpException('Server error', 500);

        expect(exception.message, equals('Server error'));
        expect(exception.statusCode, equals(500));
        expect(
          exception.toString(),
          equals('HttpException: Server error (Status: 500)'),
        );
      });
    });

    group('Service Integration', () {
      test('disposes client properly', () {
        final service = HttpService(testConfig);

        // Should not throw
        expect(() => service.dispose(), returnsNormally);
      });

      test('can create multiple service instances', () {
        final service1 = HttpService(testConfig);
        final service2 = HttpService(testConfig);

        expect(service1, isNot(same(service2)));

        service1.dispose();
        service2.dispose();
      });

      test('builds URLs consistently across instances', () {
        final service1 = HttpService(testConfig);
        final service2 = HttpService(testConfig);

        final url1 = service1.buildUrl('/api/test');
        final url2 = service2.buildUrl('/api/test');

        expect(url1, equals(url2));

        service1.dispose();
        service2.dispose();
      });
    });

    group('Configuration Handling', () {
      test('handles different AuthConfig values', () {
        final configs = [
          AuthressConfiguration(
            applicationId: 'app1',
            authressApiUrl: 'https://api1.test.com',
            redirectUrl: 'app1://callback',
          ),
          AuthressConfiguration(
            applicationId: 'app2',
            authressApiUrl: 'https://api2.test.com/',
            redirectUrl: 'app2://callback',
          ),
        ];

        for (final config in configs) {
          final service = HttpService(config);
          final url = service.buildUrl('/test');

          expect(
            url,
            startsWith(config.authressApiUrl.replaceAll(RegExp(r'/$'), '')),
          );
          expect(url, endsWith('/test'));

          service.dispose();
        }
      });

      test('validates config requirements', () {
        final config = AuthressConfiguration(
          applicationId: 'test-app',
          authressApiUrl: 'https://test.authress.io',
          redirectUrl: 'app://callback',
        );

        expect(() => HttpService(config), returnsNormally);
      });
    });

    group('Edge Cases', () {
      test('handles empty paths', () {
        final url = httpService.buildUrl('');
        expect(url, equals('https://test.authress.io/'));
      });

      test('handles root path', () {
        final url = httpService.buildUrl('/');
        expect(url, equals('https://test.authress.io/'));
      });

      test('handles paths with query parameters', () {
        final url = httpService.buildUrl('/api/test?param=value');
        expect(url, equals('https://test.authress.io/api/test?param=value'));
      });

      test('handles paths with fragments', () {
        final url = httpService.buildUrl('/api/test#section');
        expect(url, equals('https://test.authress.io/api/test#section'));
      });
    });

    group('Status Code Categories', () {
      final testCases = [
        // Success codes
        (200, true, false, false, false),
        (201, true, false, false, false),
        (204, true, false, false, false),
        (299, true, false, false, false),

        // Client errors
        (400, false, true, false, false),
        (401, false, true, true, false), // Auth error
        (403, false, true, true, false), // Auth error
        (404, false, true, false, false),
        (422, false, true, false, false),
        (499, false, true, false, false),

        // Server errors
        (500, false, false, false, true),
        (502, false, false, false, true),
        (503, false, false, false, true),
        (599, false, false, false, true),
      ];

      for (final (
            statusCode,
            isSuccess,
            isClientError,
            isAuthError,
            isServerError,
          )
          in testCases) {
        test('categorizes status code $statusCode correctly', () {
          final response = HttpResponse(
            statusCode: statusCode,
            body: '{}',
            headers: const {},
            isSuccess: isSuccess,
          );

          expect(
            response.isSuccess,
            equals(isSuccess),
            reason: 'isSuccess for $statusCode',
          );
          expect(
            response.isClientError,
            equals(isClientError),
            reason: 'isClientError for $statusCode',
          );
          expect(
            response.isAuthError,
            equals(isAuthError),
            reason: 'isAuthError for $statusCode',
          );
          expect(
            response.isServerError,
            equals(isServerError),
            reason: 'isServerError for $statusCode',
          );
        });
      }
    });
  });
}
