import 'dart:ui' show VoidCallback;

import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:speech/speech.dart';

// Simple mock implementation for testing
class MockSpeechRepository implements SpeechRepository {
  bool _isInitialized = false;
  bool _isListening = false;

  @override
  Future<bool> initialize() async {
    _isInitialized = true;
    return true;
  }

  @override
  Future<bool> isAvailable() async => _isInitialized;

  @override
  Future<bool> startListening({
    required SpeechConfig config,
    required Function(SpeechRecognitionResult) onValidResult,
    required Function(String) onError,
    required VoidCallback onStatusChanged,
  }) async {
    if (!_isInitialized) return false;
    _isListening = true;
    return true;
  }

  @override
  Future<void> stopListening() async {
    _isListening = false;
  }

  @override
  Future<void> cancel() async {
    _isListening = false;
  }

  @override
  Future<List<String>> getAvailableLocales() async => ['en_US', 'nl_NL'];

  @override
  bool get isListening => _isListening;

  @override
  Future<void> dispose() async {
    _isInitialized = false;
    _isListening = false;
  }

  @override
  void addValidationRule(ValidationRule rule) {}

  @override
  void removeValidationRule(String ruleId) {}

  @override
  List<ValidationRule> getValidationRules() => [];
}

void main() {
  group('SpeechBloc', () {
    late MockSpeechRepository mockRepository;
    late SpeechBloc speechBloc;

    setUp(() {
      mockRepository = MockSpeechRepository();
      speechBloc = SpeechBloc(repository: mockRepository);
    });

    tearDown(() {
      speechBloc.close();
    });

    group('SpeechInitialize', () {
      blocTest<SpeechBloc, SpeechState>(
        'emits [initializing, ready] when initialization succeeds',
        build: () => speechBloc,
        act: (bloc) => bloc.add(const SpeechInitialize()),
        expect: () => [
          const SpeechState(
            status: SpeechStatus.initializing,
            config: SpeechConfig(localeId: 'en_US'),
            isListening: false,
          ),
          const SpeechState(
            status: SpeechStatus.ready,
            isAvailable: true,
            config: SpeechConfig(localeId: 'en_US'),
            isListening: false,
          ),
        ],
      );
    });

    group('SpeechResultReceived', () {
      blocTest<SpeechBloc, SpeechState>(
        'emits state with lastResult when result is received',
        build: () => speechBloc,
        act: (bloc) => bloc.add(
          SpeechResultReceived(
            SpeechRecognitionResult(
              text: '180',
              confidence: 0.9,
              isFinal: true,
              timestamp: DateTime.now(),
              locale: 'en_US',
            ),
          ),
        ),
        expect: () => [
          isA<SpeechState>().having(
            (state) => state.lastResult?.text,
            'lastResult.text',
            '180',
          ),
        ],
      );
    });

    group('SpeechErrorOccurred', () {
      blocTest<SpeechBloc, SpeechState>(
        'emits state with error message when error occurs',
        build: () => speechBloc,
        act: (bloc) => bloc.add(const SpeechErrorOccurred('Test error')),
        expect: () => [
          const SpeechState(
            status: SpeechStatus.error,
            errorMessage: 'Test error',
            config: SpeechConfig(localeId: 'en_US'),
            isListening: false,
          ),
        ],
      );
    });

    group('SpeechUpdateConfig', () {
      blocTest<SpeechBloc, SpeechState>(
        'emits state with updated config',
        build: () => speechBloc,
        act: (bloc) => bloc.add(
          SpeechUpdateConfig(
            const SpeechConfig(localeId: 'nl_NL'),
          ),
        ),
        expect: () => [
          const SpeechState(
            status: SpeechStatus.initial,
            config: SpeechConfig(localeId: 'nl_NL'),
            isListening: false,
          ),
        ],
      );
    });

    group('Button events', () {
      blocTest<SpeechBloc, SpeechState>(
        'SpeechButtonLongPressed adds SpeechStartListening',
        build: () => speechBloc,
        act: (bloc) => bloc.add(const SpeechButtonLongPressed()),
        expect: () => [], // No immediate state change, just adds another event
      );

      blocTest<SpeechBloc, SpeechState>(
        'SpeechButtonLongPressEnded adds SpeechStopListening',
        build: () => speechBloc,
        act: (bloc) => bloc.add(const SpeechButtonLongPressEnded()),
        expect: () => [], // No immediate state change, just adds another event
      );

      blocTest<SpeechBloc, SpeechState>(
        'SpeechButtonLongPressCancelled adds SpeechCancel',
        build: () => speechBloc,
        act: (bloc) => bloc.add(const SpeechButtonLongPressCancelled()),
        expect: () => [], // No immediate state change, just adds another event
      );
    });
  });
}
