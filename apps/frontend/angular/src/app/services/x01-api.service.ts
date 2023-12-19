import { Injectable } from '@angular/core';
import { WebSocketService } from "../infrastructure/websocket/websocket.service";
import { WebSocketActions } from '../infrastructure/websocket/websocket.actions.enum';
import { WebSocketMessage } from './../infrastructure/websocket/websocket.message.model';
import { CreateX01ScoreCommand } from './../requests/CreateX01ScoreCommand';
import { JoinX01QueueCommand } from './../requests/JoinX01QueueCommand';
import { JoinGameCommand } from './../requests/JoinGameCommand';
import { WebSocketMessageService } from '../infrastructure/websocket/websocket-message.service';
import { CreateX01GameCommand } from '../requests/CreateX01GameCommand';

@Injectable({ providedIn: 'root' })
export class X01ApiService {
  constructor(private webSocketMessageService: WebSocketMessageService) {

  }

  public createGame(gamePlayerId: string, sets: number = 1, legs: number = 3, roomId?: string) {
    var message: CreateX01GameCommand = {
      PlayerId: gamePlayerId,
      Sets: sets,
      Legs: legs
    };
    if (!!roomId) 
      message.GameId = roomId;
      
    let body: WebSocketMessage<CreateX01GameCommand> = {
      action: WebSocketActions.X01Create,
      message: message
    };
    this.webSocketMessageService.sendMessage(JSON.stringify(body));
  }

  public joinGame(gameId: string, playerId: string, playerName: string) {
    console.log("join x01 game");
    var message: JoinGameCommand = {
        GameId: gameId,
        PlayerId: playerId,
        PlayerName: playerName
    };
    let body: WebSocketMessage<JoinGameCommand> = {
        action: WebSocketActions.X01Join,
        message: message
    };
    console.log(body);
    this.webSocketMessageService.sendMessage(JSON.stringify(body));
  }

  public joinQueue(gamePlayerId: string, sets: number = 1, legs: number = 3, roomId?: string) {
    var message: JoinX01QueueCommand = {
      PlayerId: gamePlayerId,
      Sets: sets,
      Legs: legs
    };
    if (!!roomId) 
      message.GameId = roomId;
      
    let body: WebSocketMessage<JoinX01QueueCommand> = {
      action: WebSocketActions.X01JoinQueue,
      message: message
    };
    this.webSocketMessageService.sendMessage(JSON.stringify(body));
  }

  public score(gameId: string, playerId: string, score: number, input: number) {
    var message: CreateX01ScoreCommand = {
      GameId: gameId,
      PlayerId: playerId,
      Score: score,
      Input: input
    };
    let body: WebSocketMessage<CreateX01ScoreCommand> = {
      action: WebSocketActions.X01Score,
      message: message
    };
    this.webSocketMessageService.sendMessage(JSON.stringify(body));
  }
}