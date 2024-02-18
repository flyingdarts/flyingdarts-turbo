import { Injectable } from '@angular/core';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable, of } from 'rxjs';
import { isNullOrUndefined } from '../app.component';

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

  public get idToken(): string | undefined | null{
    const key = "UserStateService.Token";
    var value = this.storage.getItem(key);
    if (value == null) {
      const authressCredentialsStorage = "AuthenticationCredentialsStorage";
      const authressCredentialsValue = this.storage.getItem(authressCredentialsStorage);
      if (!isNullOrUndefined(authressCredentialsValue)) { 
       var token = JSON.parse(authressCredentialsValue!)["idToken"]
       this.idToken = token;
      return token;
      }
    }
    return value;
}
}
