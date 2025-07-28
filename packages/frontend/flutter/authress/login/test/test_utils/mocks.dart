import 'dart:convert';

import 'package:authress_login/src/models/auth_config.dart';
import 'package:authress_login/src/models/auth_state.dart';
import 'package:authress_login/src/models/user_profile.dart';
import 'package:authress_login/src/services/authentication_service.dart';
import 'package:authress_login/src/services/crypto_service.dart';
import 'package:authress_login/src/services/deep_link_service.dart';
import 'package:authress_login/src/services/http_service.dart';
import 'package:authress_login/src/services/token_service.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:mocktail/mocktail.dart';

// Mock classes using mocktail
class MockAuthenticationService extends Mock implements AuthenticationService {}

class MockTokenService extends Mock implements TokenService {}

class MockCryptoService extends Mock implements CryptoService {}

class MockHttpService extends Mock implements HttpService {}

class MockDeepLinkService extends Mock implements DeepLinkService {}

class MockHttpClient extends Mock implements http.Client {}

// Mock data for testing
class TestData {
  static const validUserId = 'test-user-123';
  static const validEmail = 'test@example.com';
  static const validAccessToken = 'valid-access-token-123';
  static const validRefreshToken = 'valid-refresh-token-456';

  static final validConfig = AuthressConfiguration(
    applicationId: 'test-app-123',
    authressApiUrl: 'https://test.authress.io',
    redirectUrl: 'flyingdarts://auth',
  );

  static final validUserProfile = UserProfile(
    userId: validUserId,
    email: validEmail,
    name: 'Test User',
    claims: {
      'roles': ['admin', 'user'],
      'groups': ['developers', 'testers'],
      'permissions': ['read:users', 'write:posts'],
    },
  );

  static final validAuthenticatedState = AuthStateAuthenticated(
    user: validUserProfile,
    accessToken: validAccessToken,
    refreshToken: validRefreshToken,
    expiresAt: DateTime.now().add(const Duration(hours: 1)),
  );

  static const errorState = AuthStateError(
    message: 'Test authentication error',
  );

  static const loadingState = AuthStateLoading();
  static const unauthenticatedState = AuthStateUnauthenticated();

  // Mock JWT token with valid structure (header.payload.signature)
  static const mockIdToken =
      'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0LXVzZXItMTIzIiwiZW1haWwiOiJ0ZXN0QGV4YW1wbGUuY29tIiwibmFtZSI6IlRlc3QgVXNlciIsInJvbGVzIjpbImFkbWluIiwidXNlciJdLCJncm91cHMiOlsiZGV2ZWxvcGVycyIsInRlc3RlcnMiXSwiZXhwIjo5OTk5OTk5OTk5fQ.signature';

  // Mock HTTP responses
  static const authUrlResponse = {
    'authenticationUrl':
        'https://test.authress.io/auth?client_id=test-app-123&redirect_uri=flyingdarts://auth',
  };

  static const tokenResponse = {
    'access_token': validAccessToken,
    'id_token': mockIdToken,
    'refresh_token': validRefreshToken,
    'expires_in': 3600,
    'token_type': 'Bearer',
  };

  static const userProfileResponse = {
    'sub': validUserId,
    'email': validEmail,
    'name': 'Test User',
    'roles': ['admin', 'user'],
    'groups': ['developers', 'testers'],
  };

  // Mock PKCE codes
  static const mockPKCECodes = PKCECodes(
    codeVerifier: 'mock-code-verifier-123',
    codeChallenge: 'mock-code-challenge-456',
    codeChallengeMethod: 'S256',
  );

  // Mock authentication callback params
  static const mockAuthCallbackParams = {
    'code': 'mock-auth-code-123',
    'nonce': 'mock-nonce-456',
    'state': 'mock-state-789',
  };
}

// Helper class for widget testing
class TestWidgetHelper {
  /// Create a minimal MaterialApp wrapper for widget testing
  static Widget wrapWidget(Widget child) {
    return MaterialApp(
      home: Scaffold(body: child),
    );
  }

  /// Create a test navigation context
  static Widget wrapWithNavigation(Widget child, {List<String>? routes}) {
    return MaterialApp(
      initialRoute: '/',
      routes: {
        '/': (context) => Scaffold(body: child),
        '/login': (context) => const Scaffold(body: Text('Login Page')),
        '/home': (context) => const Scaffold(body: Text('Home Page')),
        '/profile': (context) => const Scaffold(body: Text('Profile Page')),
        ...?routes?.asMap().map(
          (index, route) => MapEntry(
            route,
            (context) => Scaffold(body: Text('Route $route')),
          ),
        ),
      },
    );
  }
}

// Helper functions for setting up common mocks
void setupMockTokenService(MockTokenService mockTokenService) {
  when(
    () => mockTokenService.loadStoredTokens(),
  ).thenAnswer((_) async => TestData.validAuthenticatedState);

  when(
    () => mockTokenService.storeTokens(
      accessToken: any(named: 'accessToken'),
      refreshToken: any(named: 'refreshToken'),
      userProfile: any(named: 'userProfile'),
      expiresAt: any(named: 'expiresAt'),
    ),
  ).thenAnswer((_) async {});

  when(() => mockTokenService.clearTokens()).thenAnswer((_) async {});

  when(() => mockTokenService.hasValidTokens()).thenAnswer((_) async => true);

  when(
    () => mockTokenService.parseJwtPayload(any()),
  ).thenReturn(TestData.userProfileResponse);
}

void setupMockHttpService(MockHttpService mockHttpService) {
  // Mock successful authentication URL request
  when(
    () => mockHttpService.post('/api/authentication', body: any(named: 'body')),
  ).thenAnswer(
    (_) async => HttpResponse(
      statusCode: 200,
      body: json.encode(TestData.authUrlResponse),
      headers: const {},
      isSuccess: true,
    ),
  );

  // Mock successful token exchange
  when(() => mockHttpService.post(any(), body: any(named: 'body'))).thenAnswer(
    (_) async => HttpResponse(
      statusCode: 200,
      body: json.encode(TestData.tokenResponse),
      headers: const {},
      isSuccess: true,
    ),
  );

  // Mock successful user profile fetch
  when(
    () => mockHttpService.get('/v1/users/me', headers: any(named: 'headers')),
  ).thenAnswer(
    (_) async => HttpResponse(
      statusCode: 200,
      body: json.encode(TestData.userProfileResponse),
      headers: const {},
      isSuccess: true,
    ),
  );
}

void setupMockCryptoService(MockCryptoService mockCryptoService) {
  when(
    () => mockCryptoService.generatePKCECodes(),
  ).thenReturn(TestData.mockPKCECodes);

  when(
    () => mockCryptoService.calculateAntiAbuseHash(any()),
  ).thenAnswer((_) async => 'v2;1234567890;100;mock-anti-abuse-hash');
}

void setupMockDeepLinkService(MockDeepLinkService mockDeepLinkService) {
  when(() => mockDeepLinkService.initialize()).thenAnswer((_) async {});

  when(
    () => mockDeepLinkService.waitForAuthCallback(),
  ).thenAnswer((_) async => TestData.mockAuthCallbackParams);

  when(() => mockDeepLinkService.callbackUrl).thenReturn('flyingdarts://auth');

  when(() => mockDeepLinkService.cancelAuthFlow()).thenReturn(null);
}
