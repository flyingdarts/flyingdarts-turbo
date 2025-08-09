# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project adheres to Semantic Versioning.

## [0.0.2-dev.1] - 2025-08-09

### Added

- Provider-based authentication context and state management
- Go Router integration with redirect logic and route guards
- Context extensions for easy access to auth state, user, and tokens
- Automatic token lifecycle management (persist, renew, expiry checks)
- Secure token storage using `shared_preferences`
- Smart browser/deep-link handling via `webview_flutter`, `url_launcher`, and `app_links`
- Role and group-based access helpers
- Comprehensive error states and callbacks
- Core models: `AuthressConfiguration`, `AuthState` variants, `UserProfile`, `DeepLinkConfig`
- Example usage and API documentation in `README.md`

