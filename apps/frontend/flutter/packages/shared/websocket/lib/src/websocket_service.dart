import 'dart:async';
import 'dart:convert';
import 'dart:developer';
import 'package:amplify/amplify.dart';
import 'package:get_it/get_it.dart';
import 'package:rxdart/subjects.dart';
import 'package:web_socket_channel/io.dart';

import 'websocket_actions.dart';
import 'models/websocket_message.dart';

class WebSocketService {
  IOWebSocketChannel? _socket;
  final BehaviorSubject<bool> _connectedSubject = BehaviorSubject<bool>.seeded(false);
  Stream<bool> get connected$ => _connectedSubject.stream;
  final StreamController<WebSocketMessage<dynamic>> _messages = StreamController<WebSocketMessage>.broadcast();
  Stream<WebSocketMessage> get messages => _messages.stream;
  late String _websocketUri;

  final AmplifyService _amplifyService = GetIt.I<AmplifyService>();

  WebSocketService(String websocketUri) {
    _websocketUri = websocketUri;
  }

  void initialize() async {
    log('init websocket service');
    if (await _amplifyService.getAccessToken() == null) {
      return;
    }
    _socket = IOWebSocketChannel.connect('${_websocketUri}?token=${(await _amplifyService.getAccessToken())}');
    _connectedSubject.add(true);
    _connect();
  }

  void _connect() {
    _socket?.stream.listen(
      (event) {
        log(event);
        _messages.add(WebSocketMessage.fromJson(jsonDecode(event)));
      },
      onDone: () {
        _connectedSubject.add(false);
        _messages.add(WebSocketMessage(
          action: WebSocketActions.Disconnect,
          message: null,
        ));
        Future.delayed(Duration(seconds: 1), () {
          initialize();
        });
      },
      onError: (error) {
        _messages.add(WebSocketMessage(
          action: WebSocketActions.Default,
          message: error,
        ));
      },
    );
  }

  void postMessage(String payload) {
    if (_connectedSubject.value) {
      log('posting message');
      _socket?.sink.add(payload);
    }
  }

  void dispose() {
    _connectedSubject.close();
    _messages.close();
    _socket?.sink.close();
  }
}
