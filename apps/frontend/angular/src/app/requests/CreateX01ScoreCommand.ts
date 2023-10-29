import { WebSocketRequest } from "./../infrastructure/websocket/websocket.request.model";


export interface CreateX01ScoreCommand extends WebSocketRequest {
  GameId: string;
  PlayerId: string;
  Score: number;
  Input: number;
  History?: any;
  NextToThrow?: string;
}
