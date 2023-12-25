import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { AppStore } from '../app.store';
import { UserProfileApiService } from '../services/user-profile-api.service';
import { AmplifyAuthService } from '../services/amplify-auth.service';
import { UserProfileStateService } from '../services/user-profile-state.service';

@Injectable({
  providedIn: 'root'
})
export class ProfileResolver implements Resolve<boolean> {
  constructor(
    private appStore: AppStore, 
    private userApiService: UserProfileApiService, 
    private userStateService: UserProfileStateService,
    private authService: AmplifyAuthService) {}
  async resolve() {
    var userId = await this.authService.getCognitoUserId()
    this.appStore.setLoading(true);
    this.userApiService.getUserProfile(userId).subscribe(x=> {
      this.userStateService.currentUserProfileDetails = x;
      this.appStore.setProfile(x);
      this.appStore.setLoading(false);
      console.log('profile succesfully resolved')
    })
    return true;
  }
}
