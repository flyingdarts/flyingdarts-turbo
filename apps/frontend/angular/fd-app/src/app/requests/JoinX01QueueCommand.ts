import { WebSocketRequest } from '../services/websocket/websocket.request.model';

export interface JoinX01QueueCommand extends WebSocketRequest {
  PlayerId: string;
  GameId?: string;
  Sets: number;
  Legs: number;
}
