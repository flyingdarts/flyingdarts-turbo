import { Injectable } from '@angular/core';
import { Resolve, Router } from '@angular/router';
import { AppStore } from '../app.store';
import { UserProfileApiService } from '../services/user-profile-api.service';
import { AmplifyAuthService } from '../services/amplify-auth.service';
import { UserProfileStateService } from '../services/user-profile-state.service';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ProfileResolver implements Resolve<boolean> {
  constructor(
    private appStore: AppStore,
    private userApiService: UserProfileApiService,
    private userStateService: UserProfileStateService,
    private authService: AmplifyAuthService,
    private router: Router, private httpClient: HttpClient) { }
  async resolve() {
    var userId = null;
    try {
      userId = await this.authService.getCognitoUserId()

      this.appStore.setLoading(true);
      this.userApiService.getUserProfile(userId!).subscribe((x: UserProfileDetails) => {
        this.userStateService.currentUserProfileDetails = x;
        this.appStore.setProfile(x);
        this.appStore.setLoading(false);
        this.router.navigate(['/', 'lobby'])
      })
      this.httpClient.get('https://google.com').subscribe((x) => console.log (x));
      
    } catch (e) {
      console.log('error fetching profile', e);
      this.router.navigate(['/', 'login'])
    }

    return true;
  }
}
