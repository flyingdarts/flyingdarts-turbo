import 'package:api_sdk/flyingdarts_sdk.dart';
import 'package:app_config_core/configuration.dart';
import 'package:app_config_secrets/secrets.dart';
import 'package:dio/dio.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:injectable/injectable.dart';

import 'profile_state.dart';

@lazySingleton
class ProfileCubit extends Cubit<ProfileState> {
  final ConfigWriter<AuthTokens> _writeableCredentials;

  ProfileCubit(this._writeableCredentials) : super(ProfileState.initialState);

  Future<void> init() async {
    const userId = "123";
    var credentials = _writeableCredentials.data;
    var api = UsersApi(
      UsersApiConfig(
        authorizationToken: credentials.accessToken,
        rootUri: Uri.parse(
          'https://0ftaw0laa1.execute-api.eu-west-1.amazonaws.com/prod',
        ),
      ),
      Dio(),
      ErrorResponseHandler(),
    );
    var result = await api.getUser(userId);

    state.copyWith(
      userId: result.asSuccessResponse().response.userProfile.UserId,
      userName: result.asSuccessResponse().response.userProfile.UserName,
      email: result.asSuccessResponse().response.userProfile.Email,
      country: result.asSuccessResponse().response.userProfile.Country,
    );
  }
}
