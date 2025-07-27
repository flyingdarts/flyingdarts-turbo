enum AppRoutes {
  home,
  dashboard,
  speech,
  keyboard,
  profile,
  settings,
}

extension AppRoutesExtension on AppRoutes {
  String get path {
    switch (this) {
      case AppRoutes.home:
        return '/';
      case AppRoutes.dashboard:
        return '/dashboard';
      case AppRoutes.speech:
        return '/speech';
      case AppRoutes.keyboard:
        return '/keyboard';
      case AppRoutes.profile:
        return '/profile';
      case AppRoutes.settings:
        return '/settings';
    }
  }

  String get name {
    switch (this) {
      case AppRoutes.home:
        return 'HOME';
      case AppRoutes.dashboard:
        return 'DASHBOARD';
      case AppRoutes.speech:
        return 'SPEECH';
      case AppRoutes.keyboard:
        return 'KEYBOARD';
      case AppRoutes.profile:
        return 'DASHBOARD/PROFILE';
      case AppRoutes.settings:
        return 'DASHBOARD/SETTINGS';
    }
  }
}
