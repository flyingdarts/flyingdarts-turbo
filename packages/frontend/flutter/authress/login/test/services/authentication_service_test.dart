import 'dart:convert';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:authress_login/src/services/authentication_service.dart';
import 'package:authress_login/src/models/auth_state.dart';
import 'package:authress_login/src/models/deep_link_config.dart';
import 'package:authress_login/src/services/http_service.dart';
import '../test_utils/mocks.dart';

void main() {
  group('AuthenticationService', () {
    late MockTokenService mockTokenService;
    late MockHttpService mockHttpService;
    late MockDeepLinkService mockDeepLinkService;
    late MockCryptoService mockCryptoService;
    late AuthenticationService authService;

    setUp(() {
      mockTokenService = MockTokenService();
      mockHttpService = MockHttpService();
      mockDeepLinkService = MockDeepLinkService();
      mockCryptoService = MockCryptoService();

      // Set up mock fallbacks
      registerFallbackValue(TestData.validUserProfile);
      registerFallbackValue(DateTime.now());
      registerFallbackValue(<String, String?>{});

      // Set up default mock behaviors
      setupMockTokenService(mockTokenService);
      setupMockHttpService(mockHttpService);
      setupMockCryptoService(mockCryptoService);
      setupMockDeepLinkService(mockDeepLinkService);
    });

    group('Factory Constructor', () {
      test('creates service with all dependencies', () {
        final service = AuthenticationService.create(
          config: TestData.validConfig,
        );

        expect(service, isNotNull);
        expect(service.state, isA<AuthStateUnauthenticated>());
        expect(service.isAuthenticated, isFalse);
        expect(service.accessToken, isNull);
        expect(service.userProfile, isNull);
      });

      test('creates service with custom deep link config', () {
        const deepLinkConfig = DeepLinkConfig(
          scheme: 'myapp',
          host: 'auth',
        );

        final service = AuthenticationService.create(
          config: TestData.validConfig,
          deepLinkConfig: deepLinkConfig,
        );

        expect(service, isNotNull);
        expect(service.state, isA<AuthStateUnauthenticated>());
      });
    });

    group('Initialization', () {
      setUp(() {
        authService = AuthenticationService.create(
          config: TestData.validConfig,
        );
      });

      test('initializes successfully with no existing session', () async {
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => null);

        await authService.initialize();

        expect(authService.state, isA<AuthStateUnauthenticated>());
        verify(() => mockDeepLinkService.initialize()).called(1);
        verify(() => mockTokenService.loadStoredTokens()).called(1);
      });

      test('restores existing valid session', () async {
        final validAuthState = TestData.validAuthenticatedState;
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => validAuthState);

        await authService.initialize();

        expect(authService.state, isA<AuthStateAuthenticated>());
        expect(authService.isAuthenticated, isTrue);
        expect(authService.accessToken, equals(TestData.validAccessToken));
        expect(authService.userProfile?.userId, equals(TestData.validUserId));
        verify(() => mockTokenService.loadStoredTokens()).called(1);
      });

      test('refreshes expired token on initialization', () async {
        final expiredAuthState = AuthStateAuthenticated(
          user: TestData.validUserProfile,
          accessToken: 'expired-token',
          refreshToken: 'refresh-token',
          expiresAt: DateTime.now().subtract(const Duration(hours: 1)),
        );

        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => expiredAuthState);

        // Mock successful refresh
        when(
          () => mockHttpService.post(
            '/v1/clients/${TestData.validConfig.applicationId}/oauth/tokens',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body:
                '{"access_token": "new-token", "refresh_token": "new-refresh", "expires_in": 3600}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(() => mockTokenService.parseJwtPayload('new-token')).thenReturn({
          'sub': TestData.validUserId,
          'email': TestData.validEmail,
          'name': 'Test User',
        });

        await authService.initialize();

        expect(authService.state, isA<AuthStateAuthenticated>());
        expect(authService.accessToken, equals('new-token'));
        verify(
          () => mockTokenService.storeTokens(
            accessToken: 'new-token',
            refreshToken: 'new-refresh',
            userProfile: any(named: 'userProfile'),
            expiresAt: any(named: 'expiresAt'),
          ),
        ).called(1);
      });

      test('handles initialization error gracefully', () async {
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenThrow(Exception('Storage error'));

        await authService.initialize();

        expect(authService.state, isA<AuthStateUnauthenticated>());
        verify(() => mockTokenService.loadStoredTokens()).called(1);
      });
    });

    group('Authentication Flow', () {
      setUp(() {
        authService = AuthenticationService.create(
          config: TestData.validConfig,
        );
      });

      test('authenticates successfully with valid callback', () async {
        // Setup mocks for complete auth flow
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '{"authenticationUrl": "https://test.authress.io/auth"}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(
          () => mockDeepLinkService.waitForAuthCallback(),
        ).thenAnswer((_) async => TestData.mockAuthCallbackParams);

        when(
          () => mockHttpService.post(
            '/api/authentication/mock-nonce-456/tokens',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '${json.encode(TestData.tokenResponse)}',
            headers: const {},
            isSuccess: true,
          ),
        );

        // Start authentication
        await authService.authenticate();

        expect(authService.state, isA<AuthStateAuthenticated>());
        expect(authService.isAuthenticated, isTrue);
        expect(authService.accessToken, equals(TestData.validAccessToken));

        verify(() => mockCryptoService.generatePKCECodes()).called(1);
        verify(() => mockDeepLinkService.waitForAuthCallback()).called(1);
        verify(
          () => mockTokenService.storeTokens(
            accessToken: TestData.validAccessToken,
            refreshToken: TestData.validRefreshToken,
            userProfile: any(named: 'userProfile'),
            expiresAt: any(named: 'expiresAt'),
          ),
        ).called(1);
      });

      test('handles authentication cancellation', () async {
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '{"authenticationUrl": "https://test.authress.io/auth"}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(
          () => mockDeepLinkService.waitForAuthCallback(),
        ).thenAnswer((_) async => null);

        await authService.authenticate();

        expect(authService.state, isA<AuthStateError>());
        expect(
          (authService.state as AuthStateError).message,
          contains('cancelled or timed out'),
        );
      });

      test('handles authentication URL generation failure', () async {
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 400,
            body: '{"error": "Invalid request"}',
            headers: const {},
            isSuccess: false,
          ),
        );

        await authService.authenticate();

        expect(authService.state, isA<AuthStateError>());
        expect(
          (authService.state as AuthStateError).message,
          contains('Failed to get authentication URL'),
        );
      });

      test('handles callback with error parameter', () async {
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '{"authenticationUrl": "https://test.authress.io/auth"}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(
          () => mockDeepLinkService.waitForAuthCallback(),
        ).thenAnswer((_) async => {'error': 'access_denied'});

        await authService.authenticate();

        expect(authService.state, isA<AuthStateError>());
        expect(
          (authService.state as AuthStateError).message,
          contains('access_denied'),
        );
      });

      test('handles missing authorization code', () async {
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '{"authenticationUrl": "https://test.authress.io/auth"}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(
          () => mockDeepLinkService.waitForAuthCallback(),
        ).thenAnswer((_) async => {'nonce': 'test-nonce'});

        await authService.authenticate();

        expect(authService.state, isA<AuthStateError>());
        expect(
          (authService.state as AuthStateError).message,
          contains('Missing authorization code'),
        );
      });

      test('handles token exchange failure', () async {
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '{"authenticationUrl": "https://test.authress.io/auth"}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(
          () => mockDeepLinkService.waitForAuthCallback(),
        ).thenAnswer((_) async => TestData.mockAuthCallbackParams);

        when(
          () => mockHttpService.post(
            '/api/authentication/mock-nonce-456/tokens',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 400,
            body: '{"error": "invalid_grant"}',
            headers: const {},
            isSuccess: false,
          ),
        );

        await authService.authenticate();

        expect(authService.state, isA<AuthStateError>());
        expect(
          (authService.state as AuthStateError).message,
          contains('Token exchange failed'),
        );
      });

      test('authenticates with custom parameters', () async {
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '{"authenticationUrl": "https://test.authress.io/auth"}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(
          () => mockDeepLinkService.waitForAuthCallback(),
        ).thenAnswer((_) async => TestData.mockAuthCallbackParams);

        when(
          () => mockHttpService.post(
            '/api/authentication/mock-nonce-456/tokens',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '${json.encode(TestData.tokenResponse)}',
            headers: const {},
            isSuccess: true,
          ),
        );

        await authService.authenticate(
          connectionId: 'test-connection',
          tenantLookupIdentifier: 'test-tenant',
          additionalParams: {'custom': 'value'},
        );

        expect(authService.state, isA<AuthStateAuthenticated>());

        // Verify the auth URL request included custom parameters
        final capturedCall = verify(
          () => mockHttpService.post(
            '/api/authentication',
            body: captureAny(named: 'body'),
          ),
        ).captured.first;
        expect(capturedCall['connectionId'], equals('test-connection'));
        expect(capturedCall['tenantLookupIdentifier'], equals('test-tenant'));
        expect(capturedCall['custom'], equals('value'));
      });
    });

    group('Token Management', () {
      setUp(() {
        authService = AuthenticationService.create(
          config: TestData.validConfig,
        );
      });

      test('ensures valid token when authenticated and not expired', () async {
        // Mock existing valid session to set authenticated state
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => TestData.validAuthenticatedState);

        await authService.initialize();

        final token = await authService.ensureValidToken();

        expect(token, equals(TestData.validAccessToken));
        verifyNever(
          () => mockHttpService.post(
            any(that: contains('/oauth/tokens')),
            body: any(named: 'body'),
          ),
        );
      });

      test('refreshes token when expired', () async {
        final expiredState = AuthStateAuthenticated(
          user: TestData.validUserProfile,
          accessToken: 'expired-token',
          refreshToken: 'valid-refresh-token',
          expiresAt: DateTime.now().subtract(const Duration(hours: 1)),
        );

        // Mock expired session
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => expiredState);

        // Mock successful refresh
        when(
          () => mockHttpService.post(
            '/v1/clients/${TestData.validConfig.applicationId}/oauth/tokens',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: json.encode(TestData.tokenResponse),
            headers: const {},
            isSuccess: true,
          ),
        );

        await authService.initialize();
        final token = await authService.ensureValidToken();

        expect(token, equals(TestData.validAccessToken));
        verify(
          () => mockTokenService.storeTokens(
            accessToken: TestData.validAccessToken,
            refreshToken: TestData.validRefreshToken,
            userProfile: any(named: 'userProfile'),
            expiresAt: any(named: 'expiresAt'),
          ),
        ).called(2); // Once during init, once during ensureValidToken
      });

      test('returns null when token refresh fails', () async {
        final expiredState = AuthStateAuthenticated(
          user: TestData.validUserProfile,
          accessToken: 'expired-token',
          refreshToken: 'invalid-refresh-token',
          expiresAt: DateTime.now().subtract(const Duration(hours: 1)),
        );

        // Mock expired session
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => expiredState);

        // Mock failed refresh
        when(
          () => mockHttpService.post(
            '/v1/clients/${TestData.validConfig.applicationId}/oauth/tokens',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 400,
            body: '{"error": "invalid_grant"}',
            headers: const {},
            isSuccess: false,
          ),
        );

        await authService.initialize();
        final token = await authService.ensureValidToken();

        expect(token, isNull);
      });

      test('returns null when not authenticated', () async {
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => null);
        await authService.initialize();

        final token = await authService.ensureValidToken();

        expect(token, isNull);
      });
    });

    group('User Profile', () {
      setUp(() {
        authService = AuthenticationService.create(
          config: TestData.validConfig,
        );
      });

      test('fetches user profile successfully', () async {
        // Mock authenticated session
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => TestData.validAuthenticatedState);
        await authService.initialize();

        when(
          () => mockHttpService.get(
            '/v1/users/me',
            headers: any(named: 'headers'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: json.encode(TestData.userProfileResponse),
            headers: const {},
            isSuccess: true,
          ),
        );

        final profile = await authService.fetchUserProfile();

        expect(profile, isNotNull);
        expect(profile?.userId, equals(TestData.validUserId));
        expect(profile?.email, equals(TestData.validEmail));
        verify(
          () => mockHttpService.get(
            '/v1/users/me',
            headers: {
              'Authorization': 'Bearer ${TestData.validAccessToken}',
            },
          ),
        ).called(1);
      });

      test('returns null when not authenticated', () async {
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => null);
        await authService.initialize();

        final profile = await authService.fetchUserProfile();

        expect(profile, isNull);
        verifyNever(
          () => mockHttpService.get(any(), headers: any(named: 'headers')),
        );
      });

      test('handles fetch failure gracefully', () async {
        // Mock authenticated session
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => TestData.validAuthenticatedState);
        await authService.initialize();

        when(
          () => mockHttpService.get(
            '/v1/users/me',
            headers: any(named: 'headers'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 500,
            body: '{"error": "Internal server error"}',
            headers: const {},
            isSuccess: false,
          ),
        );

        final profile = await authService.fetchUserProfile();

        expect(profile, isNull);
      });
    });

    group('Logout', () {
      setUp(() {
        authService = AuthenticationService.create(
          config: TestData.validConfig,
        );
      });

      test('logs out successfully and clears state', () async {
        // Mock authenticated session first
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => TestData.validAuthenticatedState);
        await authService.initialize();

        await authService.logout();

        expect(authService.state, isA<AuthStateUnauthenticated>());
        expect(authService.isAuthenticated, isFalse);
        expect(authService.accessToken, isNull);
        expect(authService.userProfile, isNull);

        verify(() => mockDeepLinkService.cancelAuthFlow()).called(1);
        verify(() => mockTokenService.clearTokens()).called(1);
      });

      test('can logout from unauthenticated state', () async {
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => null);
        await authService.initialize();

        await authService.logout();

        expect(authService.state, isA<AuthStateUnauthenticated>());
        verify(() => mockTokenService.clearTokens()).called(1);
      });
    });

    group('State Management', () {
      setUp(() {
        authService = AuthenticationService.create(
          config: TestData.validConfig,
        );
      });

      test('notifies listeners on authentication flow state changes', () async {
        var stateChanges = <AuthState>[];
        authService.addListener(() {
          stateChanges.add(authService.state);
        });

        // Mock successful authentication flow
        when(
          () => mockHttpService.post(
            '/api/authentication',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: '{"authenticationUrl": "https://test.authress.io/auth"}',
            headers: const {},
            isSuccess: true,
          ),
        );

        when(
          () => mockDeepLinkService.waitForAuthCallback(),
        ).thenAnswer((_) async => TestData.mockAuthCallbackParams);

        when(
          () => mockHttpService.post(
            '/api/authentication/mock-nonce-456/tokens',
            body: any(named: 'body'),
          ),
        ).thenAnswer(
          (_) async => HttpResponse(
            statusCode: 200,
            body: json.encode(TestData.tokenResponse),
            headers: const {},
            isSuccess: true,
          ),
        );

        await authService.authenticate();

        expect(stateChanges.length, greaterThan(1));
        expect(stateChanges.first, isA<AuthStateLoading>());
        expect(stateChanges.last, isA<AuthStateAuthenticated>());
      });

      test('preserves state when no changes occur', () async {
        var callbackCount = 0;
        authService.addListener(() => callbackCount++);

        // Initialize with no stored tokens
        when(
          () => mockTokenService.loadStoredTokens(),
        ).thenAnswer((_) async => null);
        await authService.initialize();

        expect(
          callbackCount,
          equals(0),
        ); // No change from initial unauthenticated state
        expect(authService.state, isA<AuthStateUnauthenticated>());
      });
    });
  });
}
