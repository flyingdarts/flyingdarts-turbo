export interface X01State {
  currentInput: string;
  player: X01PlayerState;
  opponent: X01PlayerState;
  loading: boolean;
  error: string;
}


export interface X01PlayerState {
  name: string;
  score: number;
  scores: number[];
  total: number;
  sets: number;
  legs: number;
}

export const initialX01State: X01State = {
  currentInput: '',
  loading: false,
  error: '',
  player: {
    name: 'Mike',
    legs: 0,
    sets: 0,
    score: 501,
    scores: [],
    total: 6
  },
  opponent: {
    name: 'Waiting...',
    legs: 0,
    sets: 0,
    score: 501,
    scores: [],
    total: 6
  }
}