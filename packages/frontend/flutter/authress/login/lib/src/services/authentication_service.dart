import 'dart:async';
import 'dart:io' show Platform;

import 'package:flyingdarts_authress_login/src/models/deep_link_config.dart';
import 'package:flutter/foundation.dart';
import 'package:url_launcher/url_launcher.dart';

import '../models/auth_config.dart';
import '../models/auth_state.dart';
import '../models/user_profile.dart';
import 'crypto_service.dart';
import 'deep_link_service.dart';
import 'http_service.dart';
import 'token_service.dart';

/// Main authentication service that orchestrates all auth-related operations
class AuthenticationService extends ChangeNotifier {
  final AuthressConfiguration _config;
  final TokenService _tokenService;
  final HttpService _httpService;
  final DeepLinkService _deepLinkService;
  final CryptoService _cryptoService;

  AuthState _state = const AuthStateUnauthenticated();

  AuthenticationService._({
    required AuthressConfiguration config,
    required TokenService tokenService,
    required HttpService httpService,
    required DeepLinkService deepLinkService,
    required CryptoService cryptoService,
  }) : _config = config,
       _tokenService = tokenService,
       _httpService = httpService,
       _deepLinkService = deepLinkService,
       _cryptoService = cryptoService;

  /// Factory constructor with dependency injection
  factory AuthenticationService.create({
    required AuthressConfiguration config,
    DeepLinkConfig? deepLinkConfig,
  }) {
    final tokenService = TokenService();
    final httpService = HttpService(config);
    final deepLinkService = DeepLinkService(
      deepLinkConfig ?? const DeepLinkConfig(),
    );
    final cryptoService = CryptoService();

    return AuthenticationService._(
      config: config,
      tokenService: tokenService,
      httpService: httpService,
      deepLinkService: deepLinkService,
      cryptoService: cryptoService,
    );
  }

  /// Current authentication state
  AuthState get state => _state;

  /// Whether the user is currently authenticated
  bool get isAuthenticated => _state is AuthStateAuthenticated;

  /// Get the current access token if available
  String? get accessToken => _state is AuthStateAuthenticated ? (_state as AuthStateAuthenticated).accessToken : null;

  /// Get the current user profile if available
  UserProfile? get userProfile => _state is AuthStateAuthenticated ? (_state as AuthStateAuthenticated).user : null;

  /// Initialize the service and check for existing sessions
  Future<void> initialize() async {
    await _deepLinkService.initialize();
    await _checkExistingSession();
  }

  /// Check if a user session exists and is valid
  Future<bool> _checkExistingSession() async {
    try {
      final storedAuth = await _tokenService.loadStoredTokens();
      if (storedAuth == null) {
        _setState(const AuthStateUnauthenticated());
        return false;
      }

      // Check if token is expired
      if (storedAuth.isTokenExpired) {
        final refreshed = await _attemptTokenRefresh();
        return refreshed;
      }

      // Token is valid, restore session
      _setState(storedAuth);
      _scheduleTokenRefresh(storedAuth.expiresAt);
      return true;
    } catch (e) {
      debugPrint('Error checking existing session: $e');
      _setState(const AuthStateUnauthenticated());
      return false;
    }
  }

  /// Start authentication flow
  Future<void> authenticate({
    String? connectionId,
    String? tenantLookupIdentifier,
    Map<String, String>? additionalParams,
  }) async {
    _setState(const AuthStateLoading());

    try {
      // Generate authentication URL
      final authUrl = await _generateAuthenticationUrl(
        connectionId: connectionId,
        tenantLookupIdentifier: tenantLookupIdentifier,
        additionalParams: additionalParams,
      );

      // Launch browser for authentication with platform-specific handling
      if (await canLaunchUrl(Uri.parse(authUrl))) {
        if (Platform.isIOS) {
          // Use in-app WebView on iOS to prevent "Return to Safari" button issue
          await launchUrl(
            Uri.parse(authUrl),
            mode: LaunchMode.inAppWebView,
            webViewConfiguration: const WebViewConfiguration(
              enableJavaScript: true,
              enableDomStorage: true,
            ),
          );
        } else {
          // Use external browser on other platforms
          await launchUrl(
            Uri.parse(authUrl),
            mode: LaunchMode.externalApplication,
          );
        }

        // Wait for deep link callback
        final authParams = await _deepLinkService.waitForAuthCallback();

        if (authParams == null) {
          _setState(
            const AuthStateError(
              message: 'Authentication was cancelled or timed out',
            ),
          );
          return;
        }

        // Process authentication result
        await _processAuthenticationCallback(authParams);
      } else {
        _setState(
          const AuthStateError(message: 'Cannot launch authentication URL'),
        );
      }
    } catch (e) {
      _setState(
        AuthStateError(
          message: 'Authentication failed: ${e.toString()}',
          error: e,
        ),
      );
    }
  }

  /// Process authentication callback
  Future<void> _processAuthenticationCallback(
    Map<String, String> params,
  ) async {
    final error = params['error'];
    final code = params['code'];
    final nonce = params['nonce'];

    if (error != null) {
      throw Exception('Authentication error: $error');
    }

    if (code == null || nonce == null) {
      throw Exception('Missing authorization code or nonce in callback');
    }

    // Exchange code for tokens
    await _exchangeCodeForTokens(code, nonce);
  }

  /// Generate authentication URL
  Future<String> _generateAuthenticationUrl({
    String? connectionId,
    String? tenantLookupIdentifier,
    Map<String, String>? additionalParams,
  }) async {
    // Generate PKCE codes
    final pkceCodes = _cryptoService.generatePKCECodes();

    // Store code verifier for later use
    await _storePKCEVerifier(pkceCodes.codeVerifier);

    // Calculate anti-abuse hash
    final antiAbuseHash = await _cryptoService.calculateAntiAbuseHash({
      'connectionId': connectionId,
      'tenantLookupIdentifier': tenantLookupIdentifier,
      'applicationId': _config.applicationId,
    });

    // Build request body
    final requestBody = {
      'antiAbuseHash': antiAbuseHash,
      'redirectUrl': _config.redirectUrl ?? _deepLinkService.callbackUrl,
      'codeChallengeMethod': pkceCodes.codeChallengeMethod,
      'codeChallenge': pkceCodes.codeChallenge,
      'applicationId': _config.applicationId,
      if (connectionId != null) 'connectionId': connectionId,
      if (tenantLookupIdentifier != null) 'tenantLookupIdentifier': tenantLookupIdentifier,
      if (additionalParams != null) ...additionalParams,
    };

    try {
      final response = await _httpService.post(
        '/api/authentication',
        body: requestBody,
      );

      if (!response.isSuccess) {
        throw Exception(
          'Failed to get authentication URL: ${response.statusCode} ${response.body}',
        );
      }

      return response.jsonBody['authenticationUrl'] as String;
    } catch (e) {
      throw Exception('Failed to get authentication URL: $e');
    }
  }

  /// Exchange authorization code for tokens
  Future<void> _exchangeCodeForTokens(String code, String nonce) async {
    final codeVerifier = await _retrievePKCEVerifier();
    if (codeVerifier == null) {
      throw Exception(
        'Code verifier not found - authentication flow corrupted',
      );
    }

    // Calculate anti-abuse hash for token exchange
    final antiAbuseHash = await _cryptoService.calculateAntiAbuseHash({
      'client_id': _config.applicationId,
      'authenticationRequestId': nonce,
      'code': code,
    });

    final requestBody = {
      'grant_type': 'authorization_code',
      'redirect_uri': _config.redirectUrl ?? _deepLinkService.callbackUrl,
      'client_id': _config.applicationId,
      'code': code,
      'code_verifier': codeVerifier,
      'antiAbuseHash': antiAbuseHash,
    };

    try {
      final response = await _httpService.post(
        '/api/authentication/$nonce/tokens',
        body: requestBody,
      );

      if (!response.isSuccess) {
        throw Exception(
          'Token exchange failed: ${response.statusCode} ${response.body}',
        );
      }

      final data = response.jsonBody;
      await _processTokenResponse(data);
    } catch (e) {
      throw Exception('Token exchange failed: $e');
    }
  }

  /// Process token response and update state
  Future<void> _processTokenResponse(Map<String, dynamic> data) async {
    final accessToken = data['access_token'] as String;
    final idToken = data['id_token'] as String;
    final refreshToken = data['refresh_token'] as String?;
    final expiresIn = data['expires_in'] as int? ?? 3600;

    // Parse user profile from ID token
    final payload = _tokenService.parseJwtPayload(idToken);
    if (payload == null) {
      throw Exception('Invalid ID token received');
    }

    final userProfile = UserProfile.fromJson(payload);
    final expiresAt = DateTime.now().add(Duration(seconds: expiresIn));

    // Store tokens
    await _tokenService.storeTokens(
      accessToken: accessToken,
      refreshToken: refreshToken,
      userProfile: userProfile,
      expiresAt: expiresAt,
    );

    // Update state
    final authState = AuthStateAuthenticated(
      user: userProfile,
      accessToken: accessToken,
      refreshToken: refreshToken,
      expiresAt: expiresAt,
    );

    _setState(authState);
    _scheduleTokenRefresh(expiresAt);
  }

  /// Attempt to refresh tokens
  Future<bool> _attemptTokenRefresh() async {
    final storedAuth = await _tokenService.loadStoredTokens();
    if (storedAuth?.refreshToken == null) return false;

    try {
      final response = await _httpService.post(
        '/v1/clients/${_config.applicationId}/oauth/tokens',
        body: {
          'grant_type': 'refresh_token',
          'client_id': _config.applicationId,
          'refresh_token': storedAuth!.refreshToken!,
        },
      );

      if (!response.isSuccess) return false;

      await _processTokenResponse(response.jsonBody);
      return true;
    } catch (e) {
      debugPrint('Token refresh failed: $e');
      return false;
    }
  }

  /// Schedule automatic token refresh
  void _scheduleTokenRefresh(DateTime expiresAt) {
    _tokenService.scheduleTokenRefresh(expiresAt, _attemptTokenRefresh);
  }

  /// Logout current user
  Future<void> logout() async {
    _deepLinkService.cancelAuthFlow();
    await _tokenService.clearTokens();
    _setState(const AuthStateUnauthenticated());
  }

  /// Ensure we have a valid access token
  Future<String?> ensureValidToken() async {
    final currentState = _state;
    if (currentState is! AuthStateAuthenticated) return null;

    if (currentState.isTokenExpired || currentState.willExpireSoon) {
      final refreshed = await _attemptTokenRefresh();
      if (!refreshed) return null;

      // Get the updated state
      final updatedState = _state;
      return updatedState is AuthStateAuthenticated ? updatedState.accessToken : null;
    }

    return currentState.accessToken;
  }

  /// Get user profile from API
  Future<UserProfile?> fetchUserProfile() async {
    final token = await ensureValidToken();
    if (token == null) return null;

    try {
      final response = await _httpService.get(
        '/v1/users/me',
        headers: {'Authorization': 'Bearer $token'},
      );

      if (response.isSuccess) {
        return UserProfile.fromJson(response.jsonBody);
      }
    } catch (e) {
      debugPrint('Error fetching user profile: $e');
    }

    return null;
  }

  /// Store PKCE verifier temporarily
  Future<void> _storePKCEVerifier(String verifier) async {
    // This could be improved by using a more secure storage mechanism
    await _tokenService.storeTokens(
      accessToken: 'temp_verifier_$verifier',
      userProfile: const UserProfile(userId: 'temp'),
      expiresAt: DateTime.now().add(const Duration(minutes: 10)),
    );
  }

  /// Retrieve PKCE verifier
  Future<String?> _retrievePKCEVerifier() async {
    final stored = await _tokenService.loadStoredTokens();
    final token = stored?.accessToken;
    if (token?.startsWith('temp_verifier_') == true) {
      await _tokenService.clearTokens(); // Clean up
      return token!.substring('temp_verifier_'.length);
    }
    return null;
  }

  /// Update internal state and notify listeners
  void _setState(AuthState newState) {
    if (_state != newState) {
      debugPrint(
        '🔄 AuthService: ${_state.runtimeType} → ${newState.runtimeType}',
      );
      _state = newState;
      notifyListeners();
    }
  }

  @override
  void dispose() {
    _tokenService.dispose();
    _httpService.dispose();
    _deepLinkService.dispose();
    super.dispose();
  }
}
