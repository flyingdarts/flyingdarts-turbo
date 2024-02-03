import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { UserProfileApiService } from '../services/user-profile-api.service';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable, of } from 'rxjs';
import { AppStore } from '../app.store';
import { UserProfileStateService } from '../services/user-profile-state.service';
import { isNullOrUndefined } from '../app.component';

@Injectable({ providedIn: 'root' })
export class ProfileDetailsResolver implements Resolve<UserProfileDetails | null> {
  constructor(private userApiService: UserProfileApiService, private appStore: AppStore, private stateService: UserProfileStateService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<UserProfileDetails | null> {
    var isLoggedIn = !isNullOrUndefined(this.stateService.authToken);
    console.log(isLoggedIn);
    if (isLoggedIn) {
      this.stateService.idToken = (this.stateService.authToken as any)["idToken"]
      return this.userApiService.getUserProfile();
    }
    else 
      return of(null)
  }
}