import { WebSocketRequest } from '../services/websocket/websocket.request.model';

export interface CreateRoomCommand extends WebSocketRequest {
  RoomId: string;
}
