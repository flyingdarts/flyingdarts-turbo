import '../../../responses/request_success_response.dart';
import '../../models/user_profile.dart';

class GetUserProfileSuccessResponse extends RequestSuccessResponse {
  final UserProfile userProfile;

  const GetUserProfileSuccessResponse(super.httpCode, this.userProfile);
}
