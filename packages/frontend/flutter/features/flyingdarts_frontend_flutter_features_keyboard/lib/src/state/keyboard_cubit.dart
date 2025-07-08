import 'package:flutter_bloc/flutter_bloc.dart';

import 'keyboard_state.dart';

class KeyboardCubit extends Cubit<KeyboardState> {
  KeyboardCubit()
      : super(
          const KeyboardState(0, false, false, true),
        );

  void setShortcut(int value) {
    emit(
      state.copyWith(
        lastInput: value,
        shouldDisableNumpad: true,
        shouldDisableFunctions: false,
      ),
    );
  }

  void setDigit(int digit) {
    var next = '${state.lastInput}$digit';
    if (int.parse(next) > 180) {
      return;
    }
    emit(
      state.copyWith(
        lastInput: int.parse(next),
        shouldDisableShortcuts: true,
        shouldDisableFunctions: false,
      ),
    );
  }

  void clear() {
    emit(
      state.copyWith(
          lastInput: 0, shouldDisableNumpad: false, shouldDisableShortcuts: false, shouldDisableFunctions: true),
    );
  }

  void check() {
    emit(
      state.copyWith(
        lastInput: 0,
        shouldDisableNumpad: false,
        shouldDisableShortcuts: false,
        shouldDisableFunctions: true,
      ),
    );
  }

  void noScore() {
    emit(
      state.copyWith(
        lastInput: 0,
        shouldDisableNumpad: false,
        shouldDisableShortcuts: false,
        shouldDisableFunctions: true,
      ),
    );
  }

  void ok() {
    emit(
      state.copyWith(
        lastInput: 0,
        shouldDisableNumpad: false,
        shouldDisableShortcuts: false,
        shouldDisableFunctions: true,
      ),
    );
  }
}
