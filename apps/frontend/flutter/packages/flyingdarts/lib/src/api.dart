import 'package:dio/dio.dart';
import 'package:dio_smart_retry/dio_smart_retry.dart';
import 'package:get_it/get_it.dart';
import 'package:configuration/configuration.dart';
import 'package:injectable/injectable.dart';
import 'package:websocket/websocket.dart';

import 'flavor.dart';

const rootWebSocketUri = Named("root_websocket_uri");
const rootUsersApiUri = Named("root_usersApi_uri");

var getIt = GetIt.instance;

@module
abstract class ApiModule {
  @dev
  @acc
  @rootWebSocketUri
  String get rootWebSocketUriDev => "wss://r051b8g6e7.execute-api.eu-west-1.amazonaws.com/Development/";

  @prod
  @rootWebSocketUri
  String get rootWebSocketUriProd => "wss://pd3h2kmulf.execute-api.eu-west-1.amazonaws.com/Production/";

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

  @singleton
  Dio get dio {
    final instance = Dio();
    instance.interceptors.add(RetryInterceptor(
      dio: instance,
      retries: 5,
      retryDelays: [
        const Duration(milliseconds: 250),
        const Duration(milliseconds: 500),
        const Duration(seconds: 1),
        const Duration(seconds: 2),
        const Duration(seconds: 4),
      ],
    ));

    return instance;
  }
}
