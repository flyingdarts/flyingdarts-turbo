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
}
