import '../flyingdarts_api_config.dart';
import 'package:injectable/injectable.dart';

@injectable
class UsersApiConfig extends FlyingdartsApiConfig {
  const UsersApiConfig({
    @authorizationToken required super.authorizationToken,
    @rootUri required super.rootUri,
  });
}
