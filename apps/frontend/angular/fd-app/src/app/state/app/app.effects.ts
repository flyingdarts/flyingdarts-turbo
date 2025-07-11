import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { map, switchMap } from 'rxjs/operators';
import {
  UserProfileDetails,
  UserRepository,
} from 'src/app/repositories/user.repository';
import {
  loadUser,
  loadUserFailure,
  loadUserSuccess,
  loadX01GameSettingsFromStorage,
  loadX01GameSettingsFromStorageFailure,
  loadX01GameSettingsFromStorageSuccess,
} from './app.actions';

@Injectable()
export class AppEffects {
  constructor(private actions$: Actions, private userService: UserRepository) {}

  loadUser$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(loadUser),
      switchMap(() =>
        this.userService.getUserProfile().pipe(
          map((user: UserProfileDetails | null) => {
            if (user) {
              return loadUserSuccess({ user });
            } else {
              return loadUserFailure({ error: 'User profile not found' });
            }
          })
        )
      )
    );
  });

  loadX01GameSettingsFromStorage$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(loadX01GameSettingsFromStorage),
      map(() => {
        try {
          const settings = localStorage.getItem('X01GameSettings');
          if (!settings) {
            return loadX01GameSettingsFromStorageFailure({
              error: 'No settings found',
            });
          }
          const parsedSettings = JSON.parse(settings);
          return loadX01GameSettingsFromStorageSuccess({
            settings: parsedSettings,
          });
        } catch (error) {
          return loadX01GameSettingsFromStorageFailure({
            error: 'Failed to parse settings',
          });
        }
      })
    );
  });
}
