import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { map } from "rxjs/operators";
import {
  loadX01GameSettingsFromStorage,
  loadX01GameSettingsFromStorageFailure,
  loadX01GameSettingsFromStorageSuccess,
} from "./app.actions";

@Injectable()
export class AppEffects {
  constructor(private actions$: Actions) {}

  loadX01GameSettingsFromStorage$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(loadX01GameSettingsFromStorage),
      map(() => {
        try {
          const settings = localStorage.getItem("X01GameSettings");
          if (!settings) {
            return loadX01GameSettingsFromStorageFailure({
              error: "No settings found",
            });
          }
          const parsedSettings = JSON.parse(settings);
          return loadX01GameSettingsFromStorageSuccess({
            settings: parsedSettings,
          });
        } catch (error) {
          return loadX01GameSettingsFromStorageFailure({
            error: "Failed to parse settings",
          });
        }
      })
    );
  });
}
