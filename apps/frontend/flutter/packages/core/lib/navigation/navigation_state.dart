import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flyingdarts_features/dartboard/pages/dartboard_page.dart';
import 'package:flyingdarts_features/keyboard/keyboard.dart';
import 'package:flyingdarts_features/language/state/language_cubit.dart';
import 'package:flyingdarts_features/speech/pages/speech_page.dart';
import 'package:flyingdarts_features/speech/state/speech_bloc.dart';
import 'package:flutter/foundation.dart';

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
            BlocProvider(create: (ctx) => LanguageCubit()..init()),
          ],
          child: const SpeechPage(),
        ),
        KeyboardPage(),
        DartboardPage(),
      ],
      true,
      kDebugMode ? true : false,
    );
    return navigationState;
  }
}
