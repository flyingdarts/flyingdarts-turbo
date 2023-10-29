import 'dart:async';
import 'package:rxdart/subjects.dart';
import 'package:web_socket_channel/io.dart';

import 'websocket_actions.dart';
import 'websocket_message.dart';
import 'websocket_request.dart';

class WebSocketService<T extends WebSocketRequest> {
  IOWebSocketChannel? _socket;
  final BehaviorSubject<bool> _connectedSubject = BehaviorSubject<bool>.seeded(false);
  Stream<bool> get connected$ => _connectedSubject.stream;
  final StreamController<WebSocketMessage<T>> _messages = StreamController<WebSocketMessage<T>>.broadcast();
  Stream<WebSocketMessage<T>> get messages => _messages.stream;

  WebSocketService() {
    _initialize();
  }

  void _initialize() {
    _socket = IOWebSocketChannel.connect('wss://ewwi6tm25a.execute-api.eu-west-1.amazonaws.com/Development');
    _connect();
  }

  void _connect() {
    _socket?.stream.listen(
      (event) {
        _connectedSubject.add(true);
        _messages.add(WebSocketMessage<T>(
          action: WebSocketActions.Connect,
          message: event as T,
        ));
      },
      onDone: () {
        _connectedSubject.add(false);
        _messages.add(WebSocketMessage<T>(
          action: WebSocketActions.Disconnect,
          message: null,
        ));
        Future.delayed(Duration(seconds: 1), () {
          _initialize();
        });
      },
      onError: (error) {
        _messages.add(WebSocketMessage<T>(
          action: WebSocketActions.Default,
          message: error as T,
        ));
      },
    );
  }

  void postMessage(String payload) {
    if (_connectedSubject.value) {
      _socket?.sink.add(payload);
    }
  }

  void dispose() {
    _connectedSubject.close();
    _messages.close();
    _socket?.sink.close();
  }
}
