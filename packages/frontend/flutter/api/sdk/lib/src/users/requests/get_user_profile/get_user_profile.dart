import 'package:dio/dio.dart';

import '../../../exceptions/invalid_format_exception.dart';
import '../../../responses/request_failed_response.dart';
import '../../../responses/request_response.dart';
import '../../models/user_profile.dart';
import '../../options/options_builder.dart';
import '../../users_api.dart';
import '../../users_endpoints.dart';
import 'success_response.dart';

extension GetUserProfile on UsersApi {
  Future<RequestResponse<GetUserProfileSuccessResponse, RequestFailedResponse>>
  getUser(String userId) async {
    Uri uri = UsersEndpoints.applyPathSections(
      config.rootUri,
      UsersEndpoints.profile,
    );
    Map<String, dynamic> queryParameters = {'userId': userId};

    try {
      Options options = OptionsBuilder(config).useAuthorizationToken().build();

      var res = await dio.get(
        uri.toString(),
        queryParameters: queryParameters,
        options: options,
      );

      if (res.data == null) {
        throw InvalidFormatException(null, "GET", uri.toString());
      }

      try {
        var userProfile = UserProfile.fromJson(res.data);

        return SuccessResponse(
          GetUserProfileSuccessResponse(res.statusCode!, userProfile),
        );
      } catch (err) {
        throw InvalidFormatException(err, "GET", uri.toString());
      }
    } on DioError catch (err) {
      var response = responseHandler
          .handleErrorResponse<GetUserProfileSuccessResponse>(err);
      return response;
    }
  }
}
