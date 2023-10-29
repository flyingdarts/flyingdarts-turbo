import { WebSocketRequest } from "./../infrastructure/websocket/websocket.request.model";

export interface JoinGameCommand extends WebSocketRequest {
  Game?: Game;
  GameId: string;
  PlayerId: string;
  PlayerName: string;
  Metadata?: any;
  FirstToThrow?: string;
  History?: any;
}

export interface Game {
  X01: X01;
}

export interface X01 {
  DoubleIn: boolean;
  DoubleOut: boolean;
  Legs: number;
  Sets: number;
  StartingScore: number;
}