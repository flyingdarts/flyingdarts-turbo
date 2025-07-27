import 'dart:async';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:app_links/app_links.dart';
import 'package:authress_login/src/services/deep_link_service.dart';
import 'package:authress_login/src/models/deep_link_config.dart';

// Mock for AppLinks
class MockAppLinks extends Mock implements AppLinks {}

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('DeepLinkService', () {
    late MockAppLinks mockAppLinks;
    late DeepLinkService deepLinkService;
    late StreamController<Uri> uriStreamController;

    setUp(() {
      mockAppLinks = MockAppLinks();
      uriStreamController = StreamController<Uri>.broadcast();
      
      // Set up mock behaviors
      when(() => mockAppLinks.getInitialLink()).thenAnswer((_) async => null);
      when(() => mockAppLinks.uriLinkStream).thenAnswer((_) => uriStreamController.stream);
    });

    tearDown(() {
      uriStreamController.close();
      deepLinkService.dispose();
    });

    group('Initialization', () {
      test('initializes successfully with default config', () async {
        deepLinkService = DeepLinkService();
        
        await deepLinkService.initialize();
        
        verify(() => mockAppLinks.getInitialLink()).called(1);
        expect(deepLinkService.callbackUrl, equals('flyingdarts://auth'));
      });

      test('initializes with custom config', () async {
        const customConfig = DeepLinkConfig(
          scheme: 'myapp',
          host: 'callback',
          timeoutDuration: Duration(minutes: 10),
        );
        
        deepLinkService = DeepLinkService(customConfig);
        
        await deepLinkService.initialize();
        
        expect(deepLinkService.callbackUrl, equals('myapp://callback'));
      });

      test('handles initial link when app is opened from deep link', () async {
        final initialUri = Uri.parse('flyingdarts://auth?code=test123&nonce=abc456');
        when(() => mockAppLinks.getInitialLink()).thenAnswer((_) async => initialUri);
        
        deepLinkService = DeepLinkService();
        
        // Start waiting for callback before initialization
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        await deepLinkService.initialize();
        
        final result = await callbackFuture;
        
        expect(result, isNotNull);
        expect(result!['code'], equals('test123'));
        expect(result['nonce'], equals('abc456'));
      });

      test('handles initialization errors gracefully', () async {
        when(() => mockAppLinks.getInitialLink()).thenThrow(Exception('Platform error'));
        
        deepLinkService = DeepLinkService();
        
        // Should not throw
        await expectLater(deepLinkService.initialize(), completes);
      });
    });

    group('Deep Link Processing', () {
      setUp(() async {
        deepLinkService = DeepLinkService();
        await deepLinkService.initialize();
      });

      test('processes valid auth deep link', () async {
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        // Simulate incoming deep link
        final uri = Uri.parse('flyingdarts://auth?code=auth123&nonce=nonce456&state=state789');
        uriStreamController.add(uri);
        
        final result = await callbackFuture;
        
        expect(result, isNotNull);
        expect(result!['code'], equals('auth123'));
        expect(result['nonce'], equals('nonce456'));
        expect(result['state'], equals('state789'));
      });

      test('ignores non-matching deep links', () async {
        final callbackFuture = deepLinkService.waitForAuthCallback().timeout(
          const Duration(milliseconds: 100),
          onTimeout: () => null,
        );
        
        // Simulate non-matching deep link
        final uri = Uri.parse('different://scheme?code=auth123');
        uriStreamController.add(uri);
        
        final result = await callbackFuture;
        expect(result, isNull);
      });

      test('processes deep link with custom config', () async {
        const customConfig = DeepLinkConfig(scheme: 'myapp', host: 'oauth');
        deepLinkService.dispose();
        deepLinkService = DeepLinkService(customConfig);
        await deepLinkService.initialize();
        
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        // Simulate matching deep link
        final uri = Uri.parse('myapp://oauth?token=test123');
        uriStreamController.add(uri);
        
        final result = await callbackFuture;
        
        expect(result, isNotNull);
        expect(result!['token'], equals('test123'));
      });

      test('handles multiple deep links correctly', () async {
        // First callback
        final firstCallback = deepLinkService.waitForAuthCallback();
        
        final firstUri = Uri.parse('flyingdarts://auth?code=first123');
        uriStreamController.add(firstUri);
        
        final firstResult = await firstCallback;
        expect(firstResult!['code'], equals('first123'));
        
        // Second callback
        final secondCallback = deepLinkService.waitForAuthCallback();
        
        final secondUri = Uri.parse('flyingdarts://auth?code=second456');
        uriStreamController.add(secondUri);
        
        final secondResult = await secondCallback;
        expect(secondResult!['code'], equals('second456'));
      });
    });

    group('Timeout Handling', () {
      setUp(() async {
        deepLinkService = DeepLinkService();
        await deepLinkService.initialize();
      });

      test('times out when no callback received', () async {
        const shortTimeout = Duration(milliseconds: 100);
        const config = DeepLinkConfig(timeoutDuration: shortTimeout);
        
        deepLinkService.dispose();
        deepLinkService = DeepLinkService(config);
        await deepLinkService.initialize();
        
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        final result = await callbackFuture;
        expect(result, isNull);
      });

      test('cancels timeout when callback received', () async {
        const longTimeout = Duration(seconds: 10);
        const config = DeepLinkConfig(timeoutDuration: longTimeout);
        
        deepLinkService.dispose();
        deepLinkService = DeepLinkService(config);
        await deepLinkService.initialize();
        
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        // Send callback immediately
        final uri = Uri.parse('flyingdarts://auth?code=quick123');
        uriStreamController.add(uri);
        
        final result = await callbackFuture;
        expect(result!['code'], equals('quick123'));
      });

      test('handles custom timeout duration', () async {
        const veryShortTimeout = Duration(milliseconds: 50);
        const config = DeepLinkConfig(timeoutDuration: veryShortTimeout);
        
        deepLinkService.dispose();
        deepLinkService = DeepLinkService(config);
        await deepLinkService.initialize();
        
        final startTime = DateTime.now();
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        final result = await callbackFuture;
        final elapsed = DateTime.now().difference(startTime);
        
        expect(result, isNull);
        expect(elapsed.inMilliseconds, greaterThanOrEqualTo(50));
        expect(elapsed.inMilliseconds, lessThan(200));
      });
    });

    group('Callback Management', () {
      setUp(() async {
        deepLinkService = DeepLinkService();
        await deepLinkService.initialize();
      });

      test('cancels ongoing auth flow', () async {
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        // Cancel the flow
        deepLinkService.cancelAuthFlow();
        
        final result = await callbackFuture;
        expect(result, isNull);
      });

      test('handles new callback request while previous is active', () async {
        final firstCallback = deepLinkService.waitForAuthCallback();
        
        // Start second callback (should cancel first)
        final secondCallback = deepLinkService.waitForAuthCallback();
        
        // First should be cancelled
        final firstResult = await firstCallback;
        expect(firstResult, isNull);
        
        // Send deep link for second
        final uri = Uri.parse('flyingdarts://auth?code=second123');
        uriStreamController.add(uri);
        
        final secondResult = await secondCallback;
        expect(secondResult!['code'], equals('second123'));
      });

      test('handles stream errors gracefully', () async {
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        // Simulate stream error
        uriStreamController.addError(Exception('Stream error'));
        
        expect(callbackFuture, throwsA(isA<Exception>()));
      });

      test('ignores deep link when no active completer', () async {
        // Send deep link without waiting for callback
        final uri = Uri.parse('flyingdarts://auth?code=ignored123');
        
        // Should not throw
        expect(() => uriStreamController.add(uri), returnsNormally);
      });
    });

    group('Edge Cases', () {
      setUp(() async {
        deepLinkService = DeepLinkService();
        await deepLinkService.initialize();
      });

      test('handles empty query parameters', () async {
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        final uri = Uri.parse('flyingdarts://auth');
        uriStreamController.add(uri);
        
        final result = await callbackFuture;
        
        expect(result, isNotNull);
        expect(result!.isEmpty, isTrue);
      });

      test('handles complex query parameters', () async {
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        final uri = Uri.parse('flyingdarts://auth?code=abc123&state=some%20state&error=none&custom=value');
        uriStreamController.add(uri);
        
        final result = await callbackFuture;
        
        expect(result!['code'], equals('abc123'));
        expect(result['state'], equals('some state'));
        expect(result['error'], equals('none'));
        expect(result['custom'], equals('value'));
      });

      test('handles URL decoding correctly', () async {
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        final uri = Uri.parse('flyingdarts://auth?redirect_uri=https%3A%2F%2Fexample.com%2Fcallback');
        uriStreamController.add(uri);
        
        final result = await callbackFuture;
        
        expect(result!['redirect_uri'], equals('https://example.com/callback'));
      });
    });

    group('Service Lifecycle', () {
      test('disposes properly', () async {
        deepLinkService = DeepLinkService();
        await deepLinkService.initialize();
        
        final callbackFuture = deepLinkService.waitForAuthCallback();
        
        // Dispose should cancel active operations
        deepLinkService.dispose();
        
        final result = await callbackFuture;
        expect(result, isNull);
      });

      test('can be reinitialized after dispose', () async {
        deepLinkService = DeepLinkService();
        await deepLinkService.initialize();
        
        deepLinkService.dispose();
        
        // Should be able to initialize again
        await expectLater(deepLinkService.initialize(), completes);
      });
    });
  });
} 