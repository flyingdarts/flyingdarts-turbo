import 'package:amplify_authenticator/amplify_authenticator.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:get_it/get_it.dart';
import 'package:language/language.dart';
import 'package:navigation/navigation.dart';
import 'package:internationalization/internationalization.dart';
import 'package:profile/profile.dart';
import 'package:ui/ui.dart';
import 'package:loading_animation_widget/loading_animation_widget.dart';
import 'package:provider/provider.dart';

import 'routes/app_router.dart';

/// App widget that wraps the flutter app.
class App extends StatelessWidget {
  /// Constructor
  const App({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    var cubit = context.watch<NavigationCubit>();
    return Authenticator(
      child: MaterialApp.router(
        theme: myTheme,
        routeInformationParser: AppRouter.router.routeInformationParser,
        routeInformationProvider: AppRouter.router.routeInformationProvider,
        routerDelegate: AppRouter.router.routerDelegate,
        builder: (ctx, child) {
          if (cubit.state.isLoading) {
            return Scaffold(
              body: Center(
                child: LoadingAnimationWidget.staggeredDotsWave(
                  color: Colors.white,
                  size: 200,
                ),
              ),
            );
          } else {
            GetIt.I<ProfileCubit>().init();
            GetIt.I<LanguageCubit>().init();

            return child!;
          }
        },
        localizationsDelegates: const [
          AppLocalizations.delegate,
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
        ],
        supportedLocales: const [
          Locale('en'),
          Locale('nl'),
        ],
      ),
    );
  }
}
