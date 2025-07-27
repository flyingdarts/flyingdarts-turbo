mixin FallbackProvider<TValue> {
  TValue? _fallbackValue;

  void setFallback(TValue value) {
    this._fallbackValue = value;
  }

  TValue? get fallbackValue {
    return _fallbackValue;
  }
}
