import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { UserProfileStateService } from '../services/user-profile-state.service';
import { ApiClient } from '../services/api.client';
import { AmplifyAuthService } from '../services/amplify-auth.service';
import { GetUserProfileCommand } from '../requests/GetUserProfileCommand';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';

@Injectable({
  providedIn: 'root'
})
export class OnboardingGuard implements CanActivate {
  constructor(
    private router: Router,
    private apiClient: ApiClient,
    private authService: AmplifyAuthService
  ) {

  }
  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean> {

    return true;
  }
  // async canActivate(
  //   route: ActivatedRouteSnapshot,
  //   state: RouterStateSnapshot) {


  //   if (!this.stateService.currentUserProfileDetails.isRegistered!) {
  //     return true;
  //   } else {
  //     if (!this.stateService.currentUserProfileDetails.cameraPermissionGranted!) {
  //       this.router.navigate(['/', 'onboarding', { outlets: { 'onboarding-outlet': ['camera'] } }]);
  //       return true;
  //     } else {
  //       this.router.navigate(['/', 'lobby']);
  //       return false;
  //     }
  //   }
  // }

}
