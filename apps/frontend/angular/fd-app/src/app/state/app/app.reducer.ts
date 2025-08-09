import { createReducer, on } from '@ngrx/store';

import {
  loadUser,
  loadUserFailure,
  loadUserSuccess,
  loadX01GameSettingsFromStorage,
  loadX01GameSettingsFromStorageFailure,
  loadX01GameSettingsFromStorageSuccess,
  setIdToken,
  setLoading,
  setUser,
  setX01GameSettings,
  toggleTheme,
} from './app.actions';
import { initialAppState } from './app.state';

export const appStateReducer = createReducer(
  initialAppState,
  on(setUser, (prevState, { user }) => ({
    ...prevState,
    user: user,
  })),
  on(loadUser, prevState => ({
    ...prevState,
    loading: true,
  })),
  on(loadUserSuccess, (prevState, { user }) => ({
    ...prevState,
    user: user,
    loading: false,
  })),
  on(loadUserFailure, (prevState, { error }) => ({
    ...prevState,
    loading: false,
    error: error,
  })),
  on(loadX01GameSettingsFromStorage, prevState => {
    return {
      ...prevState,
      loading: true,
    };
  }),
  on(loadX01GameSettingsFromStorageSuccess, (prevState, { settings }) => {
    return {
      ...prevState,
      loading: false,
      gameSettings: {
        ...prevState.gameSettings,
        x01: settings,
      },
    };
  }),
  on(loadX01GameSettingsFromStorageFailure, (prevState, { error }) => {
    return {
      ...prevState,
      loading: false,
      error: error,
    };
  }),
  on(setIdToken, (prevState, { idToken }) => ({
    ...prevState,
    idToken: idToken,
  })),
  on(setLoading, (prevState, { loading }) => ({
    ...prevState,
    loading: loading,
  })),
  on(setX01GameSettings, (prevState, { x01 }) => ({
    ...prevState,
    gameSettings: {
      ...prevState.gameSettings,
      x01: x01,
    },
  })),
  on(toggleTheme, prevState => {
    const nextTheme: 'light' | 'dark' = prevState.themeSettings.mode == 'light' ? 'dark' : 'light';
    return {
      ...prevState,
      themeSettings: {
        ...prevState.themeSettings,
        mode: nextTheme,
      },
    };
  })
);
