import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';

import 'core/router/app_router.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return AuthressProvider(
      config: const AuthressConfiguration(
        applicationId: 'app_2YKyhM6M31XVtuCeuDsSJ2',
        authressApiUrl: 'https://authress.flyingdarts.net',
      ),
      deepLinkConfig: const DeepLinkConfig(scheme: 'flyingdarts', host: 'auth', path: ''),
      child: MaterialApp.router(
        title: 'Authress Login Demo v2',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
          useMaterial3: true,
          appBarTheme: const AppBarTheme(centerTitle: true, elevation: 0),
        ),
        routerConfig: AppRouter.router,
        builder: (context, child) {
          // AuthressProvider handles deep links internally
          return child ?? const SizedBox();
        },
      ),
    );
  }
}

/// Example of how to use AuthressGuard widget in your own pages
class ExampleProtectedPage extends StatelessWidget {
  const ExampleProtectedPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Protected Content')),
      body: AuthressPageGuard(
        authenticatedChild: _buildAuthenticatedContent(context),
        unauthenticatedChild: const Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [Icon(Icons.login, size: 48), SizedBox(height: 16), Text('Please log in to view this content')],
          ),
        ),
        loadingChild: const Center(child: CircularProgressIndicator()),
      ),
    );
  }

  Widget _buildAuthenticatedContent(BuildContext context) {
    final authContext = context.authress;

    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Welcome, ${authContext.user?.name ?? 'User'}!', style: Theme.of(context).textTheme.headlineSmall),
          const SizedBox(height: 16),

          // User info card
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('User ID: ${authContext.user?.userId}'),
                  if (authContext.user?.email != null) Text('Email: ${authContext.user!.email}'),
                ],
              ),
            ),
          ),

          const SizedBox(height: 16),

          // Role/Group demonstration
          if (authContext.hasRole('admin'))
            const Chip(
              avatar: Icon(Icons.admin_panel_settings, size: 16),
              label: Text('Admin'),
              backgroundColor: Colors.red,
            ),

          if (authContext.hasGroup('managers'))
            const Chip(
              avatar: Icon(Icons.manage_accounts, size: 16),
              label: Text('Manager'),
              backgroundColor: Colors.blue,
            ),

          const Spacer(),

          // Logout button
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: () async {
                // Using AuthressProvider's extension method
                await context.authress.logout();
              },
              child: const Text('Logout'),
            ),
          ),
        ],
      ),
    );
  }
}
