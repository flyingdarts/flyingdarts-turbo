import { WebSocketRequest } from '../services/websocket/websocket.request.model';

export interface CreateX01GameCommand extends WebSocketRequest {
  PlayerId: string;
  GameId?: string;
  Sets: number;
  Legs: number;
}
