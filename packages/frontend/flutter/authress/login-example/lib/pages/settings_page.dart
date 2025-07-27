import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

/// Settings page for app configuration
class SettingsPage extends StatelessWidget {
  const SettingsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final authContext = context.authress;
    
    return Scaffold(
      backgroundColor: const Color(0xff1d2f3b),
      appBar: AppBar(
        title: const Text(
          'Settings',
          style: TextStyle(fontWeight: FontWeight.w600),
        ),
        backgroundColor: Colors.transparent,
        elevation: 0,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Account section
            _buildSection(
              context,
              'Account',
              Icons.person_outline,
              Colors.blue,
              [
                _buildSettingTile(
                  context,
                  'Profile',
                  'Manage your profile information',
                  Icons.person_outline,
                  () => context.push('/profile'),
                ),
                _buildSettingTile(
                  context,
                  'Privacy',
                  'Control your privacy settings',
                  Icons.privacy_tip_outlined,
                  () => _showComingSoon(context),
                ),
              ],
            ),
            
            const SizedBox(height: 32),
            
            // Security section
            _buildSection(
              context,
              'Security',
              Icons.security_outlined,
              Colors.green,
              [
                _buildSettingTile(
                  context,
                  'Authentication',
                  'Manage your authentication settings',
                  Icons.verified_user_outlined,
                  () => _showComingSoon(context),
                ),
                _buildSettingTile(
                  context,
                  'Access Tokens',
                  'View and manage access tokens',
                  Icons.key_outlined,
                  () => _showTokenInfo(context, authContext.accessToken),
                ),
              ],
            ),
            
            const SizedBox(height: 32),
            
            // App section
            _buildSection(
              context,
              'Application',
              Icons.settings_outlined,
              Colors.grey,
              [
                _buildSettingTile(
                  context,
                  'Theme',
                  'Customize app appearance',
                  Icons.palette_outlined,
                  () => _showComingSoon(context),
                ),
                _buildSettingTile(
                  context,
                  'Notifications',
                  'Control notification preferences',
                  Icons.notifications_outlined,
                  () => _showComingSoon(context),
                ),
                _buildSettingTile(
                  context,
                  'About',
                  'App information and version',
                  Icons.info_outline,
                  () => _showAbout(context),
                ),
              ],
            ),
            
            const SizedBox(height: 32),
            
            // Danger zone
            _buildDangerSection(context),
          ],
        ),
      ),
    );
  }

  Widget _buildSection(
    BuildContext context,
    String title,
    IconData icon,
    Color color,
    List<Widget> children,
  ) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: color.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(icon, size: 20, color: color),
            ),
            const SizedBox(width: 12),
            Text(
              title,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w600,
              ),
            ),
          ],
        ),
        
        const SizedBox(height: 16),
        
        Card(
          elevation: 2,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          child: Column(
            children: children,
          ),
        ),
      ],
    );
  }

  Widget _buildSettingTile(
    BuildContext context,
    String title,
    String subtitle,
    IconData icon,
    VoidCallback onTap,
  ) {
    return ListTile(
      leading: Icon(icon, color: Colors.grey.shade600),
      title: Text(
        title,
        style: const TextStyle(fontWeight: FontWeight.w500),
      ),
      subtitle: Text(subtitle),
      trailing: const Icon(Icons.arrow_forward_ios, size: 16),
      onTap: onTap,
    );
  }

  Widget _buildDangerSection(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.red.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: const Icon(Icons.warning_outlined, size: 20, color: Colors.red),
            ),
            const SizedBox(width: 12),
            const Text(
              'Danger Zone',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w600,
                color: Colors.red,
              ),
            ),
          ],
        ),
        
        const SizedBox(height: 16),
        
        Card(
          elevation: 2,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
            side: BorderSide(color: Colors.red.withValues(alpha: 0.3)),
          ),
          child: Column(
            children: [
              ListTile(
                leading: const Icon(Icons.logout, color: Colors.red),
                title: const Text(
                  'Logout',
                  style: TextStyle(
                    fontWeight: FontWeight.w500,
                    color: Colors.red,
                  ),
                ),
                subtitle: const Text('Sign out of your account'),
                trailing: const Icon(Icons.arrow_forward_ios, size: 16, color: Colors.red),
                onTap: () => _showLogoutDialog(context),
              ),
              const Divider(height: 1),
              ListTile(
                leading: const Icon(Icons.delete_forever, color: Colors.red),
                title: const Text(
                  'Delete Account',
                  style: TextStyle(
                    fontWeight: FontWeight.w500,
                    color: Colors.red,
                  ),
                ),
                subtitle: const Text('Permanently delete your account'),
                trailing: const Icon(Icons.arrow_forward_ios, size: 16, color: Colors.red),
                onTap: () => _showComingSoon(context),
              ),
            ],
          ),
        ),
      ],
    );
  }

  void _showComingSoon(BuildContext context) {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('This feature is coming soon!'),
        duration: Duration(seconds: 2),
      ),
    );
  }

  void _showTokenInfo(BuildContext context, String? accessToken) {
    if (accessToken == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('No access token available')),
      );
      return;
    }

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Access Token'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Your current access token:'),
            const SizedBox(height: 12),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.grey.shade100,
                borderRadius: BorderRadius.circular(8),
              ),
              child: SelectableText(
                accessToken.length > 50 
                    ? '${accessToken.substring(0, 50)}...'
                    : accessToken,
                style: const TextStyle(
                  fontFamily: 'monospace',
                  fontSize: 12,
                ),
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'Token length: ${accessToken.length} characters',
              style: TextStyle(
                fontSize: 12,
                color: Colors.grey.shade600,
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  void _showAbout(BuildContext context) {
    showAboutDialog(
      context: context,
      applicationName: 'Authress Demo',
      applicationVersion: '2.0.0',
      applicationLegalese: 'Built with Flutter and Authress',
      children: [
        const SizedBox(height: 16),
        const Text('This demo showcases the Authress authentication system.'),
        const SizedBox(height: 8),
        const Text('Features:'),
        const Text('• Secure OAuth 2.1 + PKCE authentication'),
        const Text('• Role and group-based access control'),
        const Text('• Deep link handling'),
        const Text('• Clean architecture pattern'),
      ],
    );
  }

  void _showLogoutDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Logout'),
        content: const Text('Are you sure you want to logout?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () async {
              Navigator.of(context).pop();
              await context.authress.logout();
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red,
              foregroundColor: Colors.white,
            ),
            child: const Text('Logout'),
          ),
        ],
      ),
    );
  }
} 