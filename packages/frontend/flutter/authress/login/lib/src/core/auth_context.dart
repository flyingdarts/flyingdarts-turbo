import 'package:authress_login/src/models/auth_state.dart';
import 'package:authress_login/src/models/user_profile.dart';
import 'package:authress_login/src/services/authentication_service.dart';

/// Improved AuthressContext with cleaner API and better error handling
class AuthressContext {
  /// Current authentication state
  final AuthState authState;

  /// Current user profile (null if not authenticated)
  final UserProfile? user;

  /// Access token for API calls (null if not authenticated)
  final String? accessToken;

  /// Internal reference to the authentication service
  final AuthenticationService? _authService;

  const AuthressContext({
    required this.authState,
    this.user,
    this.accessToken,
    AuthenticationService? authService,
  }) : _authService = authService;

  /// Empty context for initial state
  const AuthressContext.initial()
    : authState = const AuthStateUnauthenticated(),
      user = null,
      accessToken = null,
      _authService = null;

  /// Whether the user is authenticated
  bool get isAuthenticated => authState is AuthStateAuthenticated;

  /// Whether the user is loading/authenticating
  bool get isLoading => authState is AuthStateLoading;

  /// Whether there's an authentication error
  bool get hasError => authState is AuthStateError;

  /// Get error message if any
  String? get errorMessage {
    return authState is AuthStateError
        ? (authState as AuthStateError).message
        : null;
  }

  /// Start authentication flow
  Future<void> authenticate({
    String? connectionId,
    String? tenantLookupIdentifier,
    Map<String, String>? additionalParams,
  }) async {
    if (_authService == null) {
      throw StateError(
        'AuthressContext is not initialized with authentication service',
      );
    }

    await _authService.authenticate(
      connectionId: connectionId,
      tenantLookupIdentifier: tenantLookupIdentifier,
      additionalParams: additionalParams,
    );
  }

  /// Logout current user
  Future<void> logout() async {
    if (_authService == null) {
      throw StateError(
        'AuthressContext is not initialized with authentication service',
      );
    }
    await _authService.logout();
  }

  /// Get a valid access token, refreshing if necessary
  Future<String?> getValidToken() async {
    if (_authService == null) return accessToken;
    return _authService.ensureValidToken();
  }

  /// Refresh user profile from server
  Future<UserProfile?> refreshUserProfile() async {
    if (_authService == null) return user;
    return _authService.fetchUserProfile();
  }

  /// Check if user has a specific role
  bool hasRole(String roleName) {
    if (user?.claims == null) return false;

    final claims = user!.claims!;

    // Check various possible locations for roles
    if (claims['roles'] is List) {
      return (claims['roles'] as List).contains(roleName);
    }

    if (claims['role'] is String) {
      return claims['role'] == roleName;
    }

    if (claims['user_roles'] is List) {
      return (claims['user_roles'] as List).contains(roleName);
    }

    return false;
  }

  /// Check if user belongs to a specific group
  bool hasGroup(String groupName) {
    if (user?.claims == null) return false;

    final claims = user!.claims!;

    // Check various possible locations for groups
    if (claims['groups'] is List) {
      return (claims['groups'] as List).contains(groupName);
    }

    if (claims['group'] is String) {
      return claims['group'] == groupName;
    }

    if (claims['user_groups'] is List) {
      return (claims['user_groups'] as List).contains(groupName);
    }

    return false;
  }

  /// Check if user has any of the specified roles
  bool hasAnyRole(List<String> roleNames) {
    return roleNames.any(hasRole);
  }

  /// Check if user has all of the specified roles
  bool hasAllRoles(List<String> roleNames) {
    return roleNames.every(hasRole);
  }

  /// Check if user has any of the specified groups
  bool hasAnyGroup(List<String> groupNames) {
    return groupNames.any(hasGroup);
  }

  /// Check if user has all of the specified groups
  bool hasAllGroups(List<String> groupNames) {
    return groupNames.every(hasGroup);
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is AuthressContext &&
        other.authState == authState &&
        other.user == user &&
        other.accessToken == accessToken;
  }

  @override
  int get hashCode => Object.hash(authState, user, accessToken);

  @override
  String toString() {
    return 'AuthressContext(authenticated: $isAuthenticated, user: ${user?.userId})';
  }
}
