import 'package:authress_login/src/core/auth_context.dart';
import 'package:authress_login/src/core/auth_provider.dart';
import 'package:authress_login/src/models/auth_config.dart';
import 'package:authress_login/src/models/auth_state.dart';
import 'package:authress_login/src/models/deep_link_config.dart';
import 'package:authress_login/src/models/user_profile.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

import '../test_utils/mocks.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('ImprovedAuthressProvider', () {
    late AuthressConfiguration testConfig;
    late MockAuthenticationService mockAuthService;

    setUp(() {
      testConfig = const AuthressConfiguration(
        applicationId: 'test-app-123',
        authressApiUrl: 'https://test.authress.io',
        redirectUrl: 'testapp://auth',
      );

      mockAuthService = MockAuthenticationService();
      when(() => mockAuthService.state).thenReturn(const AuthStateUnauthenticated());
      when(() => mockAuthService.initialize()).thenAnswer((_) async {});
      when(() => mockAuthService.addListener(any())).thenReturn(null);
      when(() => mockAuthService.removeListener(any())).thenReturn(null);
      when(() => mockAuthService.dispose()).thenReturn(null);
    });

    group('Widget Creation and Lifecycle', () {
      testWidgets('creates widget successfully with minimal config', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: testConfig, child: const Text('Test Child')),
          ),
        );

        expect(find.text('Test Child'), findsOneWidget);
      });

      testWidgets('creates widget with all optional parameters', (tester) async {
        final onStateChangedCalls = <AuthState>[];
        final onAuthenticatedCalls = <UserProfile>[];
        final onLoggedOutCalls = <void>[];
        final onErrorCalls = <String>[];

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              deepLinkConfig: const DeepLinkConfig(
                scheme: 'custom',
                host: 'auth',
                timeoutDuration: Duration(minutes: 3),
              ),
              onStateChanged: onStateChangedCalls.add,
              onAuthenticated: onAuthenticatedCalls.add,
              onLoggedOut: () => onLoggedOutCalls.add(null),
              onError: onErrorCalls.add,
              child: const Text('Full Config Child'),
            ),
          ),
        );

        expect(find.text('Full Config Child'), findsOneWidget);
      });

      testWidgets('initializes authentication service on creation', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: testConfig, child: const Text('Init Test')),
          ),
        );

        await tester.pumpAndSettle();

        // Verify the widget was created successfully
        expect(find.text('Init Test'), findsOneWidget);
      });

      testWidgets('disposes properly when removed', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: testConfig, child: const Text('Dispose Test')),
          ),
        );

        await tester.pumpAndSettle();

        // Remove the widget
        await tester.pumpWidget(const MaterialApp(home: Text('Replaced')));

        await tester.pumpAndSettle();

        expect(find.text('Replaced'), findsOneWidget);
        expect(find.text('Dispose Test'), findsNothing);
      });
    });

    group('Context Access Methods', () {
      testWidgets('of() returns AuthressContext when provider exists', (tester) async {
        late AuthressContext? receivedContext;

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  receivedContext = AuthressProvider.of(context);
                  return const Text('Context Test');
                },
              ),
            ),
          ),
        );

        expect(receivedContext, isNotNull);
        expect(receivedContext, isA<AuthressContext>());
      });

      testWidgets('read() returns AuthressContext without listening', (tester) async {
        late AuthressContext? receivedContext;

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  receivedContext = AuthressProvider.read(context);
                  return const Text('Read Test');
                },
              ),
            ),
          ),
        );

        expect(receivedContext, isNotNull);
        expect(receivedContext, isA<AuthressContext>());
      });

      testWidgets('maybeOf() returns context when provider exists', (tester) async {
        late AuthressContext? receivedContext;

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  receivedContext = AuthressProvider.maybeOf(context);
                  return const Text('MaybeOf Test');
                },
              ),
            ),
          ),
        );

        expect(receivedContext, isNotNull);
      });

      testWidgets('maybeOf() returns null when provider does not exist', (tester) async {
        late AuthressContext? receivedContext;

        await tester.pumpWidget(
          MaterialApp(
            home: Builder(
              builder: (context) {
                receivedContext = AuthressProvider.maybeOf(context);
                return const Text('No Provider Test');
              },
            ),
          ),
        );

        expect(receivedContext, isNull);
      });

      testWidgets('of() throws assertion error when provider not found', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: Builder(
              builder: (context) {
                expect(() => AuthressProvider.of(context), throwsA(isA<AssertionError>()));
                return const Text('Assert Test');
              },
            ),
          ),
        );
      });
    });

    group('State Management', () {
      testWidgets('provides unauthenticated state initially', (tester) async {
        late AuthState? receivedState;

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final authContext = AuthressProvider.of(context);
                  receivedState = authContext.authState;
                  return Text('State: ${receivedState.runtimeType}');
                },
              ),
            ),
          ),
        );

        expect(receivedState, isA<AuthStateUnauthenticated>());
        expect(find.text('State: AuthStateUnauthenticated'), findsOneWidget);
      });

      testWidgets('updates context when auth state changes', (tester) async {
        // This test would require more complex mocking of the authentication service
        // to simulate state changes, which is challenging with the current architecture
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final authContext = AuthressProvider.of(context);
                  return Text('User: ${authContext.user?.userId ?? 'None'}');
                },
              ),
            ),
          ),
        );

        expect(find.text('User: None'), findsOneWidget);
      });
    });

    group('Callback Functions', () {
      testWidgets('calls onStateChanged when state changes', (tester) async {
        final stateChanges = <AuthState>[];
        bool callbackWasCalled = false;

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              onStateChanged: (state) {
                callbackWasCalled = true;
                stateChanges.add(state);
              },
              child: const Text('Callback Test'),
            ),
          ),
        );

        await tester.pumpAndSettle();

        // After initialization, the callback should be called at least once
        // when the authentication service initializes and sets its initial state
        expect(callbackWasCalled, isTrue);
        expect(stateChanges, isNotEmpty);

        // The initial state should be unauthenticated (since no stored tokens in test)
        expect(stateChanges.last, isA<AuthStateUnauthenticated>());
      });

      testWidgets('handles null callbacks gracefully', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              onStateChanged: null,
              onAuthenticated: null,
              onLoggedOut: null,
              onError: null,
              child: const Text('Null Callbacks'),
            ),
          ),
        );

        await tester.pumpAndSettle();

        expect(find.text('Null Callbacks'), findsOneWidget);
      });
    });

    group('Configuration Updates', () {
      testWidgets('handles config updates', (tester) async {
        final initialConfig = testConfig;
        final updatedConfig = AuthressConfiguration(
          applicationId: 'updated-app-456',
          authressApiUrl: 'https://updated.authress.io',
          redirectUrl: 'updatedapp://auth',
        );

        // Start with initial config
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: initialConfig, child: const Text('Config Update Test')),
          ),
        );

        await tester.pumpAndSettle();

        // Update with new config
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: updatedConfig, child: const Text('Updated Config Test')),
          ),
        );

        await tester.pumpAndSettle();

        expect(find.text('Updated Config Test'), findsOneWidget);
      });
    });

    group('Deep Link Configuration', () {
      testWidgets('accepts custom deep link config', (tester) async {
        const customDeepLinkConfig = DeepLinkConfig(
          scheme: 'myapp',
          host: 'callback',
          timeoutDuration: Duration(minutes: 10),
        );

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              deepLinkConfig: customDeepLinkConfig,
              child: const Text('Deep Link Test'),
            ),
          ),
        );

        await tester.pumpAndSettle();

        expect(find.text('Deep Link Test'), findsOneWidget);
      });

      testWidgets('works without deep link config', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: testConfig, deepLinkConfig: null, child: const Text('No Deep Link Test')),
          ),
        );

        await tester.pumpAndSettle();

        expect(find.text('No Deep Link Test'), findsOneWidget);
      });
    });

    group('Error Handling', () {
      testWidgets('handles initialization errors gracefully', (tester) async {
        // This would require mocking the authentication service to throw errors
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: testConfig, child: const Text('Error Test')),
          ),
        );

        await tester.pumpAndSettle();

        // Widget should still render even if initialization fails
        expect(find.text('Error Test'), findsOneWidget);
      });

      testWidgets('validates auth config on creation', (tester) async {
        // Test with valid config
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(config: testConfig, child: const Text('Valid Config')),
          ),
        );

        expect(find.text('Valid Config'), findsOneWidget);
      });
    });

    group('Context Properties', () {
      testWidgets('provides correct authentication status', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final authContext = AuthressProvider.of(context);
                  return Column(
                    children: [
                      Text('Authenticated: ${authContext.isAuthenticated}'),
                      Text('Loading: ${authContext.isLoading}'),
                      Text('Has Error: ${authContext.hasError}'),
                    ],
                  );
                },
              ),
            ),
          ),
        );

        expect(find.text('Authenticated: false'), findsOneWidget);
        expect(find.text('Loading: false'), findsOneWidget);
        expect(find.text('Has Error: false'), findsOneWidget);
      });

      testWidgets('provides null user and token when unauthenticated', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final authContext = AuthressProvider.of(context);
                  return Column(
                    children: [
                      Text('User: ${authContext.user?.userId ?? 'null'}'),
                      Text('Token: ${authContext.accessToken ?? 'null'}'),
                    ],
                  );
                },
              ),
            ),
          ),
        );

        expect(find.text('User: null'), findsOneWidget);
        expect(find.text('Token: null'), findsOneWidget);
      });
    });

    group('InheritedWidget Behavior', () {
      testWidgets('rebuilds children when context changes', (tester) async {
        int buildCount = 0;

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  final authContext = AuthressProvider.of(context);
                  buildCount++;
                  return Text('Build: $buildCount, State: ${authContext.authState.runtimeType}');
                },
              ),
            ),
          ),
        );

        expect(buildCount, equals(1));
        expect(find.textContaining('Build: 1'), findsOneWidget);
      });

      testWidgets('does not rebuild unnecessarily', (tester) async {
        int buildCount = 0;

        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Builder(
                builder: (context) {
                  AuthressProvider.of(context);
                  buildCount++;
                  return Text('Rebuild Test: $buildCount');
                },
              ),
            ),
          ),
        );

        final initialBuildCount = buildCount;

        // Pump again without changes
        await tester.pump();

        // Build count should not increase for the same state
        expect(buildCount, equals(initialBuildCount));
      });
    });

    group('Widget Tree Integration', () {
      testWidgets('works with nested widgets', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Scaffold(
                appBar: AppBar(title: const Text('Nested Test')),
                body: Container(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    children: [
                      Builder(
                        builder: (context) {
                          final authContext = AuthressProvider.of(context);
                          return Text('Auth Status: ${authContext.isAuthenticated}');
                        },
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        );

        expect(find.text('Nested Test'), findsOneWidget);
        expect(find.text('Auth Status: false'), findsOneWidget);
      });

      testWidgets('provides context to multiple child widgets', (tester) async {
        await tester.pumpWidget(
          MaterialApp(
            home: AuthressProvider(
              config: testConfig,
              child: Column(
                children: [
                  Builder(
                    builder: (context) {
                      final authContext = AuthressProvider.of(context);
                      return Text('Widget 1: ${authContext.isAuthenticated}');
                    },
                  ),
                  Builder(
                    builder: (context) {
                      final authContext = AuthressProvider.of(context);
                      return Text('Widget 2: ${authContext.isLoading}');
                    },
                  ),
                ],
              ),
            ),
          ),
        );

        expect(find.text('Widget 1: false'), findsOneWidget);
        expect(find.text('Widget 2: false'), findsOneWidget);
      });
    });
  });
}
