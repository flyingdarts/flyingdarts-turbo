import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Store } from '@ngrx/store';
import { map, Observable } from 'rxjs';
import { AuthressService } from 'src/app/services/authress.service';
import { AppStateActions, AppStateSelectors } from 'src/app/state/app';
import { ThemeTogglerComponent } from './components/theme-toggler/theme-toggler.component';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule, ThemeTogglerComponent],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
  standalone: true,
})
// eslint-disable-next-line @typescript-eslint/no-extraneous-class
export class NavbarComponent {
  isLoggedIn$: Observable<boolean>;
  userName$: Observable<string>;
  userPicture$: Observable<string>;
  isDarkMode$: Observable<boolean>;

  constructor(private readonly store: Store, private readonly authressService: AuthressService) {
    this.isLoggedIn$ = authressService.isLoggedIn$; // TODO: Move to state
    this.userName$ = store
      .select(AppStateSelectors.selectUserName)
      .pipe(map(name => (name == '' ? sessionStorage.getItem('userName')! : name)));
    this.userPicture$ = store.select(AppStateSelectors.selectUserPicture);
    this.isDarkMode$ = store.select(AppStateSelectors.selectThemeMode).pipe(map(mode => mode === 'dark'));
  }

  logout() {
    this.store.dispatch(AppStateActions.setLoading({ loading: true }));
    this.store.dispatch(AppStateActions.setIdToken({ idToken: undefined }));
    this.store.dispatch(AppStateActions.setUser({ user: undefined }));
    this.authressService.signout(window.location.origin);
  }
}
