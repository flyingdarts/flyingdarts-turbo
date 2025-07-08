import 'package:injectable/injectable.dart';

const authorizationToken = Named("flyingdarts_authorizationToken");
const rootUri = Named("flyingdarts_rootUri");

class FlyingdartsApiConfig {
  final String authorizationToken;
  final Uri rootUri;

  const FlyingdartsApiConfig({
    required this.authorizationToken,
    required this.rootUri,
  });
}
