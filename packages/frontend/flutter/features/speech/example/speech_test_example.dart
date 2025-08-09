import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:language/language.dart';
import 'package:speech/speech.dart';
import 'package:speech_to_text/speech_to_text.dart';

/// Example demonstrating the fixed speech recognition with debug info
class SpeechTestExample extends StatelessWidget {
  const SpeechTestExample({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Speech Recognition Test'),
        backgroundColor: Colors.blue,
      ),
      body: BlocProvider(
        create: (context) =>
            GetIt.instance<SpeechBloc>()..add(const SpeechInitialize()),
        child: const SpeechTestContent(),
      ),
    );
  }
}

class SpeechTestContent extends StatelessWidget {
  const SpeechTestContent({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<SpeechBloc, SpeechState>(
      builder: (context, state) {
        return Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            children: [
              // Debug information
              const SpeechDebugWidget(),

              const SizedBox(height: 20),

              // Main speech recognition widget
              Expanded(
                child: Center(
                  child: SpeechRecognitionWidget(
                    state: state,
                    languageState: LanguageState(
                      LocaleName('en_US', 'English'),
                      [LocaleName('en_US', 'English')],
                    ),
                    onResult: (text) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text('Recognized: $text'),
                          backgroundColor: Colors.green,
                        ),
                      );
                    },
                    onError: (error) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text('Error: $error'),
                          backgroundColor: Colors.red,
                        ),
                      );
                    },
                  ),
                ),
              ),

              const SizedBox(height: 20),

              // Instructions
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: Colors.blue.shade50,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: const Column(
                  children: [
                    Text(
                      'Instructions:',
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 16,
                      ),
                    ),
                    SizedBox(height: 8),
                    Text(
                      '1. Long press and hold the microphone button\n'
                      '2. Speak numbers (e.g., "one", "two", "three")\n'
                      '3. Release the button to stop listening\n'
                      '4. The speech recognition should continue working\n'
                      '5. Check the debug info above for status',
                      style: TextStyle(fontSize: 14),
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}
