import 'package:json_annotation/json_annotation.dart';
part 'language_config.g.dart';

@JsonSerializable()
class LanguageConfig {
  final String preferedLocaleId;

  LanguageConfig({required this.preferedLocaleId});

  factory LanguageConfig.fromJson(Map<String, dynamic> json) => _$LanguageConfigFromJson(json);

  Map<String, dynamic> toJson() => _$LanguageConfigToJson(this);
}
