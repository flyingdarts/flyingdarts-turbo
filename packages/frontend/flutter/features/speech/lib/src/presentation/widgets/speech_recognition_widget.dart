import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:language/language.dart';
import 'package:ui/ui.dart';

import '../bloc/speech_bloc.dart';
import 'speech_command_gesture_detector.dart';

/// Production-ready speech recognition widget with accessibility support
class SpeechRecognitionWidget extends StatelessWidget {
  const SpeechRecognitionWidget({
    super.key,
    required this.state,
    required this.languageState,
    this.onResult,
    this.onError,
  });

  final SpeechState state;
  final LanguageState languageState;
  final Function(String)? onResult;
  final Function(String)? onError;

  @override
  Widget build(BuildContext context) {
    return Semantics(
      label: 'Speech recognition interface',
      hint: 'Long press and hold the microphone button to speak',
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          _buildResultDisplay(),
          const SizedBox(height: 40),
          _buildMicrophoneButton(context),
          const SizedBox(height: 20),
          _buildStatusText(),
          const SizedBox(height: 20),
        ],
      ),
    );
  }

  Widget _buildResultDisplay() {
    return Column(
      children: [
        // Display the recognized text
        Semantics(
          label: 'Recognized text',
          value: state.lastEntry,
          child: Text(
            state.lastEntry,
            style: const TextStyle(
              fontFamily: 'Poppins',
              fontSize: 72,
              fontWeight: FontWeight.w700,
              color: Colors.white,
            ),
            textAlign: TextAlign.center,
          ),
        ),
        // Display current language
        Semantics(
          label: 'Current language',
          value: languageState.preferedLocale.name,
          child: Padding(
            padding: const EdgeInsets.only(top: 8.0),
            child: Text(
              languageState.preferedLocale.name,
              style: const TextStyle(
                fontFamily: 'Poppins',
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: Colors.white70,
              ),
              textAlign: TextAlign.center,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildMicrophoneButton(BuildContext context) {
    return Semantics(
      label: state.isListening ? 'Stop listening' : 'Start listening',
      hint: 'Long press and hold to speak',
      button: true,
      enabled: state.enabled,
      child: SpeechCommandGestureDetector(
        key: Keys.speechRecognitionBtn.key,
        onLongPress: () {
          context.read<SpeechBloc>().add(const SpeechButtonLongPressed());
        },
        onLongPressEnd: (details) {
          context.read<SpeechBloc>().add(const SpeechButtonLongPressEnded());
        },
        onLongPressCancel: () {
          context.read<SpeechBloc>().add(const SpeechButtonLongPressCancelled());
        },
        child: Icon(
          size: 300,
          state.isListening ? Icons.mic : Icons.mic_none,
          color: _getMicrophoneColor(),
        ),
      ),
    );
  }

  Widget _buildStatusText() {
    return Semantics(
      label: 'Status',
      value: state.error,
      child: Text(
        key: const Key("SpeechCommandStatusText"),
        state.error,
        style: TextStyle(
          fontFamily: 'Poppins',
          fontSize: 18,
          fontWeight: FontWeight.w400,
          color: _getStatusTextColor(),
        ),
        textAlign: TextAlign.center,
      ),
    );
  }

  Color _getMicrophoneColor() {
    if (!state.enabled) {
      return Colors.grey;
    }
    if (state.isListening) {
      return Colors.red;
    }
    return MyTheme.secondaryColor;
  }

  Color _getStatusTextColor() {
    if (state.error.isNotEmpty) {
      return Colors.red;
    }
    if (state.isListening) {
      return Colors.green;
    }
    return Colors.white;
  }
}

/// Factory function for creating the speech recognition widget
Widget buildSpeechRecognitionWidget(
  BuildContext context,
  SpeechState state,
  LanguageState languageState, {
  Function(String)? onResult,
  Function(String)? onError,
}) {
  return SpeechRecognitionWidget(
    state: state,
    languageState: languageState,
    onResult: onResult,
    onError: onError,
  );
}
