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

  getUserId(): string {
    console.log('[AuthressService] Getting user ID');
    var userIdentity = this.loginClient.getUserIdentity();
    var userId: string = userIdentity['userId'] as string;
    console.log(`[AuthressService] User ID: ${userId}`);
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
      const parts = token?.split('.')
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
}
