import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom, Observable } from 'rxjs';
import { AppStateActions, AppStateSelectors } from 'src/app/state/app';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home-settings.component.html',
  styleUrl: './home-settings.component.scss',
  standalone: true,
})
export class HomeSettingsComponent {
  preferredX01Sets: Observable<number>;
  preferredX01Legs: Observable<number>;

  constructor(private readonly store: Store, private readonly router: Router) {
    this.preferredX01Sets = this.store.select(
      AppStateSelectors.selectX01GameSettingsSets
    );
    this.preferredX01Legs = this.store.select(
      AppStateSelectors.selectX01GameSettingsLegs
    );
  }

  async action(type: 'increase' | 'decrease', property: 'legs' | 'sets') {
    var currentSettings = await firstValueFrom(
      this.store.select(AppStateSelectors.selectX01GameSettings)
    );

    if (type === 'decrease' && property == 'legs') {
      currentSettings = {
        ...currentSettings,
        legs: currentSettings.legs - 1,
      };
    }
    if (type === 'increase' && property === 'legs') {
      currentSettings = {
        ...currentSettings,
        legs: currentSettings.legs + 1,
      };
    }
    if (type === 'decrease' && property == 'sets') {
      currentSettings = {
        ...currentSettings,
        sets: currentSettings.sets - 1,
      };
    }
    if (type === 'increase' && property === 'sets') {
      currentSettings = {
        ...currentSettings,
        sets: currentSettings.sets + 1,
      };
    }

    this.store.dispatch(
      AppStateActions.setX01GameSettings({ x01: currentSettings })
    );
  }

  async onBack() {
    this.router.navigate(['/']);
  }
}
