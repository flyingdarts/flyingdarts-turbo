import 'package:json_annotation/json_annotation.dart';
part 'get_user_profile_query.g.dart';

@JsonSerializable()
class GetUserProfileQuery {
  final String cognitoUserName;

  GetUserProfileQuery({
    required this.cognitoUserName,
  });

  factory GetUserProfileQuery.fromJson(Map<String, dynamic> json) => _$GetUserProfileQueryFromJson(json);

  Map<String, dynamic> toJson() => _$GetUserProfileQueryToJson(this);
}
