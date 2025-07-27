import 'user_profile.dart';

/// Represents the current authentication state
sealed class AuthState {
  const AuthState();
}

/// User is not authenticated
class AuthStateUnauthenticated extends AuthState {
  const AuthStateUnauthenticated();

  @override
  String toString() => 'AuthStateUnauthenticated';
}

/// Authentication is in progress
class AuthStateLoading extends AuthState {
  const AuthStateLoading();

  @override
  String toString() => 'AuthStateLoading';
}

/// User is authenticated with profile and token
class AuthStateAuthenticated extends AuthState {
  final UserProfile user;
  final String accessToken;
  final String? refreshToken;
  final DateTime expiresAt;

  const AuthStateAuthenticated({
    required this.user,
    required this.accessToken,
    this.refreshToken,
    required this.expiresAt,
  });

  /// Check if the token is expired
  bool get isTokenExpired => DateTime.now().isAfter(expiresAt);

  /// Check if the token will expire soon (within 5 minutes)
  bool get willExpireSoon => DateTime.now().add(const Duration(minutes: 5)).isAfter(expiresAt);

  @override
  String toString() {
    return 'AuthStateAuthenticated(user: ${user.email}, tokenExpired: $isTokenExpired)';
  }
}

/// Authentication failed
class AuthStateError extends AuthState {
  final String message;
  final Object? error;

  const AuthStateError({required this.message, this.error});

  @override
  String toString() => 'AuthStateError(message: $message)';
}
