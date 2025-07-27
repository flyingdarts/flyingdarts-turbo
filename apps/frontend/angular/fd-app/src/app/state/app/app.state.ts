import { UserProfileDetails } from 'src/app/repositories/user.repository';

export interface AppState {
  idToken: string | undefined;
  user: UserProfileDetails | undefined;
  loading: boolean;
  gameSettings: GameSettings;
  themeSettings: ThemeSettings;
}

export interface UserSettings {
  gameSettings: GameSettings;
}

export interface GameSettings {
  x01: X01Settings;
}

export interface X01Settings {
  legs: number;
  sets: number;
}

export const initialAppState: AppState = {
  idToken: undefined,
  user: undefined,
  loading: false,
  gameSettings: {
    x01: {
      legs: 0,
      sets: 0,
    },
  },
  themeSettings: {
    mode: 'dark',
  },
};

export interface ThemeSettings {
  mode: 'light' | 'dark';
}
