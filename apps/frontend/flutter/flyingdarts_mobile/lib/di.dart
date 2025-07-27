import 'package:core/core.module.dart';
import 'package:get_it/get_it.dart';
import 'package:injectable/injectable.dart';
import 'package:keyboard/keyboard.dart';
import 'package:language/language.dart';
import 'package:profile/profile.dart';
import 'package:speech/speech.dart';

import 'di.config.dart';

final getIt = GetIt.instance;

@InjectableInit(
  includeMicroPackages: true,
  externalPackageModulesBefore: [ExternalModule(CorePackageModule)],
  externalPackageModulesAfter: [
    ExternalModule(LanguagePackageModule),
    ExternalModule(KeyboardPackageModule),
    ExternalModule(ProfilePackageModule),
    ExternalModule(SpeechPackageModule),
  ],
)
Future<void> setupDependencies(String flavor) async {
  getIt.init(environment: flavor);
}
