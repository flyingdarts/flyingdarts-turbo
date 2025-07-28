import { createReducer, on } from '@ngrx/store';
import { clearInput, updateInput, updateState } from './game.actions';
import { initialGameState } from './game.state';

export const gameStateReducer = createReducer(
  initialGameState,

  on(updateState, (_, { state }) => {
    return state;
  }),
  on(updateInput, (prevState, { input }) => {
    if (!validInput(prevState.input ?? '', input)) {
      return prevState;
    }

    return {
      ...prevState,
      input: `${prevState.input ?? ''}${input}`,
    };
  }),
  on(clearInput, prevState => {
    return {
      ...prevState,
      input: '',
    };
  })
);

function validInput(prev: string, next: string): boolean {
  // Concatenate prev and next
  const newInput = prev + next;

  // Convert concatenated string to a number
  const numberValue = Number(newInput);

  // Check if the number is less than 180
  return numberValue <= 180;
}
