import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppStateActions } from '../state/app';
import { Hooks } from './hooks';

@Injectable({ providedIn: 'root' })
export class StateHooksService implements Hooks {
  constructor(private readonly router: Router, private readonly store: Store) {}

  async handleGame(gameId: string): Promise<void> {
    this.store.dispatch(AppStateActions.setLoading({ loading: false }));

    await this.router.navigate(['/', 'game', gameId]);
  }
}
