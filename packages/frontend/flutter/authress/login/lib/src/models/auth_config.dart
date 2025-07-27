/// Configuration class for Authress authentication
class AuthressConfiguration {
  /// The base URL of your Authress API (e.g., 'https://login.yourdomain.com')
  final String authressApiUrl;

  /// Your application ID from Authress dashboard
  final String applicationId;

  /// URL to redirect to after successful authentication
  final String? redirectUrl;

  /// Custom domain for Authress (optional)
  final String? customDomain;

  /// Whether to enable debug logging
  final bool enableDebugLogging;

  /// Request timeout duration
  final Duration requestTimeout;

  /// Authentication timeout duration
  final Duration authTimeout;

  const AuthressConfiguration({
    required this.authressApiUrl,
    required this.applicationId,
    this.redirectUrl,
    this.customDomain,
    this.enableDebugLogging = false,
    this.requestTimeout = const Duration(seconds: 30),
    this.authTimeout = const Duration(minutes: 5),
  });

  /// Validate the configuration and throw if invalid
  void validate() {
    if (authressApiUrl.isEmpty) {
      throw ArgumentError('authressApiUrl cannot be empty');
    }

    final uri = Uri.tryParse(authressApiUrl);
    if (uri == null || !uri.hasScheme) {
      throw ArgumentError('authressApiUrl must be a valid URL with scheme');
    }

    if (!authressApiUrl.startsWith('https://')) {
      throw ArgumentError('authressApiUrl must use HTTPS for security');
    }

    if (applicationId.isEmpty) {
      throw ArgumentError('applicationId cannot be empty');
    }

    if (applicationId.contains(' ')) {
      throw ArgumentError('applicationId cannot contain spaces');
    }

    if (redirectUrl != null && redirectUrl!.isNotEmpty) {
      final uri = Uri.tryParse(redirectUrl!);
      if (uri == null) {
        throw ArgumentError('redirectUrl must be a valid URL if provided');
      }
    }

    if (requestTimeout.inMilliseconds <= 0) {
      throw ArgumentError('requestTimeout must be positive');
    }

    if (authTimeout.inMilliseconds <= 0) {
      throw ArgumentError('authTimeout must be positive');
    }
  }

  /// Create a validated copy of this config with updated values
  AuthressConfiguration copyWith({
    String? authressApiUrl,
    String? applicationId,
    String? redirectUrl,
    String? customDomain,
    bool? enableDebugLogging,
    Duration? requestTimeout,
    Duration? authTimeout,
  }) {
    final config = AuthressConfiguration(
      authressApiUrl: authressApiUrl ?? this.authressApiUrl,
      applicationId: applicationId ?? this.applicationId,
      redirectUrl: redirectUrl ?? this.redirectUrl,
      customDomain: customDomain ?? this.customDomain,
      enableDebugLogging: enableDebugLogging ?? this.enableDebugLogging,
      requestTimeout: requestTimeout ?? this.requestTimeout,
      authTimeout: authTimeout ?? this.authTimeout,
    );

    config.validate();
    return config;
  }

  @override
  String toString() {
    return 'AuthConfig(authressApiUrl: $authressApiUrl, applicationId: $applicationId, redirectUrl: $redirectUrl, customDomain: $customDomain)';
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is AuthressConfiguration &&
        other.authressApiUrl == authressApiUrl &&
        other.applicationId == applicationId &&
        other.redirectUrl == redirectUrl &&
        other.customDomain == customDomain &&
        other.enableDebugLogging == enableDebugLogging &&
        other.requestTimeout == requestTimeout &&
        other.authTimeout == authTimeout;
  }

  @override
  int get hashCode {
    return Object.hash(
      authressApiUrl,
      applicationId,
      redirectUrl,
      customDomain,
      enableDebugLogging,
      requestTimeout,
      authTimeout,
    );
  }
}
