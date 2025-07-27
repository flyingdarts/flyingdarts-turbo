import { NgModule } from '@angular/core';

import { StoreModule } from '@ngrx/store';
import { gameStateReducer } from './game.reducer';

@NgModule({
  imports: [StoreModule.forFeature('gameState', gameStateReducer)],
})
export class GameStateStoreModule {}
