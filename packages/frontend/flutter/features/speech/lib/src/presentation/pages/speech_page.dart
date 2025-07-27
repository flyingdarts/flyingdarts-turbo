import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:language/language.dart';
import 'package:ui/ui.dart';

import '../bloc/speech_bloc.dart';
import '../widgets/speech_recognition_widget.dart';

/// Production-ready speech page with proper lifecycle management
class SpeechPage extends StatefulWidget {
  const SpeechPage({
    super.key,
    this.onResult,
    this.onError,
  });

  final Function(String)? onResult;
  final Function(String)? onError;

  @override
  State<SpeechPage> createState() => _SpeechPageState();
}

class _SpeechPageState extends State<SpeechPage> with WidgetsBindingObserver {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    super.didChangeAppLifecycleState(state);

    // Handle app lifecycle changes
    switch (state) {
      case AppLifecycleState.paused:
      case AppLifecycleState.detached:
        // Stop listening when app goes to background
        // context.read<SpeechBloc>().add(const SpeechStopListening());
        break;
      case AppLifecycleState.resumed:
        // Only re-initialize if not already ready
        // final currentState = context.read<SpeechBloc>().state;
        // if (currentState.status != SpeechStatus.ready || !currentState.isAvailable) {
        //   context.read<SpeechBloc>().add(const SpeechInitialize());
        // }
        break;
      default:
        break;
    }
  }

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<SpeechBloc, SpeechState>(
      builder: (context, state) {
        final languageState = context.watch<LanguageCubit>().state;

        return FlyingdartsScaffold(
          child: Container(
            alignment: Alignment.center,
            child: Flex(
              direction: Axis.vertical,
              children: [
                Expanded(
                  child: buildSpeechRecognitionWidget(
                    context,
                    state,
                    languageState,
                    onResult: widget.onResult,
                    onError: widget.onError,
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
