import 'dart:developer';

import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:appbar/appbar.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:navigation/navigation.dart';
import 'package:ui/ui.dart';

typedef SignInCallback = void Function(bool isSignedIn);

class LoginPage extends StatelessWidget {
  const LoginPage({super.key});

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
      SignInResult signInResult = await Amplify.Auth.signInWithWebUI(provider: AuthProvider.facebook);

      if (signInResult.isSignedIn) {
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
