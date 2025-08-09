import 'package:flyingdarts_authress_login/src/core/auth_context.dart';
import 'package:flyingdarts_authress_login/src/models/deep_link_config.dart';
import 'package:flutter/material.dart';

import '../models/auth_config.dart';
import '../models/auth_state.dart';
import '../models/user_profile.dart';
import '../services/authentication_service.dart';

/// Improved AuthressProvider using the new service architecture
class AuthressProvider extends StatefulWidget {
  /// Authentication configuration
  final AuthressConfiguration config;

  /// Deep link configuration (optional)
  final DeepLinkConfig? deepLinkConfig;

  /// Child widget to wrap
  final Widget child;

  /// Called when authentication state changes
  final void Function(AuthState state)? onStateChanged;

  /// Called when user successfully authenticates
  final void Function(UserProfile user)? onAuthenticated;

  /// Called when user logs out
  final VoidCallback? onLoggedOut;

  /// Called when authentication errors occur
  final void Function(String error)? onError;

  const AuthressProvider({
    super.key,
    required this.config,
    required this.child,
    this.deepLinkConfig,
    this.onStateChanged,
    this.onAuthenticated,
    this.onLoggedOut,
    this.onError,
  });

  /// Get the current AuthressContext from the widget tree
  static AuthressContext of(BuildContext context) {
    final inherited = context.dependOnInheritedWidgetOfExactType<_InheritedAuthress>();
    assert(
      inherited != null,
      'ImprovedAuthressProvider not found in widget tree',
    );
    return inherited!.context;
  }

  /// Get the AuthressContext without listening to changes
  static AuthressContext read(BuildContext context) {
    final inherited = context.getInheritedWidgetOfExactType<_InheritedAuthress>();
    assert(
      inherited != null,
      'ImprovedAuthressProvider not found in widget tree',
    );
    return inherited!.context;
  }

  /// Check if ImprovedAuthressProvider exists in the widget tree
  static AuthressContext? maybeOf(BuildContext context) {
    final inherited = context.dependOnInheritedWidgetOfExactType<_InheritedAuthress>();
    return inherited?.context;
  }

  @override
  State<AuthressProvider> createState() => _AuthressProviderState();
}

class _AuthressProviderState extends State<AuthressProvider> {
  late final AuthenticationService _authService;
  late AuthressContext _context;
  AuthState? _previousState;

  @override
  void initState() {
    super.initState();

    // Validate configuration
    try {
      widget.config.validate();
    } catch (e) {
      debugPrint('❌ AuthConfig validation failed: $e');
      // Could throw here or handle gracefully based on requirements
    }

    // Create authentication service with dependency injection
    _authService = AuthenticationService.create(
      config: widget.config,
      deepLinkConfig: widget.deepLinkConfig,
    );

    // Initialize context
    _context = _createContext(_authService.state);

    // Listen to auth state changes
    _authService.addListener(_onAuthStateChanged);

    // Call initial state callback
    WidgetsBinding.instance.addPostFrameCallback((_) {
      // Trigger initial callback for the starting state
      _handleStateChangeCallbacks(_authService.state);
      _previousState = _authService.state;

      // Then initialize the service
      _initializeAuth();
    });
  }

  @override
  void dispose() {
    _authService.removeListener(_onAuthStateChanged);
    _authService.dispose();
    super.dispose();
  }

  @override
  void didUpdateWidget(AuthressProvider oldWidget) {
    super.didUpdateWidget(oldWidget);

    // If config changed, we might need to recreate the service
    // For simplicity, we'll just validate the new config
    if (oldWidget.config != widget.config) {
      try {
        widget.config.validate();
      } catch (e) {
        debugPrint('❌ Updated AuthConfig validation failed: $e');
      }
    }
  }

  void _onAuthStateChanged() {
    if (!mounted) return;

    final newState = _authService.state;
    final newContext = _createContext(newState);

    if (newContext != _context) {
      setState(() {
        _context = newContext;
      });

      // Call callbacks for state changes
      _handleStateChangeCallbacks(newState);
      _previousState = newState;
    }
  }

  void _handleStateChangeCallbacks(AuthState newState) {
    // General state change callback
    widget.onStateChanged?.call(newState);

    // Specific callbacks based on state type
    switch (newState) {
      case AuthStateAuthenticated(:final user):
        if (_previousState is! AuthStateAuthenticated) {
          widget.onAuthenticated?.call(user);
        }
        break;
      case AuthStateUnauthenticated():
        if (_previousState is AuthStateAuthenticated) {
          widget.onLoggedOut?.call();
        }
        break;
      case AuthStateError(:final message):
        widget.onError?.call(message);
        break;
      case AuthStateLoading():
        // No specific callback for loading
        break;
    }
  }

  AuthressContext _createContext(AuthState state) {
    return AuthressContext(
      authState: state,
      user: state is AuthStateAuthenticated ? state.user : null,
      accessToken: state is AuthStateAuthenticated ? state.accessToken : null,
      authService: _authService,
    );
  }

  Future<void> _initializeAuth() async {
    try {
      debugPrint('🚀 ImprovedAuthressProvider: Initializing authentication...');
      await _authService.initialize();
      debugPrint('✅ ImprovedAuthressProvider: Initialization complete');
    } catch (e) {
      debugPrint('❌ ImprovedAuthressProvider: Initialization failed: $e');
    }
  }

  @override
  Widget build(BuildContext context) {
    return _InheritedAuthress(context: _context, child: widget.child);
  }
}

/// InheritedWidget that provides AuthressContext to the widget tree
class _InheritedAuthress extends InheritedWidget {
  final AuthressContext context;

  const _InheritedAuthress({required this.context, required super.child});

  @override
  bool updateShouldNotify(_InheritedAuthress oldWidget) {
    return context != oldWidget.context;
  }
}
