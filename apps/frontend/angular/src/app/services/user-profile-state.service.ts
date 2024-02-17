import { Injectable } from '@angular/core';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserProfileStateService {
  private storage = localStorage;

  public clear(): void {
    this.storage.removeItem('UserStateService.UserProfileDetails');
    this.storage.removeItem('UserStateService.Token');
  }

  public get currentUserProfileDetails(): UserProfileDetails {
    const key = 'UserStateService.UserProfileDetails';
    const serializedRequest = JSON.parse(this.storage.getItem(key)!);
    return serializedRequest;
  }

  public set currentUserProfileDetails(value: UserProfileDetails | null) {
    const key = 'UserStateService.UserProfileDetails';
    this.storage.setItem(key, JSON.stringify(value));
  }

  public get userName$(): Observable<string> {
    return of(this.currentUserProfileDetails?.UserName);
  }

  public get isRegistered$():Observable<boolean> {
    return of(!!this.currentUserProfileDetails);
  }

  public set idToken(value: string) {
    const key = "UserStateService.Token";
    this.storage.setItem(key, value);
  }

  public get idToken(): string {
    const key = "UserStateService.Token";
    var value = this.storage.getItem(key);
    if (value == null) {
      const authressCredentialsStorage = "AuthenticationCredentialsStorage";
      const idToken = JSON.parse(this.storage.getItem(authressCredentialsStorage)!)["idToken"]
      this.idToken = idToken;
      return idToken;
    }
    return value;
}
}
