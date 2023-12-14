import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:injectable/injectable.dart';

@injectable
class AmplifyService {
  Future<String> getUsername() async {
    return (await Amplify.Auth.getCurrentUser()).username;
  }

  Future<void> logout() async {
    await Amplify.Auth.signOut();
  }

  Future<void> login() async {
    await Amplify.Auth.signInWithWebUI(provider: AuthProvider.facebook);
  }
}
