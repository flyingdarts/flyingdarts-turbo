import { AfterViewInit, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { filter, map, merge, Observable, pairwise, Subject } from 'rxjs';
import { DartDto } from 'src/app/dtos/dart.dto';

import { CommonModule } from '@angular/common';
import { Metadata } from 'src/app/services/websocket/websocket.message.model';
import { AppStateActions } from 'src/app/state/app';
import { GameStateActions, GameStateSelectors } from 'src/app/state/game';
import { GameState } from 'src/app/state/game/game.state';
import { FlyingdartsSdkService } from 'src/sdk/flyingdarts-sdk-service';
import { GameComponent } from '../presentation/game.component';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, GameComponent],
  templateUrl: './game.container.html',
})
export class GameContainerComponent implements AfterViewInit {
  readonly meetingId$: Observable<string>;
  readonly shouldEnableControls$: Observable<boolean>;
  readonly canCheckOut$: Observable<boolean>;
  readonly input$: Observable<string>;

  private readonly overrideShouldEnableControlsSubject = new Subject<boolean>();
  private readonly winnerSubject = new Subject<{
    winner: string | null;
    text: string | null;
  } | null>();

  readonly winner$ = this.winnerSubject.asObservable();

  private gameId = this.route.snapshot.paramMap.get('meetingId')!;
  private userId = sessionStorage.getItem('userId')!;
  private userName = sessionStorage.getItem('userName')!;

  constructor(
    private readonly sdkService: FlyingdartsSdkService,
    private readonly route: ActivatedRoute,
    private readonly store: Store
  ) {
    this.sdkService.initSdk();

    this.sdkService.instance?.metadata$.subscribe((metadata) => {
      console.log('got metadata in game container', metadata);
      this.handleMetadata(metadata);
    });

    this.store
      .select(GameStateSelectors.selectGameState)
      .pipe(
        pairwise(),
        map(([prevState, currState]) =>
          this.determineWinner(prevState, currState)
        ),
        filter((result) => result !== null)
      )
      .subscribe();

    this.meetingId$ = this.route.queryParams.pipe(
      map((params) => params['meetingId'])
    );

    const playerId = this.userId;

    this.shouldEnableControls$ = merge(
      this.store
        .select(GameStateSelectors.selectNextPlayer)
        .pipe(map((nextPlayer) => nextPlayer === playerId)),
      this.overrideShouldEnableControlsSubject
    );

    this.canCheckOut$ = this.store
      .select(GameStateSelectors.selectPlayerScore)
      .pipe(
        map((playerScore) => {
          const bogeyScores = [159, 162, 163, 165, 166, 168, 169];
          return playerScore < 170 && !bogeyScores.includes(playerScore);
        })
      );

    this.input$ = this.store
      .select(GameStateSelectors.selectGameInput)
      .pipe(map((input) => input ?? ''));
  }

  closeWinnerPopup() {
    this.winnerSubject.next(null);
  }

  ngAfterViewInit(): void {
    this.sdkService.instance?.joinGame(this.gameId, this.userId, this.userName);
    this.store.dispatch(AppStateActions.setLoading({ loading: false }));
  }

  handleMetadata(metadata: Metadata) {
    const userId = this.userId;
    if (!userId) {
      console.error('User ID not found in session storage.');
      return;
    }

    const { Players, Darts, Game, NextPlayer, WinningPlayer } = metadata;
    const player = Players.find((x) => x.PlayerId === userId);
    const opponent = Players.find((x) => x.PlayerId !== userId);

    const playerDarts: DartDto[] = Darts ? Darts[player?.PlayerId!] : [];
    const opponentDarts: DartDto[] = Darts ? Darts[opponent?.PlayerId!] : [];

    const gameState: GameState = {
      player: {
        id: player?.PlayerId ?? '',
        name: player?.PlayerName ?? '',
        score: Game.X01.StartingScore - this.sumOfHistory(playerDarts),
        scores: playerDarts.map((x) => x.Score),
        legs: isNaN(Number(player?.Legs)) ? 0 : Number(player?.Legs),
        sets: isNaN(Number(player?.Sets)) ? 0 : Number(player?.Sets),
        total: Game.X01.StartingScore,
      },
      opponent: {
        id: opponent?.PlayerId ?? '',
        name: opponent?.PlayerName ?? '',
        score: Game.X01.StartingScore - this.sumOfHistory(opponentDarts),
        scores: opponentDarts.map((x) => x.Score),
        sets: isNaN(Number(opponent?.Sets)) ? 0 : Number(opponent?.Sets),
        legs: isNaN(Number(opponent?.Legs)) ? 0 : Number(opponent?.Legs),
        total: Game.X01.StartingScore,
      },
      game: {
        legs: Game.X01.Legs,
        sets: Game.X01.Sets,
        score: Game.X01.StartingScore,
      },
      nextPlayer: NextPlayer,
      winningPlayer: WinningPlayer,
    };

    this.store.dispatch(GameStateActions.updateState({ state: gameState }));
  }

  private sumOfHistory(darts: DartDto[]): number {
    return darts.reduce((acc, dart) => acc + dart.Score, 0);
  }

  private determineWinner(prevState: GameState, currState: GameState) {
    const prevWinningPlayerId = prevState.winningPlayer;
    const currWinningPlayerId = currState.winningPlayer;

    const currPlayers = [currState.player, currState.opponent];

    const currWinningPlayer = currPlayers.find(
      (player) => player.id === currWinningPlayerId
    );

    if (
      currWinningPlayer &&
      currWinningPlayerId &&
      prevWinningPlayerId === null
    ) {
      this.winnerSubject.next({
        winner: currWinningPlayer.name,
        text: 'Wins the game!',
      });
      return;
    }

    const playerWonLeg =
      prevState.player.legs < currState.player.legs &&
      prevState.player.sets == currState.player.sets;

    const opponentWonLeg =
      prevState.opponent.legs < currState.opponent.legs &&
      prevState.opponent.sets == currState.opponent.sets;

    if (playerWonLeg || opponentWonLeg) {
      this.winnerSubject.next({
        winner: playerWonLeg
          ? currState.player.name
          : opponentWonLeg
          ? currState.opponent.name
          : null,
        text: 'Wins the leg!',
      });
      return;
    }

    const playerWonSet = prevState.player.sets < currState.player.sets;

    const opponentWonSet = prevState.opponent.sets < currState.opponent.sets;

    if (playerWonSet || opponentWonSet) {
      this.winnerSubject.next({
        winner: playerWonSet
          ? currState.player.name
          : opponentWonSet
          ? currState.opponent.name
          : null,
        text: 'Wins the set!',
      });
      return;
    }

    return null;
  }

  inputScore(input: string) {
    this.store.dispatch(GameStateActions.updateInput({ input }));
  }

  clearScore() {
    this.store.dispatch(GameStateActions.clearInput());
  }

  async quickScore(score: number, updatedScore: number) {
    this.sdkService.instance?.sendScore(
      this.gameId,
      this.userId,
      updatedScore,
      score
    );
  }

  async noScore(currentScore: number) {
    this.sdkService.instance?.sendScore(
      this.gameId,
      this.userId,
      currentScore,
      0
    );
  }

  async check(score: number) {
    this.sdkService.instance?.sendScore(this.gameId, this.userId, 0, score);
  }

  async ok(score: number, updatedScore: number) {
    this.sdkService.instance?.sendScore(
      this.gameId,
      this.userId,
      updatedScore,
      score
    );
  }
}
