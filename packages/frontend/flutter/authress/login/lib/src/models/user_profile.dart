/// User profile information returned from Authress
class UserProfile {
  /// Unique user identifier
  final String userId;

  /// User's email address
  final String? email;

  /// User's display name
  final String? name;

  /// User's profile picture URL
  final String? picture;

  /// Additional user claims/attributes
  final Map<String, dynamic>? claims;

  /// When the user was created
  final DateTime? createdDate;

  /// When the user last logged in
  final DateTime? lastLoginDate;

  const UserProfile({
    required this.userId,
    this.email,
    this.name,
    this.picture,
    this.claims,
    this.createdDate,
    this.lastLoginDate,
  });

  /// Create a UserProfile from JSON data (JWT payload)
  factory UserProfile.fromJson(Map<String, dynamic> json) {
    // Extract claims - could be nested or the entire JWT payload
    Map<String, dynamic>? claims;

    if (json['claims'] != null && json['claims'] is Map<String, dynamic>) {
      // Claims are in a nested object
      claims = json['claims'] as Map<String, dynamic>;
    } else {
      // JWT payload itself contains the claims - extract non-standard claims
      final standardJwtClaims = {
        'sub',
        'email',
        'name',
        'given_name',
        'picture',
        'avatar',
        'iss',
        'aud',
        'exp',
        'iat',
        'nbf',
        'jti',
        'userId',
        'createdDate',
        'lastLoginDate',
      };

      // Include all non-standard claims (including roles, groups, etc.)
      claims = Map<String, dynamic>.fromEntries(
        json.entries.where((entry) => !standardJwtClaims.contains(entry.key)),
      );

      // Ensure we have something even if empty
      if (claims.isEmpty) claims = null;
    }

    return UserProfile(
      userId: json['userId'] ?? json['sub'] ?? '',
      email: json['email'],
      name: json['name'] ?? json['given_name'],
      picture: json['picture'] ?? json['avatar'],
      claims: claims,
      createdDate: json['createdDate'] != null
          ? DateTime.tryParse(json['createdDate'])
          : null,
      lastLoginDate: json['lastLoginDate'] != null
          ? DateTime.tryParse(json['lastLoginDate'])
          : null,
    );
  }

  /// Convert to JSON
  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'email': email,
      'name': name,
      'picture': picture,
      'claims': claims,
      'createdDate': createdDate?.toIso8601String(),
      'lastLoginDate': lastLoginDate?.toIso8601String(),
    };
  }

  /// Create a copy with updated values
  UserProfile copyWith({
    String? userId,
    String? email,
    String? name,
    String? picture,
    Map<String, dynamic>? claims,
    DateTime? createdDate,
    DateTime? lastLoginDate,
  }) {
    return UserProfile(
      userId: userId ?? this.userId,
      email: email ?? this.email,
      name: name ?? this.name,
      picture: picture ?? this.picture,
      claims: claims ?? this.claims,
      createdDate: createdDate ?? this.createdDate,
      lastLoginDate: lastLoginDate ?? this.lastLoginDate,
    );
  }

  @override
  String toString() {
    return 'UserProfile(userId: $userId, email: $email, name: $name)';
  }
}
