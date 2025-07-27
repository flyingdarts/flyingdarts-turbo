import 'package:json_annotation/json_annotation.dart';
part 'get_user_profile_response.g.dart';

@JsonSerializable()
class GetUserProfileResponse {
  final String userId;
  final String userName;
  final String email;
  final String country;

  GetUserProfileResponse({
    required this.userId,
    required this.userName,
    required this.email,
    required this.country,
  });

  factory GetUserProfileResponse.fromJson(Map<String, dynamic> json) => _$GetUserProfileResponseFromJson(json);

  Map<String, dynamic> toJson() => _$GetUserProfileResponseToJson(this)..removeWhere((key, value) => value == null);
}
