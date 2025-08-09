import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flyingdarts_authress_login/src/core/auth_extension.dart';
import 'package:flyingdarts_authress_login/src/core/auth_provider.dart';
import 'package:flyingdarts_authress_login/src/models/auth_config.dart';
import 'package:flyingdarts_authress_login/src/models/auth_state.dart';
import 'package:flyingdarts_authress_login/src/models/user_profile.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('AuthExtension', () {
    final testConfig = AuthressConfiguration(
      applicationId: 'test-app',
      authressApiUrl: 'https://test.authress.io',
      redirectUrl: 'test://callback',
    );

    group('Extension Methods', () {
      testWidgets('extension methods exist on BuildContext', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  // Test that extension methods are available
                  expect(() => context.authState, returnsNormally);
                  expect(() => context.isAuthenticated, returnsNormally);
                  expect(() => context.currentUser, returnsNormally);
                  expect(() => context.accessToken, returnsNormally);
                  return const SizedBox();
                },
              ),
            ),
          ),
        );
      });

      testWidgets('authState returns current state', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final state = context.authState;
                  expect(state, isA<AuthState>());
                  return Text('State: ${state.runtimeType}');
                },
              ),
            ),
          ),
        );

        expect(find.textContaining('State:'), findsOneWidget);
      });

      testWidgets('isAuthenticated reflects auth state', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final isAuth = context.isAuthenticated;
                  return Text('Authenticated: $isAuth');
                },
              ),
            ),
          ),
        );

        expect(find.textContaining('Authenticated:'), findsOneWidget);
      });

      testWidgets('currentUser returns user profile', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final user = context.currentUser;
                  return Text('User: ${user?.userId ?? 'None'}');
                },
              ),
            ),
          ),
        );

        expect(find.textContaining('User:'), findsOneWidget);
      });

      testWidgets('accessToken returns token', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final token = context.accessToken;
                  return Text('Token: ${token ?? 'None'}');
                },
              ),
            ),
          ),
        );

        expect(find.textContaining('Token:'), findsOneWidget);
      });
    });

    group('Extension Method Calls', () {
      testWidgets('logout method is callable', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  return ElevatedButton(
                    onPressed: () {
                      // Should not throw
                      expect(() => context.logout(), returnsNormally);
                    },
                    child: const Text('Logout'),
                  );
                },
              ),
            ),
          ),
        );

        await tester.tap(find.byType(ElevatedButton));
        await tester.pumpAndSettle();
      });

      testWidgets('authenticate method is callable', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  return ElevatedButton(
                    onPressed: () {
                      // Should not throw
                      expect(() => context.authenticate(), returnsNormally);
                    },
                    child: const Text('Login'),
                  );
                },
              ),
            ),
          ),
        );

        await tester.tap(find.byType(ElevatedButton));
        await tester.pumpAndSettle();
      });

      testWidgets('authenticate with parameters is callable', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  return ElevatedButton(
                    onPressed: () {
                      expect(
                        () => context.authenticate(
                          connectionId: 'test-conn',
                          tenantLookupIdentifier: 'test-tenant',
                          additionalParams: {'key': 'value'},
                        ),
                        returnsNormally,
                      );
                    },
                    child: const Text('Login with params'),
                  );
                },
              ),
            ),
          ),
        );

        await tester.tap(find.byType(ElevatedButton));
        await tester.pumpAndSettle();
      });
    });

    group('Error Handling', () {
      testWidgets('throws when provider not found', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: Builder(
              builder: (context) {
                expect(() => context.authress, throwsA(isA<AssertionError>()));
                return const SizedBox();
              },
            ),
          ),
        );
      });
    });

    group('UserProfile Claims Access', () {
      test('UserProfile can store claims', () {
        const user = UserProfile(
          userId: 'test-user',
          name: 'Test User',
          claims: {
            'roles': ['admin', 'user'],
            'groups': ['developers'],
            'custom_field': 'custom_value',
          },
        );

        expect(user.claims?['roles'], equals(['admin', 'user']));
        expect(user.claims?['groups'], equals(['developers']));
        expect(user.claims?['custom_field'], equals('custom_value'));
      });

      test('UserProfile handles null claims', () {
        const user = UserProfile(userId: 'test-user', name: 'Test User');

        expect(user.claims, isNull);
      });

      test('UserProfile fromJson includes claims', () {
        final json = {
          'userId': 'test-123',
          'name': 'Test User',
          'email': 'test@example.com',
          'roles': ['admin'],
          'groups': ['developers'],
          'custom_data': 'value',
        };

        final user = UserProfile.fromJson(json);

        expect(user.userId, equals('test-123'));
        expect(user.name, equals('Test User'));
        expect(user.email, equals('test@example.com'));
        expect(user.claims?['roles'], equals(['admin']));
        expect(user.claims?['groups'], equals(['developers']));
        expect(user.claims?['custom_data'], equals('value'));
      });
    });
  });
}
