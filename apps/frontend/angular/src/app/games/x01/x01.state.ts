export interface X01State {
  currentInput: string;
  player: X01PlayerState;
  opponent: X01PlayerState;
  loading: boolean;
  error: string;
  currentPlayer: string;
  winningPlayer: string | null;
}


export interface X01PlayerState {
  name: string;
  score: number;
  scores: number[];
  total: number;
  sets: number;
  legs: number;
  country: string;
}

export const initialX01State: X01State = {
  currentInput: '',
  currentPlayer: '',
  winningPlayer: null,
  loading: false,
  error: '',
  player: {
    name: 'Player',
    legs: 0,
    sets: 0,
    score: 501,
    scores: [],
    total: 6,
    country: 'nl'
  },
  opponent: {
    name: 'Waiting...',
    legs: 0,
    sets: 0,
    score: 501,
    scores: [],
    total: 6,
    country: 'nl'
  }
}