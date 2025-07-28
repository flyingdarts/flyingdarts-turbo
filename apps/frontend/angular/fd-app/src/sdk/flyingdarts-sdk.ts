import { filter, Observable, Subject, tap } from 'rxjs';
import { CreateX01GameCommand } from 'src/app/requests/CreateX01GameCommand';
import { CreateX01ScoreCommand } from 'src/app/requests/CreateX01ScoreCommand';
import { JoinGameCommand } from 'src/app/requests/JoinGameCommand';
import { WebSocketMessageQueueService } from 'src/app/services/websocket/websocket-message-queue.service';
import { WebSocketActions } from 'src/app/services/websocket/websocket.actions.enum';
import { Metadata, WebSocketMessage } from 'src/app/services/websocket/websocket.message.model';
import { WebSocketService } from 'src/app/services/websocket/websocket.service';
import { FlyingdartsRepository } from './flyingdarts.repository';

export interface StateNavigationHooks {
  gameCreated(gameId: string): void;
}
export class FlyingdartsSdk {
  private metadataSubject = new Subject<Metadata>();

  readonly metadata$: Observable<Metadata> = this.metadataSubject.asObservable();

  constructor(
    public readonly webSocketService: WebSocketService,
    private readonly webSocketMessageQueueService: WebSocketMessageQueueService,
    private readonly repository: FlyingdartsRepository // init
  ) {
    this.webSocketService.messages$
      .pipe(
        tap(message => {
          console.log('[SDK] WebSocket message received:', JSON.stringify(message, null, 2));
        }),
        filter(message => [WebSocketActions.X01Join, WebSocketActions.X01Score].includes(message.action)),
        tap(message => {
          console.log('[SDK] WebSocket message passed filter:', JSON.stringify(message, null, 2));
          if (message.metadata) {
            console.log('[SDK] Emitting metadata:', JSON.stringify(message.metadata, null, 2));
            this.metadataSubject.next(message.metadata);
          }
        })
      )
      .subscribe();
  }

  public get connectionState(): Observable<boolean> {
    return this.webSocketService.isConnected$;
  }

  public createGame(gamePlayerId: string, sets: number = 1, legs: number = 3) {
    var message: CreateX01GameCommand = {
      PlayerId: gamePlayerId,
      Sets: sets,
      Legs: legs,
    };

    let body: WebSocketMessage<CreateX01GameCommand> = {
      action: WebSocketActions.X01Create,
      message: message,
    };
    console.log('[SDK] Sending CreateX01GameCommand:', JSON.stringify(body, null, 2));
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
    console.log('[SDK] Sending JoinGameCommand:', JSON.stringify(body, null, 2));
    this.webSocketMessageQueueService.sendMessage(JSON.stringify(body));
  }

  public sendScore(gameId: string, playerId: string, score: number, input: number) {
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
    console.log('[SDK] Sending CreateX01ScoreCommand:', JSON.stringify(body, null, 2));
    this.webSocketMessageQueueService.sendMessage(JSON.stringify(body));
  }
}
