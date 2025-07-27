import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:keyboard/keyboard.dart';
import 'package:language/language.dart';
import 'package:speech/speech.dart';

import '../../pages/dashboard.dart';
import '../../pages/profile.dart';
import '../../pages/settings.dart';
import '../shell/home_shell.dart';
import 'route_utils.dart';

class AppRouter {
  static GoRouter get router => _router;

  static final GoRouter _router = GoRouter(
    initialLocation: AppRoutes.dashboard.path,
    redirectLimit: 5,
    debugLogDiagnostics: true, // Enable debug logging
    redirect: (context, state) {
      // Debug logging
      print('🔍 GoRouter Debug:');
      print('  - Location: ${state.uri.toString()}');
      print('  - Matched Location: ${state.matchedLocation}');
      print('  - Full Path: ${state.fullPath}');
      print('  - Extra: ${state.extra}');

      // Redirect root path to dashboard
      if (state.matchedLocation == '/') {
        print('  - Redirecting / to ${AppRoutes.dashboard.path}');
        return AppRoutes.dashboard.path;
      }
      print('  - No redirect needed');
      return null;
    },
    routes: [
      // Shell route for the main navigation
      StatefulShellRoute.indexedStack(
        builder: (context, state, navigationShell) {
          return HomeShell(navigationShell: navigationShell);
        },
        branches: [
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: AppRoutes.dashboard.path,
                name: AppRoutes.dashboard.name,
                builder: (context, state) {
                  print('🏗️ Building DashboardPage route');
                  return const DashboardPage();
                },
                routes: [
                  GoRoute(
                    path: AppRoutes.profile.path,
                    name: AppRoutes.profile.name,
                    builder: (context, state) {
                      print('🏗️ Building ProfilePage route');
                      return const ProfilePage();
                    },
                  ),
                  GoRoute(
                    path: AppRoutes.settings.path,
                    name: AppRoutes.settings.name,
                    builder: (context, state) {
                      print('🏗️ Building SettingsPage route');
                      return const SettingsPage();
                    },
                  ),
                ],
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: AppRoutes.speech.path,
                name: AppRoutes.speech.name,
                builder: (context, state) {
                  return MultiBlocProvider(
                    providers: [
                      BlocProvider<LanguageCubit>(
                        create: (context) => GetIt.instance<LanguageCubit>(),
                      ),
                      BlocProvider<SpeechBloc>(
                        create: (context) => GetIt.instance<SpeechBloc>(),
                      ),
                    ],
                    child: SpeechPage(
                      onResult: (text) {
                        print('Recognized: $text');
                      },
                      onError: (error) => print('Error: $error'),
                    ),
                  );
                },
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: AppRoutes.keyboard.path,
                name: AppRoutes.keyboard.name,
                builder: (context, state) {
                  return BlocProvider(
                    create: (context) => GetIt.instance<KeyboardCubit>(),
                    child: const KeyboardPage(),
                  );
                },
              ),
            ],
          ),
        ],
      ),
    ],
    errorBuilder: (context, state) => Scaffold(
      body: Center(
        child: Text(
          state.error.toString(),
        ),
      ),
    ),
  );
}
