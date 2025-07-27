import 'dart:async';
import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../models/auth_config.dart';

/// HTTP service with proper error handling, timeouts, and retry logic
class HttpService {
  final AuthressConfiguration _config;
  late final http.Client _client;

  // Configurable timeouts
  static const Duration _defaultTimeout = Duration(seconds: 30);
  static const Duration _authTimeout = Duration(minutes: 1);
  static const int _maxRetries = 3;

  HttpService(this._config) {
    _client = http.Client();
  }

  /// Build URL with proper path handling
  String buildUrl(String path) {
    final baseUrl = _config.authressApiUrl.endsWith('/')
        ? _config.authressApiUrl.substring(0, _config.authressApiUrl.length - 1)
        : _config.authressApiUrl;

    final cleanPath = path.startsWith('/') ? path : '/$path';
    return '$baseUrl$cleanPath';
  }

  /// GET request with automatic retry and error handling
  Future<HttpResponse> get(
    String path, {
    Map<String, String>? headers,
    Duration? timeout,
    int? maxRetries,
  }) async {
    return _executeWithRetry(
      () => _client.get(
        Uri.parse(buildUrl(path)),
        headers: _buildHeaders(headers),
      ),
      timeout: timeout ?? _defaultTimeout,
      maxRetries: maxRetries ?? _maxRetries,
    );
  }

  /// POST request with automatic retry and error handling
  Future<HttpResponse> post(
    String path, {
    Map<String, String>? headers,
    Object? body,
    Duration? timeout,
    int? maxRetries,
  }) async {
    return _executeWithRetry(
      () => _client.post(
        Uri.parse(buildUrl(path)),
        headers: _buildHeaders(headers),
        body: body is String ? body : json.encode(body),
      ),
      timeout: timeout ?? _authTimeout,
      maxRetries: maxRetries ?? _maxRetries,
    );
  }

  /// Execute request with retry logic
  Future<HttpResponse> _executeWithRetry(
    Future<http.Response> Function() requestFn, {
    required Duration timeout,
    required int maxRetries,
  }) async {
    int attempt = 0;
    Exception? lastException;

    while (attempt <= maxRetries) {
      try {
        final response = await requestFn().timeout(timeout);

        // Log response for debugging
        if (kDebugMode) {
          debugPrint('HTTP ${response.request?.method} ${response.request?.url} -> ${response.statusCode}');
        }

        return HttpResponse(
          statusCode: response.statusCode,
          body: response.body,
          headers: response.headers,
          isSuccess: response.statusCode >= 200 && response.statusCode < 300,
        );
      } on TimeoutException {
        lastException = HttpException('Request timed out after ${timeout.inSeconds}s');
      } on http.ClientException catch (e) {
        lastException = HttpException('Network error: ${e.message}');
      } catch (e) {
        lastException = HttpException('Unexpected error: $e');
      }

      attempt++;
      if (attempt <= maxRetries) {
        // Exponential backoff
        final delay = Duration(milliseconds: 1000 * (1 << (attempt - 1)));
        debugPrint('HTTP request failed, retrying in ${delay.inMilliseconds}ms (attempt $attempt/$maxRetries)');
        await Future.delayed(delay);
      }
    }

    throw lastException ?? HttpException('Request failed after $maxRetries retries');
  }

  /// Build request headers with defaults
  Map<String, String> _buildHeaders(Map<String, String>? customHeaders) {
    final headers = <String, String>{
      'Content-Type': 'application/json',
      'X-Powered-By': 'Authress Login SDK; Flutter; 2.0.0',
      'User-Agent': 'AuthressLoginFlutter/2.0.0',
    };

    if (customHeaders != null) {
      headers.addAll(customHeaders);
    }

    return headers;
  }

  void dispose() {
    _client.close();
  }
}

/// HTTP response wrapper with additional metadata
class HttpResponse {
  final int statusCode;
  final String body;
  final Map<String, String> headers;
  final bool isSuccess;

  const HttpResponse({
    required this.statusCode,
    required this.body,
    required this.headers,
    required this.isSuccess,
  });

  /// Parse response body as JSON
  Map<String, dynamic> get jsonBody => json.decode(body);

  /// Check if response indicates authentication error
  bool get isAuthError => statusCode == 401 || statusCode == 403;

  /// Check if response is a client error
  bool get isClientError => statusCode >= 400 && statusCode < 500;

  /// Check if response is a server error
  bool get isServerError => statusCode >= 500;
}

/// Custom HTTP exception
class HttpException implements Exception {
  final String message;
  final int? statusCode;

  const HttpException(this.message, [this.statusCode]);

  @override
  String toString() => 'HttpException: $message${statusCode != null ? ' (Status: $statusCode)' : ''}';
}
