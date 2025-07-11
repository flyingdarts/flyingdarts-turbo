import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';
import {
  UserProfileDetails,
  UserRepository,
} from '../repositories/user.repository';
import { AuthressService } from '../services/authress.service';
import { AppStateActions } from '../state/app';

@Injectable({
  providedIn: 'root',
})
export class UserProfileResolver implements Resolve<Promise<boolean>> {
  constructor(
    private readonly authressService: AuthressService,
    private readonly userRepository: UserRepository,
    private readonly store: Store
  ) {}

  async resolve(route: ActivatedRouteSnapshot): Promise<boolean> {
    this.store.dispatch(AppStateActions.setLoading({ loading: true }));

    try {
      let profile = await firstValueFrom(this.userRepository.getUserProfile());

      if (!profile) {
        const authProviderUserId = this.authressService.getUserId();
        const authProviderUserName =
          (await this.authressService.getUserName()) ?? 'John Doe';

        profile = await firstValueFrom(
          this.userRepository.createUserProfile(
            authProviderUserId,
            'mike@flyingdarts.net',
            authProviderUserName,
            'NL'
          )
        );
      }

      this.setUser(profile);
      return true;
    } catch (err) {
      console.error('Error resolving user profile:', err);
      return false;
    } finally {
      this.store.dispatch(AppStateActions.setLoading({ loading: false }));
    }
  }

  private setUser(profile: UserProfileDetails) {
    sessionStorage.setItem('userName', profile.UserName);
    sessionStorage.setItem('userId', profile.UserId!);

    this.store.dispatch(AppStateActions.setUser({ user: profile }));
  }
}
