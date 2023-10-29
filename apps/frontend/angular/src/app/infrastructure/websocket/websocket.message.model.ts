import { Metadata } from 'src/app/games/x01/Metadata';
import { WebSocketActions } from './websocket.actions.enum';
import { WebSocketRequest } from './websocket.request.model';


export interface WebSocketMessage<T = WebSocketRequest> {
  action: WebSocketActions;
  message?: T;
  metadata?: Metadata
}