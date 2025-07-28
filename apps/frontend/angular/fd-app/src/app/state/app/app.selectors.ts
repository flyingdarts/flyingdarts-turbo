import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState, GameSettings, ThemeSettings } from './app.state';

// Selectors
export const selectAppState = createFeatureSelector<AppState>('appState');

export const selectLoading = createSelector(selectAppState, (state: AppState) => state.loading);

export const selectUser = createSelector(selectAppState, (state: AppState) => state.user);

export const selectUserName = createSelector(selectUser, user => user?.UserName ?? '');

export const selectUserId = createSelector(selectUser, user => user?.UserId ?? '');

export const selectUserPicture = createSelector(selectUser, user => user?.Picture ?? '');

export const selectIdToken = createSelector(selectAppState, (state: AppState) => state.idToken);

export const selectGameSettings = createSelector(selectAppState, (state: AppState) => state.gameSettings);

export const selectX01GameSettings = createSelector(selectGameSettings, (settings: GameSettings) => settings.x01);

export const selectX01GameSettingsLegs = createSelector(selectX01GameSettings, (x01: { legs: number; sets: number }) => x01.legs);

export const selectX01GameSettingsSets = createSelector(selectX01GameSettings, (x01: { legs: number; sets: number }) => x01.sets);

export const selectThemeSettings = createSelector(selectAppState, (state: AppState) => state.themeSettings);

export const selectThemeMode = createSelector(selectThemeSettings, (settings: ThemeSettings) => settings.mode);
