import 'dart:developer';

import 'package:amplify_auth_cognito/amplify_auth_cognito.dart';
import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:appbar/appbar.dart';
import 'package:configuration/configuration.dart';
import 'package:configuration_secrets/secrets.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:navigation/navigation.dart';
import 'package:ui/ui.dart';

typedef SignInCallback = void Function(bool isSignedIn);

class LoginScreen extends StatelessWidget {
  const LoginScreen({super.key});

  @override
  Widget build(BuildContext context) {
    var cubit = context.watch<NavigationCubit>();
    return Scaffold(
      appBar: const MyAppBar(),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Center(
          child: Flex(
            direction: Axis.vertical,
            children: [
              Expanded(
                child: Align(
                  alignment: Alignment.bottomCenter,
                  child: LoginWithFacebookButton(
                    onPressed: () async {
                      cubit.setIsLoading(true);
                      await _signInAsync(
                        (signedIn) => log(
                          signedIn.toString(),
                        ),
                      );
                    },
                  ),
                ),
              )
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _signInAsync(SignInCallback callback) async {
    try {
      SignInResult signInResult = await Amplify.Auth.signInWithWebUI(provider: AuthProvider.google);

      if (signInResult.isSignedIn) {
        var writeableCredentials = GetIt.I<WriteableConfiguration<Credentials>>();
        var session = await Amplify.Auth.fetchAuthSession();
        var accessToken = (session as CognitoAuthSession).userPoolTokensResult.value.accessToken.toJson();
        var idToken = session.userPoolTokensResult.value.idToken.toJson();
        var refreshToken = session.userPoolTokensResult.value.refreshToken;
        var credentials = Credentials(accessToken: accessToken, idToken: idToken, refreshToken: refreshToken);
        await writeableCredentials.update(credentials);
        log("signed in");
      } else {
        log("not signed in");
      }
      callback(signInResult.isSignedIn);
    } catch (e) {
      // Handle the authentication error
      log('Authentication error: $e');
    }
  }
}
