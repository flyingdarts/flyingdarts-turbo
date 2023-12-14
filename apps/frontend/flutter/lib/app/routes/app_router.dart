import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:navigation/navigation.dart';
import 'package:go_router/go_router.dart';

import '../../pages/home/home_page.dart';
import '../../pages/login/login_page.dart';
import 'route_utils.dart';

class AppRouter {
  static GoRouter get router => _router;

  static final GoRouter _router = GoRouter(
    routes: [
      GoRoute(
        path: '/',
        name: AppRoutes.home.name,
        builder: (context, state) =>
            context.read<NavigationCubit>().state.isLoggedIn ? const MyHomePage() : const LoginPage(),
      ),
    ],
    errorBuilder: (context, state) => Scaffold(
      body: Center(
        child: Text(state.error.toString()),
      ),
    ),
  );
}
