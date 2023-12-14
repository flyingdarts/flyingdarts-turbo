import 'dart:convert';

import 'package:amplify/amplify.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:injectable/injectable.dart';
import 'package:websocket/websocket.dart';

import '../models/get_user_profile_query.dart';
import 'profile_state.dart';

@lazySingleton
class ProfileCubit extends Cubit<ProfileState> {
  final WebSocketService _webSocketService;
  final AmplifyService _amplifyService;

  ProfileCubit(this._webSocketService, this._amplifyService) : super(ProfileState.initialState);

  Future<void> init() async {
    var userId = await _amplifyService.getUsername();
    var socketMessage = WebSocketMessage<GetUserProfileQuery>(
      action: "user/profile/get",
      message: GetUserProfileQuery(cognitoUserName: userId),
    );
    var message = {
      "action": socketMessage.action,
      "message": socketMessage.message,
    };
    debugPrint(jsonEncode(message));
    _webSocketService.postMessage(jsonEncode(message));
    _webSocketService.messages.listen((WebSocketMessage message) {
      if (message.action == "user/profile/get") {
        var body = message.message;
        emit(
          state.copyWith(
            userId: body["UserId"],
            userName: body["UserName"],
            country: body["Country"],
            email: body["Email"],
          ),
        );
      }
    });
  }
}
