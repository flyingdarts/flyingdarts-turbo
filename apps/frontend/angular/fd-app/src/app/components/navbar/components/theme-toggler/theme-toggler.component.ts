import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppStateActions, AppStateSelectors } from 'src/app/state/app';

@Component({
  selector: 'app-theme-toggler',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './theme-toggler.component.html',
  styleUrl: './theme-toggler.component.scss',
})
export class ThemeTogglerComponent {
  readonly themeMode$: Observable<string>;

  constructor(private readonly store: Store) {
    this.themeMode$ = this.store.select(AppStateSelectors.selectThemeMode);
  }
  toggleTheme() {
    this.store.dispatch(AppStateActions.toggleTheme());
  }
}
