abstract class WebSocketRequest {}

class MessageRequest extends WebSocketRequest {
  late DateTime Date;
  late String Message;
  late String Owner;
}
