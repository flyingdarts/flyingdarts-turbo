import { createAction, props } from '@ngrx/store';
import { GameStateStates } from '.';

export enum GameStateActions {
  updateState = '[GameState] Update State',
  updateInput = '[GameState] Input Update',
  clearInput = `[GameState] Input Clear`,
}

export const updateState = createAction(
  GameStateActions.updateState,
  props<{ state: GameStateStates.GameState }>()
);

export const updateInput = createAction(
  GameStateActions.updateInput,
  props<{ input: string }>()
);

export const clearInput = createAction(GameStateActions.clearInput);
