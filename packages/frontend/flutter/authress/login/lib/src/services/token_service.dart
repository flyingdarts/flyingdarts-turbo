import 'dart:async';
import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/auth_state.dart';
import '../models/user_profile.dart';

/// Service responsible for token management, storage, and refresh logic
class TokenService {
  static const String _tokenKey = 'authress_access_token';
  static const String _refreshTokenKey = 'authress_refresh_token';
  static const String _userProfileKey = 'authress_user_profile';
  static const String _tokenExpiryKey = 'authress_token_expiry';

  Timer? _tokenRefreshTimer;

  /// Store tokens securely with user profile
  Future<void> storeTokens({
    required String accessToken,
    String? refreshToken,
    required UserProfile userProfile,
    required DateTime expiresAt,
  }) async {
    final prefs = await SharedPreferences.getInstance();
    await Future.wait([
      prefs.setString(_tokenKey, accessToken),
      prefs.setString(_userProfileKey, json.encode(userProfile.toJson())),
      prefs.setString(_tokenExpiryKey, expiresAt.toIso8601String()),
      if (refreshToken != null) prefs.setString(_refreshTokenKey, refreshToken),
    ]);
  }

  /// Load stored tokens and validate expiry
  Future<AuthStateAuthenticated?> loadStoredTokens() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      
      final token = prefs.getString(_tokenKey);
      final userProfileJson = prefs.getString(_userProfileKey);
      final expiryStr = prefs.getString(_tokenExpiryKey);
      final refreshToken = prefs.getString(_refreshTokenKey);

      if (token == null || userProfileJson == null || expiryStr == null) {
        return null;
      }

      final userProfile = UserProfile.fromJson(json.decode(userProfileJson));
      final expiry = DateTime.parse(expiryStr);

      return AuthStateAuthenticated(
        user: userProfile,
        accessToken: token,
        refreshToken: refreshToken,
        expiresAt: expiry,
      );
    } catch (e) {
      debugPrint('Error loading stored tokens: $e');
      return null;
    }
  }

  /// Clear all stored tokens
  Future<void> clearTokens() async {
    final prefs = await SharedPreferences.getInstance();
    await Future.wait([
      prefs.remove(_tokenKey),
      prefs.remove(_refreshTokenKey),
      prefs.remove(_userProfileKey),
      prefs.remove(_tokenExpiryKey),
    ]);
  }

  /// Check if tokens exist and are valid
  Future<bool> hasValidTokens() async {
    final authState = await loadStoredTokens();
    return authState != null && !authState.isTokenExpired;
  }

  /// Schedule automatic token refresh
  void scheduleTokenRefresh(DateTime expiresAt, Future<bool> Function() refreshCallback) {
    cancelTokenRefresh();

    final refreshTime = expiresAt.subtract(const Duration(minutes: 5));
    final delay = refreshTime.difference(DateTime.now());

    if (delay.isNegative) return;

    _tokenRefreshTimer = Timer(delay, () async {
      try {
        await refreshCallback();
      } catch (e) {
        debugPrint('Automatic token refresh failed: $e');
      }
    });
  }

  /// Cancel any scheduled token refresh
  void cancelTokenRefresh() {
    _tokenRefreshTimer?.cancel();
    _tokenRefreshTimer = null;
  }

  /// Parse JWT payload safely
  Map<String, dynamic>? parseJwtPayload(String token) {
    try {
      final parts = token.split('.');
      if (parts.length != 3) return null;

      String payload = parts[1];
      
      // Add padding if needed
      switch (payload.length % 4) {
        case 2: payload += '=='; break;
        case 3: payload += '='; break;
      }

      final decodedBytes = base64Decode(payload);
      final decodedString = utf8.decode(decodedBytes);
      return json.decode(decodedString) as Map<String, dynamic>;
    } catch (e) {
      debugPrint('JWT parsing error: $e');
      return null;
    }
  }

  void dispose() {
    cancelTokenRefresh();
  }
} 