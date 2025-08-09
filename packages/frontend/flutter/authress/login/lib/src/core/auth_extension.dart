import 'package:flyingdarts_authress_login/src/core/auth_context.dart';
import 'package:flyingdarts_authress_login/src/core/auth_provider.dart';
import 'package:flyingdarts_authress_login/src/models/auth_state.dart';
import 'package:flyingdarts_authress_login/src/models/user_profile.dart';
import 'package:flutter/material.dart';

/// Extension methods for easier access to authentication
extension AuthressProviderBuilderContextExtension on BuildContext {
  /// Get the current AuthressContext
  AuthressContext get authress => AuthressProvider.of(this);

  /// Get the current authentication state
  AuthState get authState => authress.authState;

  /// Check if user is authenticated
  bool get isAuthenticated => authress.isAuthenticated;

  /// Get current user profile (null if not authenticated)
  UserProfile? get currentUser => authress.user;

  /// Get current access token (null if not authenticated)
  String? get accessToken => authress.accessToken;

  /// Logout the current user
  Future<void> logout() => authress.logout();

  /// Start authentication flow
  Future<void> authenticate({
    String? connectionId,
    String? tenantLookupIdentifier,
    Map<String, String>? additionalParams,
  }) => authress.authenticate(
    connectionId: connectionId,
    tenantLookupIdentifier: tenantLookupIdentifier,
    additionalParams: additionalParams,
  );
}
