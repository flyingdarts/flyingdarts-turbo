library;

import 'package:injectable/injectable.dart';

// Micro-package module for dependency injection
export 'speech.module.dart';
export 'src/data/repositories/speech_repository.dart';
// Data layer
export 'src/data/services/speech_to_text_service.dart';
export 'src/data/services/speech_validation_service_impl.dart';
// Domain interfaces
export 'src/domain/interfaces/speech_recognition_service.dart';
export 'src/domain/interfaces/speech_validation_service.dart';
// Domain models
export 'src/domain/models/speech_config.dart';
export 'src/domain/models/speech_recognition_result.dart';
// Presentation layer
export 'src/presentation/bloc/speech_bloc.dart';
export 'src/presentation/pages/speech_page.dart';
export 'src/presentation/widgets/speech_command_gesture_detector.dart';
export 'src/presentation/widgets/speech_recognition_widget.dart';
// Widgetbook exports
export 'src/widgetbook.generator.dart';

// @microPackageInit => short const
@InjectableInit.microPackage()
void initMicroPackage() {} // will not be called but needed for code generation
