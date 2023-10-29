import { WebSocketRequest } from "./../infrastructure/websocket/websocket.request.model";


export interface JoinX01QueueCommand extends WebSocketRequest {
  PlayerId: string;
  GameId?: string;
}
