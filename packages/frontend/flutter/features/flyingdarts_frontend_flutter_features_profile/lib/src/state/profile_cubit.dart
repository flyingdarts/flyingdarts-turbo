import 'package:amplify/amplify.dart';
import 'package:configuration/configuration.dart';
import 'package:configuration_secrets/secrets.dart';
import 'package:dio/dio.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flyingdarts_sdk/flyingdarts_sdk.dart';
import 'package:injectable/injectable.dart';
import 'package:websocket/websocket.dart';
import '../models/get_user_profile_query.dart';
import 'profile_state.dart';

@lazySingleton
class ProfileCubit extends Cubit<ProfileState> {
  final WebSocketService _webSocketService;
  final AmplifyService _amplifyService;
  final WriteableConfiguration<Credentials> _writeableCredentials;
  final Dio _dio;

  ProfileCubit(
    this._webSocketService,
    this._amplifyService,
    this._writeableCredentials,
    this._dio,
  ) : super(ProfileState.initialState);

  Future<void> init() async {
    if (await _amplifyService.getUsername() == null) return;
    var userId = await _amplifyService.getUsername();
    var credentials = _writeableCredentials.config;
    var api = UsersApi(
      UsersApiConfig(
        authorizationToken: credentials.accessToken,
        rootUri: Uri.parse('https://0ftaw0laa1.execute-api.eu-west-1.amazonaws.com/prod'),
      ),
      Dio(),
      ErrorResponseHandler(),
    );
    var result = await api.getUser(userId!);

    state.copyWith(
      userId: result.asSuccessResponse().response.userProfile.UserId,
      userName: result.asSuccessResponse().response.userProfile.UserName,
      email: result.asSuccessResponse().response.userProfile.Email,
      country: result.asSuccessResponse().response.userProfile.Country,
    );
  }
}
