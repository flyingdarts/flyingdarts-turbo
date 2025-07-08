import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import 'navigation_state.dart';

class NavigationCubit extends Cubit<NavigationState> {
  NavigationCubit() : super(NavigationState.defaults());

  void setPage(int page) {
    emit(state.copyWith(selectedPage: page));
  }

  void setPages(List<Widget> pages) {
    emit(state.copyWith(pages: pages));
  }

  void setIsLoading(bool isLoading) {
    emit(state.copyWith(isLoading: isLoading));
  }

  void setIsLoggedIn(bool isLoggedIn) {
    emit(state.copyWith(isLoggedIn: isLoggedIn));
  }

  Future<void> init() async {
    setPage(0);
    setIsLoading(false);

    setupHubListener();
  }

  Future<void> initTest() async {
    setPage(0);
    setIsLoading(false);
  }

  void setupHubListener() {
    Amplify.Hub.listen(
      HubChannel.Auth,
      (AuthHubEvent event) {
        switch (event.type) {
          case AuthHubEventType.signedIn:
            setIsLoading(false);
            setIsLoggedIn(true);
            break;
          case AuthHubEventType.signedOut:
            setIsLoading(false);
            setIsLoggedIn(false);
            break;
          case AuthHubEventType.sessionExpired:
            safePrint('The session has expired.');
            break;
          case AuthHubEventType.userDeleted:
            safePrint('The user has been deleted.');
            break;
        }
      },
    );
  }
}
