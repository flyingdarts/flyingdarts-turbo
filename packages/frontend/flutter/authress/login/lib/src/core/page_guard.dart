import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';

/// Widget that shows different content based on authentication state
class AuthressPageGuard extends StatelessWidget {
  /// Widget to show when authenticated
  final Widget authenticatedChild;

  /// Widget to show when not authenticated (optional)
  final Widget? unauthenticatedChild;

  /// Widget to show during loading (optional)
  final Widget? loadingChild;

  /// Widget to show on error (optional)
  final Widget? errorChild;

  /// Required roles for access (optional)
  final List<String>? requiredRoles;

  /// Required groups for access (optional)
  final List<String>? requiredGroups;

  /// Widget to show when user lacks required roles/groups
  final Widget? accessDeniedChild;

  const AuthressPageGuard({
    super.key,
    required this.authenticatedChild,
    this.unauthenticatedChild,
    this.loadingChild,
    this.errorChild,
    this.requiredRoles,
    this.requiredGroups,
    this.accessDeniedChild,
  });

  @override
  Widget build(BuildContext context) {
    final authContext = context.authress;

    // Show loading state
    if (authContext.isLoading) {
      return loadingChild ?? const Center(child: CircularProgressIndicator());
    }

    // Show error state
    if (authContext.hasError) {
      return errorChild ??
          Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(Icons.error_outline, size: 48, color: Colors.red),
                const SizedBox(height: 16),
                Text('Authentication Error', style: Theme.of(context).textTheme.headlineSmall),
                if (authContext.errorMessage != null) ...[const SizedBox(height: 8), Text(authContext.errorMessage!)],
              ],
            ),
          );
    }

    // Check authentication
    if (!authContext.isAuthenticated) {
      return unauthenticatedChild ?? const Center(child: Text('Please log in to access this content'));
    }

    // Check role requirements
    if (requiredRoles != null && !authContext.hasAllRoles(requiredRoles!)) {
      return accessDeniedChild ??
          const Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.lock_outline, size: 48, color: Colors.orange),
                SizedBox(height: 16),
                Text('Access Denied'),
                SizedBox(height: 8),
                Text('You do not have the required permissions to access this content.'),
              ],
            ),
          );
    }

    // Check group requirements
    if (requiredGroups != null && !authContext.hasAllGroups(requiredGroups!)) {
      return accessDeniedChild ??
          const Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.group_off_outlined, size: 48, color: Colors.orange),
                SizedBox(height: 16),
                Text('Access Denied'),
                SizedBox(height: 8),
                Text('You do not belong to the required groups to access this content.'),
              ],
            ),
          );
    }

    // User is authenticated and has required permissions
    return authenticatedChild;
  }
}
