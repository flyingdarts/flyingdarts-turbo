import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

/// Route guard that protects routes based on authentication state
class AuthressRouteGuard {
  /// Check if the user is authenticated and redirect if needed
  static String? redirectLogic(BuildContext context, GoRouterState state) {
    final authContext = context.authress;

    final isLoginRoute = state.matchedLocation.startsWith('/login');
    final isAuthRoute = state.matchedLocation.startsWith('/auth');
    final isPublicRoute = _isPublicRoute(state.matchedLocation);

    // If we're in a loading state, stay on current route
    if (authContext.isLoading) {
      debugPrint(
        '🔄 AuthressRouteGuard: Loading state, staying on current route',
      );
      return null;
    }

    // If authenticated and trying to access login, redirect to home
    if (authContext.isAuthenticated && isLoginRoute) {
      debugPrint(
        '✅ AuthressRouteGuard: Authenticated user trying to access login, redirecting to /home',
      );
      return '/home';
    }

    // If not authenticated and trying to access protected route
    if (!authContext.isAuthenticated &&
        !isLoginRoute &&
        !isAuthRoute &&
        !isPublicRoute) {
      debugPrint(
        '🔒 AuthressRouteGuard: Unauthenticated user trying to access protected route, redirecting to login',
      );
      return '/login?redirect=${Uri.encodeComponent(state.matchedLocation)}';
    }

    // Allow navigation
    return null;
  }

  /// Check if the current user has required roles for a route
  static String? roleGuard(
    BuildContext context,
    GoRouterState state, {
    required List<String> requiredRoles,
    String? redirectTo,
  }) {
    final authContext = context.authress;

    // First check authentication
    final authRedirect = redirectLogic(context, state);
    if (authRedirect != null) return authRedirect;

    // If authenticated, check roles
    if (authContext.isAuthenticated == true) {
      final hasRequiredRoles = authContext.hasAllRoles(requiredRoles);
      if (!hasRequiredRoles) {
        debugPrint(
          '🚫 AuthressRouteGuard: User lacks required roles: $requiredRoles',
        );
        return redirectTo ?? '/unauthorized';
      }
    }

    return null;
  }

  /// Check if the current user belongs to required groups for a route
  static String? groupGuard(
    BuildContext context,
    GoRouterState state, {
    required List<String> requiredGroups,
    String? redirectTo,
  }) {
    final authContext = context.authress;

    // First check authentication
    final authRedirect = redirectLogic(context, state);
    if (authRedirect != null) return authRedirect;

    // If authenticated, check groups
    if (authContext.isAuthenticated == true) {
      final hasRequiredGroups = authContext.hasAllGroups(requiredGroups);
      if (!hasRequiredGroups) {
        debugPrint(
          '🚫 AuthressRouteGuard: User lacks required groups: $requiredGroups',
        );
        return redirectTo ?? '/unauthorized';
      }
    }

    return null;
  }

  /// Helper to check if a route is public (doesn't require authentication)
  static bool _isPublicRoute(String location) {
    final publicRoutes = [
      '/login',
      '/auth',
      '/signup',
      '/forgot-password',
      '/reset-password',
      '/privacy',
      '/terms',
      '/about',
      '/contact',
      '/unauthorized',
      '/error',
    ];

    return publicRoutes.any((route) => location.startsWith(route));
  }
}
