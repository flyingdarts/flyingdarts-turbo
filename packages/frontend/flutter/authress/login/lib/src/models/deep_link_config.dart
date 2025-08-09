/// Configuration for deep link handling
class DeepLinkConfig {
  final String scheme;
  final String host;
  final String path;
  final Duration timeoutDuration;

  const DeepLinkConfig({
    this.scheme = 'flyingdarts',
    this.host = 'auth',
    this.path = '',
    this.timeoutDuration = const Duration(minutes: 5),
  });

  /// Check if a URI matches this configuration
  bool matches(Uri uri) {
    return uri.scheme == scheme &&
        uri.host == host &&
        (path.isEmpty || uri.path.startsWith(path));
  }

  /// Generate callback URL for this configuration
  String get callbackUrl => '$scheme://$host${path.isNotEmpty ? path : ''}';

  @override
  String toString() => '$scheme://$host$path';
}
