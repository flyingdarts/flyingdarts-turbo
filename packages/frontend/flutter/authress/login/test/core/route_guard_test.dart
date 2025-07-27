import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:go_router/go_router.dart';

import '../../../login/lib/src/core/auth_context.dart';
import '../../../login/lib/src/core/page_guard.dart';
import '../../../login/lib/src/core/route_guard.dart';
import '../../../login/lib/src/models/auth_state.dart';
import '../../../login/lib/src/models/user_profile.dart';
import '../../../login/lib/src/services/authentication_service.dart';

// Mock for GoRouterState
class MockGoRouterState extends Mock implements GoRouterState {}

// Mock for AuthenticationService
class MockAuthenticationService extends Mock implements AuthenticationService {}

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('AuthressRouteGuard', () {
    late MockGoRouterState mockRouterState;

    setUp(() {
      mockRouterState = MockGoRouterState();
    });

    group('redirectLogic', () {
      testWidgets('allows navigation when authenticated and not accessing login', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/dashboard');

        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(userId: 'user-123'),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.redirectLogic(context, mockRouterState);
                expect(result, isNull);
                return const SizedBox();
              },
            ),
          ),
        );
      });

      testWidgets('redirects to home when authenticated user tries to access login', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/login');

        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(userId: 'user-123'),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.redirectLogic(context, mockRouterState);
                expect(result, equals('/'));
                return const SizedBox();
              },
            ),
          ),
        );
      });

      testWidgets('redirects to login when unauthenticated user tries to access protected route', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/dashboard');

        final authContext = AuthressContext(authState: AuthStateUnauthenticated());

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.redirectLogic(context, mockRouterState);
                expect(result, equals('/login?redirect=${Uri.encodeComponent('/dashboard')}'));
                return const SizedBox();
              },
            ),
          ),
        );
      });

      testWidgets('allows access to public routes when unauthenticated', (tester) async {
        final publicRoutes = ['/login', '/auth', '/signup', '/privacy', '/terms', '/about'];

        for (final route in publicRoutes) {
          when(() => mockRouterState.matchedLocation).thenReturn(route);

          final authContext = AuthressContext(authState: AuthStateUnauthenticated());

          await tester.pumpWidget(
            TestAuthressProvider(
              authContext: authContext,
              child: Builder(
                builder: (context) {
                  final result = AuthressRouteGuard.redirectLogic(context, mockRouterState);
                  expect(result, isNull, reason: 'Public route $route should allow access');
                  return const SizedBox();
                },
              ),
            ),
          );
        }
      });

      testWidgets('handles unauthenticated state with loading', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/dashboard');

        final authContext = AuthressContext(authState: AuthStateLoading());

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.redirectLogic(context, mockRouterState);
                expect(result, isNull, reason: 'Loading state should stay on current route');
                return const SizedBox();
              },
            ),
          ),
        );
      });

      testWidgets('handles error state gracefully', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/dashboard');

        final authContext = AuthressContext(authState: AuthStateError(message: 'Test error'));

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.redirectLogic(context, mockRouterState);
                expect(
                  result,
                  equals('/login?redirect=${Uri.encodeComponent('/dashboard')}'),
                  reason: 'Error state should redirect to login',
                );
                return const SizedBox();
              },
            ),
          ),
        );
      });
    });

    group('roleGuard', () {
      testWidgets('allows access when user has required roles', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/admin');

        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'roles': ['admin'],
              },
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.roleGuard(context, mockRouterState, requiredRoles: ['admin']);
                expect(result, isNull);
                return const SizedBox();
              },
            ),
          ),
        );
      });

      testWidgets('denies access when user lacks required roles', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/admin');

        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'roles': ['user'],
              }, // Missing admin role
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.roleGuard(
                  context,
                  mockRouterState,
                  requiredRoles: ['admin'],
                  redirectTo: '/access-denied',
                );
                expect(result, equals('/access-denied'));
                return const SizedBox();
              },
            ),
          ),
        );
      });

      testWidgets('handles unauthenticated users', (tester) async {
        when(() => mockRouterState.matchedLocation).thenReturn('/admin');

        final authContext = AuthressContext(authState: AuthStateUnauthenticated());

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: Builder(
              builder: (context) {
                final result = AuthressRouteGuard.roleGuard(context, mockRouterState, requiredRoles: ['admin']);
                expect(result, equals('/login?redirect=${Uri.encodeComponent('/admin')}'));
                return const SizedBox();
              },
            ),
          ),
        );
      });
    });

    group('AuthressGuard Widget', () {
      testWidgets('shows authenticated child when user is authenticated', (tester) async {
        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(userId: 'user-123'),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(authenticatedChild: Text('Welcome User')),
          ),
        );

        expect(find.text('Welcome User'), findsOneWidget);
      });

      testWidgets('shows unauthenticated child when user is not authenticated', (tester) async {
        final authContext = AuthressContext(authState: AuthStateUnauthenticated());

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(
              authenticatedChild: Text('Welcome User'),
              unauthenticatedChild: Text('Please Login'),
            ),
          ),
        );

        expect(find.text('Please Login'), findsOneWidget);
        expect(find.text('Welcome User'), findsNothing);
      });

      testWidgets('shows loading child when in loading state', (tester) async {
        final authContext = AuthressContext(authState: AuthStateLoading());

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(
              authenticatedChild: Text('Welcome User'),
              unauthenticatedChild: Text('Please Login'),
              loadingChild: Text('Loading...'),
            ),
          ),
        );

        expect(find.text('Loading...'), findsOneWidget);
        expect(find.text('Welcome User'), findsNothing);
        expect(find.text('Please Login'), findsNothing);
      });

      testWidgets('shows error child when in error state', (tester) async {
        final authContext = AuthressContext(authState: AuthStateError(message: 'Authentication failed'));

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(
              authenticatedChild: Text('Welcome User'),
              errorChild: Text('Error occurred'),
            ),
          ),
        );

        expect(find.text('Error occurred'), findsOneWidget);
        expect(find.text('Welcome User'), findsNothing);
      });

      testWidgets('enforces role requirements', (tester) async {
        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'roles': ['user'],
              }, // No admin role
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(authenticatedChild: Text('Admin Panel'), requiredRoles: ['admin']),
          ),
        );

        expect(find.text('Access Denied'), findsOneWidget);
        expect(find.text('Admin Panel'), findsNothing);
      });

      testWidgets('allows access when user has required roles', (tester) async {
        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'roles': ['admin'],
              },
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(authenticatedChild: Text('Admin Panel'), requiredRoles: ['admin']),
          ),
        );

        expect(find.text('Admin Panel'), findsOneWidget);
        expect(find.text('Access Denied'), findsNothing);
      });

      testWidgets('enforces group requirements', (tester) async {
        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'roles': ['admin'],
                'groups': ['qa'], // Missing developers group
              },
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(authenticatedChild: Text('Developer Tools'), requiredGroups: ['developers']),
          ),
        );

        expect(find.text('Access Denied'), findsOneWidget);
        expect(find.text('Developer Tools'), findsNothing);
      });

      testWidgets('allows access when user has required groups', (tester) async {
        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'groups': ['developers'],
              },
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(authenticatedChild: Text('Developer Tools'), requiredGroups: ['developers']),
          ),
        );

        expect(find.text('Developer Tools'), findsOneWidget);
        expect(find.text('Access Denied'), findsNothing);
      });

      testWidgets('enforces both role and group requirements', (tester) async {
        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'roles': ['user'], // Missing admin role
                'groups': ['developers'],
              },
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(
              authenticatedChild: Text('Admin Developer Tools'),
              requiredRoles: ['admin'],
              requiredGroups: ['developers'],
            ),
          ),
        );

        expect(find.text('Access Denied'), findsOneWidget);
      });

      testWidgets('shows access denied when user fails group requirement', (tester) async {
        final authContext = AuthressContext(
          authState: AuthStateAuthenticated(
            user: const UserProfile(
              userId: 'user-123',
              claims: {
                'roles': ['admin'],
                'groups': ['qa'], // Missing developers group
              },
            ),
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        await tester.pumpWidget(
          TestAuthressProvider(
            authContext: authContext,
            child: const AuthressPageGuard(
              authenticatedChild: Text('Admin Developer Tools'),
              requiredRoles: ['admin'],
              requiredGroups: ['developers'],
            ),
          ),
        );

        expect(find.text('Access Denied'), findsOneWidget);
      });
    });
  });
}

/// Test helper widget that provides a proper ImprovedAuthressProvider context for testing
class TestAuthressProvider extends StatelessWidget {
  final AuthressContext authContext;
  final Widget child;

  const TestAuthressProvider({super.key, required this.authContext, required this.child});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: _InheritedAuthress(
        context: authContext,
        child: Scaffold(body: child),
      ),
    );
  }
}

/// Test version of _InheritedAuthress that provides the same interface as the real one
/// This must have the exact same name and signature as the private class in auth_provider.dart
class _InheritedAuthress extends InheritedWidget {
  final AuthressContext context;

  const _InheritedAuthress({required this.context, required super.child});

  @override
  bool updateShouldNotify(_InheritedAuthress oldWidget) {
    return context != oldWidget.context;
  }
}
