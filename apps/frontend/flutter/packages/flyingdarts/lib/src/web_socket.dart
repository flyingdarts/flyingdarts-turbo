import 'package:get_it/get_it.dart';
import 'package:configuration/configuration.dart';
import 'package:injectable/injectable.dart';
import 'package:websocket/websocket.dart';

import 'flavor.dart';

const rootWebSocketUri = Named("root_websocket_uri");

var getIt = GetIt.instance;

@module
abstract class WebSocketModule {
  @dev
  @acc
  @rootWebSocketUri
  String get rootWebSocketUriDev => "wss://z7e1yqddn0.execute-api.eu-west-1.amazonaws.com/Development/";

  @prod
  @rootWebSocketUri
  String get rootWebSocketUriProd => "wss://7o8ozrkfse.execute-api.eu-west-1.amazonaws.com/Production/";

  @dev
  @acc
  @prod
  WebSocketConfig config(@rootWebSocketUri String rootWebSocketUri) {
    return WebSocketConfig(webSocketUri: rootWebSocketUri);
  }

  @dev
  @acc
  @prod
  @injectable
  WebSocketService webSocketService(WebSocketConfig config) {
    return WebSocketService(config.webSocketUri)..initialize();
  }
}
