import { WebSocketRequest } from '../services/websocket/websocket.request.model';

export interface HandleX01QueueCommand extends WebSocketRequest {
  PlayerId: string;
  GameId?: string;
  Sets: number;
  Legs: number;
}
