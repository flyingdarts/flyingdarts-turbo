import { createAction, props } from '@ngrx/store';
import { UserProfileDetails } from 'src/app/repositories/user.repository';

export enum AppStateActions {
  SetUser = '[AppState] Set User',
  LoadUser = '[AppState] Load User',
  LoadUserSuccess = '[AppState] Load User Success',
  LoadUserFailure = '[AppState] Load User Failure',
  SetIdToken = '[AppState] Set Id Token',
  SetLoading = '[AppState] Set Loading',
  SetX01GameSettings = '[AppState] X01 Game Settings',
  LoadX01GameSettingsFromStorage = '[AppState] X01 Game Settings Load',
  LoadX01GameSettingsFromStorageSuccess = '[AppState] X01 Game Settings Load Success',
  LoadX01GameSettingsFromStorageFailure = '[AppState] X01 Game Settings Load Failure',
  ToggleTheme = '[AppState] Theme Toggle',
}

export const setUser = createAction(AppStateActions.SetUser, props<{ user: UserProfileDetails | undefined }>());
export const loadUser = createAction(AppStateActions.LoadUser);
export const loadUserSuccess = createAction(AppStateActions.LoadUserSuccess, props<{ user: UserProfileDetails }>());
export const loadUserFailure = createAction(AppStateActions.LoadUserFailure, props<{ error: any }>());
export const setIdToken = createAction(AppStateActions.SetIdToken, props<{ idToken: string | undefined }>());

export const setLoading = createAction(AppStateActions.SetLoading, props<{ loading: boolean }>());

export const setX01GameSettings = createAction(AppStateActions.SetX01GameSettings, props<{ x01: { legs: number; sets: number } }>());

export const loadX01GameSettingsFromStorage = createAction(AppStateActions.LoadX01GameSettingsFromStorage);

export const loadX01GameSettingsFromStorageSuccess = createAction(
  AppStateActions.LoadX01GameSettingsFromStorageSuccess,
  props<{ settings: { legs: number; sets: number } }>()
);

export const loadX01GameSettingsFromStorageFailure = createAction(
  AppStateActions.LoadX01GameSettingsFromStorageFailure,
  props<{ error: any }>()
);

export const toggleTheme = createAction(AppStateActions.ToggleTheme);
