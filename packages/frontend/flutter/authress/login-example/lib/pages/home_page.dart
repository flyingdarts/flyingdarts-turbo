import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

/// Home page shown after successful authentication
class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final authContext = context.authress;
    
    // Handle loading state
    if (authContext.isLoading) {
      return const Scaffold(
        backgroundColor: Color(0xff1d2f3b),
        body: Center(
          child: CircularProgressIndicator(),
        ),
      );
    }
    
    // Handle error state
    if (authContext.hasError) {
      return Scaffold(
        backgroundColor: const Color(0xff1d2f3b),
        body: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline, size: 64, color: Colors.red),
              const SizedBox(height: 16),
              Text(
                'Authentication Error',
                style: Theme.of(context).textTheme.headlineSmall,
              ),
              if (authContext.errorMessage != null) ...[
                const SizedBox(height: 8),
                Text(authContext.errorMessage!),
              ],
            ],
          ),
        ),
      );
    }
    
    // Handle case where user is null
    if (authContext.user == null) {
      return const Scaffold(
        backgroundColor: Color(0xff1d2f3b),
        body: Center(
          child: Text('No user information available'),
        ),
      );
    }
    
    final user = authContext.user!;
    
    return Scaffold(
      backgroundColor: const Color(0xff1d2f3b),
      appBar: AppBar(
        title: const Text(
          'Dashboard',
          style: TextStyle(fontWeight: FontWeight.w600),
        ),
        backgroundColor: Colors.transparent,
        elevation: 0,
        actions: [
          PopupMenuButton<String>(
            onSelected: (value) {
              switch (value) {
                case 'profile':
                  context.push('/profile');
                  break;
                case 'settings':
                  context.push('/settings');
                  break;
                case 'logout':
                  _showLogoutDialog(context);
                  break;
              }
            },
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    Icon(Icons.person_outline, size: 20),
                    SizedBox(width: 12),
                    Text('Profile'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'settings',
                child: Row(
                  children: [
                    Icon(Icons.settings_outlined, size: 20),
                    SizedBox(width: 12),
                    Text('Settings'),
                  ],
                ),
              ),
              const PopupMenuDivider(),
              const PopupMenuItem(
                value: 'logout',
                child: Row(
                  children: [
                    Icon(Icons.logout, size: 20, color: Colors.red),
                    SizedBox(width: 12),
                    Text('Logout', style: TextStyle(color: Colors.red)),
                  ],
                ),
              ),
            ],
            child: Padding(
              padding: const EdgeInsets.all(8.0),
              child: CircleAvatar(
                radius: 16,
                backgroundColor: Theme.of(context).primaryColor,
                child: user.picture != null
                    ? ClipOval(
                        child: Image.network(
                          user.picture!,
                          width: 32,
                          height: 32,
                          fit: BoxFit.cover,
                          errorBuilder: (context, error, stackTrace) {
                            return const Icon(Icons.person, size: 16, color: Colors.white);
                          },
                        ),
                      )
                    : const Icon(Icons.person, size: 16, color: Colors.white),
              ),
            ),
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Welcome header
            _buildWelcomeHeader(user),
            
            const SizedBox(height: 32),
            
            // User info card
            _buildUserInfoCard(authContext),
            
            const SizedBox(height: 24),
            
            // Permissions & roles card
            _buildPermissionsCard(authContext),
            
            const SizedBox(height: 24),
            
            // Quick actions
            _buildQuickActions(context),
            
            const SizedBox(height: 24),
            
            // Navigation cards
            _buildNavigationCards(context, authContext),
          ],
        ),
      ),
    );
  }

  Widget _buildWelcomeHeader(UserProfile? user) {
    final greeting = _getGreeting();
    final displayName = user?.name ?? user?.email?.split('@').first ?? 'User';
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          greeting,
          style: const TextStyle(
            fontSize: 16,
            color: Colors.grey,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          displayName,
          style: const TextStyle(
            fontSize: 28,
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildUserInfoCard(AuthressContext authContext) {
    final user = authContext.user!;
    
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(
                  Icons.account_circle_outlined,
                  size: 24,
                  color: Colors.blue.shade600,
                ),
                const SizedBox(width: 12),
                const Text(
                  'Account Information',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
            
            const SizedBox(height: 16),
            const Divider(),
            const SizedBox(height: 16),
            
            _buildInfoRow('User ID', user.userId),
            if (user.email != null) _buildInfoRow('Email', user.email!),
            if (user.name != null) _buildInfoRow('Name', user.name!),
            
            if (user.lastLoginDate != null)
              _buildInfoRow(
                'Last Login',
                _formatDate(user.lastLoginDate!),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildPermissionsCard(AuthressContext authContext) {
    final user = authContext.user!;
    final claims = user.claims ?? {};
    
    // Extract roles and groups
    final roles = <String>[];
    final groups = <String>[];
    
    // Check for roles in various claim formats
    if (claims['roles'] is List) {
      roles.addAll((claims['roles'] as List).cast<String>());
    }
    if (claims['role'] is String) {
      roles.add(claims['role'] as String);
    }
    if (claims['user_roles'] is List) {
      roles.addAll((claims['user_roles'] as List).cast<String>());
    }
    
    // Check for groups in various claim formats
    if (claims['groups'] is List) {
      groups.addAll((claims['groups'] as List).cast<String>());
    }
    if (claims['group'] is String) {
      groups.add(claims['group'] as String);
    }
    if (claims['user_groups'] is List) {
      groups.addAll((claims['user_groups'] as List).cast<String>());
    }
    
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(
                  Icons.security_outlined,
                  size: 24,
                  color: Colors.green.shade600,
                ),
                const SizedBox(width: 12),
                const Text(
                  'Permissions & Access',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
            
            const SizedBox(height: 16),
            const Divider(),
            const SizedBox(height: 16),
            
            if (roles.isNotEmpty) ...[
              const Text(
                'Roles:',
                style: TextStyle(
                  fontWeight: FontWeight.w600,
                  color: Colors.grey,
                ),
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 8,
                runSpacing: 4,
                children: roles.map((role) => Chip(
                  label: Text(role, style: const TextStyle(fontSize: 12)),
                  backgroundColor: Colors.blue.shade100,
                  labelStyle: TextStyle(color: Colors.blue.shade700),
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                )).toList(),
              ),
              const SizedBox(height: 12),
            ],
            
            if (groups.isNotEmpty) ...[
              const Text(
                'Groups:',
                style: TextStyle(
                  fontWeight: FontWeight.w600,
                  color: Colors.grey,
                ),
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 8,
                runSpacing: 4,
                children: groups.map((group) => Chip(
                  label: Text(group, style: const TextStyle(fontSize: 12)),
                  backgroundColor: Colors.purple.shade100,
                  labelStyle: TextStyle(color: Colors.purple.shade700),
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                )).toList(),
              ),
            ],
            
            if (roles.isEmpty && groups.isEmpty)
              const Text(
                'No specific roles or groups assigned',
                style: TextStyle(
                  color: Colors.grey,
                  fontStyle: FontStyle.italic,
                ),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildQuickActions(BuildContext context) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(
                  Icons.flash_on_outlined,
                  size: 24,
                  color: Colors.orange.shade600,
                ),
                const SizedBox(width: 12),
                const Text(
                  'Quick Actions',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
            
            const SizedBox(height: 16),
            const Divider(),
            const SizedBox(height: 16),
            
            Row(
              children: [
                Expanded(
                  child: _buildActionButton(
                    icon: Icons.person_outline,
                    label: 'Profile',
                    onTap: () => context.push('/profile'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _buildActionButton(
                    icon: Icons.settings_outlined,
                    label: 'Settings',
                    onTap: () => context.push('/settings'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildNavigationCards(BuildContext context, AuthressContext authContext) {
    final navItems = <Map<String, dynamic>>[];
    
    // Always show basic navigation
    navItems.addAll([
      {
        'title': 'Profile Management',
        'subtitle': 'View and edit your profile information',
        'icon': Icons.person_outline,
        'color': Colors.blue,
        'route': '/profile',
      },
      {
        'title': 'Account Settings',
        'subtitle': 'Manage your account preferences',
        'icon': Icons.settings_outlined,
        'color': Colors.grey,
        'route': '/settings',
      },
    ]);
    
    // Add admin section if user has admin role
    if (authContext.hasRole('admin')) {
      navItems.add({
        'title': 'Admin Panel',
        'subtitle': 'Administrative tools and controls',
        'icon': Icons.admin_panel_settings,
        'color': Colors.red,
        'route': '/admin',
      });
    }
    
    // Add manager section if user belongs to managers group
    if (authContext.hasGroup('managers')) {
      navItems.add({
        'title': 'Manager Dashboard',
        'subtitle': 'Team management and oversight',
        'icon': Icons.manage_accounts,
        'color': Colors.purple,
        'route': '/manager',
      });
    }
    
    return Column(
      children: navItems.map((item) => Padding(
        padding: const EdgeInsets.only(bottom: 12),
        child: Card(
          elevation: 1,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          child: ListTile(
            contentPadding: const EdgeInsets.all(16),
            leading: Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: (item['color'] as Color).withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(
                item['icon'] as IconData,
                color: item['color'] as Color,
                size: 24,
              ),
            ),
            title: Text(
              item['title'] as String,
              style: const TextStyle(
                fontWeight: FontWeight.w600,
                fontSize: 16,
              ),
            ),
            subtitle: Text(
              item['subtitle'] as String,
              style: TextStyle(
                color: Colors.grey.shade600,
                fontSize: 14,
              ),
            ),
            trailing: const Icon(Icons.arrow_forward_ios, size: 16),
            onTap: () => context.push(item['route'] as String),
          ),
        ),
      )).toList(),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(
              label,
              style: const TextStyle(
                fontWeight: FontWeight.w500,
                color: Colors.grey,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required String label,
    required VoidCallback onTap,
  }) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
        decoration: BoxDecoration(
          color: Colors.grey.shade100,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, size: 18, color: Colors.grey.shade700),
            const SizedBox(width: 8),
            Text(
              label,
              style: TextStyle(
                color: Colors.grey.shade700,
                fontWeight: FontWeight.w500,
              ),
            ),
          ],
        ),
      ),
    );
  }

  String _getGreeting() {
    final hour = DateTime.now().hour;
    if (hour < 12) return 'Good morning,';
    if (hour < 17) return 'Good afternoon,';
    return 'Good evening,';
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date).inDays;
    
    if (difference == 0) return 'Today';
    if (difference == 1) return 'Yesterday';
    if (difference < 7) return '$difference days ago';
    
    return '${date.day}/${date.month}/${date.year}';
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