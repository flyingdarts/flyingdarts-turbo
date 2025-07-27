import { NgModule } from '@angular/core';

import { ActionReducer, MetaReducer, StoreModule } from '@ngrx/store';
import { AppStateActions } from '.';
import { appStateReducer } from './app.reducer';
import { AppState } from './app.state';
export function appStateMetaReducer(
  reducer: ActionReducer<any>
): ActionReducer<any> {
  return (state, action) => {
    const nextState: AppState = reducer(state, action);
    switch (action.type) {
      case AppStateActions.AppStateActions.SetX01GameSettings:
        localStorage.setItem(
          'X01GameSettings',
          JSON.stringify(nextState.gameSettings.x01)
        );
    }
    return nextState;
  };
}
export const metaReducers: MetaReducer<any>[] = [appStateMetaReducer];
@NgModule({
  imports: [
    StoreModule.forFeature('appState', appStateReducer, { metaReducers }),
  ],
})
export class AppStateStoreModule {}
