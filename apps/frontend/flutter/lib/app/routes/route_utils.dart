enum AppRoutes {
  splash,
  home,
  login,
}

extension AppRoutesExtension on AppRoutes {
  String get path {
    switch (this) {
      case AppRoutes.splash:
        return '/';
      case AppRoutes.home:
        return 'home';
      case AppRoutes.login:
        return 'login';
    }
  }

  String get name {
    switch (this) {
      case AppRoutes.splash:
        return 'SPLASH';
      case AppRoutes.home:
        return 'HOME';
      case AppRoutes.login:
        return 'LOGIN';
    }
  }
}
