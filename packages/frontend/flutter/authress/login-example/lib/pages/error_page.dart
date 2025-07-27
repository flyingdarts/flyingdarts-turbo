import 'package:authress_login/authress_login.dart';
import 'package:flutter/material.dart';

/// Error page shown when authentication fails
class ErrorPage extends StatelessWidget {
  const ErrorPage({super.key});

  @override
  Widget build(BuildContext context) {
    final authState = context.authress.authState;
    // Get error message from state
    String errorMessage = 'An unknown error occurred';

    if (authState is AuthStateError) {
      errorMessage = authState.message;
    }
    
    return Scaffold(
      backgroundColor: const Color(0xff1d2f3b),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(32),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              // Error icon
              Container(
                width: 100,
                height: 100,
                decoration: BoxDecoration(
                  color: Colors.red.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(20),
                ),
                child: const Icon(
                  Icons.error_outline_rounded,
                  size: 50,
                  color: Colors.red,
                ),
              ),
              
              const SizedBox(height: 32),
              
              // Error title
              Text(
                'Authentication Failed',
                style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: Colors.red[700],
                ),
                textAlign: TextAlign.center,
              ),
              
              const SizedBox(height: 16),
              
              // Error message
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: Colors.red.withValues(alpha: 0.05),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.red.withValues(alpha: 0.2)),
                ),
                child: Text(
                  errorMessage,
                  style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                    color: Colors.red[800],
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
              
              const SizedBox(height: 32),
              
              // Action buttons
              Column(
                children: [
                  // Try again button
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton.icon(
                      onPressed: () => context.authress.authenticate(),
                      icon: const Icon(Icons.refresh_rounded),
                      label: const Text(
                        'Try Again',
                        style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                      ),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Theme.of(context).primaryColor,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                      ),
                    ),
                  ),
                  
                  const SizedBox(height: 12),
                  
                  // Go back button
                  SizedBox(
                    width: double.infinity,
                    child: OutlinedButton.icon(
                      onPressed: () => context.authress.logout(),
                      icon: const Icon(Icons.arrow_back_rounded),
                      label: const Text(
                        'Back to Login',
                        style: TextStyle(fontSize: 16),
                      ),
                      style: OutlinedButton.styleFrom(
                        foregroundColor: Theme.of(context).primaryColor,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        side: BorderSide(
                          color: Theme.of(context).primaryColor.withValues(alpha: 0.5),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
              
              const SizedBox(height: 32),
              
              // Troubleshooting tips
              _TroubleshootingTips(),
            ],
          ),
        ),
      ),
    );
  }
}

class _TroubleshootingTips extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final tips = [
      'Check your internet connection',
      'Make sure you have the latest app version',
      'Try signing in with a different account',
      'Contact support if the problem persists',
    ];
    
    return ExpansionTile(
      leading: Icon(
        Icons.help_outline_rounded,
        color: Colors.grey[600],
      ),
      title: Text(
        'Troubleshooting Tips',
        style: Theme.of(context).textTheme.titleMedium?.copyWith(
          color: Colors.grey[700],
          fontWeight: FontWeight.w600,
        ),
      ),
      children: tips.map((tip) {
        return ListTile(
          dense: true,
          leading: Icon(
            Icons.arrow_right_rounded,
            color: Colors.grey[500],
            size: 20,
          ),
          title: Text(
            tip,
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
              color: Colors.grey[600],
            ),
          ),
        );
      }).toList(),
    );
  }
} 