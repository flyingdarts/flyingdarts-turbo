import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:websocket/websocket.dart';

main() {
  test('websocket message test', () {
    var socketMessage = WebSocketMessage<TestClass>(
      action: "user/profile/get",
      message: TestClass(TestProperty: "TestValue"),
    );
    var test = socketMessage.toJson();
    var encoded = jsonEncode(socketMessage);
    var result = jsonDecode(encoded);

    expect(result["action"], "user/profile/get");
  });
}

class TestClass {
  final String TestProperty;

  TestClass({required this.TestProperty});
}
