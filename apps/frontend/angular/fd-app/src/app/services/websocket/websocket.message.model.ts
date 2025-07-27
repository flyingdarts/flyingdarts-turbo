import { DartDto } from "src/app/dtos/dart.dto";
import { GameDto } from "src/app/dtos/game.dto";
import { PlayerDto } from "src/app/dtos/player.dto";
import { WebSocketActions } from "./websocket.actions.enum";
import { WebSocketRequest } from "./websocket.request.model";

export interface WebSocketMessage<T = WebSocketRequest> {
  action: WebSocketActions;
  message?: T;
  metadata?: Metadata;
}

export class Metadata {
  Game!: GameDto;
  Players!: PlayerDto[];
  Darts!: Record<string, DartDto[]>;
  NextPlayer!: string;
  WinningPlayer!: string;
  MeetingIdentifier?: string;
  MeetingToken?: string;
}
