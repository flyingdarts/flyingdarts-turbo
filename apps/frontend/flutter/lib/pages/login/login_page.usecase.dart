import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flyingdarts_core/navigation/navigation_cubit.dart';
import 'package:flyingdarts_shared/wrappers/material.wrappers.dart';
import 'package:widgetbook_annotation/widgetbook_annotation.dart' as widgetbook;

import 'login_page.dart';

@widgetbook.UseCase(
  name: 'The home page in the app',
  type: LoginPage,
)
Widget defaultLoginPage(BuildContext context) {
  return createDefaultMaterialWidget(
    context,
    MultiBlocProvider(
      providers: [
        BlocProvider(
          create: (ctx) => NavigationCubit()..initTest(),
        ),
      ],
      child: const LoginPage(),
    ),
  );
}
