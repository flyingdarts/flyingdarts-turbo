// ignore_for_file: non_constant_identifier_names

import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'user_profile.g.dart';

@JsonSerializable(createToJson: false)
class UserProfile extends Equatable {
  final String UserId;
  final String UserName;
  final String Country;
  final String Email;

  const UserProfile({
    required this.UserId,
    required this.UserName,
    required this.Country,
    required this.Email,
  });

  factory UserProfile.fromJson(Map<String, dynamic> json) => _$UserProfileFromJson(json);

  @override
  List<Object?> get props => [UserId, UserName, Country, Email];
}
