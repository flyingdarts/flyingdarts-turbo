import 'package:json_annotation/json_annotation.dart';

part 'credentials.g.dart';

@JsonSerializable()
class Credentials {
  final String accessToken;
  final String idToken;
  final String refreshToken;

  Credentials({
    required this.accessToken,
    required this.idToken,
    required this.refreshToken,
  });

  factory Credentials.fromJson(Map<String, dynamic> json) => _$CredentialsFromJson(json);

  Map<String, dynamic> toJson() => _$CredentialsToJson(this);
}
