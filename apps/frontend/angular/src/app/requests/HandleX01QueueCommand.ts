import { WebSocketRequest } from "../infrastructure/websocket/websocket.request.model";


export interface HandleX01QueueCommand extends WebSocketRequest {
  PlayerId: string;
  GameId?: string;
  Sets: number;
  Legs: number;
}
