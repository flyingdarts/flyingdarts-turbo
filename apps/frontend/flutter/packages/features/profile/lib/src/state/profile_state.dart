class ProfileState {
  final String userId;
  final String userName;
  final String country;
  final String email;

  ProfileState(
    this.userId,
    this.userName,
    this.country,
    this.email,
  );

  static ProfileState get initialState => ProfileState(
        '',
        '',
        '',
        '',
      );

  ProfileState copyWith({
    String? userId,
    String? userName,
    String? country,
    String? email,
  }) {
    return ProfileState(
      userId ?? this.userId,
      userName ?? this.userName,
      country ?? this.country,
      email ?? this.email,
    );
  }
}
