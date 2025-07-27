import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { Store } from "@ngrx/store";
import { firstValueFrom, Observable, of, Subject, combineLatest } from "rxjs";
import { takeUntil, map, take } from "rxjs/operators";
import { GameStateActions, GameStateSelectors } from "../../state/game";
import { JoinGameCommand } from "../../requests/JoinGameCommand";
import { CreateX01ScoreCommand } from "../../requests/CreateX01ScoreCommand";
import { WebSocketService } from "../../services/websocket/websocket.service";
import { WebSocketActions } from "../../services/websocket/websocket.actions.enum";
import { WebSocketMessage } from "../../services/websocket/websocket.message.model";
import { FlyingdartsRepository } from "src/sdk/flyingdarts.repository";
import { FlyingdartsSdk } from "src/sdk/flyingdarts-sdk";
import { FlyingdartsSdkService } from "src/sdk/flyingdarts-sdk-service";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-game-debug",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./debug.container.html",
  styleUrls: ["./debug.container.scss"],
})
export class GameDebugComponent implements OnInit, OnDestroy {
  userId$: Observable<string>;
  nextPlayer$: Observable<string | null>;
  winningPlayer$: Observable<string | null>;
  enableControls$: Observable<boolean>;
  remainingScore$: Observable<number>;
  gameId: string;
  showDebug = false;

  joinGameEvents: Array<{
    timestamp: Date;
    data: JoinGameCommand;
    metadata?: any;
  }> = [];
  scoreEvents: Array<{
    timestamp: Date;
    data: CreateX01ScoreCommand;
    metadata?: any;
  }> = [];
  selectedMetadata: any = null;
  showMetadataModal = false;
  recentMessages: Array<{
    timestamp: Date;
    action: string;
    hasMessage: boolean;
    hasMetadata: boolean;
    rawMessage: any;
  }> = [];
  selectedRawMessage: any = null;
  showRawMessageModal = false;

  private destroy$ = new Subject<void>();

  constructor(
    private readonly store: Store,
    private readonly route: ActivatedRoute,
    private readonly sdk: FlyingdartsSdkService
  ) {
    this.userId$ = of(sessionStorage.getItem("userId")!);
    this.nextPlayer$ = this.store.select(GameStateSelectors.selectNextPlayer);
    this.winningPlayer$ = this.store.select(
      GameStateSelectors.selectWinningPlayer
    );
    this.enableControls$ = combineLatest([
      this.store.select(GameStateSelectors.selectNextPlayer),
      this.userId$,
    ]).pipe(
      map(([nextPlayer, userId]) => {
        return nextPlayer !== null && nextPlayer === userId;
      })
    );

    this.remainingScore$ = this.store.select(
      GameStateSelectors.selectPlayerScore
    );

    this.gameId = this.route.snapshot.paramMap.get("gameId")!;
  }

  ngOnInit(): void {
    this.sdk.instance?.webSocketService.messages$
      .pipe(takeUntil(this.destroy$))
      .subscribe((message: WebSocketMessage) => {
        this.handleWebSocketMessage(message);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private handleWebSocketMessage(message: WebSocketMessage): void {
    console.log("[DEBUG] Received WebSocket message:", {
      action: message.action,
      hasMessage: !!message.message,
      hasMetadata: !!message.metadata,
      metadata: message.metadata,
    });

    // Add to recent messages for debugging
    this.recentMessages.unshift({
      timestamp: new Date(),
      action: message.action,
      hasMessage: !!message.message,
      hasMetadata: !!message.metadata,
      rawMessage: message,
    });

    // Keep only last 5 messages
    if (this.recentMessages.length > 5) {
      this.recentMessages = this.recentMessages.slice(0, 5);
    }

    if (message.action === WebSocketActions.X01Join && message.message) {
      const joinEvent = message.message as JoinGameCommand;
      console.log(
        "[DEBUG] Processing X01Join event with metadata:",
        message.metadata
      );
      this.joinGameEvents.unshift({
        timestamp: new Date(),
        data: joinEvent,
        metadata: message.metadata,
      });

      // Keep only last 10 events
      if (this.joinGameEvents.length > 10) {
        this.joinGameEvents = this.joinGameEvents.slice(0, 10);
      }
    } else if (
      message.action === WebSocketActions.X01Score &&
      message.message
    ) {
      const scoreEvent = message.message as CreateX01ScoreCommand;
      console.log(
        "[DEBUG] Processing X01Score event with metadata:",
        message.metadata
      );
      this.scoreEvents.unshift({
        timestamp: new Date(),
        data: scoreEvent,
        metadata: message.metadata,
      });

      // Keep only last 10 events
      if (this.scoreEvents.length > 10) {
        this.scoreEvents = this.scoreEvents.slice(0, 10);
      }
    }
  }

  showRawMessage(rawMessage: any): void {
    this.selectedRawMessage = rawMessage;
    this.showRawMessageModal = true;
  }

  closeRawMessageModal(): void {
    this.showRawMessageModal = false;
    this.selectedRawMessage = null;
  }

  showMetadata(metadata: any): void {
    this.selectedMetadata = metadata;
    this.showMetadataModal = true;
  }

  closeMetadataModal(): void {
    this.showMetadataModal = false;
    this.selectedMetadata = null;
  }

  toggleDebug(): void {
    this.showDebug = !this.showDebug;
  }

  clearJoinEvents(): void {
    this.joinGameEvents = [];
  }

  clearScoreEvents(): void {
    this.scoreEvents = [];
  }

  async logRandomNumber(): Promise<void> {
    const randomNumber = Math.floor(Math.random() * (170 - 30 + 1)) + 30;
    // if remaing score is less then 170, just send remaining score
    const remainingScore = await firstValueFrom(this.remainingScore$);
    const score = remainingScore > 170 ? randomNumber : remainingScore;
    const newScore = score + remainingScore;
    const userId = sessionStorage.getItem("userId")!;
    this.sdk.instance?.sendScore(this.gameId, userId, newScore, score);
  }

  getFormattedJson(obj: any): string {
    return JSON.stringify(obj, null, 2);
  }
}

export enum NextPlayerType {
  Nobody,
  Me,
  Other,
}
