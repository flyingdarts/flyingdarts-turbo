export interface GameState {
  input?: string;
  player: PlayerState;
  opponent: PlayerState;
  game: {
    legs: number;
    sets: number;
    score: number;
  };
  nextPlayer: string | null;
  winningPlayer: string | null;
}

export interface PlayerState {
  id: string;
  name: string;
  score: number;
  scores: number[];
  total: number;
  sets: number;
  legs: number;
}

export const initialGameState: GameState = {
  player: {
    id: '',
    name: '',
    legs: 0,
    sets: 0,
    score: 501,
    scores: [],
    total: 6,
  },
  opponent: {
    id: '',
    name: '',
    legs: 0,
    sets: 0,
    score: 501,
    scores: [],
    total: 6,
  },
  game: {
    legs: 3,
    sets: 3,
    score: 501,
  },
  nextPlayer: null,
  winningPlayer: null,
};
