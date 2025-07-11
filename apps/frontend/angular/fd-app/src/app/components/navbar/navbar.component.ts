import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { map, Observable } from 'rxjs';
import { AuthressService } from 'src/app/services/authress.service';
import { AppStateActions, AppStateSelectors } from 'src/app/state/app';
import { ThemeTogglerComponent } from './components/theme-toggler/theme-toggler.component';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, ThemeTogglerComponent],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  standalone: true,
})
// eslint-disable-next-line @typescript-eslint/no-extraneous-class
export class NavbarComponent {
  isLoggedIn$: Observable<boolean>;
  userName$: Observable<string>;

  constructor(
    private readonly store: Store,
    private readonly authressService: AuthressService
  ) {
    this.isLoggedIn$ = authressService.isLoggedIn$; // TODO: Move to state
    this.userName$ = store
      .select(AppStateSelectors.selectUserName)
      .pipe(
        map((name) => (name == '' ? sessionStorage.getItem('userName')! : name))
      );
  }

  logout() {
    this.store.dispatch(AppStateActions.setLoading({ loading: true }));
    this.store.dispatch(AppStateActions.setIdToken({ idToken: undefined }));
    this.store.dispatch(AppStateActions.setUser({ user: undefined }));
    this.authressService.signout(window.location.origin);
  }
}
