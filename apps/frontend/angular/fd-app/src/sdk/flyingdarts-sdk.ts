import { filter, Observable, Subject, tap } from 'rxjs';
import { CreateX01GameCommand } from 'src/app/requests/CreateX01GameCommand';
import { CreateX01ScoreCommand } from 'src/app/requests/CreateX01ScoreCommand';
import { JoinGameCommand } from 'src/app/requests/JoinGameCommand';
import { JoinX01QueueCommand } from 'src/app/requests/JoinX01QueueCommand';
import { WebSocketMessageQueueService } from 'src/app/services/websocket/websocket-message-queue.service';
import { WebSocketActions } from 'src/app/services/websocket/websocket.actions.enum';
import {
  Metadata,
  WebSocketMessage,
} from 'src/app/services/websocket/websocket.message.model';
import { WebSocketService } from 'src/app/services/websocket/websocket.service';
import { FlyingdartsRepository } from './flyingdarts.repository';

export class FlyingdartsSdk {
  private metadataSubject = new Subject<Metadata>();

  readonly metadata$: Observable<Metadata> =
    this.metadataSubject.asObservable();

  constructor(
    private readonly webSocketService: WebSocketService,
    private readonly webSocketMessageQueueService: WebSocketMessageQueueService,
    private readonly repository: FlyingdartsRepository
  ) {
    this.webSocketService.messages$
      .pipe(
        filter((message) =>
          [WebSocketActions.X01Join, WebSocketActions.X01Score].includes(
            message.action
          )
        ),
        tap((message) => {
          if (message.metadata) {
            this.metadataSubject.next(message.metadata);
          }
        })
      )
      .subscribe((metadata) => console.log(metadata));
  }

  public get connectionState(): Observable<boolean> {
    return this.webSocketService.isConnected$;
  }

  public createGame(
    gamePlayerId: string,
    sets: number = 1,
    legs: number = 3,
    roomId?: string
  ) {
    var message: CreateX01GameCommand = {
      PlayerId: gamePlayerId,
      Sets: sets,
      Legs: legs,
    };
    if (!!roomId) message.GameId = roomId;

    let body: WebSocketMessage<CreateX01GameCommand> = {
      action: WebSocketActions.X01Create,
      message: message,
    };
    this.webSocketMessageQueueService.sendMessage(JSON.stringify(body));
  }

  public joinGame(gameId: string, playerId: string, playerName: string) {
    var message: JoinGameCommand = {
      GameId: gameId,
      PlayerId: playerId,
      PlayerName: playerName,
    };
    let body: WebSocketMessage<JoinGameCommand> = {
      action: WebSocketActions.X01Join,
      message: message,
    };
    this.webSocketMessageQueueService.sendMessage(JSON.stringify(body));
  }

  public joinQueue(
    gamePlayerId: string,
    sets: number = 1,
    legs: number = 3,
    roomId?: string
  ) {
    var message: JoinX01QueueCommand = {
      PlayerId: gamePlayerId,
      Sets: sets,
      Legs: legs,
    };
    if (!!roomId) message.GameId = roomId;

    let body: WebSocketMessage<JoinX01QueueCommand> = {
      action: WebSocketActions.X01JoinQueue,
      message: message,
    };
    this.webSocketMessageQueueService.sendMessage(JSON.stringify(body));
  }

  public sendScore(
    gameId: string,
    playerId: string,
    score: number,
    input: number
  ) {
    var message: CreateX01ScoreCommand = {
      GameId: gameId,
      PlayerId: playerId,
      Score: score,
      Input: input,
    };
    let body: WebSocketMessage<CreateX01ScoreCommand> = {
      action: WebSocketActions.X01Score,
      message: message,
    };
    this.webSocketMessageQueueService.sendMessage(JSON.stringify(body));
  }
}
