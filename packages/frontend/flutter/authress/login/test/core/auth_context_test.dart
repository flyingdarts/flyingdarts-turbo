import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:authress_login/src/core/auth_context.dart';
import 'package:authress_login/src/models/auth_state.dart';
import 'package:authress_login/src/models/user_profile.dart';
import '../test_utils/mocks.dart';

void main() {
  group('AuthressContext', () {
    late MockAuthenticationService mockAuthService;

    setUp(() {
      mockAuthService = MockAuthenticationService();

      // Set up fallback values
      registerFallbackValue(TestData.validUserProfile);
      registerFallbackValue(DateTime.now());
      registerFallbackValue(<String, String?>{});
    });

    group('Initialization', () {
      test('creates initial context', () {
        const context = AuthressContext.initial();

        expect(context.authState, isA<AuthStateUnauthenticated>());
        expect(context.user, isNull);
        expect(context.accessToken, isNull);
        expect(context.isAuthenticated, isFalse);
        expect(context.isLoading, isFalse);
        expect(context.hasError, isFalse);
        expect(context.errorMessage, isNull);
      });

      test('creates context with auth state', () {
        const context = AuthressContext(
          authState: AuthStateLoading(),
        );

        expect(context.authState, isA<AuthStateLoading>());
        expect(context.isLoading, isTrue);
        expect(context.isAuthenticated, isFalse);
        expect(context.hasError, isFalse);
      });

      test('creates authenticated context', () {
        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
          authService: mockAuthService,
        );

        expect(context.authState, isA<AuthStateAuthenticated>());
        expect(context.user, equals(TestData.validUserProfile));
        expect(context.accessToken, equals(TestData.validAccessToken));
        expect(context.isAuthenticated, isTrue);
        expect(context.isLoading, isFalse);
        expect(context.hasError, isFalse);
      });

      test('creates error context', () {
        const errorState = AuthStateError(message: 'Test error');
        const context = AuthressContext(authState: errorState);

        expect(context.authState, equals(errorState));
        expect(context.hasError, isTrue);
        expect(context.errorMessage, equals('Test error'));
        expect(context.isAuthenticated, isFalse);
        expect(context.isLoading, isFalse);
      });
    });

    group('Role Checking', () {
      test('hasRole returns true for existing role in roles array', () {
        final userWithRoles = UserProfile(
          userId: 'test-user',
          email: 'test@example.com',
          claims: {
            'roles': ['admin', 'user', 'moderator'],
          },
        );

        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: userWithRoles,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
          user: userWithRoles,
          accessToken: 'token',
        );

        expect(context.hasRole('admin'), isTrue);
        expect(context.hasRole('user'), isTrue);
        expect(context.hasRole('moderator'), isTrue);
        expect(context.hasRole('guest'), isFalse);
      });

      test('hasRole returns true for role string', () {
        final userWithRole = UserProfile(
          userId: 'test-user',
          email: 'test@example.com',
          claims: {
            'role': 'admin',
          },
        );

        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: userWithRole,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
          user: userWithRole,
          accessToken: 'token',
        );

        expect(context.hasRole('admin'), isTrue);
        expect(context.hasRole('user'), isFalse);
      });

      test('hasRole checks user_roles claim', () {
        final userWithUserRoles = UserProfile(
          userId: 'test-user',
          email: 'test@example.com',
          claims: {
            'user_roles': ['admin', 'editor'],
          },
        );

        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: userWithUserRoles,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
          user: userWithUserRoles,
          accessToken: 'token',
        );

        expect(context.hasRole('admin'), isTrue);
        expect(context.hasRole('editor'), isTrue);
        expect(context.hasRole('viewer'), isFalse);
      });

      test('hasRole returns false when not authenticated', () {
        const context = AuthressContext.initial();

        expect(context.hasRole('admin'), isFalse);
        expect(context.hasRole('user'), isFalse);
      });

      test('hasRole returns false when no claims', () {
        final userWithoutClaims = UserProfile(
          userId: 'test-user',
          email: 'test@example.com',
          claims: null,
        );

        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: userWithoutClaims,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
          user: userWithoutClaims,
          accessToken: 'token',
        );

        expect(context.hasRole('admin'), isFalse);
      });

      test(
        'hasAnyRole returns true if user has any of the specified roles',
        () {
          final context = AuthressContext(
            authState: TestData.validAuthenticatedState,
            user: TestData.validUserProfile,
            accessToken: TestData.validAccessToken,
          );

          expect(context.hasAnyRole(['admin', 'guest']), isTrue);
          expect(context.hasAnyRole(['guest', 'visitor']), isFalse);
          expect(context.hasAnyRole(['admin', 'user']), isTrue);
        },
      );

      test('hasAllRoles returns true if user has all specified roles', () {
        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        expect(context.hasAllRoles(['admin', 'user']), isTrue);
        expect(context.hasAllRoles(['admin', 'guest']), isFalse);
        expect(context.hasAllRoles(['admin']), isTrue);
      });
    });

    group('Group Checking', () {
      test('hasGroup returns true for existing group in groups array', () {
        final userWithGroups = UserProfile(
          userId: 'test-user',
          email: 'test@example.com',
          claims: {
            'groups': ['developers', 'testers', 'admins'],
          },
        );

        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: userWithGroups,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
          user: userWithGroups,
          accessToken: 'token',
        );

        expect(context.hasGroup('developers'), isTrue);
        expect(context.hasGroup('testers'), isTrue);
        expect(context.hasGroup('admins'), isTrue);
        expect(context.hasGroup('guests'), isFalse);
      });

      test('hasGroup returns true for group string', () {
        final userWithGroup = UserProfile(
          userId: 'test-user',
          email: 'test@example.com',
          claims: {
            'group': 'developers',
          },
        );

        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: userWithGroup,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
          user: userWithGroup,
          accessToken: 'token',
        );

        expect(context.hasGroup('developers'), isTrue);
        expect(context.hasGroup('testers'), isFalse);
      });

      test('hasGroup checks user_groups claim', () {
        final userWithUserGroups = UserProfile(
          userId: 'test-user',
          email: 'test@example.com',
          claims: {
            'user_groups': ['engineering', 'qa'],
          },
        );

        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: userWithUserGroups,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
          user: userWithUserGroups,
          accessToken: 'token',
        );

        expect(context.hasGroup('engineering'), isTrue);
        expect(context.hasGroup('qa'), isTrue);
        expect(context.hasGroup('sales'), isFalse);
      });

      test('hasGroup returns false when not authenticated', () {
        const context = AuthressContext.initial();

        expect(context.hasGroup('developers'), isFalse);
        expect(context.hasGroup('testers'), isFalse);
      });

      test(
        'hasAnyGroup returns true if user has any of the specified groups',
        () {
          final context = AuthressContext(
            authState: TestData.validAuthenticatedState,
            user: TestData.validUserProfile,
            accessToken: TestData.validAccessToken,
          );

          expect(context.hasAnyGroup(['developers', 'sales']), isTrue);
          expect(context.hasAnyGroup(['sales', 'marketing']), isFalse);
          expect(context.hasAnyGroup(['developers', 'testers']), isTrue);
        },
      );

      test('hasAllGroups returns true if user has all specified groups', () {
        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        expect(context.hasAllGroups(['developers', 'testers']), isTrue);
        expect(context.hasAllGroups(['developers', 'sales']), isFalse);
        expect(context.hasAllGroups(['developers']), isTrue);
      });
    });

    group('Authentication Methods', () {
      test('authenticate calls service with parameters', () async {
        when(
          () => mockAuthService.authenticate(
            connectionId: any(named: 'connectionId'),
            tenantLookupIdentifier: any(named: 'tenantLookupIdentifier'),
            additionalParams: any(named: 'additionalParams'),
          ),
        ).thenAnswer((_) async {});

        final context = AuthressContext(
          authState: const AuthStateUnauthenticated(),
          authService: mockAuthService,
        );

        await context.authenticate(
          connectionId: 'test-connection',
          tenantLookupIdentifier: 'test-tenant',
          additionalParams: {'custom': 'value'},
        );

        verify(
          () => mockAuthService.authenticate(
            connectionId: 'test-connection',
            tenantLookupIdentifier: 'test-tenant',
            additionalParams: {'custom': 'value'},
          ),
        ).called(1);
      });

      test('authenticate throws when no service', () async {
        const context = AuthressContext(authState: AuthStateUnauthenticated());

        expect(
          () => context.authenticate(),
          throwsA(isA<StateError>()),
        );
      });

      test('logout calls service', () async {
        when(() => mockAuthService.logout()).thenAnswer((_) async {});

        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          authService: mockAuthService,
        );

        await context.logout();

        verify(() => mockAuthService.logout()).called(1);
      });

      test('logout throws when no service', () async {
        final context = AuthressContext(
          authState: AuthStateAuthenticated(
            user: TestData.validUserProfile,
            accessToken: 'token',
            expiresAt: DateTime.now().add(const Duration(hours: 1)),
          ),
        );

        expect(
          () => context.logout(),
          throwsA(isA<StateError>()),
        );
      });
    });

    group('Token Management', () {
      test('getValidToken calls service', () async {
        when(
          () => mockAuthService.ensureValidToken(),
        ).thenAnswer((_) async => TestData.validAccessToken);

        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          authService: mockAuthService,
        );

        final token = await context.getValidToken();

        expect(token, equals(TestData.validAccessToken));
        verify(() => mockAuthService.ensureValidToken()).called(1);
      });

      test('getValidToken returns accessToken when no service', () async {
        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        final token = await context.getValidToken();

        expect(token, equals(TestData.validAccessToken));
      });

      test('refreshUserProfile calls service', () async {
        when(
          () => mockAuthService.fetchUserProfile(),
        ).thenAnswer((_) async => TestData.validUserProfile);

        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          authService: mockAuthService,
        );

        final profile = await context.refreshUserProfile();

        expect(profile, equals(TestData.validUserProfile));
        verify(() => mockAuthService.fetchUserProfile()).called(1);
      });

      test('refreshUserProfile returns current user when no service', () async {
        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        final profile = await context.refreshUserProfile();

        expect(profile, equals(TestData.validUserProfile));
      });
    });

    group('Equality and Hashing', () {
      test('contexts with same data are equal', () {
        final context1 = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        final context2 = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        expect(context1, equals(context2));
        expect(context1.hashCode, equals(context2.hashCode));
      });

      test('contexts with different data are not equal', () {
        final context1 = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        const context2 = AuthressContext(
          authState: AuthStateUnauthenticated(),
        );

        expect(context1, isNot(equals(context2)));
        expect(context1.hashCode, isNot(equals(context2.hashCode)));
      });

      test('identical contexts are equal', () {
        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        expect(context, equals(context));
      });
    });

    group('String Representation', () {
      test('toString shows authentication status and user info', () {
        final context = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        final string = context.toString();

        expect(string, contains('AuthressContext'));
        expect(string, contains('authenticated: true'));
        expect(string, contains('user: ${TestData.validUserId}'));
      });

      test('toString shows unauthenticated status', () {
        const context = AuthressContext.initial();

        final string = context.toString();

        expect(string, contains('AuthressContext'));
        expect(string, contains('authenticated: false'));
        expect(string, contains('user: null'));
      });
    });

    group('State Properties', () {
      test('isAuthenticated reflects AuthStateAuthenticated', () {
        final authenticatedContext = AuthressContext(
          authState: TestData.validAuthenticatedState,
          user: TestData.validUserProfile,
          accessToken: TestData.validAccessToken,
        );

        const unauthenticatedContext = AuthressContext(
          authState: AuthStateUnauthenticated(),
        );

        expect(authenticatedContext.isAuthenticated, isTrue);
        expect(unauthenticatedContext.isAuthenticated, isFalse);
      });

      test('isLoading reflects AuthStateLoading', () {
        const loadingContext = AuthressContext(
          authState: AuthStateLoading(),
        );

        const nonLoadingContext = AuthressContext(
          authState: AuthStateUnauthenticated(),
        );

        expect(loadingContext.isLoading, isTrue);
        expect(nonLoadingContext.isLoading, isFalse);
      });

      test('hasError reflects AuthStateError', () {
        const errorContext = AuthressContext(
          authState: AuthStateError(message: 'Test error'),
        );

        const nonErrorContext = AuthressContext(
          authState: AuthStateUnauthenticated(),
        );

        expect(errorContext.hasError, isTrue);
        expect(errorContext.errorMessage, equals('Test error'));
        expect(nonErrorContext.hasError, isFalse);
        expect(nonErrorContext.errorMessage, isNull);
      });
    });
  });
}
