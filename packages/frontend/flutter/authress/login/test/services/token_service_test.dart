import 'dart:convert';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:authress_login/src/services/token_service.dart';
import 'package:authress_login/src/models/auth_state.dart';
import '../test_utils/mocks.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('TokenService', () {
    late TokenService tokenService;

    setUp(() {
      tokenService = TokenService();
    });

    tearDown(() {
      tokenService.dispose();
    });

    group('Token Storage', () {
      test('stores tokens successfully', () async {
        SharedPreferences.setMockInitialValues({});
        
        await tokenService.storeTokens(
          accessToken: TestData.validAccessToken,
          refreshToken: TestData.validRefreshToken,
          userProfile: TestData.validUserProfile,
          expiresAt: DateTime.now().add(const Duration(hours: 1)),
        );

        final prefs = await SharedPreferences.getInstance();
        expect(prefs.getString('authress_access_token'), equals(TestData.validAccessToken));
        expect(prefs.getString('authress_refresh_token'), equals(TestData.validRefreshToken));
        expect(prefs.getString('authress_user_profile'), isNotNull);
        expect(prefs.getString('authress_token_expiry'), isNotNull);
      });

      test('stores tokens without refresh token', () async {
        SharedPreferences.setMockInitialValues({});
        
        await tokenService.storeTokens(
          accessToken: TestData.validAccessToken,
          userProfile: TestData.validUserProfile,
          expiresAt: DateTime.now().add(const Duration(hours: 1)),
        );

        final prefs = await SharedPreferences.getInstance();
        expect(prefs.getString('authress_access_token'), equals(TestData.validAccessToken));
        expect(prefs.getString('authress_refresh_token'), isNull);
        expect(prefs.getString('authress_user_profile'), isNotNull);
      });

      test('overwrites existing tokens', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': 'old-token',
          'authress_refresh_token': 'old-refresh',
        });
        
        await tokenService.storeTokens(
          accessToken: 'new-token',
          refreshToken: 'new-refresh',
          userProfile: TestData.validUserProfile,
          expiresAt: DateTime.now().add(const Duration(hours: 1)),
        );

        final prefs = await SharedPreferences.getInstance();
        expect(prefs.getString('authress_access_token'), equals('new-token'));
        expect(prefs.getString('authress_refresh_token'), equals('new-refresh'));
      });
    });

    group('Token Loading', () {
      test('loads stored tokens successfully', () async {
        final expiresAt = DateTime.now().add(const Duration(hours: 1));
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
          'authress_token_expiry': expiresAt.toIso8601String(),
        });

        final authState = await tokenService.loadStoredTokens();

        expect(authState, isNotNull);
        expect(authState, isA<AuthStateAuthenticated>());
        expect(authState!.accessToken, equals(TestData.validAccessToken));
        expect(authState.refreshToken, equals(TestData.validRefreshToken));
        expect(authState.user.userId, equals(TestData.validUserId));
        expect(authState.user.email, equals(TestData.validEmail));
        expect(authState.expiresAt.isAfter(DateTime.now()), isTrue);
      });

      test('returns null when no tokens stored', () async {
        SharedPreferences.setMockInitialValues({});

        final authState = await tokenService.loadStoredTokens();

        expect(authState, isNull);
      });

      test('returns null when access token missing', () async {
        SharedPreferences.setMockInitialValues({
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
          'authress_token_expiry': DateTime.now().add(const Duration(hours: 1)).toIso8601String(),
        });

        final authState = await tokenService.loadStoredTokens();

        expect(authState, isNull);
      });

      test('returns null when user profile missing', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_token_expiry': DateTime.now().add(const Duration(hours: 1)).toIso8601String(),
        });

        final authState = await tokenService.loadStoredTokens();

        expect(authState, isNull);
      });

      test('returns null when expiry missing', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
        });

        final authState = await tokenService.loadStoredTokens();

        expect(authState, isNull);
      });

      test('handles corrupted user profile data', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': 'invalid-json',
          'authress_token_expiry': DateTime.now().add(const Duration(hours: 1)).toIso8601String(),
        });

        final authState = await tokenService.loadStoredTokens();

        expect(authState, isNull);
      });

      test('handles invalid expiry date format', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
          'authress_token_expiry': 'invalid-date',
        });

        final authState = await tokenService.loadStoredTokens();

        expect(authState, isNull);
      });
    });

    group('Token Clearing', () {
      test('clears all tokens successfully', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
          'authress_token_expiry': DateTime.now().add(const Duration(hours: 1)).toIso8601String(),
        });

        await tokenService.clearTokens();

        final prefs = await SharedPreferences.getInstance();
        expect(prefs.getString('authress_access_token'), isNull);
        expect(prefs.getString('authress_refresh_token'), isNull);
        expect(prefs.getString('authress_user_profile'), isNull);
        expect(prefs.getString('authress_token_expiry'), isNull);
      });

      test('clears tokens even when some are already missing', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          // refresh token missing
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
          // expiry missing
        });

        await tokenService.clearTokens();

        final prefs = await SharedPreferences.getInstance();
        expect(prefs.getString('authress_access_token'), isNull);
        expect(prefs.getString('authress_user_profile'), isNull);
      });
    });

    group('Token Validation', () {
      test('identifies valid tokens', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
          'authress_token_expiry': DateTime.now().add(const Duration(hours: 1)).toIso8601String(),
        });

        final hasValid = await tokenService.hasValidTokens();

        expect(hasValid, isTrue);
      });

      test('identifies expired tokens as invalid', () async {
        SharedPreferences.setMockInitialValues({
          'authress_access_token': TestData.validAccessToken,
          'authress_refresh_token': TestData.validRefreshToken,
          'authress_user_profile': json.encode(TestData.validUserProfile.toJson()),
          'authress_token_expiry': DateTime.now().subtract(const Duration(hours: 1)).toIso8601String(),
        });

        final hasValid = await tokenService.hasValidTokens();

        expect(hasValid, isFalse);
      });

      test('identifies missing tokens as invalid', () async {
        SharedPreferences.setMockInitialValues({});

        final hasValid = await tokenService.hasValidTokens();

        expect(hasValid, isFalse);
      });
    });

    group('JWT Parsing', () {
      test('parses valid JWT successfully', () {
        const validJwt = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0LXVzZXIiLCJlbWFpbCI6InRlc3RAZW1haWwuY29tIiwibmFtZSI6IlRlc3QgVXNlciIsImV4cCI6OTk5OTk5OTk5OX0.signature';

        final payload = tokenService.parseJwtPayload(validJwt);

        expect(payload, isNotNull);
        expect(payload!['sub'], equals('test-user'));
        expect(payload['email'], equals('test@email.com'));
        expect(payload['name'], equals('Test User'));
        expect(payload['exp'], equals(9999999999));
      });

      test('handles JWT with padding requirements', () {
        // JWT payload that needs padding
        const jwtNeedsPadding = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0In0.signature';

        final payload = tokenService.parseJwtPayload(jwtNeedsPadding);

        expect(payload, isNotNull);
        expect(payload!['sub'], equals('test'));
      });

      test('returns null for malformed JWT', () {
        const malformedJwt = 'not.a.valid.jwt.token';

        final payload = tokenService.parseJwtPayload(malformedJwt);

        expect(payload, isNull);
      });

      test('returns null for JWT with wrong number of parts', () {
        const wrongPartsJwt = 'header.payload'; // Missing signature

        final payload = tokenService.parseJwtPayload(wrongPartsJwt);

        expect(payload, isNull);
      });

      test('returns null for JWT with invalid base64', () {
        const invalidBase64Jwt = 'header.invalid-base64-payload.signature';

        final payload = tokenService.parseJwtPayload(invalidBase64Jwt);

        expect(payload, isNull);
      });

      test('returns null for JWT with invalid JSON in payload', () {
        // Base64 encoded "invalid json"
        const invalidJsonJwt = 'header.aW52YWxpZCBqc29u.signature';

        final payload = tokenService.parseJwtPayload(invalidJsonJwt);

        expect(payload, isNull);
      });
    });

    group('Token Refresh Scheduling', () {
      test('schedules token refresh successfully', () async {
        var refreshCalled = false;
        Future<bool> mockRefresh() async {
          refreshCalled = true;
          return true;
        }

        final expiresAt = DateTime.now().add(const Duration(seconds: 1));
        tokenService.scheduleTokenRefresh(expiresAt, mockRefresh);

        // Wait for refresh to be called (should be scheduled 5 minutes before expiry, but we use 1 second for testing)
        await Future.delayed(const Duration(seconds: 2));

        expect(refreshCalled, isTrue);
      });

      test('does not schedule refresh for already expired tokens', () {
        var refreshCalled = false;
        Future<bool> mockRefresh() async {
          refreshCalled = true;
          return true;
        }

        final expiredTime = DateTime.now().subtract(const Duration(hours: 1));
        tokenService.scheduleTokenRefresh(expiredTime, mockRefresh);

        expect(refreshCalled, isFalse);
      });

      test('does not schedule refresh for tokens expiring too soon', () {
        var refreshCalled = false;
        Future<bool> mockRefresh() async {
          refreshCalled = true;
          return true;
        }

        final soonExpiry = DateTime.now().add(const Duration(minutes: 2)); // Less than 5 minute buffer
        tokenService.scheduleTokenRefresh(soonExpiry, mockRefresh);

        expect(refreshCalled, isFalse);
      });

      test('cancels existing refresh timer when scheduling new one', () async {
        var firstRefreshCalled = false;
        var secondRefreshCalled = false;

        Future<bool> firstRefresh() async {
          firstRefreshCalled = true;
          return true;
        }

        Future<bool> secondRefresh() async {
          secondRefreshCalled = true;
          return true;
        }

        // Schedule first refresh
        final firstExpiry = DateTime.now().add(const Duration(seconds: 2));
        tokenService.scheduleTokenRefresh(firstExpiry, firstRefresh);

        // Schedule second refresh immediately, should cancel first
        final secondExpiry = DateTime.now().add(const Duration(seconds: 1));
        tokenService.scheduleTokenRefresh(secondExpiry, secondRefresh);

        await Future.delayed(const Duration(seconds: 3));

        expect(firstRefreshCalled, isFalse);
        expect(secondRefreshCalled, isTrue);
      });

      test('cancels refresh timer manually', () async {
        var refreshCalled = false;
        Future<bool> mockRefresh() async {
          refreshCalled = true;
          return true;
        }

        final expiresAt = DateTime.now().add(const Duration(seconds: 1));
        tokenService.scheduleTokenRefresh(expiresAt, mockRefresh);
        
        // Cancel immediately
        tokenService.cancelTokenRefresh();

        await Future.delayed(const Duration(seconds: 2));

        expect(refreshCalled, isFalse);
      });

      test('handles refresh callback exceptions gracefully', () async {
        Future<bool> failingRefresh() async {
          throw Exception('Refresh failed');
        }

        final expiresAt = DateTime.now().add(const Duration(seconds: 1));
        
        // Should not throw when callback throws
        expect(
          () => tokenService.scheduleTokenRefresh(expiresAt, failingRefresh),
          returnsNormally,
        );

        await Future.delayed(const Duration(seconds: 2));
      });
    });

    group('Service Lifecycle', () {
      test('disposes and cancels timers', () {
        var refreshCalled = false;
        Future<bool> mockRefresh() async {
          refreshCalled = true;
          return true;
        }

        final expiresAt = DateTime.now().add(const Duration(seconds: 1));
        tokenService.scheduleTokenRefresh(expiresAt, mockRefresh);
        
        // Dispose should cancel timer
        tokenService.dispose();

        expect(refreshCalled, isFalse);
      });
    });
  });
} 