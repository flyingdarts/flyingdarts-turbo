import 'package:json_annotation/json_annotation.dart';

part 'locale_settings.g.dart';

@JsonSerializable()
class LocaleSettings {
  final String localeIdentifier;

  const LocaleSettings({required this.localeIdentifier});

  factory LocaleSettings.fromJson(Map<String, dynamic> json) =>
      _$LocaleSettingsFromJson(json);

  Map<String, dynamic> toJson() => _$LocaleSettingsToJson(this);
}
