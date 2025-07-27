import 'dart:io';

import 'package:api_sdk/flyingdarts_sdk.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';

import '../../../mocks/http_client_adapter_factory.dart';

main() {
  group('GetUserProfile', () {
    test(
      "Given a valid request and response When get user profile request is made Then it should correctly parse the result",
      () async {
        var dio = Dio(BaseOptions());
        var adapterMockFactory = HttpClientAdapterFactory();
        var adapterMock = adapterMockFactory.create();

        dio.httpClientAdapter = adapterMock;

        when(adapterMock.fetch(any, any, any)).thenAnswer(
          (realInvocation) => Future.value(
            ResponseBody.fromString(
              """
            {
                "UserId": "638397079665490631",
                "UserName": "Socratez",
                "Email": "mike@flyingdarts.net",
                "Country": "AT"
            }
          """,
              206,
              headers: {
                HttpHeaders.contentTypeHeader: ["application/json"],
              },
            ),
          ),
        );

        var responseHandler = ErrorResponseHandler();

        var api = UsersApi(
          UsersApiConfig(
            authorizationToken: "id-token",
            rootUri: Uri(scheme: "https", host: "0ftaw0laa1.execute-api.eu-west-1.amazonaws.com", path: 'prod'),
          ),
          dio,
          responseHandler,
        );

        var res = await api.getUser("google_102211021390759686933");
        var captured = verify(
          adapterMock.fetch(captureThat(anything), argThat(isNull), argThat(isNull)),
        ).captured.single;

        // Verify request made
        expect(captured, isInstanceOf<RequestOptions>());
        var capturedRequestOptions = captured as RequestOptions;
        expect(
          capturedRequestOptions.path,
          "https://0ftaw0laa1.execute-api.eu-west-1.amazonaws.com/prod/users/profile",
        );

        expect(res, isInstanceOf<SuccessResponse>(), reason: "request should succeed");
        var okRes = res.asSuccessResponse();
        expect(okRes.response, isInstanceOf<GetUserProfileSuccessResponse>());
      },
    );
  });
}
