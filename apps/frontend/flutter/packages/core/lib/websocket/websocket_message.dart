class WebSocketMessage<T> {
  String action;
  T? message;

  WebSocketMessage({required this.action, this.message});

  factory WebSocketMessage.fromJson(Map<String, dynamic> json) {
    return WebSocketMessage<T>(
      action: json['action'],
      message: json['message'] as T,
    );
  }
}
