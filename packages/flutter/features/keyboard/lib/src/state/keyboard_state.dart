class KeyboardState {
  const KeyboardState(
    this.lastInput,
    this.shouldDisableNumpad,
    this.shouldDisableShortcuts,
    this.shouldDisableFunctions,
  );

  final int lastInput;
  final bool shouldDisableNumpad;
  final bool shouldDisableShortcuts;
  final bool shouldDisableFunctions;

  KeyboardState copyWith(
      {int? lastInput,
      bool? shouldDisableNumpad,
      bool? shouldDisableShortcuts,
      bool? shouldDisableFunctions}) {
    return KeyboardState(
      lastInput ?? this.lastInput,
      shouldDisableNumpad ?? this.shouldDisableNumpad,
      shouldDisableShortcuts ?? this.shouldDisableShortcuts,
      shouldDisableFunctions ?? this.shouldDisableFunctions,
    );
  }
}
