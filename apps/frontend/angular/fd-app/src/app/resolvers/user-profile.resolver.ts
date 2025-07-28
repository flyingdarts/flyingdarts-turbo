import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';
import { UserProfileDetails, UserRepository } from '../repositories/user.repository';
import { AppStateActions } from '../state/app';

/**
 * Resolves the current session user from the Authress JWT token in localStorage.
 * No longer fetches from a repository; simply parses the token.
 */
@Injectable({
  providedIn: 'root',
})
export class SessionUserResolver implements Resolve<Promise<boolean>> {
  constructor(private readonly store: Store, private readonly userRepository: UserRepository) {}

  async resolve(): Promise<boolean> {
    this.store.dispatch(AppStateActions.setLoading({ loading: true }));
    try {
      const idTokenRaw = localStorage.getItem('AuthenticationCredentialsStorage');
      if (!idTokenRaw) throw new Error('No Authress credentials found in localStorage');
      const idTokenParsed = JSON.parse(idTokenRaw);
      const idTokenValue = idTokenParsed.idToken;
      const profile = this.createUserProfileFromToken(idTokenValue);
      this.setUser(profile);

      const user = await firstValueFrom(this.userRepository.getUser());
      sessionStorage.setItem('userId', user.UserId);
      return true;
    } catch (err) {
      console.error('Error resolving session user:', err);
      return false;
    } finally {
      this.store.dispatch(AppStateActions.setLoading({ loading: false }));
    }
  }

  /**
   * Parses a JWT token and returns a UserProfileDetails object.
   * Extracts name, email, and picture claims.
   */
  private createUserProfileFromToken(token: string): UserProfileDetails {
    console.log('[SessionUserResolver] Creating user profile from Authress token');
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      const claims = JSON.parse(jsonPayload);
      const userName = claims.name ?? '';
      const email = claims.email ?? '';
      const picture = claims.picture ?? '';
      const userId = claims.sub ?? '';
      console.log(`[SessionUserResolver] Extracted name: ${userName}, email: ${email}, picture: ${picture}, userId: ${userId}`);
      return {
        UserName: userName,
        Email: email,
        Country: 'NL',
        Picture: picture,
        UserId: userId,
      };
    } catch (error) {
      console.error(`[SessionUserResolver] Error creating user profile from token: ${error}`);
      throw error;
    }
  }

  private setUser(profile: UserProfileDetails) {
    sessionStorage.setItem('userName', profile.UserName);
    this.store.dispatch(AppStateActions.setUser({ user: profile }));
  }
}
