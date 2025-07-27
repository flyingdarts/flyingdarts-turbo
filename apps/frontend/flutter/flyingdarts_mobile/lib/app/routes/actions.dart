import 'package:flyingdarts_mobile/app/routes/route_utils.dart';

enum RouteActions {
  profile,
  settings,
  logout,
}

extension RouteActionsExtension on RouteActions {
  String get name {
    switch (this) {
      case RouteActions.profile:
        return 'profile';
      case RouteActions.settings:
        return 'settings';
      case RouteActions.logout:
        return 'logout';
    }
  }

  AppRoutes get route {
    switch (this) {
      case RouteActions.profile:
        return AppRoutes.profile;
      case RouteActions.settings:
        return AppRoutes.settings;
      case RouteActions.logout:
        return AppRoutes.home;
    }
  }

  String uppercased() {
    return '${name[0].toUpperCase()}${name.substring(1)}';
  }
}
