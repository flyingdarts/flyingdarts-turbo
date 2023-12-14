import 'package:dartboard/dartboard.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter/foundation.dart';
import 'package:get_it/get_it.dart';
import 'package:keyboard/keyboard.dart';
import 'package:language/language.dart';
import 'package:speech/speech.dart';

class NavigationState {
  final int selectedPage;
  final List<Widget> pages;
  final bool isLoading;
  late final bool isLoggedIn;

  NavigationState(this.selectedPage, this.pages, this.isLoading, this.isLoggedIn);

  NavigationState copyWith({int? selectedPage, List<Widget>? pages, bool? isLoading, bool? isLoggedIn}) {
    return NavigationState(
      selectedPage ?? this.selectedPage,
      pages ?? this.pages,
      isLoading ?? this.isLoading,
      isLoggedIn ?? this.isLoggedIn,
    );
  }

  setLoading(bool isLoading) {
    copyWith(isLoading: isLoading);
  }

  factory NavigationState.defaults() {
    final navigationState = NavigationState(
      0, // Set your desired default value for selectedPage
      [
        MultiBlocProvider(
          providers: [
            BlocProvider(create: (ctx) => SpeechBloc()..init()),
            BlocProvider(create: (ctx) => GetIt.I<LanguageCubit>()),
          ],
          child: const SpeechPage(),
        ),
        const KeyboardPage(),
        DartboardPage(),
      ],
      true,
      kDebugMode ? true : false,
    );
    return navigationState;
  }
}
