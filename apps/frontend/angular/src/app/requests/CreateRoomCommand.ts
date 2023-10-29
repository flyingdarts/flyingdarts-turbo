import { WebSocketRequest } from "./../infrastructure/websocket/websocket.request.model";


export interface CreateRoomCommand extends WebSocketRequest {
  RoomId: string;
}
