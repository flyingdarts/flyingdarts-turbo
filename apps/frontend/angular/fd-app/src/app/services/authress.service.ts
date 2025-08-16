import { Injectable } from '@angular/core';
import { AuthenticateResponse, LoginClient } from '@mikepattyn/authress-angular';

import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthressService {
  private isLoggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();

  constructor(private readonly loginClient: LoginClient) {
    console.log('[AuthressService] Initialized');
  }

  async isLoggedIn(): Promise<boolean> {
    console.log('[AuthressService] Checking if user is logged in');

    // Check for override token cookie first (for testing purposes)
    const overrideToken = this.getOverrideToken();
    if (overrideToken) {
      console.log('[AuthressService] Found override token, validating directly');
      try {
        // Validate the override token by parsing JWT and checking expiration
        const isValid = this.isValidJWT(overrideToken);
        if (isValid) {
          console.log('[AuthressService] Override token is valid');
          this.isLoggedInSubject.next(true);
          return true;
        } else {
          console.log('[AuthressService] Override token is invalid or expired');
        }
      } catch (error) {
        console.warn('[AuthressService] Override token validation failed:', error);
      }
    }

    // Fall back to normal Authress session check
    const loggedIn = await this.loginClient.userSessionExists();
    this.isLoggedInSubject.next(loggedIn);
    console.log(`[AuthressService] isLoggedIn: ${loggedIn}`);
    return loggedIn;
  }

  async authenticate(): Promise<AuthenticateResponse | null> {
    console.log('[AuthressService] Authenticating user');
    const response = await this.loginClient.authenticate({
      redirectUrl: window.location.href,
    });
    this.isLoggedInSubject.next(response !== null);
    console.log(`[AuthressService] authenticate response:`, response);
    return response;
  }

  async getToken(): Promise<string> {
    console.log('[AuthressService] Getting token');
    const accessTokenOverride = document.cookie.split(';').map(c => c.split('=')).find(c => c[0] === 'custom-jwt-token-override')?.[1];
    if (accessTokenOverride) {
      console.log('[AuthressService] Using token from cookie override');
      return accessTokenOverride;
    }
    const token = await this.loginClient.ensureToken();
    console.log('[AuthressService] Token obtained from loginClient');
    return token;
  }

  async getUserName(): Promise<string | null> {
    console.log('[AuthressService] Getting user name');
    const token = await this.getToken();
    const name = this.getName(token);
    console.log(`[AuthressService] User name: ${name}`);
    return name;
  }

  async getUserId(): Promise<string> {
    console.log('[AuthressService] Getting user ID');

    // Check for override token first
    const overrideToken = this.getOverrideToken();
    if (overrideToken) {
      try {
        const userId = this.getUserIdFromToken(overrideToken);
        if (userId) {
          console.log(`[AuthressService] User ID from override token: ${userId}`);
          return userId;
        }
      } catch (error) {
        console.warn('[AuthressService] Failed to get user ID from override token:', error);
      }
    }

    // Fall back to normal Authress flow
    var userIdentity = this.loginClient.getUserIdentity();
    var userId: string = userIdentity['userId'] as string;
    console.log(`[AuthressService] User ID from Authress: ${userId}`);
    return userId;
  }

  public async signout(redirectUrl: string): Promise<void> {
    console.log('[AuthressService] Signing out');
    await this.loginClient.logout(redirectUrl);
    this.isLoggedInSubject.next(false);
    console.log('[AuthressService] Signed out');
  }

  private getName(token: string | null): string | null {
    try {
      console.log('[AuthressService] Decoding JWT for name');
      // Split the token into its parts
      const parts = token?.split('.');
      if (parts && parts.length !== 3) {
        throw new Error('Invalid JWT token');
      }

      // Decode the payload part of the token
      const payloadBase64 = parts![1].replace(/-/g, '+').replace(/_/g, '/');
      const payloadJson = atob(payloadBase64);
      const payload = JSON.parse(payloadJson);
      // Return the 'name' property if it exists
      const name = payload.name || null;
      console.log(`[AuthressService] Decoded name: ${name}`);
      return name;
    } catch (error) {
      console.error('[AuthressService] Failed to decode JWT:', error);
      return null;
    }
  }

  private getOverrideToken(): string | null {
    // Check for the override token cookie
    const cookies = document.cookie.split(';');
    const overrideCookie = cookies.find(cookie =>
      cookie.trim().startsWith('custom-jwt-token-override=')
    );

    if (overrideCookie) {
      return overrideCookie.split('=')[1];
    }

    return null;
  }

  private isValidJWT(token: string): boolean {
    try {
      // Split the token into its parts
      const parts = token.split('.');
      if (parts.length !== 3) {
        return false;
      }

      // Decode the payload (second part)
      const payload = parts[1];
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );

      const claims = JSON.parse(jsonPayload);

      // Check if token has expired
      if (claims.exp) {
        const currentTime = Math.floor(Date.now() / 1000);
        if (currentTime > claims.exp) {
          console.log('[AuthressService] Override token has expired');
          return false;
        }
      }

      // Check if token has required claims
      if (!claims.sub) {
        console.log('[AuthressService] Override token missing sub claim');
        return false;
      }

      console.log('[AuthressService] Override token validation successful');
      return true;
    } catch (error) {
      console.error('[AuthressService] Error validating override token:', error);
      return false;
    }
  }

  private getUserIdFromToken(token: string): string | null {
    try {
      // Split the token into its parts
      const parts = token.split('.');
      if (parts.length !== 3) {
        return null;
      }

      // Decode the payload (second part)
      const payload = parts[1];
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );

      const claims = JSON.parse(jsonPayload);

      // Return the 'sub' claim which contains the user ID
      return claims.sub || null;
    } catch (error) {
      console.error('[AuthressService] Failed to extract user ID from token:', error);
      return null;
    }
  }
}
