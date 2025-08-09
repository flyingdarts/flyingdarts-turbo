import 'package:flyingdarts_authress_login/flyingdarts_authress_login.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:ui/ui.dart';

import '../app/routes/actions.dart';
import '../app/routes/route_utils.dart';

/// Dashboard page - one of the bottom tabs
class DashboardPage extends StatelessWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    return FlyingdartsScaffold(
      showActionBar: true,
      actionButtons: ActionButtonsBuilder(
        context: context,
        onMenuItemSelected: (value) {
          if (value == RouteActions.logout) {
            context.logout();
          } else {
            context.go('${AppRoutes.dashboard.path}${value.route.path}');
          }
        },
      ).buildAllActionButtons(),
      child: SingleChildScrollView(
        padding: EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Dashboard',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
            SizedBox(height: 16),
            Text(
              'Hi ${context.currentUser?.name}',
              style: TextStyle(color: Colors.white70),
            ),
            SizedBox(height: 24),
            // Add your dashboard content here
            // QrCodeScannerWidget(),
            SizedBox(height: 24),
            Text(
              'Game Features',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
            SizedBox(height: 16),
            Card(
              color: Colors.white.withOpacity(0.05),
              child: ListTile(
                leading: Icon(Icons.analytics, color: Colors.white30),
                title: Text(
                  'Statistics',
                  style: TextStyle(color: Colors.white30),
                ),
                subtitle: Text(
                  'Coming Soon',
                  style: TextStyle(color: Colors.white30),
                ),
                onTap: null, // Disabled
              ),
            ),
            SizedBox(height: 16),
            Card(
              color: Colors.white.withOpacity(0.05),
              child: ListTile(
                leading: Icon(Icons.history, color: Colors.white30),
                title: Text(
                  'Recent Games',
                  style: TextStyle(color: Colors.white30),
                ),
                subtitle: Text(
                  'Coming Soon',
                  style: TextStyle(color: Colors.white30),
                ),
                onTap: null, // Disabled
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Builder class for creating action buttons in the home page
class ActionButtonsBuilder {
  final BuildContext context;
  final Function(RouteActions) onMenuItemSelected;

  const ActionButtonsBuilder({
    required this.context,
    required this.onMenuItemSelected,
  });

  /// Builds the profile menu item
  PopupMenuItem<String> buildProfileMenuItem() {
    return PopupMenuItem(
      value: RouteActions.profile.route.path,
      child: Row(
        children: [
          Icon(Icons.person_outline, size: 20),
          SizedBox(width: 12),
          Text(
            RouteActions.profile.uppercased(),
          ),
        ],
      ),
    );
  }

  /// Builds the settings menu item
  PopupMenuItem<String> buildSettingsMenuItem() {
    return PopupMenuItem(
      value: RouteActions.settings.route.path,
      child: Row(
        children: [
          Icon(Icons.settings_outlined, size: 20),
          SizedBox(width: 12),
          Text(
            RouteActions.settings.uppercased(),
          ),
        ],
      ),
    );
  }

  /// Builds the menu divider
  PopupMenuDivider buildMenuDivider() {
    return const PopupMenuDivider(height: 1);
  }

  /// Builds the logout menu item
  PopupMenuItem<String> buildLogoutMenuItem() {
    return const PopupMenuItem(
      value: 'logout',
      child: Row(
        children: [
          Icon(
            Icons.logout,
            size: 20,
            color: Colors.red,
          ),
          SizedBox(width: 12),
          Text(
            'Logout',
            style: TextStyle(color: Colors.red),
          ),
        ],
      ),
    );
  }

  /// Builds the user avatar button
  Widget buildUserAvatarButton() {
    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: CircleAvatar(
        radius: 16,
        backgroundColor: Theme.of(context).primaryColor,
        child: context.currentUser?.picture != null
            ? ClipOval(
                child: Image.network(
                  context.currentUser!.picture!,
                  width: 32,
                  height: 32,
                  fit: BoxFit.cover,
                  errorBuilder: (context, error, stackTrace) {
                    return const Icon(
                      Icons.person,
                      size: 16,
                      color: Colors.white,
                    );
                  },
                ),
              )
            : const Icon(
                Icons.person,
                size: 16,
                color: Colors.white,
              ),
      ),
    );
  }

  /// Builds the complete popup menu button with all menu items
  PopupMenuButton<String> buildPopupMenuButton() {
    return PopupMenuButton<String>(
      onSelected: (value) {
        if (value == 'logout') {
          onMenuItemSelected(RouteActions.logout);
        } else {
          onMenuItemSelected(
            RouteActions.values.firstWhere((e) {
              return e.route.path == value;
            }),
          );
        }
      },
      itemBuilder: (context) => [
        buildProfileMenuItem(),
        buildSettingsMenuItem(),
        buildMenuDivider(),
        buildLogoutMenuItem(),
      ],
      child: buildUserAvatarButton(),
    );
  }

  /// Builds all action buttons for the home page
  List<Widget> buildAllActionButtons() {
    return [
      buildPopupMenuButton(),
    ];
  }
}
