import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { map, Observable } from 'rxjs';
import { GameStateSelectors, GameStateStates } from 'src/app/state/game';

@Component({
  selector: 'app-game-stats-ui',
  standalone: true,
  templateUrl: './game-stats.component.html',
  styleUrl: './game-stats.component.scss',
  imports: [CommonModule],
})
export class GameStatsComponent {
  readonly gameState$: Observable<GameStateStates.GameState>;

  constructor(private readonly store: Store) {
    this.gameState$ = this.store.select(GameStateSelectors.selectGameState);
    this.gameState$.pipe(map(state => state.game)).subscribe(game => {
      console.log(game);
    });
  }

  getHistory(darts: number[] | undefined): string {
    if (!darts) {
      return '';
    }
    return darts.join(' | ');
  }

  getPlayerName(player: GameStateStates.PlayerState | undefined): string {
    if (!player) {
      return '';
    }
    if (player.name.substring(0, 2) === 'sc') {
      return 'SC Player';
    }
    return player.name;
  }

  getOpponentName(opponent: GameStateStates.PlayerState | undefined): string {
    if (!opponent) {
      return '';
    }
    if (opponent.name.substring(0, 2) === 'sc') {
      return 'SC Opponent';
    }
    return opponent.name;
  }
}
