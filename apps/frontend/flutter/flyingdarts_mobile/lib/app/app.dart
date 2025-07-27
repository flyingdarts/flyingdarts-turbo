import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

import 'routes/app_router.dart';

class App extends StatelessWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context) {
    return AuthressProvider(
      config: const AuthressConfiguration(
        applicationId: 'app_2YKyhM6M31XVtuCeuDsSJ2',
        authressApiUrl: 'https://authress.flyingdarts.net',
      ),
      deepLinkConfig: const DeepLinkConfig(
        scheme: 'flyingdarts',
        host: 'auth',
      ),
      child: AuthressPageGuard(
        authenticatedChild: _buildAuthenticatedApp(),
        unauthenticatedChild: Builder(
          builder: (context) => _buildUnauthenticatedApp(context),
        ),
        loadingChild: _buildLoadingApp(),
        errorChild: _buildErrorApp(),
      ),
    );
  }

  Widget _buildUnauthenticatedApp(BuildContext context) {
    return MaterialApp(
      home: FlyingdartsScaffold(
        showAppBar: false,
        child: Padding(
          padding: EdgeInsets.all(20),
          child: Center(
            child: ElevatedButton(
              onPressed: () {
                context.authenticate();
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: MyTheme.secondaryColor,
                foregroundColor: Colors.white,
                padding: EdgeInsets.symmetric(
                  horizontal: 32,
                  vertical: 16,
                ),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: Text(
                'Please login to continue',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildLoadingApp() {
    return MaterialApp(
      home: const FlyingdartsScaffold(
        showAppBar: false,
        child: Center(
          child: LottieWidget(
            assetPath: 'assets/animations/flyingdarts_icon.json',
            width: 100,
            height: 100,
          ),
        ),
      ),
    );
  }

  Widget _buildErrorApp() {
    return MaterialApp(
      home: const FlyingdartsScaffold(
        showAppBar: false,
        child: Center(
          child: Text('Authentication error. Please try again.'),
        ),
      ),
    );
  }

  Widget _buildAuthenticatedApp() {
    return MaterialApp.router(
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.deepPurple,
        ),
        useMaterial3: true,
        appBarTheme: const AppBarTheme(
          centerTitle: true,
          elevation: 0,
        ),
      ),
      routerConfig: AppRouter.router,
    );
  }
}
