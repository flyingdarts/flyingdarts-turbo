import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:speech/speech.dart';

/// Example of how to use the speech package with micro-package architecture
class SpeechUsageExample extends StatelessWidget {
  const SpeechUsageExample({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Speech Recognition Example'),
      ),
      body: BlocProvider(
        // Get the SpeechBloc from the DI container
        create: (context) => GetIt.instance<SpeechBloc>(),
        child: const SpeechPage(
          onResult: _handleSpeechResult,
          onError: _handleSpeechError,
        ),
      ),
    );
  }

  static void _handleSpeechResult(String text) {
    print('Speech recognized: $text');
    // Handle the speech result here
    // For example, validate it's a valid dart score
    try {
      final score = int.parse(text);
      if (score >= 0 && score <= 180) {
        print('Valid dart score: $score');
      } else {
        print('Invalid dart score: $score (must be 0-180)');
      }
    } catch (e) {
      print('Could not parse as number: $text');
    }
  }

  static void _handleSpeechError(String error) {
    print('Speech recognition error: $error');
    // Handle the error here
    // For example, show a snackbar or dialog
  }
}

/// Example of how to use the speech package programmatically
class SpeechProgrammaticExample extends StatefulWidget {
  const SpeechProgrammaticExample({super.key});

  @override
  State<SpeechProgrammaticExample> createState() => _SpeechProgrammaticExampleState();
}

class _SpeechProgrammaticExampleState extends State<SpeechProgrammaticExample> {
  late final SpeechBloc _speechBloc;
  String _lastResult = '';
  String _status = 'Ready';

  @override
  void initState() {
    super.initState();
    _speechBloc = GetIt.instance<SpeechBloc>();

    // Listen to state changes
    _speechBloc.stream.listen((state) {
      setState(() {
        _lastResult = state.displayText;
        _status = state.statusMessage;
      });
    });
  }

  @override
  void dispose() {
    _speechBloc.close();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Programmatic Speech Example'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            Text('Status: $_status', style: Theme.of(context).textTheme.headlineSmall),
            const SizedBox(height: 16),
            Text('Last Result: $_lastResult', style: Theme.of(context).textTheme.bodyLarge),
            const SizedBox(height: 32),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                ElevatedButton(
                  onPressed: () => _speechBloc.add(const SpeechStartListening()),
                  child: const Text('Start Listening'),
                ),
                ElevatedButton(
                  onPressed: () => _speechBloc.add(const SpeechStopListening()),
                  child: const Text('Stop Listening'),
                ),
                ElevatedButton(
                  onPressed: () => _speechBloc.add(const SpeechCancel()),
                  child: const Text('Cancel'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
