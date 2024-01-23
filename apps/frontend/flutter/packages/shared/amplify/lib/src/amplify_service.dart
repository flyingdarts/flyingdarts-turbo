import 'package:amplify_auth_cognito/amplify_auth_cognito.dart';
import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:injectable/injectable.dart';

@injectable
class AmplifyService {
  Future<String?> getUsername() async {
    try {
      return (await Amplify.Auth.getCurrentUser()).username;
    } catch (e) {
      return null;
    }
  }

  Future<String?> getAccessToken() async {
    try {
      var session = await Amplify.Auth.fetchAuthSession();
      var token = (session as CognitoAuthSession).userPoolTokensResult.value.accessToken;
      return token.toJson();
    } catch (err) {
      return null;
    }
  }

  Future<String?> getIdToken() async {
    try {
      var session = await Amplify.Auth.fetchAuthSession();
      var token = (session as CognitoAuthSession).userPoolTokensResult.value.idToken;
      return token.toJson();
    } catch (err) {
      return null;
    }
  }

  Future<void> logout() async {
    await Amplify.Auth.signOut();
  }

  Future<void> login() async {
    await Amplify.Auth.signInWithWebUI(provider: AuthProvider.facebook);
  }
}
