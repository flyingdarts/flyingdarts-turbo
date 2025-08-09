import 'dart:async';
import 'package:flutter/foundation.dart';
import 'package:app_links/app_links.dart';

import '../models/deep_link_config.dart';

/// Service for handling deep links with configurable schemes and timeouts
class DeepLinkService {
  final DeepLinkConfig _config;
  final AppLinks _appLinks = AppLinks();

  StreamSubscription<Uri>? _linkSubscription;
  Completer<Map<String, String>?>? _authCompleter;
  Timer? _timeoutTimer;

  DeepLinkService([this._config = const DeepLinkConfig()]);

  /// Initialize deep link handling
  Future<void> initialize() async {
    try {
      // Handle initial link when app is opened from deep link
      final initialLink = await _appLinks.getInitialLink();
      if (initialLink != null) {
        debugPrint('🔗 DeepLinkService: Initial link received: $initialLink');
        _processDeepLink(initialLink);
      }

      // Listen to ongoing links while app is running
      _linkSubscription = _appLinks.uriLinkStream.listen(
        _processDeepLink,
        onError: (error) {
          debugPrint('❌ DeepLinkService: Link stream error: $error');
          _completeWithError('Deep link error: $error');
        },
      );

      debugPrint('✅ DeepLinkService: Initialized with config: $_config');
    } catch (e) {
      debugPrint('❌ DeepLinkService: Initialization failed: $e');
    }
  }

  /// Wait for an authentication deep link with timeout
  Future<Map<String, String>?> waitForAuthCallback() async {
    if (_authCompleter != null && !_authCompleter!.isCompleted) {
      debugPrint(
        '⚠️ DeepLinkService: Previous auth flow still active, cancelling',
      );
      _authCompleter!.complete(null);
    }

    _authCompleter = Completer<Map<String, String>?>();

    // Set timeout
    _timeoutTimer = Timer(_config.timeoutDuration, () {
      if (_authCompleter != null && !_authCompleter!.isCompleted) {
        debugPrint(
          '⏰ DeepLinkService: Auth callback timed out after ${_config.timeoutDuration.inMinutes} minutes',
        );
        _authCompleter!.complete(null);
      }
    });

    debugPrint('⏳ DeepLinkService: Waiting for auth callback...');
    return _authCompleter!.future;
  }

  /// Process incoming deep link
  void _processDeepLink(Uri uri) {
    debugPrint('🔗 DeepLinkService: Processing deep link: $uri');

    if (!_config.matches(uri)) {
      debugPrint('🚫 DeepLinkService: URI does not match config: $uri');
      return;
    }

    final params = uri.queryParameters;
    debugPrint('📋 DeepLinkService: Extracted params: $params');

    if (_authCompleter != null && !_authCompleter!.isCompleted) {
      _timeoutTimer?.cancel();
      debugPrint('✅ DeepLinkService: Completing auth flow with params');
      _authCompleter!.complete(params);
    } else {
      debugPrint('⚠️ DeepLinkService: No active auth completer');
    }
  }

  /// Complete auth flow with error
  void _completeWithError(String error) {
    if (_authCompleter != null && !_authCompleter!.isCompleted) {
      _timeoutTimer?.cancel();
      _authCompleter!.completeError(Exception(error));
    }
  }

  /// Cancel any ongoing auth flow
  void cancelAuthFlow() {
    if (_authCompleter != null && !_authCompleter!.isCompleted) {
      _timeoutTimer?.cancel();
      _authCompleter!.complete(null);
      debugPrint('🚫 DeepLinkService: Auth flow cancelled');
    }
  }

  /// Get the callback URL for this service
  String get callbackUrl => _config.callbackUrl;

  void dispose() {
    _timeoutTimer?.cancel();
    _linkSubscription?.cancel();
    if (_authCompleter != null && !_authCompleter!.isCompleted) {
      _authCompleter!.complete(null);
    }
  }
}
