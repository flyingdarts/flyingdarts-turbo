import { Injectable } from '@angular/core';
import { AuthenticateResponse, LoginClient } from '@mikepattyn/authress-angular';

import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthressService {
  private isLoggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();

  constructor(private readonly loginClient: LoginClient) {}

  async isLoggedIn(): Promise<boolean> {
    const loggedIn = await this.loginClient.userSessionExists();
    this.isLoggedInSubject.next(loggedIn);
    return loggedIn;
  }

  async authenticate(): Promise<AuthenticateResponse | null> {
    const response = await this.loginClient.authenticate({
      redirectUrl: window.location.href,
    });
    this.isLoggedInSubject.next(response !== null);
    return response;
  }

  async getToken(): Promise<string> {
    return await this.loginClient.ensureToken();
  }

  async getUserName(): Promise<string | null> {
    const token = await this.getToken();

    return this.getName(token);
  }

  getUserId(): string {
    var userIdentity = this.loginClient.getUserIdentity();
    var userId: string = userIdentity['userId'] as string;
    return userId;
  }

  public async signout(redirectUrl: string): Promise<void> {
    await this.loginClient.logout(redirectUrl);
    this.isLoggedInSubject.next(false);
  }

  private getName(token: string | null): string | null {
    try {
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
      return payload.name || null;
    } catch (error) {
      console.error('Failed to decode JWT:', error);
      return null;
    }
  }
}
