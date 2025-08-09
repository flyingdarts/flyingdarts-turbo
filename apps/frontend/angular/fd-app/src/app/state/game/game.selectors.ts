import { createFeatureSelector, createSelector } from '@ngrx/store';
import { GameState } from './game.state';

// Selectors
export const selectGameState = createFeatureSelector<GameState>('gameState');

export const selectGamePlayers = createSelector(selectGameState, (state: GameState) => [state.player, state.opponent]);

export const selectGameInput = createSelector(selectGameState, (state: GameState) => state.input);

export const selectPlayerScore = createSelector(selectGameState, (state: GameState) => state.player.score);

export const selectNextPlayer = createSelector(selectGameState, (state: GameState) => state.nextPlayer);

export const selectWinningPlayer = createSelector(selectGameState, (state: GameState) => state.winningPlayer);
