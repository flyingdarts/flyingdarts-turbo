import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../pages/home_page.dart';
import '../../pages/login_page.dart';
import '../../pages/profile_page.dart';
import '../../pages/settings_page.dart';
import '../../pages/loading_page.dart';
import '../../pages/error_page.dart';

class AppRouter {
  static final GoRouter _router = GoRouter(
    initialLocation: '/',
    redirect: (context, state) {
      // Use the AuthressRouteGuard for general authentication checks
      return AuthressRouteGuard.redirectLogic(context, state);
    },
    routes: [
      // Public routes
      GoRoute(
        path: '/login',
        name: 'login',
        builder: (context, state) => const LoginPage(),
      ),

      // Main authenticated routes
      GoRoute(
        path: '/',
        name: 'home',
        builder: (context, state) => const HomePage(),
      ),

      GoRoute(
        path: '/profile',
        name: 'profile',
        builder: (context, state) => const ProfilePage(),
      ),

      // Settings - requires authentication (protected by redirect logic)
      GoRoute(
        path: '/settings',
        name: 'settings',
        builder: (context, state) => const SettingsPage(),
      ),

      // Admin routes - demonstrates role-based access
      GoRoute(
        path: '/admin',
        name: 'admin',
        redirect: (context, state) {
          // First check authentication, then check for admin role
          return AuthressRouteGuard.roleGuard(
            context,
            state,
            requiredRoles: ['admin'],
            redirectTo: '/unauthorized',
          );
        },
        builder: (context, state) => const AdminPage(),
      ),

      // Manager routes - demonstrates group-based access
      GoRoute(
        path: '/manager',
        name: 'manager',
        redirect: (context, state) {
          // Check for manager group membership
          return AuthressRouteGuard.groupGuard(
            context,
            state,
            requiredGroups: ['managers'],
            redirectTo: '/unauthorized',
          );
        },
        builder: (context, state) => const ManagerPage(),
      ),

      // Error pages
      GoRoute(
        path: '/unauthorized',
        name: 'unauthorized',
        builder: (context, state) => const UnauthorizedPage(),
      ),

      GoRoute(
        path: '/error',
        name: 'error',
        builder: (context, state) => const ErrorPage(),
      ),

      // Loading page (useful for deep link handling)
      GoRoute(
        path: '/loading',
        name: 'loading',
        builder: (context, state) => const LoadingPage(),
      ),
    ],
    errorBuilder: (context, state) => const ErrorPage(),
  );

  static GoRouter get router => _router;
}

// Example admin page
class AdminPage extends StatelessWidget {
  const AdminPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Admin Panel'),
        backgroundColor: Colors.red.shade100,
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.admin_panel_settings, size: 48, color: Colors.red),
            SizedBox(height: 16),
            Text(
              'Admin Panel',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 8),
            Text('This page requires admin role'),
          ],
        ),
      ),
    );
  }
}

// Example manager page
class ManagerPage extends StatelessWidget {
  const ManagerPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Manager Dashboard'),
        backgroundColor: Colors.blue.shade100,
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.manage_accounts, size: 48, color: Colors.blue),
            SizedBox(height: 16),
            Text(
              'Manager Dashboard',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 8),
            Text('This page requires managers group membership'),
          ],
        ),
      ),
    );
  }
}

// Unauthorized page
class UnauthorizedPage extends StatelessWidget {
  const UnauthorizedPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Access Denied'),
        backgroundColor: Colors.orange.shade100,
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.lock_outline, size: 64, color: Colors.orange),
            const SizedBox(height: 16),
            const Text(
              'Access Denied',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            const Text('You do not have permission to access this resource.'),
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: () => context.go('/'),
              child: const Text('Go Home'),
            ),
          ],
        ),
      ),
    );
  }
}
