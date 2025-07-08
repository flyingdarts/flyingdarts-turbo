mixin DefaultConfiguration<TModel> {
  TModel? _default;
  void setDefault(TModel model) {
    this._default = model;
  }

  TModel? get defaultModel {
    return _default;
  }
}
