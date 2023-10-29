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
}
