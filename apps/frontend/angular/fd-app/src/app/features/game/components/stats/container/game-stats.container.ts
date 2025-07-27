import { Component } from '@angular/core';
import { GameStatsComponent } from '../presentation/game-stats.component';

@Component({
  selector: 'app-game-stats',
  standalone: true,
  templateUrl: './game-stats.container.html',
  imports: [GameStatsComponent],
})
export class GameStatsContainerComponent {}
