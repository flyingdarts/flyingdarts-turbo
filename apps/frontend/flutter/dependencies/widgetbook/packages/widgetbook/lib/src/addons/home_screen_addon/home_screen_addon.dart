import 'package:flutter/material.dart';

import '../../fields/fields.dart';
import '../common/common.dart';
import 'home_screen_setting.dart';

class HomeScreenAddon extends WidgetbookAddon<HomeScreenSetting> {
  HomeScreenAddon({required bool isConfigured})
      : assert(isConfigured),
        super(
          name: 'Homescreen',
          initialSetting: const HomeScreenSetting(),
        );

  @override
  List<Field> get fields {
    return [];
  }

  @override
  HomeScreenSetting valueFromQueryGroup(Map<String, String> group) {
    return const HomeScreenSetting();
  }

  @override
  Widget buildUseCase(
    BuildContext context,
    Widget child,
    HomeScreenSetting config,
  ) {
    return child;
  }
}
